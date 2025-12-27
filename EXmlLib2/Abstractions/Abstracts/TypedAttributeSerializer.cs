using EXmlLib2.Abstractions.Interfaces;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedAttributeSerializer<T> : IAttributeSerializer
  {
    public bool AcceptsType(Type type) => type == typeof(T);
    public string Serialize(object? value, IXmlContext ctx)
    {
      if (value == null || value.GetType() != typeof(T))
        throw new ArgumentException($"Value type ({value?.GetType()?.Name ?? "null"}) does not match attribute serializer type ({typeof(T).Name}).", nameof(value));
      T typedValue = (T)value!;
      return Serialize(typedValue, ctx);
    }
    public abstract string Serialize(T value, IXmlContext ctx);
  }
}
