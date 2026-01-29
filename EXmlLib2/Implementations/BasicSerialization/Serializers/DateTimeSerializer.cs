using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.BasicSerialization.Serializers
{
  public class DateTimeSerializer : TypedSerializer<DateTime>
  {
    protected override string Serialize(DateTime value, IXmlContext ctx)
      => ConvertToString(value);

    protected override void Serialize(DateTime value, XElement element, IXmlContext ctx)
      => element.Value = ConvertToString(value);

    private string ConvertToString(DateTime value)
    {
      return value.ToString("o"); // ISO 8601 format
    }
  }
}
