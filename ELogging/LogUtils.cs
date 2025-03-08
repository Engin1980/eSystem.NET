using ESystem.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ELogging
{
  public class LogUtils
  {
    public static LogLevel? TryConvertStringToLogLevel(string logLevel)
    {
      LogLevel? ret;
      try
      {
        ret = ConvertStringToLogLevel(logLevel);
      }
      catch (ArgumentException)
      {
        ret = null;
      }
      return ret;
    }

    public static LogLevel ConvertStringToLogLevel(string logLevel)
    {
      LogLevel ret;
      try
      {
        ret = (LogLevel)Enum.Parse(typeof(LogLevel), logLevel.ToUpper());
      }
      catch
      {
        throw new ArgumentException($"Failed to convert '{logLevel}' to known LogLevel.");
      }
      return ret;
    }

    public static List<LogRule> LoadLogRulesFromJson(EJObject logRulesList,
      string patternAttribute = "pattern", string levelAttribute = "level")
    {
      List<LogRule> ret = new();

      var lst = logRulesList.AsListOfDicts();
      foreach (var item in lst)
      {
        if (item.TryGetValue(patternAttribute, out EJObject? patternJson) == false)
          continue;
        string patternValue = patternJson.AsString();
        if (item.TryGetValue(levelAttribute, out EJObject? levelJson) == false)
          continue;
        string levelValue = levelJson.AsString();

        LogRule logRule = new(patternValue, levelValue);
        ret.Add(logRule);
      }

      return ret;
    }

    public static List<LogRule> LoadLogRulesFromXml(XElement logRulesParentElement,
      string patternAttribute = "Pattern", string levelAttribute = "Level")
    {
      List<LogRule> ret = new();

      var childElements = logRulesParentElement.Elements();
      foreach (var childElement in childElements)
      {
        string? pattern = childElement.Attribute(XName.Get(patternAttribute))?.Value;
        string? level = childElement.Attribute(XName.Get(levelAttribute))?.Value;
        if (pattern == null || level == null) continue;
        LogLevel? logLevel = LogUtils.TryConvertStringToLogLevel(level);
        if (logLevel == null) continue;
        ret.Add(new LogRule(pattern, logLevel.Value));
      }

      return ret;
    }
  }
}
