using EXmlLib2.Abstractions.Interfaces;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedElementDeserializer<T>(DerivedTypesBehavior derivedTypesBehavior = DerivedTypesBehavior.ExactTypeOnly) : 
    TypedBase<T>(derivedTypesBehavior), IElementDeserializer
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
