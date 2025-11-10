using ESystem.Asserting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESystem.WPF.KeyHooking;

/// <summary>
/// Represents a chord of one or more keyboard keys combined with optional modifier keys. Note that modifier keys are the same for all keys in the chord.
/// </summary>
public readonly struct KeyChord
{

  /// <summary>
  /// Provides formatting and parsing helpers for <see cref="KeyChord"/>.
  /// Serialized form used by this helper is: "&lt;ModifierKeys&gt;|&lt;Key1&gt;+&lt;Key2&gt;+...".
  /// Example: "Control|A+B".
  /// </summary>
  public static class Serializer
  {
    /// <summary>
    /// Separator used between keys in the serialized keys part.
    /// </summary>
    public const string Separator = "+";

    /// <summary>
    /// Separator used between the modifiers part and the keys part.
    /// </summary>
    public const string ModifierSeparator = "|";

    /// <summary>
    /// Formats the supplied <see cref="KeyChord"/> into a compact serialized string.
    /// </summary>
    /// <param name="chord">The chord to format.</param>
    /// <returns>A string in the format "&lt;ModifierKeys&gt;|&lt;Key1&gt;+&lt;Key2&gt;+...".</returns>
    public static string Format(KeyChord chord)
    {
      EAssert.Argument.IsNotNull(chord, nameof(chord));
      var modifiersPart = chord.Modifiers.ToString();
      var keysPart = string.Join(Separator, chord.Keys);
      return $"{modifiersPart}{ModifierSeparator}{keysPart}";
    }

    /// <summary>
    /// Parses a chord string produced by <see cref="Format"/> back into a <see cref="KeyChord"/>.
    /// </summary>
    /// <param name="text">The serialized chord string.</param>
    /// <returns>The parsed <see cref="KeyChord"/> instance.</returns>
    /// <exception cref="FormatException">Thrown when the input string is not in the expected format or cannot be parsed.</exception>
    public static KeyChord Parse(string text)
    {
      EAssert.Argument.IsNonEmptyString(text, nameof(text));
      var parts = text.Split(ModifierSeparator);
      if (parts.Length != 2)
        throw new FormatException("Invalid serialized KeyChord format.");
      try
      {
        var modifiers = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), parts[0]);
        var keyStrings = parts[1].Split(Separator);
        var keys = keyStrings.Select(ks => (Key)Enum.Parse(typeof(Key), ks)).ToList();
        return new KeyChord(keys, modifiers);
      }
      catch (Exception ex)
      {
        throw new FormatException($"Failed to parse KeyChord from serialized string '{text}'.", ex);
      }
    }
  }

  /// <summary>
  /// The sequence of keys that make up the chord.
  /// </summary>
  public List<Key> Keys { get; init; }

  /// <summary>
  /// The modifier keys associated with the chord (Ctrl, Alt, Shift, Windows).
  /// </summary>
  public ModifierKeys Modifiers { get; init; }

  /// <summary>
  /// Initializes a new instance of <see cref="KeyChord"/> with the specified keys and modifiers.
  /// </summary>
  /// <param name="keys">A non-empty list of keys that form the chord.</param>
  /// <param name="modifiers">Modifier keys to apply to the chord.</param>
  /// <exception cref="ArgumentNullException">Thrown when <paramref name="keys"/> is null.</exception>
  /// <exception cref="ArgumentException">Thrown when <paramref name="keys"/> is empty.</exception>
  public KeyChord(List<Key> keys, ModifierKeys modifiers)
  {
    if (keys == null) throw new ArgumentNullException(nameof(keys));
    if (keys.Count == 0) throw new ArgumentException("Keys must contain at least one key.", nameof(keys));

    this = new KeyChord { Keys = keys, Modifiers = modifiers };
  }

  /// <summary>
  /// Determines whether two <see cref="KeyChord"/> instances are equal.
  /// </summary>
  /// <param name="a">The first chord to compare.</param>
  /// <param name="b">The second chord to compare.</param>
  /// <returns>True if both chords have the same modifiers and equal key sequences; otherwise false.</returns>
  public static bool operator ==(KeyChord a, KeyChord b)
  {
    return a.Modifiers == b.Modifiers
      && a.Keys.SequenceEqual(b.Keys);
  }

  /// <summary>
  /// Determines whether two <see cref="KeyChord"/> instances are not equal.
  /// </summary>
  /// <param name="a">The first chord to compare.</param>
  /// <param name="b">The second chord to compare.</param>
  /// <returns>True if the chords differ in modifiers or key sequence; otherwise false.</returns>
  public static bool operator !=(KeyChord a, KeyChord b)
  {
    return a.Modifiers != b.Modifiers
      || a.Keys.Count != b.Keys.Count
      || !a.Keys.SequenceEqual(b.Keys);
  }

  /// <summary>
  /// Determines whether the specified object is equal to the current chord.
  /// </summary>
  /// <param name="obj">The object to compare with the current chord.</param>
  /// <returns>True if the specified object is a <see cref="KeyChord"/> equal to the current one; otherwise false.</returns>
  public override bool Equals([NotNullWhen(true)] object? obj) => obj != null && this == (KeyChord)obj;

  /// <summary>
  /// Returns a hash code for the current chord.
  /// </summary>
  /// <returns>A hash code representing the chord.</returns>
  public override int GetHashCode() => Keys.GetHashCode() + Modifiers.GetHashCode();

  /// <summary>
  /// Returns a string representation of the chord, including modifiers and keys.
  /// </summary>
  /// <returns>A human-readable string representing the chord.</returns>
  public override string ToString()
  {
    var keysStr = Keys == null ? string.Empty : string.Join(" + ", Keys);
    return $"[{Modifiers}] + {keysStr}";
  }
}
