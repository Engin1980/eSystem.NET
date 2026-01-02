using EXmlLib2.Abstractions.Interfaces;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedAttributeSerializer<T>(DerivedTypesBehavior derivedTypesBehavior = DerivedTypesBehavior.ExactTypeOnly) :
    TypedBase<T>(derivedTypesBehavior), IAttributeSerializer
  {
    public string Serialize(object? value, IXmlContext ctx)
    {
      CheckTypeSanity(value?.GetType());
      T typedValue = (T)value!;
      return Serialize(typedValue, ctx);
    }
    public abstract string Serialize(T value, IXmlContext ctx);
  }
}
