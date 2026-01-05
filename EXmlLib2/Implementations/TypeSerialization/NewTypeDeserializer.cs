using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization;

public abstract class NewTypeDeserializer<TDataFieldInfo> : IElementDeserializer where TDataFieldInfo : notnull
{
  public abstract bool AcceptsType(Type type);

  protected abstract DataFieldValueDictionary<TDataFieldInfo> CreateDataFieldValueDictionaryInstance();
  protected abstract DeserializationResult DeserializeDataField(TDataFieldInfo dataField, XElement element, IXmlContext ctx);
  protected abstract IEnumerable<TDataFieldInfo> GetTypeDataFields(Type type);
  protected abstract object CreateInstance(Type targetType, DataFieldValueDictionary<TDataFieldInfo> deserializedValues);

  public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
  {
    IEnumerable<TDataFieldInfo> dataFields = GetTypeDataFields(targetType);
    DataFieldValueDictionary<TDataFieldInfo> deserializedValues = CreateDataFieldValueDictionaryInstance();
    foreach (TDataFieldInfo dataField in dataFields)
    {
      DeserializationResult tmp = DeserializeDataField(dataField, element, ctx);
      if (tmp.HasResult)
        deserializedValues[dataField] = tmp.Value;
    }
    object ret = CreateInstance(targetType, deserializedValues);
    return ret;
  }
}
