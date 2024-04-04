using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Math.Matrices
{
  public class MultidimensionalArrayMatrix : Matrix
  {
    private double[,] inner = null;

    public MultidimensionalArrayMatrix(int rowCount, int columnCount)
      : base(rowCount, columnCount)
    {
      inner = new double[rowCount, columnCount];
    }
    public MultidimensionalArrayMatrix(double[,] data)
      : base(data.GetUpperBound(0) + 1, data.GetUpperBound(1) + 1)
    {
      this.inner = data;
    }

    public override int RowCount
    {
      get { return inner.GetUpperBound(0) + 1; }
    }

    public override int ColumnCount
    {
      get { return inner.GetUpperBound(1) + 1; }
    }

    public override double GetItemByIndex(int rowIndex, int columnIndex)
    {
      return inner[rowIndex, columnIndex];
    }

    public override void SetItemByIndex(int rowIndex, int columnIndex, double value)
    {
      inner[rowIndex, columnIndex] = value;
    }

    protected override Matrix CloneEmpty(int rowCount, int columnCount)
    {
      Matrix ret = new MultidimensionalArrayMatrix(rowCount, columnCount);
      return ret;
    }

    public double[,] GetInnerArray()
    {
      return inner;
    }
  }
}
