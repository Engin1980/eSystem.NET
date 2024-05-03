using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Miscelaneous
{
  public readonly struct Percentage
  {

    #region Fields

    private readonly double _Value;

    #endregion Fields

    #region Constructors

    public Percentage(double value)
    {
      if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
      if (value > 1) throw new ArgumentOutOfRangeException(nameof(value));
      this._Value = value;
    }

    #endregion Constructors

    #region Methods

    public static implicit operator double(Percentage value) => value._Value;

    public static Percentage Of(int value, bool validateRange = false)
          => validateRange ? new Percentage(value / 100.0) : new Percentage(EnsureInRange(value / 100.0));

    public static Percentage Of(double value, bool validateRange = false)
      => validateRange ? new Percentage(value) : new Percentage(EnsureInRange(value));

    public static Percentage Of(int value, int maximum, bool validateRange = false)
      => validateRange ? new Percentage(value / maximum) : new Percentage(EnsureInRange(value / maximum));

    public static Percentage Of(int value, int minimum, int maximum, bool validateRange = false)
      => validateRange
        ? new Percentage((value - minimum) / (maximum - minimum))
      : new Percentage(EnsureInRange((value - minimum) / (maximum - minimum)));

    public static Percentage operator -(Percentage left, Percentage right) => Percentage.Of(left._Value - right._Value, false);

    public static bool operator !=(Percentage left, Percentage right) => !left.Equals(right);

    public static Percentage operator *(Percentage left, double right) => Percentage.Of(left._Value * right, false);

    public static Percentage operator *(double left, Percentage right) => Percentage.Of(left * right._Value, false);

    public static Percentage operator /(Percentage left, double right) => Percentage.Of(left._Value / right, false);

    public static Percentage operator +(Percentage left, Percentage right) => Percentage.Of(left._Value + right._Value, false);

    public static bool operator <(Percentage left, Percentage right) => left._Value < right._Value;

    public static bool operator <=(Percentage left, Percentage right) => left._Value <= right._Value;

    public static bool operator ==(Percentage left, Percentage right) => left.Equals(right);
    public static bool operator >(Percentage left, Percentage right) => left._Value > right._Value;
    public static bool operator >=(Percentage left, Percentage right) => left._Value >= right._Value;
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
      return (obj is Percentage p) && p._Value == this._Value;
    }

    public override readonly int GetHashCode() => this._Value.GetHashCode();

    public double ToDouble() => this._Value;

    public int ToInt() => (int)this._Value * 100;
    public override readonly string ToString() => ToString("P");

    public readonly string ToString(string format)
    {
      string fullFormat = "{0:" + format + "}";
      string ret = string.Format(fullFormat, this._Value);
      return ret;
    }

    private static double EnsureInRange(double value) => Math.Max(0, Math.Min(1, value));

    #endregion Methods

  }
}
