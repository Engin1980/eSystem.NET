using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.TypeSerialization.AnyBased;
using EXmlLib2.Implementations.TypeSerialization.Factories;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization;

public class SimpleRecordDeserializer<T> : NewTypeDeserializer
{
  private Func<Type, bool> typeAccepter = q => q == typeof(T);
  private readonly Dictionary<PropertyInfo, Func<object?>> optionalProperties = [];
  private readonly Func<Type, PropertyInfo[]> propertiesProvider = TypeFromAnyDeserializer.PUBLIC_INSTANCE_PROPERTIES_PROVIDER;
  private readonly IPropertyDeserializer propertyDeserializer = new PropertyFromAnyDeserializer()
    .WithMissingPropertyElementBehavior(MissingPropertyElementBehavior.Ignore);

  public override bool AcceptsType(Type type) => typeAccepter(type);

  public SimpleRecordDeserializer<T> WithOptionalProperty<V>(Expression<Func<T, V?>> propertyExpression)
    => WithOptionalProperty(propertyExpression, () => default);

  public SimpleRecordDeserializer<T> WithOptionalProperty<V>(
    Expression<Func<T, V?>> propertyExpression, Func<V?> defaultValueProvider)
  {
    PropertyInfo propertyInfo = ExtractPropertyInfo(propertyExpression);
    optionalProperties[propertyInfo] = () => defaultValueProvider();
    return this;
  }


  private static PropertyInfo ExtractPropertyInfo<V>(Expression<Func<T, V?>> propertySelector)
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
    var pds = this.propertyDeserializer;
    DeserializationResult deserializedValue = pds.DeserializeProperty(pi, element, ctx);
    if (deserializedValue.HasResult == false)
    {
      if (optionalProperties.TryGetValue(pi, out Func<object?>? defaultValueProvider))
      {
        object? defaultValue = defaultValueProvider();
        deserializedValue = DeserializationResult.Result(defaultValue);
      }
      else
        throw new Exception("Missing required property value for '" + dataMemberName + "'.");
    }
    return deserializedValue;
  }

  protected override object CreateInstance(Type targetType, Dictionary<string, object?> deserializedValues)
  {
    var ctor = typeof(T).GetConstructors().First();
    var parameters = ctor.GetParameters();
    object?[] args = new object?[parameters.Length];
    for (int i = 0; i < parameters.Length; i++)
    {
      var param = parameters[i];
      var propName = deserializedValues.Keys.FirstOrDefault(p => string.Equals(p, param.Name, StringComparison.OrdinalIgnoreCase));
      if (propName != null)
        args[i] = deserializedValues[propName];
      else
        throw new Exception("Missing value for constructor parameter '" + param.Name + "'.");
    }

    object ret = ctor.Invoke(args);
    return ret;
  }
}
