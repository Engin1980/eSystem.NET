using ESystem.Asserting;
using ESystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Structs
{
  /// <summary>
  /// Enum representing different units of speed.
  /// </summary>
  public enum SpeedUnit
  {
    /// <summary>
    /// Kilometers per hour.
    /// </summary>
    KPH,   // Kilometers per hour

    /// <summary>
    /// Miles per hour.
    /// </summary>
    MPH,   // Miles per hour

    /// <summary>
    /// Knots (nautical miles per hour).
    /// </summary>
    KTS,   // Knots (nautical miles per hour)

    /// <summary>
    /// Meters per second.
    /// </summary>
    MPS,   // Meters per second

    /// <summary>
    /// Feet per second.
    /// </summary>
    FPS,   // Feet per second

    /// <summary>
    /// Feet per minute.
    /// </summary>
    FPM    // Feet per minute
  }

  public readonly struct Speed
  {
    public double Value { get; }
    public SpeedUnit Unit { get; }

    public Speed(double value, SpeedUnit unit)
    {
      EAssert.Argument.IsTrue(value >= 0, nameof(value), $"Speed cannot be negative (provided: {value}.");
      Value = value;
      Unit = unit;
    }

    public override string ToString() =>
        $"{Value} {Unit}";

    public Speed To(SpeedUnit targetUnit)
    {
      if (Unit == targetUnit)
        return this;

      // Convert to meters per second first
      double valueInMPS = Unit switch
      {
        SpeedUnit.KPH => Value / 3.6,
        SpeedUnit.MPH => Value * 0.44704,
        SpeedUnit.KTS => Value * 0.514444,
        SpeedUnit.MPS => Value,
        SpeedUnit.FPS => Value * 0.3048,  // Convert FPS to MPS
        SpeedUnit.FPM => Value / 60 * 0.3048, // Convert FPM to MPS
        _ => throw new UnexpectedEnumValueException(Unit)
      };

      // Then convert from meters per second to target unit
      double newValue = targetUnit switch
      {
        SpeedUnit.KPH => valueInMPS * 3.6,
        SpeedUnit.MPH => valueInMPS / 0.44704,
        SpeedUnit.KTS => valueInMPS / 0.514444,
        SpeedUnit.MPS => valueInMPS,
        SpeedUnit.FPS => valueInMPS / 0.3048,  // Convert MPS to FPS
        SpeedUnit.FPM => valueInMPS / 0.3048 * 60,  // Convert MPS to FPM
        _ => throw new UnexpectedEnumValueException(Unit)
      };

      return new Speed(newValue, targetUnit);
    }

    public static implicit operator double(Speed speed) => speed.Value;

    public static Speed operator +(Speed speed, double value) => new(speed.Value + value, speed.Unit);

    public static Speed operator -(Speed speed, double value) => new(speed.Value - value, speed.Unit);

    public static Speed operator *(Speed speed, double value) => new(speed.Value * value, speed.Unit);

    public static Speed operator /(Speed speed, double value) => new(speed.Value / value, speed.Unit);

    public static bool operator ==(Speed speed, double value) => speed.Value == value;

    public static bool operator !=(Speed speed, double value) => speed.Value != value;

    public static bool operator <(Speed speed, double value) => speed.Value < value;

    public static bool operator >(Speed speed, double value) => speed.Value > value;

    public static bool operator <=(Speed speed, double value) => speed.Value <= value;

    public static bool operator >=(Speed speed, double value) => speed.Value >= value;

    public override bool Equals(object? obj) => obj is Speed speed && this == speed;

    public override int GetHashCode() => HashCode.Combine(Value, Unit);
  }

}
