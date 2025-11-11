namespace ESystem.WPF.KeyHooking
{
  public class FormatOptions
  {
    public (string, string)? ModifierBrackets { get; set; } = ("", "");
    public string ModifierSeparator { get; set; } = "|";
    public Dictionary<string, List<string>> ModifierMapping { get; set; } = new()
    {
      { "Ctrl", new() { "Control", "Ctrl" } },
      { "Alt", new() { "Alt" } },
      { "Shift", new() { "Shift" } },
      { "Win", new() { "Windows", "Win" } }
    };
    public (string, string)? KeysBrackets { get; set; } = ("", "");
    public string KeysSeparator { get; set; } = ",";
    public string ModifierKeySeparator { get; set; } = "+";
  }
}
