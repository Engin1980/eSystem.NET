using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2
{
  public class EXmlException : Exception
  {
    public object? Sender { get; private set; }
    public EXmlException(string message, Exception innerException) : base(message, innerException)
    {
    }



    public EXmlException(object sender, string message, Exception? innerException = null) : base(message, innerException)
    {
      this.Sender = sender;
    }

    public EXmlException(string? message) : base(message)
    {
    }
  }
}
