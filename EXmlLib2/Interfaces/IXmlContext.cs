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
    /// <summary>
    /// Name of attribute where the original data type is stored
    /// </summary>
    string TypeNameAttribute { get; }
    /// <summary>
    /// Name of element used to store items of an enumerable when no specific name is provided
    /// </summary>
    string? DefaultItemXmlName { get; }

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
