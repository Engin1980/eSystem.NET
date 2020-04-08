using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Extensions
{
  public static class TypeExt
  {
    /// <summary>
    /// Returns true if "type" is implementing "interface".
    /// </summary>
    /// <param name="type"></param>
    /// <param name="_interface"></param>
    /// <returns></returns>
    public static bool IsImplementationOf(this Type type, Type _interface)
    {      
      if (type.GetInterface(_interface.FullName, false) != null)
        return true;
      else
        return false;
    }
  }
}
