using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;

namespace EXmlLib2.Abstractions.Wrappers;

public class NullableObjectAttributeSerializerWrapper : IAttributeSerializer
{
  private readonly IAttributeSerializer innerSerializer;
  public NullableObjectAttributeSerializerWrapper(IAttributeSerializer innerSerializer)
  {
    EAssert.Argument.IsNotNull(innerSerializer, nameof(innerSerializer));
    this.innerSerializer = innerSerializer;
  }

  public bool AcceptsType(Type type) => innerSerializer.AcceptsType(type);

  public string Serialize(object? value, IXmlContext ctx)
  {
    if (value == null)
      return ctx.DefaultNullString;
    else
      return innerSerializer.Serialize(value, ctx);
  }
}
