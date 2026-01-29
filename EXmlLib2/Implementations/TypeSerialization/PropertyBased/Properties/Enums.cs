using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties
{
  public enum MissingPropertyXmlSourceBehavior
  {
    ThrowException,
    Ignore,
    ReturnDefault,
  }

  public enum NameCaseMatching
  {
    Exact,
    IgnoreCase
  }

  public enum XmlSourceOrder
  {
    AttributeFirst,
    ElementFirst,
    AttributeOnly,
    ElementOnly
  }
}
