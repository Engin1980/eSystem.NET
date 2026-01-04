using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Miscelaneous
{
  public sealed class BiDictionary<TKey, TValue> where TKey : notnull where TValue : notnull
  {
    private readonly Dictionary<TKey, TValue> forward;
    private readonly Dictionary<TValue, TKey> reverse;

    public BiDictionary()
      : this(null, null)
    {
    }

    public BiDictionary(
      IEqualityComparer<TKey>? keyComparer,
      IEqualityComparer<TValue>? valueComparer)
    {
      forward = new Dictionary<TKey, TValue>(keyComparer);
      reverse = new Dictionary<TValue, TKey>(valueComparer);
    }

    public int Count => forward.Count;

    public TValue this[TKey key] => forward[key];
    public TKey GetKey(TValue value) => reverse[value];

    public bool TryGetValue(TKey key, out TValue value)
      => forward.TryGetValue(key, out value!);

    public bool TryGetKey(TValue value, out TKey key)
      => reverse.TryGetValue(value, out key!);

    public TValue TryGetValueOrDefault(TKey key, TValue defaultValue = default!)
      => forward.TryGetValue(key, out var value) ? value : defaultValue;
    public TKey TryGetKeyOrDefault(TValue value, TKey defaultValue = default!)
      => reverse.TryGetValue(value, out var key) ? key : defaultValue;

    public bool ContainsKey(TKey key) => forward.ContainsKey(key);
    public bool ContainsValue(TValue value) => reverse.ContainsKey(value);

    public IReadOnlyDictionary<TKey, TValue> Forward => forward;
    public IReadOnlyDictionary<TValue, TKey> Reverse => reverse;

    public void Add(TKey key, TValue value)
    {
      if (forward.ContainsKey(key))
        throw new ArgumentException($"Duplicate key {key}.", nameof(key));

      if (reverse.ContainsKey(value))
        throw new ArgumentException($"Duplicate value {value}.", nameof(value));

      forward.Add(key, value);
      reverse.Add(value, key);
    }

    public void Set(TKey key, TValue value)
    {
      if (forward.ContainsKey(key))
      {
        reverse.Remove(forward[key]);
        forward.Remove(key);
      }
      if (reverse.ContainsKey(value))
      {
        forward.Remove(reverse[value]);
        reverse.Remove(value);
      }

      forward.Add(key, value);
      reverse.Add(value, key);
    }

    public bool RemoveByKey(TKey key)
    {
      if (!forward.TryGetValue(key, out var value))
        return false;

      forward.Remove(key);
      reverse.Remove(value);
      return true;
    }

    public bool RemoveByValue(TValue value)
    {
      if (!reverse.TryGetValue(value, out var key))
        return false;

      reverse.Remove(value);
      forward.Remove(key);
      return true;
    }

    public void Clear()
    {
      forward.Clear();
      reverse.Clear();
    }
  }

}
