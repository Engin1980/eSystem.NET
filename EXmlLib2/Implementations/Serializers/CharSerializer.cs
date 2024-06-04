using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class CharSerializer : IElementSerializer<char>, IAttributeSerializer<char>
  {
    public string Serialize(char value, IXmlContext ctx) => value.ToString();
    public void Serialize(char value, XElement element, IXmlContext ctx) => element.Value = Serialize(value, ctx);
  }
}
