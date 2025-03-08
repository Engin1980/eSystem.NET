using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ELogging
{
  public class XmlLoader
  {
    public List<LogRule> Load(XElement logRulesParentElement, string patternAttribute = "Pattern", string levelAttribute = "Level")
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
