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
  public class CharSerializer : TypedSerializer<char>
  {
    protected override string Serialize(char value, IXmlContext ctx) => value.ToString();
    protected override void Serialize(char value, XElement element, IXmlContext ctx) => element.Value = Serialize(value, ctx);
  }
}
