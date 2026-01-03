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
  private readonly Stack<T> items = new();

  public int Count => items.Count;
  public void Push(T item) => items.Push(item);
  public T GetByType(Type type)
  {
    T ret;
    try
    {
      ret = items.First(q => q.AcceptsType(type));
    }
    catch (Exception ex)
    {
      EXmlException eex = new($"Failed to find element of {typeof(T)} for type {type}.", ex);
      throw eex;
    }
    return ret;
  }

  public void Set(IEnumerable<T> newItems)
  {
    items.Clear();
    foreach (T item in newItems.Reverse())
    {
      items.Push(item);
    }
  }
}
