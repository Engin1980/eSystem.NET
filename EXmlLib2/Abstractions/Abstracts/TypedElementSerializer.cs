using ESystem.Asserting;
using ESystem.Exceptions;
using EXmlLib2.Abstractions.Interfaces;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedElementSerializer<T>(DerivedTypesBehavior derivedTypesBehavior = DerivedTypesBehavior.ExactTypeOnly) : 
    TypedBase<T>(derivedTypesBehavior), IElementSerializer
  {
    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      CheckTypeSanity(value?.GetType());
      T typedValue = (T)value!;
      Serialize(typedValue, element, ctx);
    }

    public abstract void Serialize(T value, XElement element, IXmlContext ctx);
  }
}
