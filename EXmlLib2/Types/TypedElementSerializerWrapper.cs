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
  public class TypedElementSerializerWrapper<T> : IElementSerializer
  {
    private readonly IElementSerializer<T> inner;

    public TypedElementSerializerWrapper(IElementSerializer<T> typedElementSerializer)
    {
      EAssert.Argument.IsNotNull(typedElementSerializer, nameof(typedElementSerializer));
      this.inner = typedElementSerializer;
    }

    public bool AcceptsValue(object? value) => value == null ? false : value.GetType() == typeof(T);

    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      EAssert.Argument.IsTrue(
        value != null && value.GetType() == typeof(T), 
        nameof(value),
        $"Value type ({value?.GetType()?.Name ?? "null"}) does not match attribute serializer type ({typeof(T).Name})");
      T typedValue = (T)value!;
      inner.Serialize(typedValue, element, ctx);
    }
  }
}
