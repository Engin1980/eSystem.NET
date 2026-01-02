using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Types;
using EXmlLib2.Types.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions
{
  public interface IXmlContext
  {
    public interface IByTypeSelectableList<T> where T : ISelectableByType
    {
      void Push(T item);
      T GetByType(Type type);
    }

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
    public SerializerDeserializerRegistry<IElementSerializer> ElementSerializers { get; }
    public SerializerDeserializerRegistry<IAttributeSerializer> AttributeSerializers { get; }
    public SerializerDeserializerRegistry<IElementDeserializer> ElementDeserializers { get; }
    public SerializerDeserializerRegistry<IAttributeDeserializer> AttributeDeserializers { get; }

    void SetData<T>(string key, T data);
    T? GetData<T>(string key);
    T GetOrSetData<T>(string key, Func<T> newDataProvider);
    void SerializeToElement(object? value, XElement elm, IElementSerializer serializer);
  }
}
