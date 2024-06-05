using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Interfaces
{
  public interface IElementDeserializer
  {
    public bool AcceptsType(Type type);
    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx);
  }

  public interface IElementDeserializer<T>
  {
    public T? Deserialize(XElement element, IXmlContext ctx);
  }
}
