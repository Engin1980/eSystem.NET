using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Abstractions.Wrappers;

public class NullableObjectAttributeDeserializerWrapper : IAttributeDeserializer
{
  private readonly IAttributeDeserializer innerDeserializer;
  public NullableObjectAttributeDeserializerWrapper(IAttributeDeserializer innerDeserializer)
  {
    EAssert.Argument.IsNotNull(innerDeserializer, nameof(innerDeserializer));
    this.innerDeserializer = innerDeserializer;
  }
  public bool AcceptsType(Type type) => innerDeserializer.AcceptsType(type);
  public object Deserialize(string value, Type targetType, IXmlContext ctx)
  {
    if (value == ctx.DefaultNullString)
      return null!;
    else
      return innerDeserializer.Deserialize(value, targetType, ctx) ?? throw new ESystem.Exceptions.UnexpectedNullException();
  }
}
