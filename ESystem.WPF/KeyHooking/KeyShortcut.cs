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
    public static class Serializer
    {
      public const string Separator = "+";
      public static string Format(KeyShortcut shortcut)
      {
        return $"{shortcut.Modifiers}{Separator}{shortcut.Key}";
      }
      public static KeyShortcut Parse(string serialized)
      {
        var parts = serialized.Split(Separator);
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
          throw new FormatException($"Failed to parse KeyShortcut from serialized string '{serialized}'.", ex);
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
