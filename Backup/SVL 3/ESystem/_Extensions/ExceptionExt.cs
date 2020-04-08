using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Extensions
{
  public static class ExceptionExt
  {
    /// <summary>
    /// Returns joined message from exception and all inner exceptions.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    [Obsolete("Use GetMessages() extension method instead.")]
    public static string Info(this Exception exception)
    {
      string ret = exception.GetMessages();
      return ret;
    }

    /// <summary>
    /// Returns joined message from exception and all inner exceptions.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static string GetMessages(this Exception exception)
    {
      StringBuilder ret = new StringBuilder();

      Exception ex = exception;
      while (ex != null)
      {
        ret.Append(ex.Message);
        ret.Append(" --> ");
        ex = ex.InnerException;
      }

      return ret.ToString();
    }

    /// <summary>
    /// Returns joined ToString() sets from exception and all inner exceptions.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static string GetToStrings(this Exception exception)
    {
      StringBuilder ret = new StringBuilder();

      Exception ex = exception;
      while (ex != null)
      {
        ret.Append(ex.ToString());
        ret.Append(" --> ");
        ex = ex.InnerException;
      }

      return ret.ToString();
    }
  }

}
