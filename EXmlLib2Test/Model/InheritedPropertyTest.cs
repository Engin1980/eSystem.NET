using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2Test.Model
{
  internal class InheritedPropertyTest
  {
    public PropertyParent ParentA { get; set; } = null;
    public PropertyParent ParentB { get; set; } = null;
    public PropertyParent PropertyParentNull { get; set; } = null;
  }

  internal class PropertyParent
  {
    public int Int { get; set; } = 22;
  }

  internal class PropertyChild : PropertyParent
  {
    public int OtherInt { get; set; } = 33;
  }


}
