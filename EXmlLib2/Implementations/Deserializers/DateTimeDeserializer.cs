using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Deserializers
{
  public class DateTimeDeserializer : IAttributeDeserializer<DateTime>, IElementDeserializer<DateTime>
  {
    public DateTime Deserialize(string value, IXmlContext ctx)
    {
      return ConvertFromString(value);
    }

    public DateTime Deserialize(XElement element, IXmlContext ctx)
    {
      return ConvertFromString(element.Value);
    }

    private DateTime ConvertFromString(string value)
    {
      return DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
  }
}
