using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class StringSerializer : IElementSerializer<string>, IAttributeSerializer<string>
  {
    public string Serialize(string value, IXmlContext ctx) => value;
    public void Serialize(string value, XElement element, IXmlContext ctx) => element.Value = Serialize(value, ctx);
  }
}
