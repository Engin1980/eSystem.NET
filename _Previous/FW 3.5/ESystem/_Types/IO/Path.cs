using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem._Types.IO
{
  public static class Path
  {
    /// <summary>
    /// Returns relative path of the target against the source path. Default separator @"\" is expected.
    /// </summary>
    /// <param name="sourcePath">Current location</param>
    /// <param name="target">Item to create relative path to.</param>
    /// <returns>Returns relative path of the target from the current location.</returns>
    public static string GetRelativePath(string sourcePath, string target)
    {
      char SEPARATOR = '\\';
      string targetItem = System.IO.Path.GetFileName(target);
      string targetPath = System.IO.Path.GetDirectoryName(target);

      List<string> ss = new List<string>();
      ss.AddRange(sourcePath.Split(SEPARATOR));
      List<string> tt = new List<string>();
      tt.AddRange(targetPath.Split(SEPARATOR));

      if (ss[ss.Count - 1] == "")
        ss.RemoveAt(ss.Count - 1);

      while (ss.Count > 0 && tt.Count > 0 && ss[0] == tt[0])
      {
        ss.RemoveAt(0);
        tt.RemoveAt(0);
      }

      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < ss.Count; i++)
        sb.Append(".." + SEPARATOR);

      for (int i = 0; i < tt.Count; i++)
      {
        sb.Append(tt[i] + SEPARATOR);
      }

      sb.Append(targetItem);

      return sb.ToString();
    }
  }
}
