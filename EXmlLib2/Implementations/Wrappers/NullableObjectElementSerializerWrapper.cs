using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Wrappers;

public class NullableObjectElementSerializerWrapper : IElementSerializer
{
  private readonly IElementSerializer innerSerializer;

  public NullableObjectElementSerializerWrapper(IElementSerializer innerSerializer)
  {
    EAssert.Argument.IsNotNull(innerSerializer, nameof(innerSerializer));
    this.innerSerializer = innerSerializer;
  }

  public bool AcceptsType(Type type) => innerSerializer.AcceptsType(type);

  public void Serialize(object? value, Type expectedType, XElement element, IXmlContext ctx)
  {
    if (value == null)
      element.Value = ctx.DefaultNullString;
    else
      innerSerializer.Serialize(value, expectedType, element, ctx);
  }
}
