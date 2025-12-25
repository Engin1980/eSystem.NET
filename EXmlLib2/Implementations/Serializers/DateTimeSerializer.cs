using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class DateTimeSerializer : IAttributeSerializer<DateTime>, IElementSerializer<DateTime>
  {
    public string Serialize(DateTime value, IXmlContext ctx)
      => ConvertToString(value);

    public void Serialize(DateTime value, XElement element, IXmlContext ctx)
      => element.Value = ConvertToString(value);

    private string ConvertToString(DateTime value)
    {
      return value.ToString("o"); // ISO 8601 format
    }
  }
}
