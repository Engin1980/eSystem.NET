using ESystem.Asserting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESystem.WPF.KeyHooking
{
  /// <summary>
  /// Represents a keyboard shortcut composed of a single key and optional modifier keys.
  /// </summary>
  public readonly struct KeyShortcut
  {
    /// <summary>
    /// Helper methods for formatting and parsing <see cref="KeyShortcut"/> instances to/from a compact string.
    /// Format used: "&lt;Modifiers&gt;+&lt;Key&gt;" where Modifiers is the <see cref="ModifierKeys"/> enum name.
    /// Example: "Control+A" (or "None+A" when no modifiers are present).
    /// </summary>
    public static class Serializer
    {
      /// <summary>
      /// Separator used between modifiers and key in the serialized form.
      /// </summary>
      public const string Separator = "+";

      /// <summary>
      /// Formats the given <see cref="KeyShortcut"/> into a compact string suitable for saving to text files or config.
      /// </summary>
      /// <param name="shortcut">The shortcut instance to format.</param>
      /// <returns>A string in the format "&lt;Modifiers&gt;+&lt;Key&gt;".</returns>
      public static string Format(KeyShortcut shortcut)
      {
        EAssert.Argument.IsNotNull(shortcut, nameof(shortcut));
        return $"{shortcut.Modifiers}{Separator}{shortcut.Key}";
      }

      /// <summary>
      /// Parses a string produced by <see cref="Format"/> back into a <see cref="KeyShortcut"/>.
      /// </summary>
      /// <param name="text">The serialized string to parse.</param>
      /// <returns>The parsed <see cref="KeyShortcut"/>.</returns>
      /// <exception cref="FormatException">Thrown when the string is not in the expected format or values cannot be parsed.</exception>
      public static KeyShortcut Parse(string text)
      {
        EAssert.Argument.IsNonEmptyString(text, nameof(text));
        var parts = text.Split(Separator);
        if (parts.Length != 2)
          throw new FormatException("Invalid serialized KeyShortcut format.");
        try
        {
          var modifiers = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), parts[0]);
          var key = (Key)Enum.Parse(typeof(Key), parts[1]);
          return new KeyShortcut { Key = key, Modifiers = modifiers };
        }
        catch (Exception ex)
        {
          throw new FormatException($"Failed to parse KeyShortcut from serialized string '{text}'.", ex);
        }
      }
    }

    /// <summary>
    /// The main key of the shortcut (e.g. <see cref="Key.A"/>).
    /// </summary>
    public Key Key { get; init; }

    /// <summary>
    /// The modifier keys associated with the shortcut (Ctrl, Alt, Shift, Windows).
    /// </summary>
    public ModifierKeys Modifiers { get; init; }

    /// <summary>
    /// Returns a string representation of the shortcut, including modifiers and key.
    /// </summary>
    /// <returns>A human-readable string representing the key shortcut.</returns>
    public override string ToString()
    {
      return $"{Modifiers} + {Key}";
    }
  }
}
