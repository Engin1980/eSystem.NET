using ESystem.WPF.KeyHooking;
using System.Windows.Input;

namespace KeyHookNUnit
{
  public class KeyChordTests
  {
    public FormatOptions bracketedFormatOptions = new()
    {
      ModifierBrackets = ("[", "]"),
      KeysBrackets = ("[", "]")
    };

    public static object[] BracketedFormatCases =
    {
      new object []{new KeyChord([Key.A], ModifierKeys.None), "A"},
      new object []{new KeyChord([Key.A], ModifierKeys.Control), "Ctrl+A"},
      new object []{new KeyChord([Key.A], ModifierKeys.Control|ModifierKeys.Alt|ModifierKeys.Windows), "[Ctrl|Alt|Win]+A"},

      new object []{new KeyChord(
        [Key.A, Key.B],
        ModifierKeys.None), "[A,B]"},
      new object []{new KeyChord(
        [Key.A, Key.B],
        ModifierKeys.Control), "Ctrl+[A,B]"},
      new object []{new KeyChord(
        [Key.A, Key.B],
        ModifierKeys.Control|ModifierKeys.Alt|ModifierKeys.Windows), "[Ctrl|Alt|Win]+[A,B]"}
    };


    public static object[] FormatCases =
    {
      new object []{new KeyChord([Key.A], ModifierKeys.None), "A"},
      new object []{new KeyChord([Key.A], ModifierKeys.Control), "Ctrl+A"},
      new object []{new KeyChord([Key.A], ModifierKeys.Control|ModifierKeys.Alt|ModifierKeys.Windows), "Ctrl|Alt|Win+A"},

      new object []{new KeyChord(
        [Key.A, Key.B], 
        ModifierKeys.None), "A,B"},
      new object []{new KeyChord(
        [Key.A, Key.B], 
        ModifierKeys.Control), "Ctrl+A,B"},
      new object []{new KeyChord(
        [Key.A, Key.B], 
        ModifierKeys.Control|ModifierKeys.Alt|ModifierKeys.Windows), "Ctrl|Alt|Win+A,B"}
    };

    [Test, TestCaseSource(nameof(FormatCases))]
    public void Format(KeyChord ksIn, string expectedString)
    {
      string s = ksIn.Format();
      Assert.That(s, Is.EqualTo(expectedString));
    }

    [Test, TestCaseSource(nameof(FormatCases))]
    public void Parse(KeyChord expectedKs, string inputString)
    {
      KeyChord ksOut = KeyChord.Parse(inputString);
      Assert.That(ksOut.Keys, Is.EqualTo(expectedKs.Keys));
      Assert.That(ksOut.Modifiers, Is.EqualTo(expectedKs.Modifiers));
    }

    [Test, TestCaseSource(nameof(FormatCases))]
    public void FormatBracketed(KeyChord ksIn, string expectedString)
    {
      string s = ksIn.Format(bracketedFormatOptions);
      Assert.That(s, Is.EqualTo(expectedString));
    }

    [Test, TestCaseSource(nameof(FormatCases))]
    public void ParseBracketed(KeyChord expectedKs, string inputString)
    {
      KeyChord ksOut = KeyChord.Parse(inputString, bracketedFormatOptions);
      Assert.That(ksOut.Keys, Is.EqualTo(expectedKs.Keys));
      Assert.That(ksOut.Modifiers, Is.EqualTo(expectedKs.Modifiers));
    }
  }
}