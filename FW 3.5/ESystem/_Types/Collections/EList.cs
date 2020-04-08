#define FW35

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;


using ESystem.Extensions;

namespace ESystem.Collections
{

	public class EList<T> : List<T>
	{

		/// <summary>
		/// Returns index of last item in collection
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public int LastIndex {
			get { return ((IList)this).LastIndex(); }
		}

		/// <summary>
		/// Creates elist from list.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static EList<T> Create(IEnumerable<T> list)
		{
			EList<T> ret = new EList<T>();
			ret.AddRange(list);

			return ret;
		}

		/// <summary> 
		/// Cuts list into two parts by index. 
		/// </summary> 
		/// <param name="index">Index where to split.</param> 
		/// <param name="excludeSplittingItem">If True, indexed item will not appear in any created lists. 
		/// If False, indexed item will be the first in the second list.</param> 
		/// <returns></returns> 
		public virtual EList<T>[] SplitByIndex(int index, bool excludeSplittingItem)
		{

			List<T>[] pom = ((IList<T>)this).SplitByIndex(index, excludeSplittingItem);

			EList<T>[] ret = new EList<T>[2];
			ret[0] = EList<T>.Create(pom[0]);
			ret[1] = EList<T>.Create(pom[1]);

			return ret;
		}

		/// <summary> 
		/// Returns items in range from collection, from index to the end of source list. 
		/// </summary> 
		/// <param name="startIndex">Start index, inclusive.</param> 
		/// <returns></returns> 
		public EList<T> EGetRange(int startIndex)
		{
			EList<T> ret = EGetRange(startIndex, this.Count - startIndex);

			return ret;
		}

		/// <summary> 
		/// Returns items in range from collection. 
		/// </summary> 
		/// <param name="startIndex">Start index, inclusive.</param> 
		/// <param name="count">Count.</param> 
		/// <returns></returns> 
		public EList<T> EGetRange(int startIndex, int count)
		{

			EList<T> ret = EList<T>.Create(((IList<T>)this).EGetRange(startIndex, count));

			return ret;
		}

		/// <summary> 
		/// Returns random item from collection. 
		/// </summary> 
		/// <returns></returns> 
		public T GetRandom(Random rnd)
		{
			return ((IList<T>)this).GetRandom(rnd);
		}

		/// <summary> 
		/// Returns random item from collection. 
		/// </summary> 
		/// <returns></returns> 
		public T GetRandom()
		{
			return ((IList<T>)this).GetRandom();
		}
		/// <summary> 
		/// Returns random item from collection. 
		/// </summary> 
		/// <returns></returns> 
		public T GetRandom(int seed)
		{
			return ((IList<T>)this).GetRandom(seed);
		}

		/// <summary> 
		/// Adds new item into collection, if item is not already inserted. 
		/// Return true if item inserted, false if not. 
		/// </summary> 
		/// <param name="item"></param> 
		[Obsolete("Obsolete. Use AddDistinct(item) instead.")]
		public bool AddIfNotContained(T item)
		{
			return AddDistinct(item);
		}

		/// <summary> 
		/// Adds new item into collection, if item is not already inserted. 
		/// Return true if item inserted, false if not. 
		/// </summary> 
		/// <param name="item"></param> 
		public bool AddDistinct(T item)
		{
			if (!this.Contains(item)) {
				this.Add(item);
				return true;
			} else {
				return false;
			}
		}

		/// <summary> 
		/// Returns all items for which predicate is true.
		/// </summary> 
		/// <param name="predicate">Function to select.</param> 
		/// <returns>Items for which function is true</returns> 
		public K GetBy<K>(System.Func<T, bool> predicate) where K : IList<T>, new()
		{
			K ret = new K();

			foreach (T fItem in this) {
				if (predicate(fItem)) {
					ret.Add(fItem);
				}
			}

			return (ret);
		}

		/// <summary> 
		/// Returns all items for which predicate is true.
		/// </summary> 
		/// <param name="predicate">Function to select.</param> 
		/// <returns>Items for which function is true</returns> 
		public EList<T> GetBy(System.Func<T, bool> predicate)
		{
			EList<T> ret = new EList<T>();

			ret = GetBy<EList<T>>(predicate);

			return (ret);
		}

		/// <summary>
		/// Removes item accepted by predicate from current collection and 
		/// returns them in new collection.
		/// </summary>
		/// <param name="predicate">Predicate to match item to be removed.</param>
		/// <returns>Removed items in new list.</returns>
		/// <remarks></remarks>
		public K CutBy<K>(System.Func<T, bool> predicate) where K : IList<T>, new()
		{
			K ret = new K();

			ret = this.GetBy<K>(predicate);

			foreach (T fItem in ret) {
				this.Remove(fItem);
			}

			return (ret);
		}

		/// <summary>
		/// Removes item accepted by predicate from current collection and 
		/// returns them in new collection.
		/// </summary>
		/// <param name="predicate">Predicate to match item to be removed.</param>
		/// <returns>Removed items in new list.</returns>
		/// <remarks></remarks>
		public EList<T> CutBy(System.Func<T, bool> predicate)
		{
			EList<T> ret = new EList<T>();

			ret = CutBy<EList<T>>(predicate);

			return (ret);
		}

	}

}
