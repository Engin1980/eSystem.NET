using ESystem.WPF.KeyHooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.WPF.KeyHooking.Exceptions
{
  public class KeyChordNotFoundException(KeyChord keyChord) : KeyHookException($"Key chord '{keyChord}' not found.")
  {
    public KeyChord KeyChord { get; init; } = keyChord;
  }
}
