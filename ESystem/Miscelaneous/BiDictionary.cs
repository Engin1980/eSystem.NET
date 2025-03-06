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
  public class BiDictionary<AType, BType> where AType : notnull where BType : notnull
  {
    private readonly Dictionary<AType, BType> dctA = new();
    private readonly Dictionary<BType, AType> dctB = new();

    public ICollection<AType> As => dctA.Keys;

    public ICollection<BType> Bs => dctB.Keys;

    public HashSet<KeyValuePair<AType, BType>> ToHashSet() => dctA.ToHashSet();

    public int Count => dctA.Count;

    public bool IsReadOnly => false;

    public BType this[AType item]
    {
      get => dctA[item];
      set
      {
        dctA[item] = value;
        dctB[value] = item;
      }
    }

    public AType this[BType item]
    {
      get => dctB[item];
      set
      {
        dctA[value] = item;
        dctB[item] = value;
      }
    }

    public void Add(AType a, BType b)
    {
      dctA.Add(a, b);
      dctB.Add(b, a);
    }

    public void Clear()
    {
      dctA.Clear();
      dctB.Clear();
    }

    public bool ContainsA(AType a) => dctA.ContainsKey(a);
    public bool ContainsB(BType b) => dctB.ContainsKey(b);

    public IEnumerator<KeyValuePair<AType, BType>> GetEnumerator() => dctA.GetEnumerator();

    public void Remove(AType a)
    {
      var b = dctA[a];
      dctA.Remove(a);
      dctB.Remove(b);
    }

    public void Remove(BType b)
    {
      var a = dctB[b];
      dctA.Remove(a);
      dctB.Remove(b);
    }
  }
}
