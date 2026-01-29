using ESystem.Logging;
using EXmlLib2.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Types.Internal;

public class SerializerDeserializerRegistry<T> where T : ISelectableByType
{
  private readonly List<T> items = [];
  public int Count => items.Count;
  public void AddFirst(IEnumerable<T> newItems) => items.InsertRange(0, newItems);
  public void AddLast(IEnumerable<T> newItems) => items.AddRange(newItems);
  public void AddFirst(T item) => items.Insert(0, item);
  public void AddLast(T item) => items.Add(item);
  public T GetByType(Type type)
  {
    T? ret = TryGetByType(type);
    if (ret == null)
    {
      EXmlException eex = new($"Failed to find {typeof(T).Name} for type {type}.");
      throw eex;
    }
    return ret;
  }

  public T? TryGetByType(Type type)
  {
    T? ret = items.FirstOrDefault(q => q.AcceptsType(type));
    return ret;
  }

  public void Set(IEnumerable<T> newItems)
  {
    items.Clear();
    foreach (T item in newItems)
    {
      items.Add(item);
    }
  }
}
