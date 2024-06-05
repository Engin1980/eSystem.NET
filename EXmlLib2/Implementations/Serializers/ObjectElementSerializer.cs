using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class ObjectElementSerializer : IElementSerializer
  {
    public bool AcceptsValue(object? value)
    {
      throw new NotImplementedException();
    }

    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      throw new NotImplementedException();
    }
  }
}
