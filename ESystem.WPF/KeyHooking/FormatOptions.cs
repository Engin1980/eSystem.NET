using System.Windows.Input;

namespace ESystem.WPF.KeyHooking
{
  public class FormatOptions
  {
    public (string, string)? ModifierBrackets { get; set; } = ("", "");
    public string ModifierSeparator { get; set; } = "|";
    public Dictionary<ModifierKeys, List<string>> ModifierMapping { get; set; } = new()
    {
      { ModifierKeys.Control, new() { "Control", "Ctrl" } },
      { ModifierKeys.Alt, new() { "Alt" } },
      { ModifierKeys.Shift, new() { "Shift" } },
      { ModifierKeys.Windows, new() { "Windows", "Win" } }
    };
    public (string, string)? KeysBrackets { get; set; } = ("", "");
    public string KeysSeparator { get; set; } = ",";
    public string ModifierKeySeparator { get; set; } = "+";
  }
}
