using EXmlLib2.Abstractions.Interfaces;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedElementDeserializer<T> : TypedBase<T>, IElementDeserializer
  {
    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
    {
      CheckTypeSanity(targetType);
      T ret = Deserialize(element, ctx);
      return ret;
    }
    public abstract T Deserialize(XElement element, IXmlContext ctx);
  }
}
