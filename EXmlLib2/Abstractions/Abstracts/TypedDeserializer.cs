using EXmlLib2.Abstractions.Interfaces;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedDeserializer<T> : IElementDeserializer, IAttributeDeserializer
  {
    public bool AcceptsType(Type type) => typeof(T) == type;
    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
    {
      T ret = Deserialize(element, ctx);
      return ret;
    }
    public abstract T Deserialize(XElement element, IXmlContext ctx);
    public object? Deserialize(string value, Type targetType, IXmlContext ctx)
    {
      T tmp = Deserialize(value, ctx);
      return tmp;
    }
    public abstract T Deserialize(string value, IXmlContext ctx);
  }
}
