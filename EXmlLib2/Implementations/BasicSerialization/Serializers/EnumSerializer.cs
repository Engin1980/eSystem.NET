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
  public class EnumSerializer : IAttributeSerializer, IElementSerializer
  {
    public bool AcceptsType(Type type) => type.IsEnum;

    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      element.Value = Serialize(value, ctx);
    }

    public string Serialize(object? value, IXmlContext ctx) => value!.ToString() ?? throw new EXmlException("Enum value must be not nul.");
  }
}
