using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Deserializers
{
  public class NullableCharDeserializer : IAttributeDeserializer<char?>, IElementDeserializer<char?>
  {
    private readonly CharDeserializer inner = new();

    public char? Deserialize(XElement element, IXmlContext ctx)
    {
      char? ret = Deserialize(element.Value, ctx);
      return ret;
    }

    public char? Deserialize(string value, IXmlContext ctx)
    {
      char? ret;
      if (value == ctx.DefaultNullString)
        ret = null;
      else
        ret = inner.Deserialize(value, ctx);
      return ret;
    }
  }
}
