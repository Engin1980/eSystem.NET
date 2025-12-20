//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ESystem.Miscelaneous
//{
//  public class BiDictionary<Type> where Type : notnull
//  {
//    private readonly Dictionary<Type, Type> dctA = new();
//    private readonly Dictionary<Type, Type> dctB = new();

//    public ICollection<Type> As => dctA.Keys;

//    public ICollection<Type> Bs => dctB.Keys;

//    public int Count => dctA.Count;

//    public bool IsReadOnly => false;

//    public Type GetB(Type a) => dctA[a];
//    public Type GetA(Type b) => dctB[b];

//    public void Add(Type a, Type b)
//    {
//      dctA.Add(a, b);
//      dctB.Add(b, a);
//    }

//    public void Clear()
//    {
//      dctA.Clear();
//      dctB.Clear();
//    }

//    public bool ContainsA(Type a) => dctA.ContainsKey(a);
//    public bool ContainsB(Type b) => dctB.ContainsKey(b);

//    public IEnumerator<KeyValuePair<AType, BType>> GetEnumerator() => dctA.GetEnumerator();

//    public void RemoveA(AType a)
//    {
//      var b = dctA[a];
//      dctA.Remove(a);
//      dctB.Remove(b);
//    }

//    public void RemoveB(BType b)
//    {
//      var a = dctB[b];
//      dctA.Remove(a);
//      dctB.Remove(b);
//    }
//  }
//}
