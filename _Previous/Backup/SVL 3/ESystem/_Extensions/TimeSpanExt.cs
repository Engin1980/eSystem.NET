using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ESystem.Extensions
{
  public static class TimeSpanExt
  {
    /// <summary>
    /// Divides two timespans and returns rate between them.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static double DivideBy(this TimeSpan a, TimeSpan b)
    {
      return Convert.ToDouble(a.Ticks) / b.Ticks;
    }

    /// <summary>
    /// Returns opposite timespan value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TimeSpan Negate(this TimeSpan value)
    {
      TimeSpan ret = new TimeSpan(-value.Ticks);
      return ret;
    }

    /// <summary>
    /// Creates new timespan instance from string in format like "-3. 13:34:43,40".
    /// </summary>
    /// <param name="value"></param>
    /// <param name="timeSpanAsString"></param>
    /// <returns></returns>
    /// <remarks>
    /// There can be leading negation char "-".
    /// Day value from minute value can be separated only by dot ".", or by dot and space ". ".
    /// Second from milisecond can be separated by dot "." or by comma ",".
    /// Information should present following parts: (second), (minute and second), (hour and minute and second),
    /// (day, hour, minute and second). The last part, miliseconds, are optional.
    /// </remarks>
    public static TimeSpan ParseFrom(this TimeSpan value, string timeSpanAsString)
    {
      TimeSpan ret;

      string rgx = @"(-)?(\d+.( )?)(\d+:)(\d+:)(\d+)[.|,](\d+)";

      Match m = Regex.Match(timeSpanAsString, rgx);

      if (m.Success)
      {
        bool isNeg = false;
        int days = 0;
        int hours = 0;
        int mins = 0;
        int secs = 0;
        int milisecs = 0;

        if (m.Groups[1].Success)
          isNeg = true;

        if (m.Groups[2].Success)
          days = int.Parse(m.Groups[3].Value);

        if (m.Groups[5].Success)
          hours = int.Parse(m.Groups[6].Value);

        if (m.Groups[7].Success)
          mins = int.Parse(m.Groups[8].Value);

        secs = int.Parse(m.Groups[9].Value);

        if (m.Groups[10].Success)
        {
          milisecs = int.Parse(m.Groups[10].Value);
        }

        ret = new TimeSpan (
          days, hours, mins, secs, milisecs);

        if (isNeg)
          ret = ret.Negate();

      }
      else
        // invalid format
        throw new ArgumentException ("String " + timeSpanAsString + " is not correct timespan format");

      return ret;
    }

    /// <summary>
    /// Returns TimeSpan as formatted by DateTime format string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string ToString(this TimeSpan value, string format)
    {
      StringBuilder sb = new StringBuilder(format);

      sb.Replace("fff", value.Milliseconds.ToString("000"));

      sb.Replace("dd", value.Days.ToString("00"));
      sb.Replace("hh", value.Hours.ToString("00"));
      sb.Replace("mm", value.Minutes.ToString("00"));
      sb.Replace("ss", value.Seconds.ToString("00"));
      sb.Replace("ff", (value.Milliseconds/ 10).ToString("00"));

      sb.Replace("d", value.Days.ToString("0"));
      sb.Replace("h", value.Hours.ToString("0"));
      sb.Replace("m", value.Minutes.ToString("0"));
      sb.Replace("s", value.Seconds.ToString("0"));
      sb.Replace("f", (value.Milliseconds / 100).ToString("0"));

      string ret = sb.ToString();

      return ret;
    }
  }
}
