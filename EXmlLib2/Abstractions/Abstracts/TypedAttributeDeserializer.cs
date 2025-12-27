using EXmlLib2.Abstractions.Interfaces;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedAttributeDeserializer<T> : IAttributeDeserializer
  {
    public bool AcceptsType(Type type) => typeof(T) == type;
    public object? Deserialize(string value, Type targetType, IXmlContext ctx)
    {
      T tmp = Deserialize(value, ctx);
      return tmp;
    }
    public abstract T Deserialize(string value, IXmlContext ctx);
  }
}
