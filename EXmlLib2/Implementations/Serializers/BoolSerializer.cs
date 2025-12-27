using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class BoolSerializer : TypedSerializer<bool>
  {
    protected override string Serialize(bool value, IXmlContext ctx) => value ? ctx.DefaultTrueString : ctx.DefaultFalseString;
    protected override void Serialize(bool value, XElement element, IXmlContext ctx) => element.Value = Serialize(value, ctx);
  }
}
