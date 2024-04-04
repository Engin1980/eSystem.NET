using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Extensions
{
  public static class ICollectionExt
  {

    /// <summary>
    /// Returns index of last item in collection
    /// </summary>
    public static int LastIndex(this ICollection arr)
    {
      return arr.Count - 1;
    }    

    /// <summary>
    /// Returns all items in enumeration fulfilling predicate conditions in new list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="coll"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<T> FindAll<T>(this IEnumerable<T> coll, System.Func<T, bool> predicate)
    {
      List<T> ret = new List<T>();

      foreach (T fItem in coll)
      {
        if (predicate(fItem))
          ret.Add(fItem);
      }

      return ret;

    }

    /// <summary>
    /// Removes item fulfiled by predicate from current collection and 
    /// returns them in new collection.
    /// </summary>
    /// <param name="predicate">Predicate to match item to be removed.</param>
    /// <returns>Removed items in new list.</returns>
    /// <remarks></remarks>
    public static List<T> Cut<T>(this ICollection<T> coll, System.Func<T, bool> predicate)
    {
      List<T> ret = new List<T>();

      ret = FindAll(coll, predicate);
      RemoveRange(coll, ret);

      return (ret);
    }

    /// <summary>
    /// Removes item fulfiled by predicate from current collection into the parameter.
    /// </summary>
    /// <param name="predicate">Predicate to match item to be removed.</param>
    /// <returns>Removed items in new list.</returns>
    /// <remarks></remarks>
    public static List<T> CutTo<T>(this ICollection<T> coll,
      ICollection<T> targetCollection, System.Func<T, bool> predicate)
    {
      List<T> ret = new List<T>();

      ret = FindAll(coll, predicate);
      RemoveRange(coll, ret);

      foreach (T fItem in ret)
      {
        targetCollection.Add(fItem);
      }

      return (ret);
    }

    /// <summary>
    /// Removes items from collection, which are in other enumeration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="coll"></param>
    /// <param name="items"></param>
    public static void RemoveRange<T>(this ICollection<T> coll, IEnumerable<T> items)
    {
      foreach (T fItem in items)
      {
        if (coll.Contains(fItem))
        {
          coll.Remove(fItem);
        }
      }
    }

    /// <summary>
    /// Removes from the collection items fulfiling the predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="coll"></param>
    /// <param name="predicate"></param>
    public static void RemoveRange<T>(this ICollection<T> coll, System.Func<T, bool> predicate)
    {
      List<T> remList = new List<T>();

      foreach (var fItem in coll)
      {
        if (predicate(fItem))
          remList.Add(fItem);
      } // foreach (var fItem in coll)

      foreach (var fItem in remList)
      {
        coll.Remove(fItem);
      } // foreach (var fItem in remList)

    }

    /// <summary>
    /// Leaves in collection only items fulfilling the predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="coll"></param>
    /// <param name="predicate"></param>
    public static void LeaveRange<T>(this ICollection<T> coll, System.Func<T, bool> predicate)
    {
      coll.RemoveRange(i => (!predicate(i)));
    }

    /// <summary> 
    /// Adds new item into collection, if item is not already inserted. 
    /// Return true if item inserted, false if not. 
    /// </summary> 
    /// <param name="item"></param> 
    public static bool AddDistinct<T>(this ICollection<T> coll, T item)
    {
      if (!coll.Contains(item))
      {
        coll.Add(item);
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary> 
    /// Adds new item into collection, if item is not already inserted. 
    /// </summary> 
    /// <param name="items"></param> 
    public static void AddDistinct<T>(this ICollection<T> coll, IEnumerable<T> items)
    {
      items.EForEach((T i) => AddDistinct(coll, i));
    }

  }
}
