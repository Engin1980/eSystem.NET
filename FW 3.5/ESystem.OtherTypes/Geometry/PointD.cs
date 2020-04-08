using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Geometry
{

  /// <summary>
  /// Represents point with double-precision.
  /// </summary>
  /// <remarks></remarks>
  public struct PointD
  {

    /// <summary>
    /// X axis value
    /// </summary>
    /// <remarks></remarks>
    public readonly double X;
    /// <summary>
    /// Y axis value
    /// </summary>
    /// <remarks></remarks>

    public readonly double Y;
    /// <summary>
    /// Initializes a new Instance of ESystem.Geometry.PointD
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public PointD(double x, double y)
    {
      this.X = x;
      this.Y = y;
    }

    public override string ToString()
    {
      return (X.ToString() + "; ") + Y.ToString();
    }

    /// <summary>
    /// Converts implicitly Point to PointD.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static implicit operator PointD(Point point)
    {
      return new PointD(point.X, point.Y);
    }

    /// <summary>
    /// Converts explicitly PointD to Point.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static explicit operator Point(PointD point)
    {
      return new Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
    }

    public static bool operator ==(PointD a, PointD b)
    {
      return a.Equals(b);
    }
    public static bool operator !=(PointD a, PointD b)
    {
      return !a.Equals(b);
    }

    public override bool Equals(object obj)
    {
      bool ret = false;

      if (obj is PointD)
      {
        PointD other = (PointD)obj;
        ret = (this.X == other.X) && (this.Y == other.Y);
      }
      else
      {
        ret = false;
      }

      return ret;
    }

    public override int GetHashCode()
    {
      return X.GetHashCode() ^ Y.GetHashCode();
    }

    //Public Function CompareTo(ByVal other As PointD) As Integer _
    //  Implements IComparable(Of ESystem.Geometry.PointD).CompareTo



    //End Function
  }
}
