using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.WPF.KeyHooking.Exceptions
{
  public class KeyChordDetectionInterruptedException() : KeyHookException("Key chord detection was interrupted by modifiers change.")
  {
  }
}
