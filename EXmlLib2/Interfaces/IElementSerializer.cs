using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Interfaces
{
  public interface IElementSerializer
  {
    public bool AcceptsValue(object? value);
    public void Serialize(object? value, XElement element, IXmlContext ctx);
  }

  public interface IElementSerializer<T>
  {
    public void Serialize(T value, XElement element, IXmlContext ctx);
  }
}
