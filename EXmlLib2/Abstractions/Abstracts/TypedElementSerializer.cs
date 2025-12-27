using EXmlLib2.Abstractions.Interfaces;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedElementSerializer<T> : IElementSerializer
  {
    public bool AcceptsType(Type type) => type == typeof(T);
    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      if (value == null || value.GetType() != typeof(T))
        throw new ArgumentException($"Value type ({value?.GetType()?.Name ?? "null"}) does not match attribute serializer type ({typeof(T).Name})", nameof(value));
      T typedValue = (T)value!;
      Serialize(typedValue, element, ctx);
    }
    public abstract void Serialize(T value, XElement element, IXmlContext ctx);
  }
}
