using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Interfaces
{
  public interface IAttributeDeserializer
  {
    public bool AcceptsType(Type type);
    public object? Deserialize(string value, Type targetType, IXmlContext ctx);
  }

  public interface IAttributeDeserializer<T>
  {
    public T Deserialize(string value, IXmlContext ctx);
  }
}
