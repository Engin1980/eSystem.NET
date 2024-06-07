using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2Test
{
  internal static class Utils
  {
    internal static void CompareProperties<T>(T exp, T act)
    {
      if ((exp == null && act != null) || (exp != null && act == null))
        throw new AssertionException($"Expected value is {exp} but actual value is {act}");
      if (exp != null && act != null && exp.GetType() != act.GetType())
        throw new AssertionException($"Expected type is {exp.GetType()} but actual type is {act.GetType()}");
      if (exp == null && act == null) return;

      var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

      properties.ForEach(prop =>
      {
        var expVal = prop.GetValue(exp);
        var actVal = prop.GetValue(act);
        if (!Equals(expVal, actVal))
          throw new AssertionException($"Property {typeof(T).Name}.{prop.Name} value differs - exp={expVal} vs act={actVal}.");
      });
    }
  }
}
