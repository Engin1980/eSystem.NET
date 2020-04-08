using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using ESystem.Extensions;

namespace Geometry
{

  /// <summary>
  /// Represents line (vector) from point to point.
  /// </summary>
  /// <remarks></remarks>
  public struct LineD
  {

    private const double ERR_PERC = 0.05;
    /// <summary>
    /// First end-point of line.
    /// </summary>
    /// <remarks></remarks>
    public readonly PointD X;
    /// <summary>
    /// Second end-point of line
    /// </summary>
    /// <remarks></remarks>
    public readonly PointD Y;
    /// <summary>
    /// Slope of line. A in y=ax+b.
    /// </summary>
    /// <remarks></remarks>
    public readonly double A;
    /// <summary>
    /// Intersection with Y-axis. B in y=ax+b.
    /// </summary>
    /// <remarks></remarks>

    public readonly double B;
    /// <summary>
    /// Initializes a new Instance of ESystem.Geometry.Line
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public LineD(PointD x, PointD y)
    {
      this.X = x;
      this.Y = y;

      this.A = getA(x, y);
      this.B = getB(x, this.A);
    }

    /// <summary>
    /// Initializes a new Instance of ESystem.Geometry.Line
    /// </summary>
    /// <param name="ax"></param>
    /// <param name="ay"></param>
    /// <param name="bx"></param>
    /// <param name="by"></param>
    public LineD(double ax, double ay, double bx, double @by)
      : this(new PointD(ax, ay), new PointD(bx, @by))
    {
    }

    private static double getA(PointD x, PointD y)
    {
      double ret = ((x.Y - y.Y) / (x.X - y.X));
      return ret;
    }
    private static double getB(PointD x, double a)
    {
      double ret = x.Y - (a * x.X);
      return ret;
    }
    private static double getB(PointD k, PointD l)
    {
      double ret = k.Y - (getA(k, l) * k.X);
      return ret;
    }

    /// <summary>
    /// Calculates Y for X.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public double GetY(double x)
    {
      double ret = 0;
      if ((IsVertical()))
      {
        ret = this.X.Y;
      }
      else
      {
        ret = A * x + B;
      }

      return ret;
    }
    /// <summary>
    /// Calculates X for Y.
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public double GetX(double y)
    {
      double ret = 0;
      if (IsVertical())
      {
        ret = this.X.X;
      }
      else
      {
        ret = (y - B) / A;
      }
      return ret;
    }

    /// <summary>
    /// Returns true if line is vertical (top-down).
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool IsVertical()
    {
      return double.IsInfinity(this.A);
    }
    /// <summary>
    /// Returns true if line is horizontal (left-right)
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool IsHorizontal()
    {
      return (this.A == 0);
    }

    /// <summary>
    /// Returns true if a point is on the line (with some precision; see overloads).
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool IsOnLine(PointD point)
    {
      return IsOnLine(point, ERR_PERC);
    }

    /// <summary>
    /// Returns true if a point is on the line (with some precision; see overloads).
    /// </summary>
    /// <param name="point"></param>
    /// <param name="acceptedErrorPercentage"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool IsOnLine(PointD point, double acceptedErrorPercentage)
    {
      bool ret = false;

      ret = point.X.IsBetween(X.X, Y.X);
      if (!ret)
      {
        return ret;
      }

      ret = point.Y.IsBetween(X.Y, Y.Y);
      if (!ret)
      {
        return ret;
      }

      ret = IsOnLineAxis(point, acceptedErrorPercentage);

      return ret;
    }

    /// <summary>
    /// Returns true if point is on the axix of the line (with some precision; see overloads).
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool IsOnLineAxis(PointD point)
    {
      return IsOnLineAxis(point, ERR_PERC);
    }

    /// <summary>
    /// Returns true if point is on the axix of the line.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="acceptedErrorPercentage">Requested precision (0 = absolute, 1 is 100% difference)</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool IsOnLineAxis(PointD point, double acceptedErrorPercentage)
    {
      bool ret = false;

      if (this.IsVertical())
      {
        ret = point.Y.IsBetween(X.Y, Y.Y);
      }
      else
      {
        double cY = GetY(point.X);

        if (cY.IsBetween(point.Y * (1 - acceptedErrorPercentage), point.Y * (1 + acceptedErrorPercentage)))
        {
          ret = true;
        }
        else
        {
          ret = false;
        }
      }

      return ret;
    }

    /// <summary>
    /// Returns intersection of of this line with other line, null if there is no intersection, random one,
    /// if there are multiple intersections.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public System.Nullable<PointD> GetIntersection(LineD other)
    {
      System.Nullable<PointD> ret = default(System.Nullable<PointD>);

      if (this.IsVertical())
      {
        if (other.IsVertical())
        {
          ret = GetBothVerticalIntersection(this, other);
        }
        else
        {
          ret = GetVerticalIntersection(this, other);
        }
      }
      else if (other.IsVertical())
      {
        ret = GetVerticalIntersection(other, this);
      }
      else if (this.IsHorizontal() && other.IsHorizontal())
      {
        ret = GetBothHorizotnalIntersection(this, other);
      }
      else
      {
        ret = GetSkewIntersection(this, other);
      }


      return ret;
    }

    private static System.Nullable<PointD> GetBothHorizotnalIntersection(LineD line, LineD other)
    {
      System.Nullable<PointD> ret = null;

      if (line.X.Y == other.X.Y)
      {
        if (line.X.X.IsBetween(other.X.X, other.Y.X))
        {
          ret = line.X;
        }
        else if (line.Y.X.IsBetween(other.X.X, other.Y.X))
        {
          ret = line.Y;
        }
        else
        {
          ret = null;
        }
      }
      else
      {
        ret = null;
      }

      return ret;
    }

    private static System.Nullable<PointD> GetHorizontalIntersection(LineD horizontal, LineD other)
    {
      System.Nullable<PointD> ret = null;

      ret = GetSkewIntersection(horizontal, other);

      return ret;
    }

    private static System.Nullable<PointD> GetBothVerticalIntersection(LineD line, LineD other)
    {
      System.Nullable<PointD> ret = null;

      if (line.X.X == other.X.X)
      {
        if (line.X.Y.IsBetween(other.X.Y, other.Y.Y))
        {
          ret = line.X;
        }
        else if (line.Y.Y.IsBetween(other.X.Y, other.Y.Y))
        {
          ret = line.Y;
        }
        else
        {
          ret = null;
        }
      }
      else
      {
        ret = null;
      }

      return ret;
    }

    private static System.Nullable<PointD> GetSkewIntersection(LineD line, LineD other)
    {
      System.Nullable<PointD> ret = default(System.Nullable<PointD>);

      double x = ((other.B - line.B) / (line.A - other.A));
      double y = other.A * x + other.B;

      ret = new PointD(x, y);

      if (!line.IsOnLine(ret.Value))
      {
        ret = null;
      }
      else if (!other.IsOnLine(ret.Value))
      {
        ret = null;
      }

      return ret;
    }

    private static System.Nullable<PointD> GetVerticalIntersection(LineD vertical, LineD other)
    {
      System.Nullable<PointD> ret = default(System.Nullable<PointD>);

      double y = other.A * vertical.X.X + other.B;
      ret = new PointD(vertical.X.X, y);

      if (!vertical.IsOnLine(ret.Value))
      {
        ret = null;
      }
      else if (!other.IsOnLine(ret.Value))
      {
        ret = null;
      }

      return ret;
    }
  }
}
