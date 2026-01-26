using ESystem.Exceptions;
using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Reflection;
using System.Xml.Linq;
using static EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties.SimplePropertyAsElement;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

public class SimplePropertyAsAttribute : IPropertySerializer, IPropertyDeserializer
{
  private MissingPropertyElementBehavior missingPropertyElementBehavior = MissingPropertyElementBehavior.Ignore;
  private NameCaseMatching nameCaseMatching = NameCaseMatching.IgnoreCase;

  public SimplePropertyAsAttribute WithMissingPropertyElementBehavior(MissingPropertyElementBehavior behavior)
  {
    missingPropertyElementBehavior = behavior;
    return this;
  }

  public SimplePropertyAsAttribute WithNameCaseMatching(NameCaseMatching matching)
  {
    nameCaseMatching = matching;
    return this;
  }

  private XAttribute? GetAttributeByName(XElement element, string name)
  {
    XAttribute? ret;
    if (nameCaseMatching == NameCaseMatching.Exact)
    {
      ret = element.Attribute(XName.Get(name));
    }
    else
    {
      ret = element.Attributes().FirstOrDefault(q => string.Equals(q.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));
    }
    return ret;
  }

  public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
  {
    DeserializationResult ret;
    XAttribute? attribute = GetAttributeByName(element, propertyInfo.Name);
    if (attribute == null)
    {
      ret = missingPropertyElementBehavior switch
      {
        MissingPropertyElementBehavior.Ignore => DeserializationResult.NoResult(),
        MissingPropertyElementBehavior.ThrowException => throw new InvalidOperationException($"Cannot find attribute for property '{propertyInfo.Name}' in element '{element.Name}'."),
        _ => throw new UnexpectedEnumValueException(missingPropertyElementBehavior),
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
    XAttribute attr = ctx.SerializeToAttribute(propertyInfo.Name, propertyValue, propertyInfo.PropertyType, ser);
    element.Add(attr);
  }
}




