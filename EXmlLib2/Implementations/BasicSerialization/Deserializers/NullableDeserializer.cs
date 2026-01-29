using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.BasicSerialization.Deserializers
{
  public class NullableDeserializer : IAttributeDeserializer, IElementDeserializer
  {
    public bool AcceptsType(Type type) => Nullable.GetUnderlyingType(type) != null;

    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
    {
      object? ret;
      if (element.Value == ctx.DefaultNullString)
        ret = null;
      else
      {
        Type type = Nullable.GetUnderlyingType(targetType) ?? throw new EXmlException("Unexpected non-nullable type.");
        IElementDeserializer deserializer = ctx.ElementDeserializers.GetByType(type);
        ret = deserializer.Deserialize(element, type, ctx);
      }
      return ret;
    }

    public object? Deserialize(string value, Type targetType, IXmlContext ctx)
    {
      object? ret;
      if (value == ctx.DefaultNullString)
        ret = null;
      else
      {
        Type type = Nullable.GetUnderlyingType(targetType) ?? throw new EXmlException("Unexpected non-nullable type.");
        IAttributeDeserializer deserializer = ctx.AttributeDeserializers.GetByType(type);
        ret = deserializer.Deserialize(value, type, ctx);
      }
      return ret;
    }
  }
}
