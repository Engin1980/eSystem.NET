using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Math.Matrices
{
  public class ArrayOfArrayMatrix : Matrix
  {
    private double[][] inner;

    public ArrayOfArrayMatrix(double[][] value) : base (value.Length, value[0].Length)
    {
      this.inner = value;
    }

    public ArrayOfArrayMatrix(int rowCount, int columnCount)
      : base(rowCount, columnCount)
    {
      inner = new double[rowCount][];
      for (int i = 0; i < inner.Length; i++)
      {
        inner[i] = new double[columnCount];
      }
    }

    public override int RowCount
    {
      get { return inner.Length; }
    }

    public override int ColumnCount
    {
      get { return inner[0].Length; }
    }

    public override double GetItemByIndex(int rowIndex, int columnIndex)
    {
      return inner[rowIndex][columnIndex];
    }

    public override void SetItemByIndex(int rowIndex, int columnIndex, double value)
    {
      inner[rowIndex][columnIndex] = value;
    }

    protected override Matrix CloneEmpty(int rowCount, int columnCount)
    {
      Matrix ret = new ArrayOfArrayMatrix(rowCount, columnCount);
      return ret;
    }

    public double[][] GetInnerArray()
    {
      return inner;
    }
  }
}
