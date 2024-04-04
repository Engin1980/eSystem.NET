using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Extensions
{
  public static class IListExt
  {
    /// <summary> 
    /// Returns items in range from collection. 
    /// </summary> 
    /// <param name="startIndex">Start index, inclusive.</param> 
    /// <param name="count">Count.</param> 
    /// <returns></returns> 
    public static List<T> EGetRange<T>(this IList<T> list, int startIndex, int count)
    {
      List<T> ret = new List<T>();

      for (int index = startIndex; index <= startIndex + count - 1; index++)
      {
        ret.Add(list[index]);
      }

      return ret;
    }

    /// <summary> 
    /// Returns random item from collection. 
    /// </summary> 
    /// <returns></returns> 
    /// <exception cref="Exception">Thrown when list is empty and random item cannot be returned.</exception>
    public static T GetRandom<T>(this IList<T> list, Random rnd)
    {
      if (list.Count == 0)
      {
        throw new Exception("Unable to get random item from empty collection.");
      }
      else
      {
        return list[rnd.Next(0, list.Count)];
      }
    }

    /// <summary> 
    /// Returns random item from collection. 
    /// </summary> 
    /// <returns></returns> 
    public static T GetRandom<T>(this IList<T> coll)
    {
      return GetRandom(coll, new Random());
    }
    /// <summary> 
    /// Returns random item from collection. 
    /// </summary> 
    /// <returns></returns> 
    public static T GetRandom<T>(this IList<T> coll, int seed)
    {
      return GetRandom(coll, new Random(seed));
    }


    /// <summary> 
    /// Returns items in range from collection, from index to the end of source list. 
    /// </summary> 
    /// <param name="startIndex">Start index, inclusive.</param> 
    /// <returns></returns> 
    public static List<T> EGetRange<T>(this IList<T> list, int startIndex)
    {
      List<T> ret = list.EGetRange(startIndex, list.Count - startIndex);

      return ret;
    }

    /// <summary> 
    /// Cuts list into two parts by index. 
    /// </summary> 
    /// <param name="index">Index where to split.</param> 
    /// <param name="excludeSplittingItem">If True, indexed item will not appear in any created lists. 
    /// If False, indexed item will be the first in the second list.</param> 
    /// <returns></returns> 
    public static List<T>[] SplitByIndex<T>(this IList<T> list, int index, bool excludeSplittingItem)
    {
      List<T>[] ret = new List<T>[2];

      ret[0] = list.EGetRange(0, index);

      if (excludeSplittingItem)
      {
        ret[1] = list.EGetRange(index + 1);
      }
      else
      {
        ret[1] = list.EGetRange(index);
      }

      return ret;
    }
  }
}
