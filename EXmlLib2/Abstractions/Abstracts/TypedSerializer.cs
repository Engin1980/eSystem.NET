using EXmlLib2.Abstractions.Interfaces;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedSerializer<T> : IElementSerializer, IAttributeSerializer
  {
    public bool AcceptsType(Type type) => type == typeof(T);
    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      T typedValue = ConvertToTypedObject(value);
      Serialize(typedValue, element, ctx);
    }
    public string Serialize(object? value, IXmlContext ctx)
    {
      T typedValue = ConvertToTypedObject(value);
      return Serialize(typedValue, ctx);
    }
    protected abstract void Serialize(T value, XElement element, IXmlContext ctx);
    protected abstract string Serialize(T value, IXmlContext ctx);

    private static T ConvertToTypedObject(object? value)
    {
      if (value == null || value.GetType() != typeof(T))
        throw new ArgumentException($"Value type ({value?.GetType()?.Name ?? "null"}) does not match serializer type ({typeof(T).Name})", nameof(value));
      T typedValue = (T)value!;
      return typedValue;
    }
  }
}
