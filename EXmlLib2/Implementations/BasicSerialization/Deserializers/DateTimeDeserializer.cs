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
  public class DateTimeDeserializer : TypedDeserializer<DateTime>
  {
    public override DateTime Deserialize(string value, IXmlContext ctx)
    {
      return ConvertFromString(value);
    }

    public override DateTime Deserialize(XElement element, IXmlContext ctx)
    {
      return ConvertFromString(element.Value);
    }

    private DateTime ConvertFromString(string value)
    {
      return DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
  }
}
