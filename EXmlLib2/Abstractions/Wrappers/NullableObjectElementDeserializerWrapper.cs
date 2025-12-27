using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Wrappers;

public class NullableObjectElementDeserializerWrapper : IElementDeserializer
{
  private readonly IElementDeserializer innerDeserializer;
  public NullableObjectElementDeserializerWrapper(IElementDeserializer innerDeserializer)
  {
    EAssert.Argument.IsNotNull(innerDeserializer, nameof(innerDeserializer));
    this.innerDeserializer = innerDeserializer;
  }
  public bool AcceptsType(Type type) => innerDeserializer.AcceptsType(type);
  public object Deserialize(XElement element, Type targetType, IXmlContext ctx)
  {
    if (element.Value == ctx.DefaultNullString)
      return null!;
    else
      return innerDeserializer.Deserialize(element, targetType, ctx) ?? throw new ESystem.Exceptions.UnexpectedNullException();
  }
}
