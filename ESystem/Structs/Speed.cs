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
  /// Enum representing different units of speed.
  /// </summary>
  public enum SpeedUnit
  {
    /// <summary>
    /// Kilometers per hour.
    /// </summary>
    [Display(Name = "km/h")]
    KPH,   // Kilometers per hour

    /// <summary>
    /// Miles per hour.
    /// </summary>
    [Display(Name = "NM/h")]
    NMPH,   // Miles per hour

    /// <summary>
    /// Knots (nautical miles per hour).
    /// </summary>
    [Display(Name = "kts")]
    KTS,   // Knots (nautical miles per hour)

    /// <summary>
    /// Meters per second.
    /// </summary>
    [Display(Name = "m/s")]
    MPS,   // Meters per second

    /// <summary>
    /// Feet per second.
    /// </summary>
    [Display(Name = "ft/s")]
    FPS,   // Feet per second

    /// <summary>
    /// Feet per minute.
    /// </summary>
    [Display(Name = "ft/m")]
    FPM    // Feet per minute
  }

  public struct Speed : IXmlSerializable
  {
    public double Value { get; private set; }
    public SpeedUnit Unit { get; private set; }

    public static Speed? Of(double? value, SpeedUnit unit) => value == null ? null : Of(value.Value, unit);
    public static Speed Of(double value, SpeedUnit unit) => new(value, unit);

    public Speed(double value, SpeedUnit unit)
    {
      EAssert.Argument.IsTrue(value >= 0, nameof(value), $"Speed cannot be negative (provided: {value}.");
      Value = value;
      Unit = unit;
    }

    public readonly override string ToString() =>
        $"{Value} {Unit}";

    public readonly Speed To(SpeedUnit targetUnit)
    {
      if (Unit == targetUnit)
        return this;

      // Convert to meters per second first
      double valueInMPS = Unit switch
      {
        SpeedUnit.KPH => Value / 3.6,
        SpeedUnit.NMPH => Value * 0.44704,
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
        SpeedUnit.NMPH => valueInMPS / 0.44704,
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

    public readonly override bool Equals(object? obj) => obj is Speed speed && this == speed;

    public readonly override int GetHashCode() => HashCode.Combine(Value, Unit);

    public readonly XmlSchema? GetSchema() => null;

    private static readonly System.Globalization.CultureInfo enUs = System.Globalization.CultureInfo.GetCultureInfo("en-US");

    public void ReadXml(XmlReader reader)
    {
      string content = reader.ReadElementContentAsString();
      string[] pts = content.Split(' ');
      if (pts.Length == 0)
      {
        this.Value = 0;
        this.Unit = SpeedUnit.KTS;
      }
      else if (pts.Length == 1)
      {
        this.Value = double.Parse(pts[0], enUs);
        this.Unit = SpeedUnit.KTS;
      }
      else if (pts.Length == 2)
      {
        this.Value = double.Parse(pts[0], enUs);
        this.Unit = Enums.FromDisplayName<SpeedUnit>(pts[1]) ?? throw new Exception("Unknown speed unit " + pts[1]);
      }
      else
        throw new Exception($"Invalid format for Weight: {content}");
    }

    public readonly void WriteXml(XmlWriter writer)
    {
      string s = Value.ToString(enUs) + " " + Enums.GetDisplayName(Unit);
      writer.WriteString(s);
    }
  }

}
