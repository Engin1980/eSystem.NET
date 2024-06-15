using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Types
{
  public class XmlIterableInfo
  {
    internal TypeStringDict XmlItemName { get; } = new();

    public XmlIterableInfo WithItemXmlName(string name)
    {
      XmlItemName.Set(null, name);
      return this;
    }

    public XmlIterableInfo WithItemXmlName(Type type, string name)
    {
      XmlItemName.Set(type, name);
      return this;
    }
  }
}
