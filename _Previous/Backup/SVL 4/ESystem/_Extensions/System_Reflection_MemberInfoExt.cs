using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ESystem2._Extensions
{
  public static class System_Reflection_MemberInfoExt
  {
    /// <summary>
    /// Returns all custom attributes defined for memberinfo.
    /// </summary>
    /// <typeparam name="T">Type of requested custom attribute.</typeparam>
    /// <param name="memberInfo">Member to search through.</param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static T[] GetCustomAttributes<T> (this MemberInfo memberInfo, bool inherit) where T : Attribute
    {
      object[] pom = null;
      pom = memberInfo.GetCustomAttributes(typeof(T), inherit);

      T[] ret = new T[pom.Length];
      for (int i = 0; i < pom.Length; i++)
			{
        ret[i] = (T) pom[i];
			}
      return ret;
    }

    public static T GetCustomAttribute<T> (this MemberInfo memberInfo, bool inherit) where T : Attribute
    {
      T[] pom = memberInfo.GetCustomAttributes<T>(inherit);
      T ret;

      if (pom.Length > 0)
        ret = pom[0];
      else
        ret = null;

      return ret;
    }
  }
}
