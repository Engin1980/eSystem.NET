using ESystem.Asserting;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ESystem
{
  public static class Extensions
  {
    public static StringBuilder AppendIf(this StringBuilder sb, Func<bool> condition, string text)
    {
      if (condition())
        sb.Append(text);
      return sb;
    }

    public static StringBuilder AppendIf(this StringBuilder sb, bool condition, string text)
    {
      if (condition)
        sb.Append(text);
      return sb;
    }

    public static bool None<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
      return items.Any(predicate) == false;
    }

    [Obsolete("Use Select() instead.")]
    public static Tout Pipe<Tin, Tout>(this Tin obj, Func<Tin, Tout> selector)
    {
      return selector(obj);
    }

    public static Tout Select<Tin, Tout>(this Tin obj, Func<Tin, Tout> selector)
    {
      return selector(obj);
    }

    public static List<Tin> FlattenRecursively<Tin, Tcol>(this IEnumerable<Tin> lst, Func<Tcol, List<Tin>> subListSelector)
    {
      List<Tin> ret = new();

      foreach (var item in lst)
      {
        if (item is Tcol subListSource)
        {
          List<Tin> subList = subListSelector(subListSource);
          List<Tin> flatted = subList.FlattenRecursively(subListSelector);
          ret.AddRange(flatted);
        }
        else
          ret.Add(item);
      }

      return ret;
    }

    private static Random? rnd;
    public static T GetRandom<T>(this List<T> lst)
    {
      EAssert.Argument.IsNotNull(lst, nameof(lst));
      EAssert.Argument.IsTrue(lst.Count > 0, nameof(lst), "List must be non-empty.");

      if (rnd == null) rnd = new Random();
      int index = rnd.Next(0, lst.Count);
      T ret = lst[index];
      return ret;
    }

    public static T? GetRandomOrDefault<T>(this List<T> lst)
    {
      EAssert.Argument.IsNotNull(lst, nameof(lst));
      if (lst.Count == 0) return default;

      T ret = GetRandom(lst);
      return ret;
    }

    public static IEnumerable<T> TapEach<T>(this IEnumerable<T> lst, Action<T> action)
    {
      foreach (var item in lst)
      {
        action(item);
      }
      return lst;
    }

    public static T Tap<T>(this T obj, Action<T> action)
    {
      action(obj);
      return obj;
    }

    public static BindingList<T> ToBindingList<T>(this IEnumerable<T> items)
    {
      BindingList<T> ret;
      if (items is List<T> lst)
        ret = new(lst);
      else
        ret = new(items.ToList());
      return ret;
    }

    public static string GetFullMessage(this Exception ex, string delimiter = " <== ")
    {
      List<string> tmp = new();
      while (ex != null)
      {
        tmp.Add(ex.Message);
        ex = ex.InnerException!;
      }
      string ret = string.Join(delimiter, tmp);
      return ret;
    }

    public static int Clip(this int value, int minimumInclusive, int maximumInclusive) => Math.Max(Math.Min(value, maximumInclusive), minimumInclusive);
    public static double Clip(this double value, double minimumInclusive, double maximumInclusive) => Math.Max(Math.Min(value, maximumInclusive), minimumInclusive);

    public static TValue? TryGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
    {
      TValue? ret;
      if (dictionary.TryGetValue(key, out ret!) == false)
      {
        ret = default;
      }
      return ret;
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueProducer) where TKey : notnull
    {
      TValue ret;
      if (dictionary.TryGetValue(key, out ret!) == false)
      {
        ret = valueProducer();
        dictionary[key] = ret;
      }
      return ret;
    }
  }
}
