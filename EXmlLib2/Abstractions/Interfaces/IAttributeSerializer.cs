using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Abstractions.Interfaces
{
  public interface IAttributeSerializer : ISelectableByType
  {
    string Serialize(object? value, Type expectedType, IXmlContext ctx);
  }
}
