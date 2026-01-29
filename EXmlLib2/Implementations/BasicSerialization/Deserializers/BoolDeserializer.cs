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
  public class BoolDeserializer : TypedDeserializer<bool>
  {
    public bool UseAlsoDefaultParseMethod { get; private set; }

    public BoolDeserializer(bool useAlsoDefaultParseMethod)
    {
      UseAlsoDefaultParseMethod = useAlsoDefaultParseMethod;
    }

    public override bool Deserialize(string value, IXmlContext ctx)
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

    public override bool Deserialize(XElement element, IXmlContext ctx)
    {
      return Deserialize(element.Value, ctx);
    }
  }
}
