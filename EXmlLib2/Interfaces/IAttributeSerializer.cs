using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Interfaces
{
  public interface IAttributeSerializer
  {
    public bool AcceptsValue(object? value);
    public string Serialize(object? value, IXmlContext ctx);
  }

  public interface IAttributeSerializer<T>
  {
    public string Serialize(T value, IXmlContext ctx);
  }
}
