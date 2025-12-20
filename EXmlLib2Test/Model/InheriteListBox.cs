using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2Test.Model
{
  internal abstract class A
  {
    public int PropertyA { get; set; }
  }

  internal class B : A
  {
    public int PropertyB { get; set; }
  }
  internal class C : A
  {
    public int PropertyC { get; set; }
  }

  internal class InheriteListBox
  {
    public List<A> Values { get; set; } = null!;
  }
}
