using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class BoolSerializer : IElementSerializer<bool>, IAttributeSerializer<bool>
  {
    public string Serialize(bool value, IXmlContext ctx) => value ? ctx.DefaultTrueString : ctx.DefaultFalseString;
    public void Serialize(bool value, XElement element, IXmlContext ctx) => element.Value = Serialize(value, ctx);
  }
}
