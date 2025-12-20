using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.WPF.KeyHooking.Exceptions
{
  public class KeyShortcutNotFoundException(KeyShortcut keyShortcut) : KeyHookException($"Key shortcut '{keyShortcut}' not found.")
  {
    public KeyShortcut KeyShortcut { get; init; } = keyShortcut;
  }
}
