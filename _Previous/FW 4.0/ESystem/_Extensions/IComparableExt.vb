Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module IComparableExt

    ''' <summary>
    ''' Checks if value is between bounds, both bounds are inclusive.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="value"></param>
    ''' <param name="firstBound">Inclusive lower bound.</param>
    ''' <param name="secondBound">Inclusive uper bound.</param>
    ''' <returns>True if value is between bounds (both inclusive).</returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function IsBetween(Of T As IComparable(Of T))( _
      ByVal value As IComparable(Of T), _
      ByVal firstBound As T, _
      ByVal secondBound As T) As Boolean

      If (firstBound.CompareTo(secondBound) > 0) Then
        Dim x = firstBound
        firstBound = secondBound
        secondBound = x
      End If

      Dim ret As Boolean = _IsBetween(Of T)(firstBound, value, secondBound)
      Return ret

    End Function

    ''' <summary>
    ''' Returns true if item is in set of other items.
    ''' </summary>
    ''' <param name="current"></param>
    ''' <param name="items"></param>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    <Extension()> _
    Public Function IsIn(Of T)(ByVal current As IComparable(Of T), ByVal ParamArray items() As T) As Boolean

      For Each fItem As T In items
        If current.CompareTo(fItem) = 0 Then
          Return True
        End If
      Next

      Return False

    End Function

    ''' <summary>
    ''' Returns true if item is not in set of other items.
    ''' </summary>
    ''' <param name="current"></param>
    ''' <param name="items"></param>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    <Extension()> _
    Public Function IsNotIn(Of T)(ByVal current As IComparable(Of T), ByVal ParamArray items() As T) As Boolean

      For Each fItem As T In items
        If current.CompareTo(fItem) = 0 Then
          Return False
        End If
      Next

      Return True

    End Function

    Private Function _IsBetween(Of T As IComparable(Of T))( _
      ByVal lower As T, _
      ByVal value As IComparable(Of T), _
      ByVal upper As T) As Boolean

      If (value.CompareTo(lower) < 0) Then
        Return False
      ElseIf (value.CompareTo(upper) > 0) Then
        Return False
      Else
        Return True
      End If
    End Function


  End Module

End Namespace
