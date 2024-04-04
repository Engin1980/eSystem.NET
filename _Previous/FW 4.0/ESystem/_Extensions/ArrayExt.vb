Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module ArrayExt
    ''' <summary>
    ''' Returns index of last item in collection
    ''' </summary>
    <Extension()> _
    Public Function LastIndex(ByVal arr As Array) As Integer
      Return arr.Length - 1
    End Function

    <Extension()> _
  Public Sub EForEach(Of T)(ByVal coll As Array, ByVal predicate As System.Action(Of T))

      For Each Item As T In coll
        predicate(Item)
      Next

    End Sub

    ''' <summary>
    ''' Returns items joined together by separator.
    ''' </summary>
    ''' <param name="separator"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function ToString(ByVal arr As Array, ByVal separator As String) As String
      Dim ret As New Text.StringBuilder()

      For i As Integer = 0 To arr.Length - 1
        If i > 0 Then
          ret.Append(separator)
        End If
        ret.Append(arr.GetValue(i))
      Next

      Return ret.ToString()
    End Function
  End Module

End Namespace
