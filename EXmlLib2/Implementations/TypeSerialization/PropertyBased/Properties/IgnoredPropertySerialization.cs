using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties.Abstractions;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

public class IgnoredPropertySerialization : IPropertySerializer, IPropertyDeserializer
{
  private readonly Dictionary<Type, Func<object?>> defaultValueFactories = [];

  public IgnoredPropertySerialization WithDefaultValue(Type type, Func<object?> defaultValueFactory)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    EAssert.Argument.IsNotNull(defaultValueFactory, nameof(defaultValueFactory));
    defaultValueFactories[type] = defaultValueFactory;
    return this;
  }

  public IgnoredPropertySerialization WithDefaultValue<T>(Func<T?> defaultValueFactory)
  {
    EAssert.Argument.IsNotNull(defaultValueFactory, nameof(defaultValueFactory));
    return WithDefaultValue(typeof(T), () => defaultValueFactory());
  }

  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
  {
    // no-op
  }

  public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
  {
    DeserializationResult ret;
    ret = defaultValueFactories.TryGetValue(propertyInfo.PropertyType, out var factory)
      ? DeserializationResult.From(factory())
      : DeserializationResult.FromNoResult();
    return ret;
  }
}




