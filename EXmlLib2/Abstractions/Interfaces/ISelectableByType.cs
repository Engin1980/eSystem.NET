using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Abstractions.Interfaces
{
  public interface ISelectableByType
  {
    bool AcceptsType(Type type);
  }
}
