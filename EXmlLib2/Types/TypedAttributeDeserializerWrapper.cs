using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Types
{
  internal class TypedAttributeDeserializerWrapper<T> : IAttributeDeserializer
  {
    private readonly IAttributeDeserializer<T> inner;

    public TypedAttributeDeserializerWrapper(IAttributeDeserializer<T> deserializer)
    {
      this.inner = deserializer;
    }

    public bool AcceptsType(Type type) => type == typeof(T);

    public object? Deserialize(string value, Type targetType, IXmlContext ctx)
    {
      object? ret = this.inner.Deserialize(value, ctx);
      return ret;
    }
  }
}
