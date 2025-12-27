using EXmlLib2.Implementations.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Abstractions.Interfaces
{
  public interface IElementSerializer : ISelectableByType
  {
    void Serialize(object? value, XElement element, IXmlContext ctx);
  }
}
