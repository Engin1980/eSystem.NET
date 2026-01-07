using ESystem.Exceptions;
using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

public class SimplePropertyAsElement : IPropertySerializer, IPropertyDeserializer
{
  public enum MissingPropertyElementBehavior
  {
    ThrowException,
    Ignore
  }

  private MissingPropertyElementBehavior missingPropertyElementBehavior = MissingPropertyElementBehavior.Ignore;

  public SimplePropertyAsElement WithMissingPropertyElementBehavior(MissingPropertyElementBehavior behavior)
  {
    missingPropertyElementBehavior = behavior;
    return this;
  }

  public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
  {
    DeserializationResult ret;

    XElement? propertyElement = element.Element(XName.Get(propertyInfo.Name));
    if (propertyElement == null)
    {
      ret = missingPropertyElementBehavior switch
      {
        MissingPropertyElementBehavior.Ignore => DeserializationResult.NoResult(),
        MissingPropertyElementBehavior.ThrowException => throw new InvalidOperationException($"Cannot find element for property '{propertyInfo.Name}' in element '{element.Name}'."),
        _ => throw new UnexpectedEnumValueException(missingPropertyElementBehavior),
      };
    }
    else
    {
      var deserializer = ctx.ElementDeserializers.GetByType(propertyInfo.PropertyType);
      object? tmp = deserializer.Deserialize(propertyElement, propertyInfo.PropertyType, ctx); //TODO: ctx.DeserializeFromElement(element, instanceType, deserializer);
      ret = tmp == null ? DeserializationResult.Null() : DeserializationResult.ValueResult(tmp);
    }

    return ret;
  }

  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
  {
    var ser = ctx.ElementSerializers.GetByType(propertyInfo.PropertyType);
    XElement propElement = new(propertyInfo.Name);
    ctx.SerializeToElement(propertyValue, propertyInfo.PropertyType, propElement, ser);
    element.Add(propElement);
  }
}




