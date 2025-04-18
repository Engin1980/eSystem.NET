using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Structs
{
  using System;

  public readonly struct Heading
  {
    public double Value { get; }

    public Heading(double value)
    {
      Value = Normalize(value);
    }

    private static double Normalize(double value) => (value % 360 + 360) % 360; // Ensures value is in [0, 360)

    public override string ToString() => $"{Value}°";

    public override bool Equals(object? obj) => obj is Heading other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Heading a, Heading b) => a.Value == b.Value;

    public static bool operator !=(Heading a, Heading b) => !(a == b);

    public static Heading operator +(Heading a, double degrees) => new(a.Value + degrees);

    public static Heading operator -(Heading a, double degrees) => new(a.Value - degrees);

    /// <summary>
    /// Checks if the heading is in arc sector FROM lower TO upper (direction is important), inclusive.
    /// </summary>
    /// <param name="lower">Lower arc boundary.</param>
    /// <param name="upper">Upper arc boundary.</param>
    /// <returns>True if lower -> this -> upper; False otherwise.</returns>
    public bool IsBetween(Heading lower, Heading upper) => IsBetween(lower.Value, Value, upper.Value);

    /// <summary>
    /// Checks if the heading is in arc sector FROM lower TO upper (direction is important), inclusive.
    /// </summary>
    /// <param name="lower">Lower arc boundary.</param>
    /// <param name="upper">Upper arc boundary.</param>
    /// <returns>True if lower -> this -> upper; False otherwise.</returns>
    public bool IsBetween(double lower, double upper)
    {
      // Normalize the values to ensure they're in [0, 360)
      lower = Normalize(lower);
      upper = Normalize(upper);

      // Handle cases where lower > upper (wrap around)
      if (lower > upper)
        return Value >= lower || Value <= upper;

      return Value >= lower && Value <= upper;
    }

    /// <summary>
    /// Checks if the heading is in arc sector FROM lower TO upper (direction is important), inclusive.
    /// </summary>
    /// <param name="lower">Lower arc boundary.</param>
    /// /// <param name="current">Current heading.</param>
    /// <param name="upper">Upper arc boundary.</param>
    /// <returns>True if lower -> this -> upper; False otherwise.</returns>
    public static bool IsBetween(Heading lower, Heading current, Heading upper)  => IsBetween(lower.Value, current.Value, upper.Value);

    /// <summary>
    /// Checks if the heading is in arc sector FROM lower TO upper (direction is important), inclusive.
    /// </summary>
    /// <param name="lower">Lower arc boundary.</param>
    /// /// <param name="current">Current heading.</param>
    /// <param name="upper">Upper arc boundary.</param>
    /// <returns>True if lower -> this -> upper; False otherwise.</returns>
    public static bool IsBetween(double lower, double current, double upper)
    {
      // Normalize the values to ensure they're in [0, 360)
      lower = Normalize(lower);
      upper = Normalize(upper);

      // Handle cases where lower > upper (wrap around)
      if (lower > upper)
        return current >= lower || current <= upper;

      return current >= lower && current <= upper;
    }

  }
}
