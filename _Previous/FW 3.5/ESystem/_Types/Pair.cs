using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace ESystem
{

  /// <summary>
  /// Represents pair of values, with values A and B.
  /// </summary>
  /// <typeparam name="T">First type</typeparam>
  /// <typeparam name="K">Second type</typeparam>
  /// <remarks>Overrides ToString() and Equals(...)</remarks>
  public class Pair<T, K>
  {
    private T _A;
    public T A
    {
      get { return _A; }
      set { _A = value; }
    }
    private K _B;
    public K B
    {
      get { return _B; }
      set { _B = value; }
    }

    /// <summary>
    /// Creates new instance with default empty values.
    /// </summary>
    /// <remarks></remarks>

    public Pair()
    {
    }

    /// <summary>
    /// Creates new instance with default values.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <remarks></remarks>
    public Pair(T a, K b)
    {
      this.A = a;
      this.B = b;
    }

    public override string ToString()
    {
      return "{" + A.ToString() + "; " + B.ToString() + "}";
    }

    public override bool Equals(object obj)
    {
      if ((obj is Pair<T, K>))
      {
        Pair<T, K> o = (Pair<T, K>)obj;
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
