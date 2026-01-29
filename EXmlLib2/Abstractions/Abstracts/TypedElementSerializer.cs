using ESystem.Asserting;
using ESystem.Exceptions;
using EXmlLib2.Abstractions.Interfaces;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Abstracts
{
  public abstract class TypedElementSerializer<T> : TypedBase<T>, IElementSerializer
  {
    public void Serialize(object? value, Type expectedType, XElement element, IXmlContext ctx)
    {
      CheckTypeSanity(value?.GetType());
      CheckTypeSanity(expectedType);
      T typedValue = (T)value!;
      Serialize(typedValue, element, ctx);
    }

    public abstract void Serialize(T value, XElement element, IXmlContext ctx);
  }
}
