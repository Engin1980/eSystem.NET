using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Types
{
  internal class TypeStringDict
  {
    private string? defaultName;
    private readonly Dictionary<Type, string> inner = new();

    internal void Set(Type? type, string value)
    {
      if (type == null)
        defaultName = value;
      else
        inner[type] = value;
    }
    internal string? Get(Type? type)
    {
      string? ret;
      if (type == null)
        ret = defaultName;
      else if (inner.ContainsKey(type))
        ret = inner[type];
      else
        ret = null;
      return ret;
    }
  }
}
