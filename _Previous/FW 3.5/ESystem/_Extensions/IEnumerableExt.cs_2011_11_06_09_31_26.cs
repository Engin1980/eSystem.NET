using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ESystem.Extensions
{
  public static class IEnumerableExt
  {
    /// <summary>
    /// Returns all items as string, delimited by delimiter string separator.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string ToString(this IEnumerable array, string separator)
    {
      StringBuilder s = new StringBuilder();
      bool isFirst = true;

      foreach (object fItem in array)
      {
        if (isFirst)
          isFirst = false;
        else
          s.Append(separator);
        s.Append(fItem.ToString());
      }

      return s.ToString();
    }

    /// <summary>
    /// Returns first item fulfiling the predicate, or null if enum is empty or nothing found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="coll"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T EFirst<T>(this IEnumerable<T> coll, System.Func<T, bool> predicate)
    {
      T ret = default(T);

      try
      {
        ret = coll.First(predicate);
      }
      catch (InvalidOperationException)
      {
      }

      return ret;
    }

    /// <summary>
    /// Applies operation over all elements of enumeration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="coll"></param>
    /// <param name="predicate"></param>
    public static void EForEach<T>(this IEnumerable<T> coll, System.Action<T> operation)
    {
      foreach (T Item in coll)
      {
        operation(Item);
      }
    }

  }

}
