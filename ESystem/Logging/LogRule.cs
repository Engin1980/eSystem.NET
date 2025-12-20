using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Logging
{
  public class LogRule
  {
    public LogLevel LogLevel { get; private set; }

    public string Pattern { get; private set; }

    public LogRule(string pattern, LogLevel logLevel)
    {
      Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
      LogLevel = logLevel;
    }

    public LogRule(string pattern, string logLevel)
      : this(pattern, LogUtils.ConvertStringToLogLevel(logLevel)) { }    

    internal bool IsAcceptingLogLevel(LogLevel level)
    {
      bool ret = (int)level >= (int)LogLevel;
      return ret;
    }

    internal bool IsPatternMatch(string senderName) => System.Text.RegularExpressions.Regex.IsMatch(senderName, Pattern);
  }
}
