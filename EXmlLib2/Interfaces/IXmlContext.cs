using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Interfaces
{
  public interface IXmlContext
  {
    CultureInfo DefaultCultureInfo { get; }
    string DefaultNullString { get; }
    string DefaultTrueString { get; }
    string DefaultFalseString { get; }

    string TypeNameAttribute { get; }

    IAttributeDeserializer GetAttributeDeserializer(Type targetType);
    IAttributeDeserializer<T> GetAttributeDeserializer<T>();
    IElementDeserializer<T> GetElementDeserializer<T>();
    IElementDeserializer GetElementDeserializer(Type targetType);
    IAttributeSerializer<T> GetAttributeSerializer<T>(T? value);
    IAttributeSerializer GetAttributeSerializer(object? value);
    IElementSerializer<T> GetElementSerializer<T>();
    IElementSerializer GetElementSerializer(object? value);
    void SerializeToElement(object? value, XElement elm, IElementSerializer serializer);
  }
}
