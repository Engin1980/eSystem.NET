using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Interfaces
{
  public interface IElementDeserializer : ISelectableByType
  {
    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx);
  }
}
