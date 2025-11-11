using ESystem.WPF.KeyHooking;

namespace KeyHookNUnit
{
  public class KeyShortuctTests
  {
    public static object[] FormatCases =
    {
      new object []{new KeyShortcut(System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.None), "A"},
      new object []{new KeyShortcut(System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Control), "Ctrl+A"},
      new object []{new KeyShortcut(System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Control|System.Windows.Input.ModifierKeys.Alt|System.Windows.Input.ModifierKeys.Windows), "Ctrl|Alt|Win+A"}
    };
    [Test, TestCaseSource(nameof(FormatCases))]
    public void KeyShortcutFormat(KeyShortcut ksIn, string expectedString)
    {
      string s = ksIn.Format();
      Assert.That(s, Is.EqualTo(expectedString));
    }

    public void KeyShortcutParse(KeyShortcut expectedKs, string inputString)
    {
      KeyShortcut ksOut = KeyShortcut.Parse(inputString);
      Assert.That(ksOut.Key, Is.EqualTo(expectedKs.Key));
      Assert.That(ksOut.Modifiers, Is.EqualTo(expectedKs.Modifiers));
    }
  }
}