using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.BasicSerialization.Serializers
{
  internal class NullableSerializer : IAttributeSerializer, IElementSerializer
  {
    private static Type? GetUnderlyingType(Type type) => Nullable.GetUnderlyingType(type);
    private static object GetInnerValue(object value)
    {
      object ret;
      var underType = GetUnderlyingType(value.GetType());
      if (underType != null)
        ret = Convert.ChangeType(value, GetUnderlyingType(value.GetType()) ?? throw new ESystem.Exceptions.UnexpectedNullException()) ?? throw new ESystem.Exceptions.UnexpectedNullException();
      else
        ret = value;
      return ret;
    }
      
    public bool AcceptsType(Type type) => GetUnderlyingType(type) != null;

    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      if (value == null)
        element.Value = ctx.DefaultNullString;
      else
      {
        object innerValue = GetInnerValue(value);
        EAssert.IsNotNull(innerValue, nameof(innerValue));
        IElementSerializer ser = ctx.ElementSerializers.GetByType(innerValue.GetType());
        ctx.SerializeToElement(innerValue, element, ser);
      }
    }

    public string Serialize(object? value, IXmlContext ctx)
    {
      if (value == null)
        return ctx.DefaultNullString;
      else
      {
        object innerValue = GetInnerValue(value);
        EAssert.IsNotNull(innerValue, nameof(innerValue));
        IAttributeSerializer ser = ctx.AttributeSerializers.GetByType(innerValue.GetType());
        string ret = ser.Serialize(innerValue, ctx);
        return ret;
      }
    }
  }
}
