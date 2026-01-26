using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties
{
  public enum MissingPropertyElementBehavior
  {
    ThrowException,
    Ignore,
    ReturnNull
  }

  public enum NameCaseMatching
  {
    Exact,
    IgnoreCase
  }

  public enum XmlSourceOrder
  {
    AttributesFirst,
    ElementsFirst
  }
}
