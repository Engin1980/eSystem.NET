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
  /// Enum representing different units of distance.
  /// </summary>
  public enum DistanceUnit
  {
    /// <summary>
    /// Millimeters (1 mm = 0.001 meters).
    /// </summary>
    Millimeters,  // Millimeters

    /// <summary>
    /// Meters (the base unit of length in the International System of Units).
    /// </summary>
    Meters,       // Meters

    /// <summary>
    /// Kilometers (1 km = 1000 meters).
    /// </summary>
    Kilometers,   // Kilometers

    /// <summary>
    /// Miles (1 mile = 1609.34 meters).
    /// </summary>
    Miles,        // Miles

    /// <summary>
    /// Nautical miles (1 nautical mile = 1852 meters).
    /// </summary>
    NauticalMiles, // Nautical miles

    /// <summary>
    /// Feet (1 foot = 0.3048 meters).
    /// </summary>
    Feet          // Feet
  }

  public readonly struct Distance
  {
    public double Value { get; }
    public DistanceUnit Unit { get; }

    public Distance(double value, DistanceUnit unit)
    {
      EAssert.Argument.IsTrue(value >= 0, nameof(value), $"Distance cannot be negative (provided: {value}).");
      Value = value;
      Unit = unit;
    }

    public override string ToString()
    {
      return $"{Value} {Unit}";
    }

    public Distance To(DistanceUnit targetUnit)
    {
      if (Unit == targetUnit)
        return this;

      double valueInMeters = Unit switch
      {
        DistanceUnit.Millimeters => Value / 1000,
        DistanceUnit.Meters => Value,
        DistanceUnit.Kilometers => Value * 1000,
        DistanceUnit.Miles => Value * 1609.34,
        DistanceUnit.NauticalMiles => Value * 1852,
        DistanceUnit.Feet => Value * 0.3048,
        _ => throw new UnexpectedEnumValueException(Unit)
      };

      double newValue = targetUnit switch
      {
        DistanceUnit.Millimeters => valueInMeters * 1000,
        DistanceUnit.Meters => valueInMeters,
        DistanceUnit.Kilometers => valueInMeters / 1000,
        DistanceUnit.Miles => valueInMeters / 1609.34,
        DistanceUnit.NauticalMiles => valueInMeters / 1852,
        DistanceUnit.Feet => valueInMeters / 0.3048,
        _ => throw new UnexpectedEnumValueException(Unit)
      };

      return new Distance(newValue, targetUnit);
    }

    public static Distance operator +(Distance a, Distance b)
    {
      var bConverted = b.To(a.Unit);
      return new Distance(a.Value + bConverted.Value, a.Unit);
    }

    public static Distance operator -(Distance a, Distance b)
    {
      var bConverted = b.To(a.Unit);
      return new Distance(a.Value - bConverted.Value, a.Unit);
    }

    public static Distance operator *(Distance d, double scalar) => new(d.Value * scalar, d.Unit);

    public static Distance operator *(double scalar, Distance d) => d * scalar;

    public static Distance operator /(Distance d, double scalar) => new(d.Value / scalar, d.Unit);

    public static bool operator ==(Distance a, Distance b) => a.To(DistanceUnit.Meters).Value == b.To(DistanceUnit.Meters).Value;

    public static bool operator !=(Distance a, Distance b) => !(a == b);

    public static bool operator <(Distance a, Distance b) => a.To(DistanceUnit.Meters).Value < b.To(DistanceUnit.Meters).Value;

    public static bool operator <=(Distance a, Distance b) => a.To(DistanceUnit.Meters).Value <= b.To(DistanceUnit.Meters).Value;

    public static bool operator >(Distance a, Distance b) => a.To(DistanceUnit.Meters).Value > b.To(DistanceUnit.Meters).Value;

    public static bool operator >=(Distance a, Distance b) => a.To(DistanceUnit.Meters).Value >= b.To(DistanceUnit.Meters).Value;

    public override bool Equals(object? obj) => obj is Distance distance && this == distance;

    public override int GetHashCode() => HashCode.Combine(Value, Unit);

    public static implicit operator double(Distance distance) => distance.Value;
  }
}
