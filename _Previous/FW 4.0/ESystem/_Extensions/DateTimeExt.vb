Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module DateTimeExt
    ''' <summary>
    ''' Returns date instance with time set to end of day.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <Extension()> _
    Public Function DayEnd(ByVal value As DateTime) As DateTime
      Return value.Date.AddDays(1).GetGreatestPrevious()
    End Function


    ''' <summary>
    ''' Return maximum value lower than current value.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <Extension()> _
    Public Function GetGreatestPrevious(ByVal value As DateTime) As DateTime
      Return value.AddMilliseconds(-1)
    End Function

    ''' <summary>
    ''' Return minimum value greater than current value.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <Extension()> _
    Public Function GetLowestNext(ByVal value As DateTime) As DateTime
      Return value.AddMilliseconds(1)
    End Function

    ''' <summary>
    ''' Returns datetime with day 1 in current month. E.g. from 2005-05-05 returns 2005-05-01.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function GetMonthFirst(ByVal value As DateTime) As DateTime
      Return New DateTime(value.Year, value.Month, 1)
    End Function

    ''' <summary>
    ''' Returns datetime with last day in current month. E.g. from 2005-05-05 returns 2005-05-31.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function GetMonthLast(ByVal value As DateTime) As DateTime
      Return New DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month))
    End Function

    ''' <summary>
    ''' Returns datetime with day-time start, that is time 00:00 in current day.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function GetDayFirst(ByVal value As DateTime) As DateTime
      Return value.Date
    End Function

    ''' <summary>
    ''' Returns datetime with day-time end, that is time 23:59:59,999 in current day.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function GetDayLast(ByVal value As DateTime) As DateTime
      Return value.Date.AddDays(1).GetGreatestPrevious
    End Function

    ''' <summary>
    ''' Returns first day in year from this date.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function GetYearFirst(ByVal value As DateTime) As DateTime
      Return New DateTime(value.Year, 1, 1)
    End Function

    ''' <summary>
    ''' Returns last day-time in year from this date.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function GetYearLast(ByVal value As DateTime) As DateTime
      Return value.GetYearFirst().GetGreatestPrevious()
    End Function

  End Module

End Namespace
