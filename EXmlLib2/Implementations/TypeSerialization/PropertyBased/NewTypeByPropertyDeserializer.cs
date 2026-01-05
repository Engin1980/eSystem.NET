using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Factories;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using EXmlLib2.Implementations.TypeSerialization.Properties;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;
using EXmlLib2.Types;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased;

public class NewTypeByPropertyDeserializer : NewTypeDeserializer<PropertyInfo>
{
  private class PropertyInfoValueDictionary : DataFieldValueDictionary<PropertyInfo>
  {
    public override object? this[string name] => this.First(q => q.Key.Name == name).Value;
  }

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
      return WithAcceptedType(q => type.IsAssignableFrom(q));
    else
      return WithAcceptedType(q => q == type);
  }
  public NewTypeByPropertyDeserializer WithAcceptedType<T>(bool acceptDerivedTypes = false)
  {
    return WithAcceptedType(typeof(T), acceptDerivedTypes);
  }
  public NewTypeByPropertyDeserializer WithAcceptedType(Func<Type, bool> predicate)
  {
    EAssert.Argument.IsNotNull(predicate, nameof(predicate));
    var tmp = acceptsTypePredicate;
    acceptsTypePredicate = q => predicate(q) || tmp(q);
    return this;
  }

  public NewTypeByPropertyDeserializer WithPropertiesProvider(Func<Type, PropertyInfo[]> propertiesProvider)
  {
    this.propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
    return this;
  }
  public NewTypeByPropertyDeserializer WithPropertyValueUpdater(Action<object, PropertyInfo, object?> propertyValueProvider)
  {
    propertyValueUpdater = propertyValueProvider ?? throw new ArgumentNullException(nameof(propertyValueProvider));
    return this;
  }
  public NewTypeByPropertyDeserializer WithPropertyDeserializer(IPropertyDeserializer propertySerializer)
  {
    propertyDeserializer = propertySerializer ?? throw new ArgumentNullException(nameof(propertySerializer));
    return this;
  }
  public NewTypeByPropertyDeserializer WithPropertyDeserializerFor(PropertyInfo propertyInfo, IPropertyDeserializer propertyDeserializer)
  {
    EAssert.Argument.IsNotNull(propertyInfo, nameof(propertyInfo));
    EAssert.Argument.IsNotNull(propertyDeserializer, nameof(propertyDeserializer));
    propertyDeserializers[propertyInfo] = propertyDeserializer;
    return this;
  }
  public NewTypeByPropertyDeserializer WithPropertyDeserializerFor<T>(Expression<Func<T, object?>> propertyExpression, IPropertyDeserializer propertyDeserializer)
  {
    EAssert.Argument.IsNotNull(propertyExpression, nameof(propertyExpression));
    EAssert.Argument.IsNotNull(propertyDeserializer, nameof(propertyDeserializer));
    PropertyInfo propertyInfo = ExtractPropertyInfo(propertyExpression);
    return WithPropertyDeserializerFor(propertyInfo, propertyDeserializer);
  }

  public NewTypeByPropertyDeserializer WithInstanceFactory(Func<Type, DataFieldValueDictionary<PropertyInfo>, object> factoryMethod)
  {
    instanceFactory = new DelegatedInstanceFactory(factoryMethod);
    return this;
  }
  public NewTypeByPropertyDeserializer WithInstanceFactory<T>(Func<PropertyValuesDictionary<T>, object> factoryMethod)
  {
    instanceFactory = new DelegatedInstanceFactory<T>(factoryMethod);
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
    instanceFactoriesByType[type] = instanceFactory;
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
    var pds = propertyDeserializers.TryGetValue(dataField, out IPropertyDeserializer? tmp)
      ? tmp
      : propertyDeserializer;
    DeserializationResult deserializedValue = pds.DeserializeProperty(dataField, element, ctx);
    return deserializedValue;
  }
  protected override DataFieldValueDictionary<PropertyInfo> CreateDataFieldValueDictionaryInstance() => new PropertyInfoValueDictionary();

  protected override object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> deserializedValues)
  {
    IInstanceFactory<PropertyInfo> factory = instanceFactoriesByType.TryGetValue(targetType, out IInstanceFactory<PropertyInfo>? tmp)
      ? tmp
      : instanceFactory;
    object ret = factory.CreateInstance(targetType, deserializedValues);
    return ret;
  }
}