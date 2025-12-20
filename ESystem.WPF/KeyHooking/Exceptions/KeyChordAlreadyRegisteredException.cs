using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.WPF.KeyHooking.Exceptions
{
  public class KeyChordAlreadyRegisteredException(KeyChord keyChord) : KeyHookException($"Key chord '{keyChord}' is already registered.")
  {
    public KeyChord KeyChord { get; init; } = keyChord;
  }
}
