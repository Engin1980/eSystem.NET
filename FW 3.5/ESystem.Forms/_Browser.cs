using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Forms
{
  internal static class _Browser
  {
    internal static string NormalizeRelativePath(string value)
    {
      string ret = value;
      if (ret == null) ret = "";
      if (ret.Length > 0)
        if (ret[ret.Length - 1] != '\\')
          ret += "\\";

      return ret;
    }

    internal static string GetName(string relativePath, string name)
    {
      string ret = "";

      if (name == null || name.Length == 0)
        ret = "";
      else if (
          (name == ".")
          || (name == @".\")
          || (name == "\\\\")
          || (name == "\\"))
        return "";
      else if (name.StartsWith("\\\\"))
        ret = name;
      else if (name.Length > 1 && name.Substring(1, 1) == ":")
        ret = name;
      else if (name.StartsWith("\\"))
        ret = System.IO.Path.Combine(relativePath, "." + name);
      else
        ret = System.IO.Path.Combine(relativePath, name);

      if (ret.Length > 0)
        ret = System.IO.Path.GetFullPath(ret);

      return ret;
    }
  }
}
