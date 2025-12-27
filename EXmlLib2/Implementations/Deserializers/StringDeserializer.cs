using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Deserializers
{
  public class StringDeserializer : TypedDeserializer<string?>
  {
    public override string? Deserialize(XElement element, IXmlContext ctx)
    {
      string s = element.Value;
      string? ret = Deserialize(s, ctx);
      return ret;
    }

    public override string? Deserialize(string value, IXmlContext ctx)
    {
      string? ret;
      if (value == ctx.DefaultNullString)
        ret = null;
      else
        ret = value;
      return ret;
    }

  }
}
