using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Factories;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Factories;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Internal;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties.Abstractions;
using EXmlLib2.Types;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased;

public class ObjectSerializer : TypeSerializerBase
{
  public class TypeOptions<T>(ObjectSerializer parent)
  {
    public TypeOptions<T> WithPropertyDeserializer(Expression<Func<T, object?>> propertyExpression, IPropertySerializer propertySerializer)
    {
      EAssert.Argument.IsNotNull(propertyExpression, nameof(propertyExpression));
      EAssert.Argument.IsNotNull(propertySerializer, nameof(propertySerializer));
      PropertyInfo propertyInfo = ExtractPropertyInfo(propertyExpression);
      parent.WithPropertySerializerFor(propertyInfo, propertySerializer);
      return this;
    }

    public TypeOptions<T> WithIgnoredProperty(Expression<Func<T, object?>> propertyExpression) => this.WithPropertyDeserializer(propertyExpression, new IgnoredPropertySerialization());
  }

  public class DefaultOptions(ObjectSerializer parent)
  {
    public DefaultOptions WithPropertiesProvider(Func<Type, PropertyInfo[]> propertiesProvider)
    {
      parent.propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
      return this;
    }

    public DefaultOptions WithPropertySerializer(IPropertySerializer propertySerializer)
    {
      EAssert.Argument.IsNotNull(propertySerializer, nameof(propertySerializer));
      parent.defaultPropertySerializer = propertySerializer;
      return this;
    }

    public DefaultOptions WithPropertySerialization(PropertySerialization propertySerialization)
    {
      EAssert.Argument.IsNotNull(propertySerialization, nameof(propertySerialization));
      parent.defaultPropertySerializer = propertySerialization;
      return this;
    }

    public DefaultOptions WithPropertySerialization(Action<PropertySerialization> opts)
    {
      EAssert.Argument.IsNotNull(opts, nameof(opts));
      EAssert.IsTrue(parent.defaultPropertySerializer is PropertySerialization, "Default property serializer is not of type PropertySerialization.");
      opts((PropertySerialization) parent.defaultPropertySerializer);
      return this;
    }

    public DefaultOptions WithPropertyValueProvider(Func<object, PropertyInfo, object?> propertyValueProvider)
    {
      parent.propertyValueProvider = propertyValueProvider ?? throw new ArgumentNullException(nameof(propertyValueProvider));
      return this;
    }
  }

  public static readonly Func<object, PropertyInfo, object?> PROPERTY_VALUE_READER = (obj, prop) => prop.GetValue(obj);

  private Func<Type, PropertyInfo[]> propertiesProvider = PropertyProviders.PublicInstancePropertiesProvider;
  private Func<object, PropertyInfo, object?> propertyValueProvider = PropertyValueReaders.DefaultPropertyReader;
  private IPropertySerializer defaultPropertySerializer = new PropertySerialization()
    .WithXmlSourceOrder(XmlSourceOrder.AttributeFirst)
    .WithMissingXmlSourceBehavior(MissingPropertyXmlSourceBehavior.ThrowException);
  private readonly SmartPropertyInfoDictionary<IPropertySerializer> customPropertySerializers = new();
  private Func<Type, bool> acceptsTypePredicate = q => false;

  public override bool AcceptsType(Type type) => acceptsTypePredicate(type);

  public ObjectSerializer WithAcceptedType(Type type, bool acceptDerivedTypes = false)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    if (acceptDerivedTypes)
      return WithAcceptedType(q => type.IsAssignableFrom(q));
    else
      return WithAcceptedType(q => q == type);
  }
  public ObjectSerializer WithAcceptedType<T>(bool acceptDerivedTypes = false)
  {
    return WithAcceptedType(typeof(T), acceptDerivedTypes);
  }
  public ObjectSerializer WithAcceptedType(Func<Type, bool> predicate)
  {
    EAssert.Argument.IsNotNull(predicate, nameof(predicate));
    var tmp = acceptsTypePredicate;
    acceptsTypePredicate = q => predicate(q) || tmp(q);
    return this;
  }

  public ObjectSerializer WithTypeOptions<T>(Action<TypeOptions<T>> opts)
  {
    TypeOptions<T> typeOpts = new TypeOptions<T>(this);
    opts(typeOpts);
    return this;
  }

  public ObjectSerializer WithDefaultOptions(Action<DefaultOptions> opts)
  {
    DefaultOptions defaultOptions = new(this);
    opts(defaultOptions);
    return this;
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  public ObjectSerializer WithPropertySerializerFor(PropertyInfo propertyInfo, IPropertySerializer propertySerializer)
  {
    EAssert.Argument.IsNotNull(propertyInfo, nameof(propertyInfo));
    EAssert.Argument.IsNotNull(propertySerializer, nameof(propertySerializer));
    customPropertySerializers.Put(propertyInfo, propertySerializer);
    return this;
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
    var props = propertiesProvider(type);
    var ret = props.Select(q => q.Name).ToList();
    return ret;
  }

  protected object? GetPropertyValue(object value, PropertyInfo propertyInfo) => propertyValueProvider(value, propertyInfo);

  protected override void SerializeDataMember(object value, string propertyName, XElement element, IXmlContext ctx)
  {
    PropertyInfo propertyInfo = propertiesProvider(value.GetType()).First(q => q.Name == propertyName);
    object? propertyValue = GetPropertyValue(value, propertyInfo);
    IPropertySerializer psd = customPropertySerializers.TryGet(propertyInfo) ?? defaultPropertySerializer;
    psd.SerializeProperty(propertyInfo, propertyValue, element, ctx);
  }
}


