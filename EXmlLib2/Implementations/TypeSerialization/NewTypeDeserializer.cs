using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization;

public abstract class NewTypeDeserializer : IElementDeserializer
{
  public abstract bool AcceptsType(Type type);

  protected abstract DeserializationResult DeserializeDataMember(Type targetType, string dataMemberName, XElement element, IXmlContext ctx);
  protected abstract IEnumerable<string> GetDataMemberNames(Type type);
  protected abstract object CreateInstance(Type targetType, Dictionary<string, object?> deserializedValues);

  public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
  {
    IEnumerable<string> dataMemberNames = GetDataMemberNames(targetType);
    Dictionary<string, object?> deserializedValues = [];
    foreach (string dataMemberName in dataMemberNames)
    {
      DeserializationResult tmp = DeserializeDataMember(targetType, dataMemberName, element, ctx);
      if (tmp.HasResult)
        deserializedValues[dataMemberName] = tmp.Value;
    }
    object ret = CreateInstance(targetType, deserializedValues);
    return ret;
  }
}
