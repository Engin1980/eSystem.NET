using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Factories;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Factories;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Internal;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;
using EXmlLib2.Types;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.AnyBased;

public class TypeFromAnyDeserializer : NewTypeDeserializer
{
  public class TypeOptions<T>(TypeFromAnyDeserializer parent)
  {
    public TypeOptions<T> WithPropertyDeserializer(Expression<Func<T, object?>> propertyExpression, IPropertyDeserializer propertyDeserializer)
    {
      EAssert.Argument.IsNotNull(propertyExpression, nameof(propertyExpression));
      EAssert.Argument.IsNotNull(propertyDeserializer, nameof(propertyDeserializer));
      PropertyInfo propertyInfo = ExtractPropertyInfo(propertyExpression);
      parent.WithPropertyDeserializerFor(propertyInfo, propertyDeserializer);
      return this;
    }

    public TypeOptions<T> WithIgnoredProperty(Expression<Func<T, object?>> propertyExpression) 
      => this.WithPropertyDeserializer(propertyExpression, new IgnoredProperty());

    public TypeOptions<T> WithOptionalProperty(Expression<Func<T, object?>> propertyExpression)
    {
      EAssert.Argument.IsNotNull(propertyExpression, nameof(propertyExpression));
      PropertyInfo propertyInfo = ExtractPropertyInfo(propertyExpression);
      parent.WithPropertyDeserializerFor(propertyInfo, new PropertyFromAnyDeserializer().WithMissingPropertyElementBehavior(MissingPropertyElementBehavior.ReturnNull));
      return this;
    }

    public TypeOptions<T> WithInstanceFactory(Func<PropertyValuesDictionary<T>, object> factoryMethod)
    {
      parent.defaultInstanceFactory = new DelegatedInstanceFactory<T>(factoryMethod);
      return this;
    }
  }

  public class DefaultOptions(TypeFromAnyDeserializer parent)
  {
    public DefaultOptions WithPropertiesProvider(Func<Type, PropertyInfo[]> propertiesProvider)
    {
      parent.propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
      return this;
    }

    public DefaultOptions WithPropertyDeserializer(IPropertyDeserializer propertySerializer)
    {
      parent.defaultPropertyDeserializer = propertySerializer ?? throw new ArgumentNullException(nameof(propertySerializer));
      return this;
    }

    public DefaultOptions WithInstanceFactory(IInstanceFactory instanceFactory)
    {
      parent.defaultInstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
      return this;
    }
  }

  public static readonly Func<Type, PropertyInfo[]> PUBLIC_INSTANCE_PROPERTIES_PROVIDER = q => q.GetProperties(BindingFlags.Public | BindingFlags.Instance);
  private Func<Type, PropertyInfo[]> propertiesProvider = PUBLIC_INSTANCE_PROPERTIES_PROVIDER;

  private IPropertyDeserializer defaultPropertyDeserializer = new PropertyFromAnyDeserializer();
  private readonly SmartPropertyInfoDictionary<IPropertyDeserializer> propertyDeserializers = new();

  private IInstanceFactory defaultInstanceFactory = new UniversalTypeFactory();
  private readonly Dictionary<Type, IInstanceFactory> instanceFactories = [];
  private Func<Type, bool> acceptsTypePredicate = q => false;

  public override bool AcceptsType(Type type) => acceptsTypePredicate(type);

  public TypeFromAnyDeserializer WithAcceptedType(Type type, bool acceptDerivedTypes = false)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    if (acceptDerivedTypes)
      return WithAcceptedType(q => type.IsAssignableFrom(q));
    else
      return WithAcceptedType(q => q == type);
  }
  public TypeFromAnyDeserializer WithAcceptedType<T>(bool acceptDerivedTypes = false)
  {
    return WithAcceptedType(typeof(T), acceptDerivedTypes);
  }
  public TypeFromAnyDeserializer WithAcceptedType(Func<Type, bool> predicate)
  {
    EAssert.Argument.IsNotNull(predicate, nameof(predicate));
    var tmp = acceptsTypePredicate;
    acceptsTypePredicate = q => predicate(q) || tmp(q);
    return this;
  }

  public TypeFromAnyDeserializer WithTypeOptions<T>(Action<TypeOptions<T>> opts)
  {
    TypeOptions<T> typeOpts = new TypeOptions<T>(this);
    opts(typeOpts);
    return this;
  }

  public TypeFromAnyDeserializer WithDefaultOptions(Action<DefaultOptions> opts)
  {
    DefaultOptions defaultOptions = new(this);
    opts(defaultOptions);
    return this;
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  public TypeFromAnyDeserializer WithPropertyDeserializerFor(PropertyInfo propertyInfo, IPropertyDeserializer propertyDeserializer)
  {
    EAssert.Argument.IsNotNull(propertyInfo, nameof(propertyInfo));
    EAssert.Argument.IsNotNull(propertyDeserializer, nameof(propertyDeserializer));
    propertyDeserializers.Put(propertyInfo, propertyDeserializer);
    return this;
  }

  public TypeFromAnyDeserializer WithInstanceFactory(Func<Type, Dictionary<string, object?>, object> factoryMethod)
  {
    defaultInstanceFactory = new DelegatedInstanceFactory(factoryMethod);
    return this;
  }

  public TypeFromAnyDeserializer WithInstanceFactoryFor(Type type, IInstanceFactory instanceFactory)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    EAssert.Argument.IsNotNull(instanceFactory, nameof(instanceFactory));
    instanceFactories[type] = instanceFactory;
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

  protected override IEnumerable<string> GetDataMemberNames(Type type)
  {
    var props = propertiesProvider(type);
    var ret = props.Select(q => q.Name).ToList();
    return ret;
  }

  protected override DeserializationResult DeserializeDataMember(Type targetType, string dataMemberName, XElement element, IXmlContext ctx)
  {
    PropertyInfo pi = propertiesProvider(targetType).First(q => q.Name == dataMemberName);
    var pds = propertyDeserializers.TryGet(pi) ?? defaultPropertyDeserializer;
    DeserializationResult deserializedValue = pds.DeserializeProperty(pi, element, ctx);
    return deserializedValue;
  }

  protected override object CreateInstance(Type targetType, Dictionary<string, object?> deserializedValues)
  {
    IInstanceFactory factory = instanceFactories.TryGetValue(targetType, out IInstanceFactory? tmp)
      ? tmp
      : defaultInstanceFactory;
    object ret = factory.CreateInstance(targetType, deserializedValues);
    return ret;
  }
}