using ESystem.Asserting;
using ESystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

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
    [Display(Name = "mm")]
    Millimeters,  // Millimeters

    /// <summary>
    /// Meters (the base unit of length in the International System of Units).
    /// </summary>
    [Display(Name = "m")]
    Meters,       // Meters

    /// <summary>
    /// Kilometers (1 km = 1000 meters).
    /// </summary>
    [Display(Name = "km")]
    Kilometers,   // Kilometers

    /// <summary>
    /// Miles (1 mile = 1609.34 meters).
    /// </summary>
    [Display(Name = "mi")]
    Miles,        // Miles

    /// <summary>
    /// Nautical miles (1 nautical mile = 1852 meters).
    /// </summary>
    [Display(Name = "NM")]
    NauticalMiles, // Nautical miles

    /// <summary>
    /// Feet (1 foot = 0.3048 meters).
    /// </summary>
    [Display(Name = "ft")]
    Feet          // Feet
  }

  public struct Distance : IXmlSerializable
  {
    public double Value { get; private set; }
    public DistanceUnit Unit { get; private set; }

    public static Distance? Of(double? value, DistanceUnit unit) => value == null ? null : Of(value.Value, unit);
    public static Distance Of(double value, DistanceUnit unit) => new(value, unit);

    public Distance(double value, DistanceUnit unit)
    {
      EAssert.Argument.IsTrue(value >= 0, nameof(value), $"Distance cannot be negative (provided: {value}).");
      Value = value;
      Unit = unit;
    }

    public readonly override string ToString()
    {
      return $"{Value} {Unit}";
    }

    public readonly Distance To(DistanceUnit targetUnit)
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

    public readonly override bool Equals(object? obj) => obj is Distance distance && this == distance;

    public readonly override int GetHashCode() => HashCode.Combine(Value, Unit);

    public static implicit operator double(Distance distance) => distance.Value;

    public readonly XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
      string content = reader.ReadElementContentAsString();
      string[] pts = content.Split(' ');
      if (pts.Length == 0)
      {
        this.Value = 0;
        this.Unit = DistanceUnit.Meters;
      }
      else if (pts.Length == 1)
      {
        this.Value = double.Parse(pts[0], enUs);
        this.Unit = DistanceUnit.Meters;
      }
      else if (pts.Length == 2)
      {
        this.Value = double.Parse(pts[0], enUs);
        this.Unit = Enums.FromDisplayName<DistanceUnit>(pts[1]) ?? throw new ApplicationException("Unknown distance unit " + pts[1]);
      }
      else
        throw new Exception($"Invalid format for Weight: {content}");
    }

    private static readonly System.Globalization.CultureInfo enUs = System.Globalization.CultureInfo.GetCultureInfo("en-US");

    public readonly void WriteXml(XmlWriter writer)
    {
      string s = Value.ToString(enUs) + " " + Enums.GetDisplayName(Unit);
      writer.WriteString(s);
    }
  }
}
