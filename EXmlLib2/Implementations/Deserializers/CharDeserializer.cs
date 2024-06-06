using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Deserializers
{
  public class CharDeserializer : IAttributeDeserializer<char>, IElementDeserializer<char>
  {
    public char Deserialize(string value, IXmlContext ctx)
    {
      char ret = char.Parse(value);
      return ret;
    }

    public char Deserialize(XElement element, IXmlContext ctx)
    {
      return this.Deserialize(element.Value, ctx);
    }
  }
}
