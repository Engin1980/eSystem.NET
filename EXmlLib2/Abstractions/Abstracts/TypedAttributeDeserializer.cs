using EXmlLib2.Abstractions.Interfaces;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedAttributeDeserializer<T>(DerivedTypesBehavior derivedTypesBehavior = DerivedTypesBehavior.ExactTypeOnly) :
    TypedBase<T>(derivedTypesBehavior), IAttributeDeserializer
  {
    public object? Deserialize(string value, Type targetType, IXmlContext ctx)
    {
      CheckTypeSanity(targetType);
      T tmp = Deserialize(value, ctx);
      return tmp;
    }
    public abstract T Deserialize(string value, IXmlContext ctx);
  }
}
