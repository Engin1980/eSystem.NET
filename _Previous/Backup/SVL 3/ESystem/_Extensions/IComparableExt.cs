using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Extensions
{
  public static class IComparableExt
  {
    /// <summary>
    /// Checks if value is between bounds, both bounds are inclusive.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="firstBound">Inclusive lower bound.</param>
    /// <param name="secondBound">Inclusive uper bound.</param>
    /// <returns>True if value is between bounds (both inclusive).</returns>
    /// <remarks></remarks>
    public static bool IsBetween<T>(this IComparable<T> value, T firstBound, T secondBound) 
      where T : IComparable<T>
    {
      if ((firstBound.CompareTo(secondBound) > 0))
      {
        T x = firstBound;
        firstBound = secondBound;
        secondBound = x;
      }

      bool ret;

      if ((value.CompareTo(firstBound) < 0))
      {
        // je mensi nez minimum
        ret = false;
      }
      else if ((value.CompareTo(secondBound) > 0))
      {
        // je vetsi nez maximum
        ret = false;
      }
      else
      {
        // je v intervalu
        ret = true;
      }

      return ret;
    }

    /// <summary>
    /// Returns true if item is in set of other items.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="items"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsIn<T>(this IComparable<T> current, params T[] items)
    {
      foreach (T fItem in items)
      {
        if (current.CompareTo(fItem) == 0)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Returns true if item is not in set of other items.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="items"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsNotIn<T>(this IComparable<T> current, params T[] items)
    {
      foreach (T fItem in items)
      {
        if (current.CompareTo(fItem) == 0)
        {
          return false;
        }
      }

      return true;
    }

  }
}
