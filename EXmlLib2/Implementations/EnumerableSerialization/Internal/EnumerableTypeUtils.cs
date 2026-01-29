using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Implementations.EnumerableSerialization.Internal
{
  internal class EnumerableTypeUtils
  {
    public static bool IsListOrArray(Type type)
    {
      if (type.IsArray)
        return true;
      if (type.IsGenericType && type.IsAssignableTo(typeof(System.Collections.IEnumerable)))
        return true;
      return false;
    }

    public static Type GetItemTypeForIEnumerable(Type type)
    {
      Type ret;
      if (type.IsGenericType && type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
      {
        Type ienumerableInterface = type.GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        ret = ienumerableInterface.GetGenericArguments()[0];
      }
      else
        ret = typeof(object);
      return ret;
    }
  }
}
