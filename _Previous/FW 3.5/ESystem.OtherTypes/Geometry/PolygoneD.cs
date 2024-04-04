using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Geometry
{
  /// <summary>
  /// Represents polygone of double points.
  /// </summary>
  /// <remarks></remarks>
  public class PolygoneD
  {

    /// <summary>
    /// Private field for property Points
    /// </summary>
    /// <default>New EList(Of PointD)</default>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private List<PointD> _Points = new List<PointD>();
    /// <summary>
    /// Defines Points
    /// </summary>
    /// <remarks>Default value is New EList(Of PointD)</remarks>
    /// <default>New EList(Of PointD)</default>
    public List<PointD> Points
    {
      get { return _Points; }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("Cannot insert null into points.");
        }
        _Points = value;
      }
    }

    /// <summary>
    /// Returns set of lines creating polygon. If last point is different from first, polygon is
    /// automatically closed.
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public LineD[] GetLines()
    {
      var ret = new List<LineD>();

      PointD start = default(PointD);
      int startIndex = 0;

      if (Points[0] == Points[Points.Count - 1])
      {
        start = Points[0];
        startIndex = 1;
      }
      else
      {
        start = Points[Points.Count - 1];
        startIndex = 0;
      }

      for (int i = startIndex; i <= Points.Count; i++)
      {
        ret.Add(new LineD(start, Points[i]));
        start = Points[i];
      }

      return ret.ToArray();
    }

    /// <summary>
    /// Returns true if point is inside of polygon.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool IsInside(PointD point)
    {
      bool ret = false;

      LineD genLine = new LineD(new PointD(int.MinValue, point.Y), point);

      LineD[] lines = this.GetLines();

      int intersectionsCount = 0;

      for (int i = 0; i <= lines.Length; i++)
      {
        if ((lines[i].GetIntersection(genLine).HasValue))
        {
          intersectionsCount += 1;
        }
      }


      if ((intersectionsCount % 2 == 0))
      {
        ret = false;
      }
      else
      {
        ret = true;
      }

      return ret;
    }

  }
}
