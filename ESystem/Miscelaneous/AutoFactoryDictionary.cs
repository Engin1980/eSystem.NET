using ESystem.Asserting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ESystem.Miscelaneous;

internal class AutoFactoryDictionary<K, V> : IDictionary<K, V> where K : notnull
{
  private readonly Dictionary<K, V> dict = new();
  private readonly Func<V> valueFactory;

  public AutoFactoryDictionary(Func<V> valueFactory)
  {
    EAssert.Argument.IsNotNull(valueFactory, nameof(valueFactory));
    this.valueFactory = valueFactory;
  }

  public V this[K key]
  {
    get
    {
      if (!dict.TryGetValue(key, out V? value))
      {
        value = valueFactory();
        dict[key] = value;
      }
      return value;
    }
    set => dict[key] = value;
  }

  public ICollection<K> Keys => dict.Keys;

  public ICollection<V> Values => dict.Values;
  public int Count => dict.Count;

  public bool IsReadOnly => false;

  public void Add(K key, V value) => dict.Add(key, value);

  public void Add(KeyValuePair<K, V> item) => dict.Add(item.Key, item.Value);

  public void Clear() => dict.Clear();

  public bool Contains(KeyValuePair<K, V> item) => dict.Contains(item);

  public bool ContainsKey(K key) => dict.ContainsKey(key);

  public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) => throw new NotSupportedException();

  public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => dict.GetEnumerator();

  public bool Remove(K key) => dict.Remove(key);

  public bool Remove(KeyValuePair<K, V> item) => dict.Remove(item.Key);

  public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value) => dict.TryGetValue(key, out value);

  IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();
}
