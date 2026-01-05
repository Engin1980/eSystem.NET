using ESystem.Asserting;
using ESystem.Miscelaneous;
using EXmlLib2.Abstractions.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Abstracts;

internal abstract class NewTypeSerializer<TDataFieldInfo> : IElementSerializer
{
  public abstract bool AcceptsType(Type type);

  public void Serialize(object? value, XElement element, IXmlContext ctx)
  {
    EAssert.Argument.IsNotNull(value, nameof(value));

    IEnumerable<TDataFieldInfo> dataFields = GetTypeDataFields(value.GetType());
    foreach (TDataFieldInfo dataField in dataFields)
    {
      SerializeDataField(value, dataField, element, ctx);
    }
  }

  protected abstract void SerializeDataField(object value, TDataFieldInfo dataField, XElement element, IXmlContext ctx);
  protected abstract IEnumerable<TDataFieldInfo> GetTypeDataFields(Type type);
}

public interface IPropertySerializer
{
  void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx);
}
public class SimplePropertyAsElement : IPropertySerializer, IPropertyDeserializer
{
  public const string TYPE_ATTRIBUTE = "__instanceType";

  public object? DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
  {
    throw new NotImplementedException();
  }

  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
  {
    var ser = ctx.ElementSerializers.GetByType(propertyInfo.PropertyType);
    XElement propElement = new(propertyInfo.Name);
    ctx.SerializeToElement(propertyValue, propElement, ser);

    if (propertyValue != null && propertyValue.GetType() != propertyInfo.PropertyType)
    {
      propElement.SetAttributeValue(TYPE_ATTRIBUTE, propertyValue.GetType().AssemblyQualifiedName);
    }

    element.Add(propElement);
  }
}
public class SimplePropertyAsAttribute : IPropertySerializer
{
  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
  {
    if (propertyValue != null && propertyValue.GetType() != propertyInfo.PropertyType)
      throw new InvalidOperationException($"Cannot serialize property '{propertyInfo.Name}' as attribute because its runtime value type '{propertyValue.GetType().FullName}' differs from declared property type '{propertyInfo.PropertyType.FullName}' (type ${propertyInfo.DeclaringType}).");

    var ser = ctx.AttributeSerializers.GetByType(propertyInfo.PropertyType);
    XAttribute attr = ctx.SerializeToAttribute(propertyInfo.Name, propertyValue, ser);
    element.Add(attr);
  }
}
public class IgnoredProperty : IPropertySerializer
{
  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
  {
    // no-op
  }
}
public class PolymorphicPropertyAsElement : IPropertySerializer
{
  public enum MissingDefinitionBehavior
  {
    UseDefault,
    ThrowException
  }
  private readonly BiDictionary<Type, string> typeToXmlNameMapping = new();
  private MissingDefinitionBehavior missingDefinitionBehavior = MissingDefinitionBehavior.UseDefault;

  public PolymorphicPropertyAsElement WithMissingDefinitionBehavior(MissingDefinitionBehavior behavior)
  {
    this.missingDefinitionBehavior = behavior;
    return this;
  }
  public PolymorphicPropertyAsElement With(Type type, string xmlName)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    EAssert.Argument.IsNonEmptyString(xmlName, nameof(xmlName));
    typeToXmlNameMapping.Set(type, xmlName);
    return this;
  }
  public PolymorphicPropertyAsElement With<T>(string xmlName)
  {
    EAssert.Argument.IsNonEmptyString(xmlName, nameof(xmlName));
    return this.With(typeof(T), xmlName);
  }

  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
  {
    string elementName = propertyValue == null
      ? propertyInfo.Name
      : typeToXmlNameMapping.TryGetValueOrDefault(propertyValue.GetType(), propertyInfo.Name);

    var ser = ctx.ElementSerializers.GetByType(propertyValue?.GetType() ?? propertyInfo.PropertyType);
    XElement propElement = new(elementName);
    ctx.SerializeToElement(propertyValue, propElement, ser);
    element.Add(propElement);
  }
}

public interface IInstanceFactory<TDataFieldInfo> where TDataFieldInfo : notnull
{
  object CreateInstance(Type targetType, IDictionary<TDataFieldInfo, object?> deserializedValues);
}
public class PublicParameterlessConstructorInstanceFromPropertiesFactory : IInstanceFactory<PropertyInfo>
{
  public object CreateInstance(Type targetType, IDictionary<PropertyInfo, object?> deserializedValues)
  {
    object ret = Activator.CreateInstance(targetType)
      ?? throw new InvalidOperationException($"Cannot create instance of type '{targetType.FullName}' using parameterless constructor.");
    foreach (var kvp in deserializedValues)
    {
      kvp.Key.SetValue(ret, kvp.Value);
    }
    return ret;
  }
}

internal class NewTypeByPropertySerializer : NewTypeSerializer<PropertyInfo>
{
  public static readonly Func<Type, PropertyInfo[]> PUBLIC_INSTANCE_PROPERTIES_PROVIDER = q => q.GetProperties(BindingFlags.Public | BindingFlags.Instance);
  public static readonly Func<object, PropertyInfo, object?> PROPERTY_VALUE_READER = (obj, prop) => prop.GetValue(obj);

  private Func<Type, PropertyInfo[]> propertiesProvider = PUBLIC_INSTANCE_PROPERTIES_PROVIDER;
  private Func<object, PropertyInfo, object?> propertyValueProvider = PROPERTY_VALUE_READER;
  private IPropertySerializer propertySerializer = new SimplePropertyAsElement();
  private readonly Dictionary<PropertyInfo, IPropertySerializer> propertySerializers = [];
  private Func<Type, bool> acceptsTypePredicate = q => false;

  public override bool AcceptsType(Type type) => acceptsTypePredicate(type);

  public NewTypeByPropertySerializer WithAcceptedType(Type type, bool acceptDerivedTypes = false)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    if (acceptDerivedTypes)
      return this.WithAcceptedType(q => type.IsAssignableFrom(q));
    else
      return this.WithAcceptedType(q => q == type);
  }
  public NewTypeByPropertySerializer WithAcceptedType<T>(bool acceptDerivedTypes = false)
  {
    return this.WithAcceptedType(typeof(T), acceptDerivedTypes);
  }
  public NewTypeByPropertySerializer WithAcceptedType(Func<Type, bool> predicate)
  {
    EAssert.Argument.IsNotNull(predicate, nameof(predicate));
    var tmp = this.acceptsTypePredicate;
    this.acceptsTypePredicate = q => predicate(q) || tmp(q);
    return this;
  }

  public NewTypeByPropertySerializer WithPropertiesProvider(Func<Type, PropertyInfo[]> propertiesProvider)
  {
    this.propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
    return this;
  }

  public NewTypeByPropertySerializer WithPropertyValueProvider(Func<object, PropertyInfo, object?> propertyValueProvider)
  {
    this.propertyValueProvider = propertyValueProvider ?? throw new ArgumentNullException(nameof(propertyValueProvider));
    return this;
  }

  public NewTypeByPropertySerializer WithPropertySerializer(IPropertySerializer propertySerializer)
  {
    this.propertySerializer = propertySerializer ?? throw new ArgumentNullException(nameof(propertySerializer));
    return this;
  }

  public NewTypeByPropertySerializer WithPropertySerializerFor(PropertyInfo propertyInfo, IPropertySerializer propertySerializer)
  {
    EAssert.Argument.IsNotNull(propertyInfo, nameof(propertyInfo));
    EAssert.Argument.IsNotNull(propertySerializer, nameof(propertySerializer));
    this.propertySerializers[propertyInfo] = propertySerializer;
    return this;
  }
  public NewTypeByPropertySerializer WithPropertySerializerFor<T>(Expression<Func<T, object?>> propertyExpression, IPropertySerializer propertySerializer)
  {
    EAssert.Argument.IsNotNull(propertyExpression, nameof(propertyExpression));
    EAssert.Argument.IsNotNull(propertySerializer, nameof(propertySerializer));
    PropertyInfo propertyInfo = ExtractPropertyInfo(propertyExpression);
    return this.WithPropertySerializerFor(propertyInfo, propertySerializer);
  }

  private static PropertyInfo ExtractPropertyInfo<T>(
  Expression<Func<T, object?>> propertySelector)
  {
    if (propertySelector is null)
      throw new ArgumentNullException(nameof(propertySelector));

    Expression body = propertySelector.Body;

    // ošetření boxing / castu na object
    if (body is UnaryExpression unary &&
        unary.NodeType == ExpressionType.Convert)
    {
      body = unary.Operand;
    }

    if (body is not MemberExpression member)
      throw new ArgumentException(
        "Expression must select a property.",
        nameof(propertySelector));

    if (member.Member is not PropertyInfo property)
      throw new ArgumentException(
        "Expression does not refer to a property.",
        nameof(propertySelector));

    return property;
  }

  protected override IEnumerable<PropertyInfo> GetTypeDataFields(Type type) => propertiesProvider(type);

  protected object? GetPropertyValue(object value, PropertyInfo propertyInfo) => propertyValueProvider(value, propertyInfo);

  protected override void SerializeDataField(object value, PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
  {
    object? propertyValue = GetPropertyValue(value, propertyInfo);
    IPropertySerializer psd = this.propertySerializers.TryGetValue(propertyInfo, out IPropertySerializer? tmp)
      ? tmp
      : this.propertySerializer;
    psd.SerializeProperty(propertyInfo, propertyValue, element, ctx);
  }
}


/****/


internal abstract class NewTypeDeserializer<TDataFieldInfo> : IElementDeserializer where TDataFieldInfo : notnull
{
  public abstract bool AcceptsType(Type type);

  protected abstract object? DeserializeDataField(TDataFieldInfo dataField, XElement element, IXmlContext ctx);
  protected abstract IEnumerable<TDataFieldInfo> GetTypeDataFields(Type type);
  protected abstract object CreateInstance(Type targetType, IDictionary<TDataFieldInfo, object?> deserializedValues);

  public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
  {
    IEnumerable<TDataFieldInfo> dataFields = GetTypeDataFields(targetType);
    Dictionary<TDataFieldInfo, object?> deserializedValues = [];
    foreach (TDataFieldInfo dataField in dataFields)
    {
      object? deserializedValue = DeserializeDataField(dataField, element, ctx);
      deserializedValues[dataField] = deserializedValue;
    }
    object ret = CreateInstance(targetType, deserializedValues);
    return ret;
  }
}

public interface IPropertyDeserializer
{
  object? DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx);
}

internal class NewTypeByPropertyDeserializer : NewTypeDeserializer<PropertyInfo>
{
  public static readonly Func<Type, PropertyInfo[]> PUBLIC_INSTANCE_PROPERTIES_PROVIDER = q => q.GetProperties(BindingFlags.Public | BindingFlags.Instance);
  public static readonly Action<object, PropertyInfo, object?> PROPERTY_VALUE_WRITER = (obj, prop, val) => prop.SetValue(obj, val);

  private Func<Type, PropertyInfo[]> propertiesProvider = PUBLIC_INSTANCE_PROPERTIES_PROVIDER;
  private Action<object, PropertyInfo, object?> propertyValueUpdater = PROPERTY_VALUE_WRITER; //TODO naming convention
  private IPropertyDeserializer propertyDeserializer = new SimplePropertyAsElement();
  private readonly Dictionary<PropertyInfo, IPropertyDeserializer> propertyDeserializers = [];
  private IInstanceFactory<PropertyInfo> instanceFactory = new PublicParameterlessConstructorInstanceFromPropertiesFactory();
  private readonly Dictionary<Type, IInstanceFactory<PropertyInfo>> instanceFactoriesByType = [];

  private Func<Type, bool> acceptsTypePredicate = q => false;

  public override bool AcceptsType(Type type) => acceptsTypePredicate(type);

  public NewTypeByPropertyDeserializer WithAcceptedType(Type type, bool acceptDerivedTypes = false)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    if (acceptDerivedTypes)
      return this.WithAcceptedType(q => type.IsAssignableFrom(q));
    else
      return this.WithAcceptedType(q => q == type);
  }
  public NewTypeByPropertyDeserializer WithAcceptedType<T>(bool acceptDerivedTypes = false)
  {
    return this.WithAcceptedType(typeof(T), acceptDerivedTypes);
  }
  public NewTypeByPropertyDeserializer WithAcceptedType(Func<Type, bool> predicate)
  {
    EAssert.Argument.IsNotNull(predicate, nameof(predicate));
    var tmp = this.acceptsTypePredicate;
    this.acceptsTypePredicate = q => predicate(q) || tmp(q);
    return this;
  }

  public NewTypeByPropertyDeserializer WithPropertiesProvider(Func<Type, PropertyInfo[]> propertiesProvider)
  {
    this.propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
    return this;
  }
  public NewTypeByPropertyDeserializer WithPropertyValueUpdater(Action<object, PropertyInfo, object?> propertyValueProvider)
  {
    this.propertyValueUpdater = propertyValueProvider ?? throw new ArgumentNullException(nameof(propertyValueProvider));
    return this;
  }
  public NewTypeByPropertyDeserializer WithPropertyDeserializer(IPropertyDeserializer propertySerializer)
  {
    this.propertyDeserializer = propertySerializer ?? throw new ArgumentNullException(nameof(propertySerializer));
    return this;
  }
  public NewTypeByPropertyDeserializer WithPropertyDeserializerFor(PropertyInfo propertyInfo, IPropertyDeserializer propertyDeserializer)
  {
    EAssert.Argument.IsNotNull(propertyInfo, nameof(propertyInfo));
    EAssert.Argument.IsNotNull(propertyDeserializer, nameof(propertyDeserializer));
    this.propertyDeserializers[propertyInfo] = propertyDeserializer;
    return this;
  }
  public NewTypeByPropertyDeserializer WithPropertyDeserializerFor<T>(Expression<Func<T, object?>> propertyExpression, IPropertyDeserializer propertyDeserializer)
  {
    EAssert.Argument.IsNotNull(propertyExpression, nameof(propertyExpression));
    EAssert.Argument.IsNotNull(propertyDeserializer, nameof(propertyDeserializer));
    PropertyInfo propertyInfo = ExtractPropertyInfo(propertyExpression);
    return this.WithPropertyDeserializerFor(propertyInfo, propertyDeserializer);
  }

  public NewTypeByPropertyDeserializer WithInstanceFactory(IInstanceFactory<PropertyInfo> instanceFactory)
  {
    this.instanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
    return this;
  }
  public NewTypeByPropertyDeserializer WithInstanceFactoryFor(Type type, IInstanceFactory<PropertyInfo> instanceFactory)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    EAssert.Argument.IsNotNull(instanceFactory, nameof(instanceFactory));
    this.instanceFactoriesByType[type] = instanceFactory;
    return this;
  }

  private static PropertyInfo ExtractPropertyInfo<T>(Expression<Func<T, object?>> propertySelector)
  {
    if (propertySelector is null)
      throw new ArgumentNullException(nameof(propertySelector));

    Expression body = propertySelector.Body;

    // ošetření boxing / castu na object
    if (body is UnaryExpression unary &&
        unary.NodeType == ExpressionType.Convert)
    {
      body = unary.Operand;
    }

    if (body is not MemberExpression member)
      throw new ArgumentException(
        "Expression must select a property.",
        nameof(propertySelector));

    if (member.Member is not PropertyInfo property)
      throw new ArgumentException(
        "Expression does not refer to a property.",
        nameof(propertySelector));

    return property;
  }

  protected override IEnumerable<PropertyInfo> GetTypeDataFields(Type type) => propertiesProvider(type);

  protected override object? DeserializeDataField(PropertyInfo dataField, XElement element, IXmlContext ctx)
  {
    var pds = this.propertyDeserializers.TryGetValue(dataField, out IPropertyDeserializer? tmp)
      ? tmp
      : this.propertyDeserializer;
    object? deserializedValue = pds.DeserializeProperty(dataField, element, ctx);
    return deserializedValue;
  }

  protected override object CreateInstance(Type targetType, IDictionary<PropertyInfo, object?> deserializedValues)
  {
    IInstanceFactory<PropertyInfo> factory = this.instanceFactoriesByType.TryGetValue(targetType, out IInstanceFactory<PropertyInfo>? tmp)
      ? tmp
      : this.instanceFactory;
    object ret = factory.CreateInstance(targetType, deserializedValues);
    return ret;
  }
}