using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Deserializers
{
  public class NullableNumberDeserializer : IElementDeserializer, IAttributeDeserializer
  {
    private record TypeAndDeserializer(Type Type, Func<string, CultureInfo, object> parser);

    private static readonly TypeAndDeserializer[] parsers = new TypeAndDeserializer[]
    {
      new(typeof(byte?), (s, c) => byte.Parse(s,c)),
      new(typeof(short?), (s, c) => short.Parse(s,c)),
      new(typeof(int?), (s, c) => int.Parse(s, c)),
      new(typeof(long?), (s, c) => long.Parse(s,c)),
      new(typeof(float?), (s, c) => float.Parse(s,c)),
      new(typeof(double?), (s, c) => int.Parse(s,c))
    };

    private static object? Parse(string s, Type type, IXmlContext ctx)
    {
      object? ret;
      if (s == ctx.DefaultNullString)
        ret = null;
      else
      {
        CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        TypeAndDeserializer tmp = parsers.First(q => q.Type == type);
        ret = tmp.parser.Invoke(s, cultureInfo);
      }
      return ret;
    }

    public bool AcceptsType(Type type) => parsers.Any(q => q.Type == type);

    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
    {
      string s = element.Value;
      object? ret = Parse(s, targetType, ctx);
      return ret;
    }

    public object? Deserialize(string value, Type targetType, IXmlContext ctx)
    {
      object? ret = Parse(value, targetType, ctx);
      return ret;
    }
  }
}
