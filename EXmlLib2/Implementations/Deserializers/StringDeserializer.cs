using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Deserializers
{
  public class StringDeserializer : IElementDeserializer<string?>, IAttributeDeserializer<string?>
  {
    public string? Deserialize(XElement element, IXmlContext ctx)
    {
      string s = element.Value;
      string? ret = Deserialize(s, typeof(string), ctx);
      return ret;
    }

    public string? Deserialize(string value, Type targetType, IXmlContext ctx)
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
