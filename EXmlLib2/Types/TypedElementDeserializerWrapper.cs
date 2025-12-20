using ESystem.Asserting;
using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Types
{
  public class TypedElementDeserializerWrapper<T> : IElementDeserializer
  {
    private readonly IElementDeserializer<T> inner;

    public TypedElementDeserializerWrapper(IElementDeserializer<T> inner)
    {
      EAssert.Argument.IsNotNull(inner, nameof(inner));
      this.inner = inner;
    }

    public bool AcceptsType(Type type)
    {
      return type == typeof(T);
    }

    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
    {
      T? ret = inner.Deserialize(element, ctx);
      return ret;
    }
  }
}
