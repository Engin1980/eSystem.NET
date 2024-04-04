using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESystemTest
{
  public static class EAssert
  {
    public static void AreEqual(IEnumerable a, IEnumerable b)
    {
      Assert.IsTrue(IsIEnumerableEqual(a, b));
    }
    public static void AreEqual(object a, object b)
    {
      Assert.AreEqual (a, b);
    }

    private static bool IsIEnumerableEqual(IEnumerable a, IEnumerable b)
    {
      IEnumerator aEnumerator = a.GetEnumerator();
      IEnumerator bEnumerator = b.GetEnumerator();

      object aItem;
      object bItem;

      while (aEnumerator.MoveNext())
      {
        if (!bEnumerator.MoveNext())
          return false;

        aItem = aEnumerator.Current;
        bItem = bEnumerator.Current;

        if (aItem is IEnumerable)
          if (bItem is IEnumerable)
          {
            if (!IsIEnumerableEqual((IEnumerable)aItem, (IEnumerable)bItem))
              return false;
          }
          else
            return false;
        else
          if (!aItem.Equals(bItem))
            return false;
      }

      if (bEnumerator.MoveNext())
        return false;
      else
        return true;
    }
  }
}
