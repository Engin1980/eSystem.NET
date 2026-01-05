using ESystem.Asserting;
using ESystem.Exceptions;
using ESystem.Logging;
using ESystem.Miscelaneous;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Types;
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
using static EXmlLib2.Abstractions.Abstracts.SimplePropertyAsElement;

namespace EXmlLib2.Abstractions.Abstracts;

public abstract class NewTypeSerializer<TDataFieldInfo> : IElementSerializer
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

  public enum MissingPropertyElementBehavior
  {
    ThrowException,
    Ignore
  }

  private MissingPropertyElementBehavior missingPropertyElementBehavior = MissingPropertyElementBehavior.Ignore;

  public SimplePropertyAsElement WithMissingPropertyElementBehavior(MissingPropertyElementBehavior behavior)
  {
    this.missingPropertyElementBehavior = behavior;
    return this;
  }

  public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
  {
    DeserializationResult ret;

    XElement? propertyElement = element.Element(XName.Get(propertyInfo.Name));
    if (propertyElement == null)
    {
      ret = this.missingPropertyElementBehavior switch
      {
        MissingPropertyElementBehavior.Ignore => DeserializationResult.NoResult(),
        MissingPropertyElementBehavior.ThrowException => throw new InvalidOperationException($"Cannot find element for property '{propertyInfo.Name}' in element '{element.Name}'."),
        _ => throw new UnexpectedEnumValueException(this.missingPropertyElementBehavior),
      };
    }
    else
    {
      Type instanceType = DetermineIntanceType(propertyElement, propertyInfo);
      var deserializer = ctx.ElementDeserializers.GetByType(instanceType);
      object? tmp = deserializer.Deserialize(propertyElement, instanceType, ctx); //TODO: ctx.DeserializeFromElement(element, instanceType, deserializer);
      ret = tmp == null ? DeserializationResult.Null() : DeserializationResult.ValueResult(tmp);
    }

    return ret;
  }

  private static Type DetermineIntanceType(XElement element, PropertyInfo propertyInfo)
  {
    Type ret;
    XAttribute? attribute = element.Attribute(XName.Get(TYPE_ATTRIBUTE));
    if (attribute != null)
    {
      string assemblyQualifiedTypeName = attribute.Value;
      ret = Type.GetType(assemblyQualifiedTypeName)
        ?? throw new InvalidOperationException($"Cannot determine instance type from assembly qualified name '{assemblyQualifiedTypeName}' for property '{propertyInfo.Name}'.");
    }
    else
      ret = propertyInfo.PropertyType;
    return ret;
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
public class SimplePropertyAsAttribute : IPropertySerializer, IPropertyDeserializer
{
  private MissingPropertyElementBehavior missingPropertyElementBehavior = MissingPropertyElementBehavior.Ignore;

  public SimplePropertyAsAttribute WithMissingPropertyElementBehavior(MissingPropertyElementBehavior behavior)
  {
    this.missingPropertyElementBehavior = behavior;
    return this;
  }

  public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
  {
    DeserializationResult ret;
    XAttribute? attribute = element.Attribute(XName.Get(propertyInfo.Name));
    if (attribute == null)
    {
      ret = this.missingPropertyElementBehavior switch
      {
        MissingPropertyElementBehavior.Ignore => DeserializationResult.NoResult(),
        MissingPropertyElementBehavior.ThrowException => throw new InvalidOperationException($"Cannot find attribute for property '{propertyInfo.Name}' in element '{element.Name}'."),
        _ => throw new UnexpectedEnumValueException(this.missingPropertyElementBehavior),
      };
    }
    else
    {
      var deserializer = ctx.AttributeDeserializers.GetByType(propertyInfo.PropertyType);
      object? tmp = deserializer.Deserialize(attribute.Value, propertyInfo.PropertyType, ctx); //ctx.DeserializeFromAttribute(attribute, propertyInfo.PropertyType, deserializer);
      ret = tmp == null ? DeserializationResult.Null() : DeserializationResult.ValueResult(tmp);
    }

    return ret;
  }

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
public class PolymorphicNamePropertyAsElement : IPropertySerializer
{
  public enum MissingDefinitionBehavior
  {
    UseDefault,
    ThrowException
  }
  private readonly BiDictionary<Type, string> typeToXmlNameMapping = new();
  private MissingDefinitionBehavior missingDefinitionBehavior = MissingDefinitionBehavior.UseDefault;

  public PolymorphicNamePropertyAsElement WithMissingDefinitionBehavior(MissingDefinitionBehavior behavior)
  {
    this.missingDefinitionBehavior = behavior;
    return this;
  }
  public PolymorphicNamePropertyAsElement With(Type type, string xmlName)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    EAssert.Argument.IsNonEmptyString(xmlName, nameof(xmlName));
    typeToXmlNameMapping.Set(type, xmlName);
    return this;
  }
  public PolymorphicNamePropertyAsElement With<T>(string xmlName)
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
  object CreateInstance(Type targetType, DataFieldValueDictionary<TDataFieldInfo> deserializedValues);
}
public class PublicParameterlessConstructorInstanceFromPropertiesFactory : IInstanceFactory<PropertyInfo>
{
  public object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> deserializedValues)
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

public class UniversalTypeFactory : IInstanceFactory<PropertyInfo>
{
  private enum TypeKind
  {
    Class,
    Struct,
    RecordClass,
    RecordStruct
  }

  public object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> propertyValues)
  {
    TypeKind typeKind = GetTypeKind(targetType);
    object ret = typeKind switch
    {
      TypeKind.RecordClass => CreateAndInitStructOrRecord(targetType, true, propertyValues),
      TypeKind.RecordStruct => CreateAndInitStructOrRecord(targetType, true, propertyValues),
      TypeKind.Struct => CreateAndInitStructOrRecord(targetType, false, propertyValues),
      TypeKind.Class => CreateAndInitClass(targetType, propertyValues),
      _ => throw new UnexpectedEnumValueException(typeKind)
    };
    return ret;
  }

  private object CreateAndInitStructOrRecord(Type type, bool isForRecords, DataFieldValueDictionary<PropertyInfo> propertyValues)
  {
    if (!isForRecords)
      EAssert.IsTrue(type.IsValueType, $"Generic type T must be struct (provided '{type}').");

    var ctors = type.GetConstructors();
    EAssert.IsTrue(ctors.Length > 0, $"Generic type '{type}' must have public constructor.");

    // here we are looking for .ctor with best match of parameters against properties
    ConstructorInfo? bestCtor = null;
    object?[]? bestArgs = null;
    int bestScore = -1;

    foreach (var ctor in ctors)
    {
      var parameters = ctor.GetParameters();
      var args = new object?[parameters.Length];
      int score = 0;
      bool usable = true;

      for (int i = 0; i < parameters.Length; i++)
      {
        var param = parameters[i];
        var prop = propertyValues.Keys.FirstOrDefault(p => string.Equals(p.Name, param.Name, StringComparison.OrdinalIgnoreCase));

        if (prop != null)
        {
          var value = propertyValues[prop];

          if (value != null && !param.ParameterType.IsInstanceOfType(value))
          {
            try
            {
              value = Convert.ChangeType(value, param.ParameterType);
            }
            catch
            {
              usable = false;
              break;
            }
          }

          args[i] = value;
          score++;
        }
        else if (param.HasDefaultValue)
          args[i] = param.DefaultValue;
        else if (param.IsOptional)
          args[i] = Type.Missing;
        else
        {
          usable = false;
          break;
        }
      }

      if (!usable) continue;

      if (score > bestScore)
      {
        bestScore = score;
        bestCtor = ctor;
        bestArgs = args;
      }
    }

    if (bestCtor == null || bestArgs == null)
      throw new InvalidOperationException($"No available constructor found to create an instance of {type}.");

    object ret = bestCtor.Invoke(bestArgs);
    return ret;
  }

  private object CreateAndInitClass(Type instanceType, DataFieldValueDictionary<PropertyInfo> propertyValues)
  {
    var pom = new PublicParameterlessConstructorInstanceFromPropertiesFactory();
    object ret = pom.CreateInstance(instanceType, propertyValues);
    return ret;
  }

  private static TypeKind GetTypeKind(Type type)
  {
    bool isRecord =
        type.GetProperty(
            "EqualityContract",
            BindingFlags.Instance | BindingFlags.NonPublic
        ) != null;

    if (isRecord)
      return type.IsValueType ? TypeKind.RecordStruct : TypeKind.RecordClass;

    return type.IsValueType ? TypeKind.Struct : TypeKind.Class;
  }
}

public class DelegatedInstanceFactory : IInstanceFactory<PropertyInfo>
{
  private readonly Func<Type, DataFieldValueDictionary<PropertyInfo>, object> factoryMethod;
  public DelegatedInstanceFactory(Func<Type, DataFieldValueDictionary<PropertyInfo>, object> factoryMethod)
  {
    this.factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
  }
  public object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> deserializedValues)
  {
    object ret = factoryMethod(targetType, deserializedValues);
    return ret;
  }
}

public class DelegatedInstanceFactory<T> : IInstanceFactory<PropertyInfo>
{
  private readonly Func<PropertyValuesDictionary<T>, object> factoryMethod;
  public DelegatedInstanceFactory(Func<PropertyValuesDictionary<T>, object> factoryMethod)
  {
    this.factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
  }
  public object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> deserializedValues)
  {
    EAssert.IsTrue(typeof(T) == targetType, $"DelegatedInstanceFactory<{typeof(T)}> unable to create instance of '{targetType}'.");
    PropertyValuesDictionary<T> tmp = new();
    deserializedValues.ToList().ForEach(kv => tmp[kv.Key] = kv.Value);
    object ret = factoryMethod(tmp);
    return ret;
  }
}

public class NewTypeByPropertySerializer : NewTypeSerializer<PropertyInfo>
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

public abstract class DataFieldValueDictionary<TDataFieldInfo> : Dictionary<TDataFieldInfo, object?> where TDataFieldInfo : notnull
{
  public abstract object? this[string name] { get; }
  public T? Get<T>(string name)
  {
    object? tmp = this[name];
    T? ret = (T?)tmp;
    return ret;
  }
}

public class PropertyInfoValueDictionary : DataFieldValueDictionary<PropertyInfo>
{
  public override object? this[string name] => this.First(q => q.Key.Name == name).Value;
}

public abstract class NewTypeDeserializer<TDataFieldInfo> : IElementDeserializer where TDataFieldInfo : notnull
{
  public abstract bool AcceptsType(Type type);

  protected abstract DataFieldValueDictionary<TDataFieldInfo> CreateDataFieldValueDictionaryInstance();
  protected abstract DeserializationResult DeserializeDataField(TDataFieldInfo dataField, XElement element, IXmlContext ctx);
  protected abstract IEnumerable<TDataFieldInfo> GetTypeDataFields(Type type);
  protected abstract object CreateInstance(Type targetType, DataFieldValueDictionary<TDataFieldInfo> deserializedValues);

  public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
  {
    IEnumerable<TDataFieldInfo> dataFields = GetTypeDataFields(targetType);
    DataFieldValueDictionary<TDataFieldInfo> deserializedValues = CreateDataFieldValueDictionaryInstance();
    foreach (TDataFieldInfo dataField in dataFields)
    {
      DeserializationResult tmp = DeserializeDataField(dataField, element, ctx);
      if (tmp.HasResult)
        deserializedValues[dataField] = tmp.Value;
    }
    object ret = CreateInstance(targetType, deserializedValues);
    return ret;
  }
}

public readonly struct DeserializationResult
{
  public bool HasResult { get; }
  public bool IsNull { get => Value == null; }
  public object? Value { get; }

  private DeserializationResult(bool hasResult, object? value)
  {
    HasResult = hasResult;
    Value = value;
  }

  public static DeserializationResult NoResult() => new(false, null);

  public static DeserializationResult Null() => new(true, null);

  public static DeserializationResult ValueResult(object value) => new(true, value);
}

public interface IPropertyDeserializer
{

  DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx);
}

public class NewTypeByPropertyDeserializer : NewTypeDeserializer<PropertyInfo>
{
  public static readonly Func<Type, PropertyInfo[]> PUBLIC_INSTANCE_PROPERTIES_PROVIDER = q => q.GetProperties(BindingFlags.Public | BindingFlags.Instance);
  public static readonly Action<object, PropertyInfo, object?> PROPERTY_VALUE_WRITER = (obj, prop, val) => prop.SetValue(obj, val);

  private Func<Type, PropertyInfo[]> propertiesProvider = PUBLIC_INSTANCE_PROPERTIES_PROVIDER;
  private Action<object, PropertyInfo, object?> propertyValueUpdater = PROPERTY_VALUE_WRITER; //TODO naming convention
  private IPropertyDeserializer propertyDeserializer = new SimplePropertyAsElement();
  private readonly Dictionary<PropertyInfo, IPropertyDeserializer> propertyDeserializers = [];
  private IInstanceFactory<PropertyInfo> instanceFactory = new UniversalTypeFactory();
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

  public NewTypeByPropertyDeserializer WithInstanceFactory(Func<Type, DataFieldValueDictionary<PropertyInfo>, object> factoryMethod)
  {
    this.instanceFactory = new DelegatedInstanceFactory(factoryMethod);
    return this;
  }
  public NewTypeByPropertyDeserializer WithInstanceFactory<T>(Func<PropertyValuesDictionary<T>, object> factoryMethod)
  {
    this.instanceFactory = new DelegatedInstanceFactory<T>(factoryMethod);
    return this;
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

  protected override DeserializationResult DeserializeDataField(PropertyInfo dataField, XElement element, IXmlContext ctx)
  {
    var pds = this.propertyDeserializers.TryGetValue(dataField, out IPropertyDeserializer? tmp)
      ? tmp
      : this.propertyDeserializer;
    DeserializationResult deserializedValue = pds.DeserializeProperty(dataField, element, ctx);
    return deserializedValue;
  }
  protected override DataFieldValueDictionary<PropertyInfo> CreateDataFieldValueDictionaryInstance() => new PropertyInfoValueDictionary();

  protected override object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> deserializedValues)
  {
    IInstanceFactory<PropertyInfo> factory = this.instanceFactoriesByType.TryGetValue(targetType, out IInstanceFactory<PropertyInfo>? tmp)
      ? tmp
      : this.instanceFactory;
    object ret = factory.CreateInstance(targetType, deserializedValues);
    return ret;
  }
}