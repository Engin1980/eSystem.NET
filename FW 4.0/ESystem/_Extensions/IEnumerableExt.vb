Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module IEnumerableExt

    <Extension()> _
  Public Function ToString(ByVal array As IEnumerable, ByVal separator As String) As String
      Dim s As New Text.StringBuilder
      Dim isFirst As Boolean = True

      For Each fItem As Object In array
        If (isFirst) Then
          s.Append(separator)
        End If
        s.Append(fItem.ToString())
      Next

      Return s.ToString()
    End Function

    <Extension()> _
    Public Function EFirst(Of T)(ByVal coll As IEnumerable(Of T), ByVal predicate As System.Func(Of T, Boolean)) As T
      Dim ret As T

      If coll.Count = 0 Then
        ret = Nothing
      Else
        Try
          ret = coll.First(predicate)
        Catch ex As InvalidOperationException
          ret = Nothing
        End Try
      End If

      Return ret

    End Function

    <Extension()> _
    Public Sub EForEach(Of T)(ByVal coll As IEnumerable(Of T), ByVal predicate As System.Action(Of T))

      For Each Item As T In coll
        predicate(Item)
      Next

    End Sub

  End Module

End Namespace
