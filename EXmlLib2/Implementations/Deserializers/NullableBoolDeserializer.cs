using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Deserializers
{
  public class NullableBoolDeserializer : IAttributeDeserializer<bool?>, IElementDeserializer<bool?>
  {
    private readonly BoolDeserializer inner;
    public bool UseAlsoDefaultParseMethod => inner.UseAlsoDefaultParseMethod;

    public NullableBoolDeserializer(bool useAlsoDefaultParseMethod)
    {
      this.inner = new(useAlsoDefaultParseMethod);
    }

    public bool? Deserialize(XElement element, IXmlContext ctx)
    {
      bool? ret = Deserialize(element.Value, ctx);
      return ret;
    }

    public bool? Deserialize(string value, IXmlContext ctx)
    {
      bool? ret;
      if (value == ctx.DefaultNullString)
        ret = null;
      else
        ret = inner.Deserialize(value, ctx);
      return ret;
    }
  }
}
