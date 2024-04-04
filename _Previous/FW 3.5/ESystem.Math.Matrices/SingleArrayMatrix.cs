using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Math.Matrices
{
  public class SingleArrayMatrix : Matrix
  {
    private double[] inner;
    private int columnCount;
    private int rowCount;

    public SingleArrayMatrix(double[] value, int columnCount) : base (columnCount, CalculateRowCount(value, columnCount))
    {
      this.inner = value;
      this.columnCount = columnCount;
      this.rowCount = CalculateRowCount(this.inner, columnCount);
    }

    public SingleArrayMatrix(int rowCount, int columnCount) : base (rowCount, columnCount)
    {
      inner = new double[rowCount * columnCount];
      this.columnCount = columnCount;
      this.rowCount = CalculateRowCount(this.inner, columnCount);
    }

    public override int RowCount
    {
      get { return rowCount; }
    }

    public override int ColumnCount
    {
      get { return columnCount; }
    }

    public override double GetItemByIndex(int rowIndex, int columnIndex)
    {
      int index = CalculateRealIndex(rowIndex, columnIndex);
      return inner[index];
    }

    public override void SetItemByIndex(int rowIndex, int columnIndex, double value)
    {
      int index = CalculateRealIndex(rowIndex, columnIndex);
      inner[index] = value;
    }

    private int CalculateRealIndex(int rowIndex, int columnIndex)
    {
      int ret = columnCount * rowIndex + columnIndex;
      return ret;
    }

    protected override Matrix CloneEmpty(int rowCount, int columnCount)
    {
      Matrix ret = new SingleArrayMatrix(rowCount, columnCount);
      return ret;
    }

    private static int CalculateRowCount (int totalArrayLength, int columnCount)
    {
      int divRes = totalArrayLength % columnCount;
      if (divRes > 0)
        throw new EMatrixException("Single-array-matrix with totally " + totalArrayLength + " cannot be divided into rows with row length " + columnCount);
      return totalArrayLength / columnCount;
    }
    private static int CalculateRowCount (double [] array, int columnCount)
    {
      return CalculateRowCount(array.Length, columnCount);
    }

    public double[] GetInnerArray()
    {
      return inner;
    }
  }
}
