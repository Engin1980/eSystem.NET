using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Deserializers
{
  public class BoolDeserializer : IAttributeDeserializer<bool>, IElementDeserializer<bool>
  {
    public bool UseAlsoDefaultParseMethod { get; private set; }

    public BoolDeserializer(bool useAlsoDefaultParseMethod)
    {
      UseAlsoDefaultParseMethod = useAlsoDefaultParseMethod;
    }

    public bool Deserialize(string value, IXmlContext ctx)
    {
      bool ret;
      if (value == ctx.DefaultTrueString)
        ret = true;
      else if (value == ctx.DefaultFalseString)
        ret = false;
      else
        ret = bool.Parse(value);
      return ret;
    }

    public bool Deserialize(XElement element, IXmlContext ctx)
    {
      return this.Deserialize(element.Value, ctx);
    }
  }
}
