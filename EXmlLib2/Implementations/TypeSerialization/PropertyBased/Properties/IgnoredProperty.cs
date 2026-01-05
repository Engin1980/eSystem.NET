using EXmlLib2.Abstractions;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

public class IgnoredProperty : IPropertySerializer
{
  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
  {
    // no-op
  }
}




