using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.BasicSerialization.Deserializers
{
  public class CharDeserializer : TypedDeserializer<char>
  {
    public override char Deserialize(string value, IXmlContext ctx)
    {
      char ret = char.Parse(value);
      return ret;
    }

    public override char Deserialize(XElement element, IXmlContext ctx)
    {
      return Deserialize(element.Value, ctx);
    }
  }
}
