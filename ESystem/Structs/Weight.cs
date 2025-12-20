using ESystem.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ESystem.Structs
{
  /// <summary>
  /// Enum representing different units of weight.
  /// </summary>
  public enum WeightUnit
  {
    /// <summary>
    /// Grams (g), 1 gram = 0.001 kilograms.
    /// </summary>
    [Display(Name = "g", Order = 0)]
    Grams,

    /// <summary>
    /// Kilograms (kg), base unit in SI.
    /// </summary>
    [Display(Name = "kg", Order = 1)]
    Kilograms,

    /// <summary>
    /// Tons (t), 1 ton = 1000 kilograms.
    /// </summary>
    [Display(Name = "t", Order = 2)]
    Tons,

    /// <summary>
    /// Pounds (lb), 1 pound = 0.45359237 kilograms.
    /// </summary>
    [Display(Name = "lb", Order = 3)]
    Pounds
  }

  /// <summary>
  /// Represents a weight with a specific value and unit.
  /// </summary>
  public struct Weight : IXmlSerializable
  {
    public double Value { get; private set; }
    public WeightUnit Unit { get; private set; }

    public static Weight? Of(double? value, WeightUnit unit) => value == null ? null : Of(value.Value, unit);
    public static Weight Of(double value, WeightUnit unit) => new(value, unit);

    public Weight(double value, WeightUnit unit)
    {
      Value = value;
      Unit = unit;
    }

    /// <summary>
    /// Converts this weight to a target unit.
    /// </summary>
    public readonly Weight To(WeightUnit targetUnit)
    {
      if (Unit == targetUnit)
        return this;

      // Convert to kilograms first
      double valueInKg = Unit switch
      {
        WeightUnit.Grams => Value / 1000.0,
        WeightUnit.Kilograms => Value,
        WeightUnit.Tons => Value * 1000.0,
        WeightUnit.Pounds => Value * 0.45359237,
        _ => throw new UnexpectedEnumValueException(Unit)
      };

      // Convert from kilograms to target unit
      double result = targetUnit switch
      {
        WeightUnit.Grams => valueInKg * 1000.0,
        WeightUnit.Kilograms => valueInKg,
        WeightUnit.Tons => valueInKg / 1000.0,
        WeightUnit.Pounds => valueInKg / 0.45359237,
        _ => throw new UnexpectedEnumValueException(Unit)
      };

      return new Weight(result, targetUnit);
    }

    public override readonly string ToString() => $"{Value} {Enums.GetDisplayName(Unit)}";

    public override readonly bool Equals(object? obj) => obj is Weight w && w.Value == Value && w.Unit == Unit;

    public override readonly int GetHashCode() => HashCode.Combine(Value, Unit);

    public readonly XmlSchema? GetSchema() => null;
    private static readonly System.Globalization.CultureInfo enUs = System.Globalization.CultureInfo.GetCultureInfo("en-US");

    public void ReadXml(XmlReader reader)
    {
      string content = reader.ReadElementContentAsString();
      string[] pts = content.Split(' ');
      if (pts.Length == 0)
      {
        this.Value = 0;
        this.Unit = WeightUnit.Kilograms;
      }
      else if (pts.Length == 1)
      {
        this.Value = double.Parse(pts[0], enUs);
        this.Unit = WeightUnit.Kilograms;
      }
      else if (pts.Length == 2)
      {
        this.Value = double.Parse(pts[0], enUs);
        this.Unit = Enums.FromDisplayName<WeightUnit>(pts[1]) ?? throw new ApplicationException("Unknown weight unit " + pts[1]);
      }
      else
        throw new Exception($"Invalid format for Weight: {content}");
    }

    public readonly void WriteXml(XmlWriter writer)
    {
      string s = Value.ToString(enUs) + " " + Enums.GetDisplayName(Unit);
      writer.WriteString(s);
    }

    public static implicit operator double(Weight weight) => weight.Value;

    // Arithmetic operators with double
    public static Weight operator +(Weight weight, double value) => new(weight.Value + value, weight.Unit);

    public static Weight operator -(Weight weight, double value) => new(weight.Value - value, weight.Unit);

    public static Weight operator *(Weight weight, double value) => new(weight.Value * value, weight.Unit);

    public static Weight operator /(Weight weight, double value) => new(weight.Value / value, weight.Unit);

    // Comparison operators with double
    public static bool operator ==(Weight weight, double value) => weight.Value == value;

    public static bool operator !=(Weight weight, double value) => weight.Value != value;

    public static bool operator <(Weight weight, double value) => weight.Value < value;

    public static bool operator >(Weight weight, double value) => weight.Value > value;

    public static bool operator <=(Weight weight, double value) => weight.Value <= value;

    public static bool operator >=(Weight weight, double value) => weight.Value >= value;
  }

}
