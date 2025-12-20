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
    public static KeyShortcut Parse(string str) => Parse(str, (FormatOptions?)null);
    public static KeyShortcut Parse(string str, FormatOptions? options = null)
    {
      var chord = FormatParseUtils.ParseKeyShortcut(str, options);
      return chord;
    }

    public static KeyShortcut Parse(string str, Action<FormatOptions>? options = null)
    {
      var chord = FormatParseUtils.ParseKeyShortcut(str, options);
      return chord;
    }

    public static bool TryParse(string str, out KeyShortcut chord, FormatOptions? options = null)
    {
      return FormatParseUtils.TryParseKeyShortcut(str, out chord, options);
    }
    public static bool TryParse(string str, out KeyShortcut chord, Action<FormatOptions>? options = null)
    {
      return FormatParseUtils.TryParseKeyShortcut(str, out chord, options);
    }

    public string Format() => this.Format((FormatOptions?)null);
    public string Format(FormatOptions? options = null)
    {
      return FormatParseUtils.Format(this, options);
    }
    public string Format(Action<FormatOptions>? options = null)
    {
      return FormatParseUtils.Format(this, options);
    }

    public KeyShortcut(Key key, ModifierKeys modifiers = ModifierKeys.None)
    {
      Key = key;
      Modifiers = modifiers;
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
