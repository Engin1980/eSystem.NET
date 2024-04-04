using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Extensions
{
  public static class DateTimeExt
  {
    /// <summary>
    /// Returns date instance with time set to end of day.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime DayEnd(this DateTime value)
    {
      return value.Date.AddDays(1).GetGreatestPrevious();
    }


    /// <summary>
    /// Return maximum value lower than current value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime GetGreatestPrevious(this DateTime value)
    {
      return value.AddMilliseconds(-1);
    }

    /// <summary>
    /// Return minimum value greater than current value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime GetLowestNext(this DateTime value)
    {
      return value.AddMilliseconds(1);
    }

    /// <summary>
    /// Returns datetime with day 1 in current month. E.g. from 2005-05-05 returns 2005-05-01.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static DateTime GetMonthFirst(this DateTime value)
    {
      return new DateTime(value.Year, value.Month, 1);
    }

    /// <summary>
    /// Returns datetime with last day in current month. E.g. from 2005-05-05 returns 2005-05-31.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static DateTime GetMonthLast(this DateTime value)
    {
      return new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));
    }

    /// <summary>
    /// Returns datetime with day-time start, that is time 00:00 in current day.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static DateTime GetDayFirst(this DateTime value)
    {
      return value.Date;
    }

    /// <summary>
    /// Returns datetime with day-time end, that is time 23:59:59,999 in current day.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static DateTime GetDayLast(this DateTime value)
    {
      return value.Date.AddDays(1).GetGreatestPrevious();
    }

    /// <summary>
    /// Returns first day in year from this date.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static DateTime GetYearFirst(this DateTime value)
    {
      return new DateTime(value.Year, 1, 1);
    }

    /// <summary>
    /// Returns last day-time in year from this date.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static DateTime GetYearLast(this DateTime value)
    {
      return value.GetYearFirst().GetGreatestPrevious();
    }

  }

}
