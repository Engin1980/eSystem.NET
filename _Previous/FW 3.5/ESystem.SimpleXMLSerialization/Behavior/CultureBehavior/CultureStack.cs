using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace ESystem.SimpleXmlSerialization.Behavior.CultureBehavior
{
  class CultureStack
  {
    private Stack<CultureInfo> inner = new Stack<CultureInfo>();

    public CultureStack()
    {
      this.inner.Push(CultureInfo.CurrentUICulture);
    }

    public void Push (string culture)
    {
      CultureInfo ci = null;

      try
      {
        ci = CultureInfo.CreateSpecificCulture(culture);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to find culture info for culture-string " + culture + ".", ex);
      } // end catch


      Push(ci);
    }

    public void Push (CultureInfo culture)
    {
      inner.Push(culture);
    }

    public void Pop()
    {
      if (inner.Count == 1)
        throw new Exception("Unable to pop from culture info stack. There is only default value left in the stack.");
      else
        inner.Pop();
    }

    public CultureInfo Current
    {
      get
      {
        return inner.Peek();
      }
    }
  }
}
