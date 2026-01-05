using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

public interface IPropertyDeserializer
{
  DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx);
}
