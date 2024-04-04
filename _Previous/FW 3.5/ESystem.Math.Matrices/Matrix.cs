

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Math.Matrices
{
  public abstract class Matrix
  {
    public static int MAX_LISTING_TO_STRING_SIZE_ROWS = 50;
    public static int MAX_LISTING_TO_STRING_SIZE_COLUMNS = 20;
    public static string FORMATTING_STRING = "N3";

    #region Fields

    #endregion Fields

    #region Properties

    public double this[int row, int column]
    {
      get
      {
        if (row < 1 || row > RowCount)
          throw new Exceptions.InvalidDimensionException(row, 1, RowCount);
        if (column < 1 || column > ColumnCount)
          throw new Exceptions.InvalidDimensionException(column, 1, ColumnCount);

        return GetItemByCoordinate(row, column);
      }
      set
      {
        if (row < 1 || row > RowCount)
          throw new Exceptions.InvalidDimensionException(row, 1, RowCount);
        if (column < 1 || column > ColumnCount)
          throw new Exceptions.InvalidDimensionException(column, 1, ColumnCount);

        SetItemByCoordinate(row, column, value);
      }
    }

    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }
#if FW40
    public Tuple<int, int> Dimension { get { return new Tuple<int, int>(RowCount, ColumnCount); } }
#endif

    #endregion Properties

    #region .ctor

    protected Matrix(int rowCount, int columnCount)
    {
      if (rowCount < 1)
        throw new EMatrixException("Row-count must be greater than 0.");
      if (columnCount < 1)
        throw new EMatrixException("Column-count must be greater than 0.");
    }

    #endregion .ctor

    #region Public methods - instance

    #region Access

    public double GetItemByCoordinate(int row, int column)
    {
      return GetItemByIndex(row - 1, column - 1);
    }
    public void SetItemByCoordinate(int row, int column, double value)
    {
      SetItemByIndex(row - 1, column - 1, value);
    }

    public abstract double GetItemByIndex(int rowIndex, int columnIndex);
    public abstract void SetItemByIndex(int rowIndex, int columnIndex, double value);

    #endregion Access

    #region Behavioral methods

    protected abstract Matrix CloneEmpty(int rowCount, int columnCount);
    protected Matrix CloneEmpty()
    {
      Matrix ret = this.CloneEmpty(this.RowCount, this.ColumnCount);
      return ret;
    }
    public Matrix Clone()
    {
      Matrix ret = this.CloneEmpty();
      Matrix.Copy(this, ret);
      return ret;
    }

    public void Reset(double valueToSet)
    {
      this.ApplyFilter(() => valueToSet);
    }

    public string GetDimensionsToString(string delimiter)
    {
      return RowCount + delimiter + ColumnCount;
    }
    public string GetDimensionsToString()
    {
      return GetDimensionsToString("x");
    }
    public override string ToString()
    {
      //if (this.RowCount > MAX_LISTING_TO_STRING_SIZE_ROWS || this.ColumnCount > MAX_LISTING_TO_STRING_SIZE_COLUMNS)
        return "Matrix {" + GetDimensionsToString() + "}";
      //else
      //  return ToFormattedString();
    }
    public string ToFormattedString()
    {
      StringBuilder rowSb = new StringBuilder();
      StringBuilder sb = new StringBuilder();

      for (int i = 0; i < RowCount; i++)
      {
        rowSb = new StringBuilder();
        for (int j = 0; j < ColumnCount; j++)
        {
          if (j > 0) rowSb.Append ("\\t");
          rowSb.Append(this.GetItemByIndex(i, j).ToString(FORMATTING_STRING));
        }
        sb.AppendLine(rowSb.ToString());
      }

      return sb.ToString();
    }

    public Matrix CopyOut(int startRowIndex, int rowCount, int startColumnIndex, int columnCount)
    {
      Matrix ret = this.CloneEmpty(rowCount, columnCount);
      Matrix.Copy(this, startRowIndex, startColumnIndex, ret, 0, 0, rowCount, columnCount);
      return ret;
    }

    public void ApplyFilter(Func<double, double> filterOperation)
    {
      for (int i = 0; i < RowCount; i++)
        for (int j = 0; j < ColumnCount; j++)
          this[i, j] = filterOperation(this[i, j]);
    }
    public void ApplyFilter(Func<double> filterOperation)
    {
      for (int i = 0; i < RowCount; i++)
        for (int j = 0; j < ColumnCount; j++)
          this[i, j] = filterOperation();
    }

    public double[] ToSingleArray()
    {
      SingleArrayMatrix trg = new SingleArrayMatrix(RowCount, ColumnCount);
      Matrix.Copy(this, trg);

      double[] ret = trg.GetInnerArray();
      return ret;
    }
    public double [][] ToArrayOfArrays()
    {
      ArrayOfArrayMatrix trg = new ArrayOfArrayMatrix(RowCount, ColumnCount);
      Matrix.Copy(this, trg);

      double[][] ret = trg.GetInnerArray();
      return ret;
    }
    public double[,] ToMultidimensionalArrays()
    {
      MultidimensionalArrayMatrix trg = new MultidimensionalArrayMatrix(RowCount, ColumnCount);
      Matrix.Copy(this, trg);

      double[,] ret = trg.GetInnerArray();
      return ret;
    }

    #endregion Behavioral methods

    #region Operations

    public Matrix Transpond()
    {
      Matrix ret = this.CloneEmpty(ColumnCount, RowCount);

      for (int i = 1; i <= this.RowCount; i++)
        for (int j = 1; j <= this.ColumnCount; j++)
          ret[j, i] = this[i, j];

      return ret;
    }

    public static Matrix MultiplyPerItem(Matrix a, Matrix b)
    {
      Matrix ret = Matrix.Calculate(a, b, (x, y) => x * y);
      return ret;
    }
    public static Matrix DividePerItem(Matrix a, Matrix b)
    {
      Matrix ret = Matrix.Calculate(a, b, (x, y) => x / y);
      return ret;
    }

    #endregion Operations

    #endregion Public methods - instance

    #region Public methods - static

    public static void Copy(Matrix source, Matrix target)
    {
      if (AreDimensionsEqual(source, target) == false)
        throw new Exceptions.InvalidDimensionException(source, target);

      Matrix.Copy(source, 0, 0, target, 0, 0, source.RowCount, source.ColumnCount);
    }
    public static void Copy(Matrix source, int sourceStartRowIndex, int sourceStartColumnIndex,
        Matrix target, int targetStartRowIndex, int targetStartColumnIndex,
        int rowCount, int columnCount)
    {
      for (int i = 1; i <= rowCount; i++)
        for (int j = 1; j <= columnCount; j++)
          target[targetStartRowIndex + i, targetStartColumnIndex + j] = source[sourceStartRowIndex + i, sourceStartColumnIndex + j];
    }

    public static void ForAll(Matrix a, Action<double> itemAction)
    {
      for (int i = 1; i <= a.RowCount; i++)
      {
        for (int j = 1; j <= a.ColumnCount; j++)
        {
          itemAction(a[i, j]);
        }
      }
    }

    public static Matrix Calculate(Matrix a, Matrix b, Func<double, double, double> itemOperation)
    {
      Matrix ret = a.CloneEmpty();
      Matrix.Calculate(a, b, ret, itemOperation);
      return ret;
    }

    public static Matrix Calculate(Matrix a, Func<double, double> itemOperation)
    {
      Matrix ret = a.CloneEmpty();
      Matrix.Calculate(a, ret, itemOperation);
      return ret;
    }

    public static void Calculate(Matrix a, Matrix b, Matrix target, Func<double, double, double> itemOperation)
    {
      if (AreDimensionsEqual(a, b) == false)
        throw new Exceptions.InvalidDimensionException(a, b);
      if (AreDimensionsEqual(a, target) == false)
        throw new Exceptions.InvalidDimensionException(a, target);

      for (int i = 01; i <= a.RowCount; i++)
        for (int j = 1; j <= a.ColumnCount; j++)
          target[i, j] = itemOperation(a[i, j], b[i, j]);
    }

    public static void Calculate(Matrix a, Matrix target, Func<double, double> itemOperation)
    {
      for (int i = 1; i <= a.RowCount; i++)
        for (int j = 1; j <= a.ColumnCount; j++)
          target[i, j] = itemOperation(a[i, j]);
    }

    public static Matrix CreateNewMatrix(int rowCount, int columnCount)
    {
      Matrix ret = new MultidimensionalArrayMatrix(rowCount, columnCount);
      return ret;
    }

    public static bool AreDimensionsEqual(Matrix a, Matrix b)
    {
      bool ret = false;
#if FW35
      ret = (a.ColumnCount == b.ColumnCount) && (a.RowCount == b.RowCount);
#else
  ret = a.Dimension == b.Dimension;
#endif
      return ret;
    }


    public static bool AreEqual(Matrix a, Matrix b)
    {
      int c;
      int d;
      return Matrix.AreEqual(a, b, out c, out d);
    }
    public static bool AreEqual(Matrix a, Matrix b, out int unequalRow, out int unequalColumn)
    {
      bool ret = true;
      unequalColumn = -1;
      unequalRow = -1;

      ret = AreDimensionsEqual(a, b);
      if (ret == true)
      {
        for (int i = 1; i <= a.RowCount; i++)
          for (int j = 1; j <= a.ColumnCount; j++)
          {
            if (a[i, j] != b[i, j])
            {
              unequalRow = i;
              unequalColumn = j;
              ret = false;
              goto returnValue;
            }
          }
      }

    returnValue:
      return ret;
    }

    #endregion Public methods - static

    #region Operators

    public static Matrix operator +(Matrix a, Matrix b)
    {
      if (AreDimensionsEqual(a, b) == false)
        throw new Exceptions.InvalidDimensionException(a, b);

      Matrix ret = Matrix.Calculate(a, b, (x, y) => x + y);
      return ret;
    }
    public static Matrix operator +(Matrix a, double b)
    {
      Matrix ret = Matrix.Calculate(a, x => x + b);
      return ret;
    }
    public static Matrix operator +(double a, Matrix b)
    {
      Matrix ret = b + a;
      return ret;
    }
    public static Matrix operator -(Matrix a, Matrix b)
    {
      if (AreDimensionsEqual(a, b) == false)
        throw new Exceptions.InvalidDimensionException(a, b);

      Matrix ret = Matrix.Calculate(a, b, (x, y) => x - y);
      return ret;
    }
    public static Matrix operator -(Matrix a, double b)
    {
      Matrix ret = Matrix.Calculate(a, x => x - b);
      return ret;
    }
    public static Matrix operator -(double a, Matrix b)
    {
      Matrix ret = Matrix.Calculate(b, x => a - x);
      return ret;
    }
    public static Matrix operator *(Matrix a, double b)
    {
      Matrix ret = Matrix.Calculate(a, x => x * b);
      return ret;
    }
    public static Matrix operator *(double a, Matrix b)
    {
      Matrix ret = b * a;
      return ret;
    }
    public static Matrix operator /(Matrix a, double b)
    {
      if (b == 0) throw new DivideByZeroException();

      Matrix ret = Matrix.Calculate(a, x => x / b);
      return ret;
    }
    public static Matrix operator /(double a, Matrix b)
    {
      Matrix ret = Matrix.Calculate(b, x => a / x);
      return ret;
    }

    #endregion Operators

    #region Private methods

    #endregion Private methods

  }
}
