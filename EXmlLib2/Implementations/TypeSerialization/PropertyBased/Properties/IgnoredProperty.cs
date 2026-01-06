using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

public class IgnoredProperty : IPropertySerializer, IPropertyDeserializer
{
  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
  {
    // no-op
  }

  public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
  {
    return DeserializationResult.NoResult();
  }
}




