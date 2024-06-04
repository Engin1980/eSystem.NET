using ESystem.Asserting;
using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Types
{
  internal class TypedAttributeSerializerWrapper<T> : IAttributeSerializer
  {
    private readonly IAttributeSerializer<T> inner;

    public TypedAttributeSerializerWrapper(IAttributeSerializer<T> inner)
    {
      EAssert.Argument.IsNotNull(inner, nameof(inner));
      this.inner = inner;
    }

    public bool AcceptsValue(object? value) => value != null && value.GetType() == typeof(T);

    public string Serialize(object? value, IXmlContext ctx)
    {
      EAssert.Argument.IsTrue(value != null && value.GetType() == typeof(T), "Value type does not match attribute serializer type.");
      T typedValue = (T)value!;
      string ret = inner.Serialize(typedValue, ctx);
      return ret;
    }
  }
}
