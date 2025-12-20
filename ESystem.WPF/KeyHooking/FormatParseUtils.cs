using ESystem.Asserting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESystem.WPF.KeyHooking
{

  internal static class FormatParseUtils
  {
    public static string Format(KeyChord chord, Action<FormatOptions>? options = null)
    {
      FormatOptions opts = new();
      options?.Invoke(opts);
      return Format(chord, opts);
    }

    public static string Format(KeyChord chord, FormatOptions? options = null)
    {
      string ret;
      options ??= new FormatOptions();
      string m = Format(chord.Modifiers, options);
      string k = Format(chord.Keys, options);
      if (string.IsNullOrEmpty(m))
        ret = k;
      else
        ret = $"{m}{options.ModifierKeySeparator}{k}";
      return ret;
    }

    public static string Format(KeyShortcut shortcut, Action<FormatOptions>? options = null)
    {
      FormatOptions opts = new();
      options?.Invoke(opts);
      return Format(shortcut, opts);
    }

    public static string Format(KeyShortcut shortcut, FormatOptions? options = null)
    {
      string ret;
      options ??= new FormatOptions();
      string m = Format(shortcut.Modifiers, options);
      string k = Format([shortcut.Key], options);
      if (string.IsNullOrEmpty(m))
        ret = k;
      else
        ret = $"{m}{options.ModifierKeySeparator}{k}";
      return ret;
    }

    private static string Format(List<Key> keys, FormatOptions options)
    {
      List<string> strings = [];
      foreach (var key in keys)
      {
        strings.Add(key.ToString()!);
      }

      string ret = string.Join(options.KeysSeparator, strings);
      if (strings.Count > 1 && options.KeysBrackets != null)
        ret = $"{options.KeysBrackets.Value.Item1}{ret}{options.KeysBrackets.Value.Item2}";
      return ret;
    }

    private static string Format(ModifierKeys modifiers, FormatOptions options)
    {
      if (modifiers == ModifierKeys.None)
        return string.Empty;
      List<string> parts = new();
      if (modifiers.HasFlag(ModifierKeys.Control))
        parts.Add("Ctrl");
      if (modifiers.HasFlag(ModifierKeys.Alt))
        parts.Add("Alt");
      if (modifiers.HasFlag(ModifierKeys.Shift))
        parts.Add("Shift");
      if (modifiers.HasFlag(ModifierKeys.Windows))
        parts.Add("Win");

      string ret = string.Join(options.ModifierSeparator, parts);
      if (parts.Count > 1 && options.ModifierBrackets != null)
        ret = $"{options.ModifierBrackets.Value.Item1}{ret}{options.ModifierBrackets.Value.Item2}";
      return ret;
    }

    public static KeyChord ParseKeyChord(string str, FormatOptions? options = null)
    {
      options ??= new FormatOptions();
      string[] pts = str.Split(options.ModifierKeySeparator);
      EAssert.IsTrue(
        pts.Length >= 1 || pts.Length <= 2,
        () => $"Failed to split modifier / key(s) part using {options.ModifierKeySeparator}. Got {pts.Length} parts, expected 1-2.");

      ModifierKeys modifiers;
      List<Key> keys;
      if (pts.Length == 1)
      {
        modifiers = ModifierKeys.None;
        keys = ParseKeys(pts[0], options);
      }
      else
      {
        modifiers = ParseModifiers(pts[0], options);
        keys = ParseKeys(pts[1], options);
      }
      EAssert.IsTrue(
        keys.Count > 0,
        () => $"KeyChord must have at least one key. Got {keys.Count} keys.");
      KeyChord ret = new()
      {
        Keys = keys,
        Modifiers = modifiers
      };
      return ret;
    }

    private static List<Key> ParseKeys(string str, FormatOptions options)
    {
      str = RemoveBracketsIfRequired(str, options.KeysBrackets);
      string[] pts = str.Split(options.KeysSeparator);
      List<Key> keys = [];
      foreach (string key in pts)
      {
        if (Enum.TryParse<Key>(key, out Key k))
        {
          keys.Add(k);
        }
        else
        {
          throw new FormatException($"Failed to parse key '{key}'.");
        }
      }
      return keys;
    }

    private static string RemoveBracketsIfRequired(string str, (string, string)? brackets)
    {
      if (brackets == null) return str;
      if (str.StartsWith(brackets.Value.Item1) && str.EndsWith(brackets.Value.Item2))
        str = str.Substring(brackets.Value.Item1.Length, str.Length - brackets.Value.Item1.Length - brackets.Value.Item2.Length);
      return str;
    }

    private static ModifierKeys ParseModifiers(string str, FormatOptions options)
    {
      str = RemoveBracketsIfRequired(str, options.ModifierBrackets);
      string[] pts = str.Split(options.ModifierSeparator);
      ModifierKeys modifiers = ModifierKeys.None;
      foreach (string mod in pts)
      {
        string modTrimmed = mod.Trim();

        bool found = false;
        foreach (var kvp in options.ModifierMapping)
        {
          if (kvp.Value.Contains(modTrimmed, StringComparer.OrdinalIgnoreCase))
          {
            modifiers |= kvp.Key;
            found = true;
          }
        }

        if (!found)
        {
          throw new FormatException($"Failed to parse modifier '{modTrimmed}'.");
        }
      }
      return modifiers;
    }

    public static KeyChord ParseKeyChord(string str, Action<FormatOptions>? options = null)
    {
      var opts = new FormatOptions();
      options?.Invoke(opts);
      return ParseKeyChord(str, opts);
    }

    public static KeyShortcut ParseKeyShortcut(string str, FormatOptions? options = null)
    {
      options ??= new();
      string[] pts = str.Split(options.ModifierKeySeparator);
      EAssert.IsTrue(
        pts.Length >= 1 || pts.Length <= 2,
        () => $"Failed to split modifier / key(s) part using {options.ModifierKeySeparator}. Got {pts.Length} parts, expected 1-2.");

      ModifierKeys modifiers;
      List<Key> keys;
      if (pts.Length == 1)
      {
        modifiers = ModifierKeys.None;
        keys = ParseKeys(pts[0], options);
      }
      else
      {
        modifiers = ParseModifiers(pts[0], options);
        keys = ParseKeys(pts[1], options);
      }
      EAssert.IsTrue(
        keys.Count == 1,
        () => $"KeyShortcut must have exactly one key. Got {keys.Count} keys.");
      KeyShortcut ret = new()
      {
        Key = keys[0],
        Modifiers = modifiers
      };
      return ret;
    }

    public static KeyShortcut ParseKeyShortcut(string str, Action<FormatOptions>? options = null)
    {
      var opts = new FormatOptions();
      options?.Invoke(opts);
      return ParseKeyShortcut(str, opts);
    }

    internal static bool TryParseKeyChord(string str, out KeyChord chord, FormatOptions? options)
    {
      try
      {
        chord = ParseKeyChord(str, options);
        return true;
      }
      catch (Exception)
      {
        chord = default;
        return false;
      }
    }

    internal static bool TryParseKeyChord(string str, out KeyChord chord, Action<FormatOptions>? options)
    {
      try
      {
        chord = ParseKeyChord(str, options);
        return true;
      }
      catch (Exception)
      {
        chord = default;
        return false;
      }
    }

    internal static bool TryParseKeyShortcut(string str, out KeyShortcut keyShortcut, FormatOptions? options)
    {
      try
      {
        keyShortcut = ParseKeyShortcut(str, options);
        return true;
      }
      catch (Exception)
      {
        keyShortcut = default;
        return false;
      }
    }

    internal static bool TryParseKeyShortcut(string str, out KeyShortcut keyShortcut, Action<FormatOptions>? options)
    {
      try
      {
        keyShortcut = ParseKeyShortcut(str, options);
        return true;
      }
      catch (Exception)
      {
        keyShortcut = default;
        return false;
      }
    }
  }
}
