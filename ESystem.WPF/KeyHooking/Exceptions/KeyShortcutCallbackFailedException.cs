using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.WPF.KeyHooking.Exceptions
{
  public class KeyShortcutCallbackFailedException(KeyHook keyHook, KeyShortcut keyShortcut, Exception cause) : KeyHookException("Callback for key shortcut failed.", cause)
  {
    public KeyHook KeyHook { get; init; } = keyHook;
    public KeyShortcut KeyShortcut { get; init; } = keyShortcut;
  }
}
