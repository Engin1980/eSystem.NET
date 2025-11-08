using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.WPF.KeyHooking.Exceptions
{
  public class InternalKeyHooKException(string message, Exception? cause = null) : KeyHookException(message, cause)
  {
  }
}
