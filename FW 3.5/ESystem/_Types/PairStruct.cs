using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace ESystem
{

/// <summary>
/// Represents pair of two values. Ummutable structure.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="K"></typeparam>
/// <remarks></remarks>
public struct PairStruct<T, K>
{
  public readonly T A;

  public readonly K B;
  public PairStruct(T a, K b)
  {
    this.A = a;    this.B = b;
  }

  public override string ToString()
  {
    return "(" + A.ToString() + "; " + B.ToString() + ")";
  }
  public override bool Equals(object obj)
  {
    if ((obj is PairStruct<T, K>))
    {
      PairStruct<T, K> o = (PairStruct<T, K>)obj;
      return A.Equals(o.A) && (B.Equals(o.B));
    }
    else
    {
      return false;
    }
  }

  public override int GetHashCode()
  {
    return A.GetHashCode() ^ B.GetHashCode();
  }

}

}
