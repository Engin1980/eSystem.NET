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
  public class StringSerializer : TypedSerializer<string>
  {
    protected override string Serialize(string value, IXmlContext ctx) => value;
    protected override void Serialize(string value, XElement element, IXmlContext ctx) => element.Value = Serialize(value, ctx);
  }
}
