using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.SimpleXmlSerialization
{
  [Serializable]
  public class XmlSerializeException : Exception
  {
    public List<string> OpenedElements = null;

    public XmlSerializeException() { }
    public XmlSerializeException(string message, Stack<string> openedElements = null)
      : base(message)
    {
      if (openedElements == null)
        return;

      this.OpenedElements = openedElements.ToList();
    }
    public XmlSerializeException(string message, Exception inner, Stack<string> openedElements = null)
      : base(message, inner)
    {
      if (openedElements == null)
        return;

      this.OpenedElements = openedElements.ToList();
    }
    protected XmlSerializeException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context)
      : base(info, context) { }
  }
}
