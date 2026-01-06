using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXmlLib2.Abstractions.Abstracts;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.Wrappers;

namespace EXmlLib2
{
  public static class Extensions
  {
    public static NullableObjectElementSerializerWrapper AsNullableElementWrapper(this IElementSerializer serializer)
    {
      return new NullableObjectElementSerializerWrapper(serializer);
    }
    public static NullableObjectAttributeSerializerWrapper AsNullableAttributeWrapper(this IAttributeSerializer serializer)
    {
      return new NullableObjectAttributeSerializerWrapper(serializer);
    }

    public static NullableObjectElementDeserializerWrapper AsNullableElementWrapper(this IElementDeserializer serializer)
    {
      return new NullableObjectElementDeserializerWrapper(serializer);
    }
    public static NullableObjectAttributeDeserializerWrapper AsNullableAttributeWrapper(this IAttributeDeserializer serializer)
    {
      return new NullableObjectAttributeDeserializerWrapper(serializer);
    }
  }
}
