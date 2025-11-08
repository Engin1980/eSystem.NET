using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.WPF.KeyHooking.Exceptions
{
  public class KeyShortcutAlreadyRegisteredException(KeyShortcut keyShortcut) : KeyHookException($"Key shortcut '{keyShortcut}' is already registered.")
  {
    public KeyShortcut KeyShortcut { get; init; } = keyShortcut;
  }
}
