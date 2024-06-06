using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Types
{
  public class XmlPropertyInfo
  {
    public string? XmlName { get; set; }
    public XmlRepresentation? Representation { get; set; } = null;
    public XmlObligation? Obligation { get; set; } = null;
    public IElementSerializer? CustomElementSerializer { get; set; } = null;
    public IAttributeSerializer? CustomAttributeSerializer { get; set; } = null;
    public IElementDeserializer? CustomElementDeserializer { get; set; } = null;
    public IAttributeDeserializer? CustomAttributeDeserializer { get; set; } = null;
  }
}
