using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.WPF.KeyHooking.Exceptions
{
  internal class KeyChordCallbackFailedException(KeyHook keyHook, KeyChord keyChord, Exception cause ) : KeyHookException("Callback for key chord failed.", cause)
  {
    public KeyHook KeyHook { get; init; } = keyHook;
    public KeyChord KeyChord { get; init; } = keyChord;
  }
}
