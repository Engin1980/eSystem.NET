using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased;

public class NewTypeByPropertySerializer : NewTypeSerializer
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
      return WithAcceptedType(q => type.IsAssignableFrom(q));
    else
      return WithAcceptedType(q => q == type);
  }
  public NewTypeByPropertySerializer WithAcceptedType<T>(bool acceptDerivedTypes = false)
  {
    return WithAcceptedType(typeof(T), acceptDerivedTypes);
  }
  public NewTypeByPropertySerializer WithAcceptedType(Func<Type, bool> predicate)
  {
    EAssert.Argument.IsNotNull(predicate, nameof(predicate));
    var tmp = acceptsTypePredicate;
    acceptsTypePredicate = q => predicate(q) || tmp(q);
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
    propertySerializers[propertyInfo] = propertySerializer;
    return this;
  }
  public NewTypeByPropertySerializer WithPropertySerializerFor<T>(Expression<Func<T, object?>> propertyExpression, IPropertySerializer propertySerializer)
  {
    EAssert.Argument.IsNotNull(propertyExpression, nameof(propertyExpression));
    EAssert.Argument.IsNotNull(propertySerializer, nameof(propertySerializer));
    PropertyInfo propertyInfo = ExtractPropertyInfo(propertyExpression);
    return WithPropertySerializerFor(propertyInfo, propertySerializer);
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

  protected override IEnumerable<string> GetDataMemberNames(Type type)
  {
    var props = GetPropertiesForType(type);
    var ret = props.Select(q => q.Name).ToList();
    return ret;
  }

  private PropertyInfo[] GetPropertiesForType(Type type)
  {
    return propertiesProvider(type);
  }

  protected object? GetPropertyValue(object value, PropertyInfo propertyInfo) => propertyValueProvider(value, propertyInfo);

  protected override void SerializeDataField(object value, string propertyName, XElement element, IXmlContext ctx)
  {
    PropertyInfo propertyInfo = GetPropertiesForType(value.GetType()).First(q => q.Name == propertyName);
    object? propertyValue = GetPropertyValue(value, propertyInfo);
    IPropertySerializer psd = propertySerializers.TryGetValue(propertyInfo, out IPropertySerializer? tmp)
      ? tmp
      : propertySerializer;
    psd.SerializeProperty(propertyInfo, propertyValue, element, ctx);
  }
}


