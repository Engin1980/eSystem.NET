using EXmlLib2.Abstractions.Interfaces;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedAttributeSerializer<T> : TypedBase<T>, IAttributeSerializer
  {
    public string Serialize(object? value, Type expectedType, IXmlContext ctx)
    {
      CheckTypeSanity(value?.GetType());
      CheckTypeSanity(expectedType);
      T typedValue = (T)value!;
      return Serialize(typedValue, ctx);
    }
    public abstract string Serialize(T value,  IXmlContext ctx);
  }
}
