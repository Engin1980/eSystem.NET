using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.WPF.KeyHooking.Exceptions
{
  public abstract class KeyHookException(string message, Exception? cause =  null) : Exception(message, cause)
  {
  }
}
