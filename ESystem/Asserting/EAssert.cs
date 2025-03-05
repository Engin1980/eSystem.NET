using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Asserting
{
  public class EAssert
  {
    public class Argument
    {
      public static void IsNotNull([NotNull] object? value, string argumentName)
      {
        EAssert.IsNotNull(value, $"Argument '{argumentName}' is null.");
      }

      public static void IsTrue(bool value, string argumentName, string violationDescription)
      {
        EAssert.IsTrue(value, $"Argument '{argumentName}' condition check to true failed. Volation: {violationDescription}");
      }

      public static void IsNonEmptyString([NotNull] string value, string argumentName)
      {
        EAssert.IsNonEmptyString(value, $"Argument '{argumentName}' is null or empty.");
      }
    }

    public static void IsNonEmptyString([NotNull] string value, string message = "String is empty or null.")
    {
      if (string.IsNullOrEmpty(value))
        throw new EAssertException(message);
    }

    public static void IsNotNull([NotNull] object? value, string message = "Value is null.")
    {
      if (value == null)
        throw new EAssertException(message);
    }

    public static void IsTrue(bool value, string message = "Value must be true.")
    {
      if (value != true)
        throw new EAssertException(message);
    }

    public static void IsFalse(bool value, string message = "Value must be false.")
    {
      if (value != false)
        throw new EAssertException(message);
    }
  }

}
