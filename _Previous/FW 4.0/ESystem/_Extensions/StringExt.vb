Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module StringExt

    ''' <summary>
    ''' Splits string by another string
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="delimiter"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
  Public Function Split(ByVal source As String, ByVal delimiter As String) As String()
      Dim ret As String() = source.Split(delimiter)

      Return ret
    End Function

    <Extension()> _
    Public Function RemoveCzechDiakritics(ByRef sourceString As String) As String
      Dim i As Integer
      Dim text As New System.Text.StringBuilder()

      For i = 0 To sourceString.Length - 1
        Select Case sourceString.Chars(i)
          Case "ě"c, "é"c
            text.Append("e")
          Case "Ě"c, "É"c
            text.Append("E")
          Case "ř"c
            text.Append("r")
          Case "Ř"c
            text.Append("R")
          Case "Ť"c
            text.Append("T")
          Case "ť"c
            text.Append("t")
          Case "ý"c
            text.Append("y")
          Case "Ý"c
            text.Append("Y")
          Case "ů"c, "ú"c
            text.Append("u")
          Case "Ú"c
            text.Append("U")
          Case "Í"c
            text.Append("I")
          Case "í"c
            text.Append("i")
          Case "ó"c
            text.Append("o")
          Case "Ó"c
            text.Append("O")
          Case "á"c
            text.Append("a")
          Case "Á"c
            text.Append("A")
          Case "š"c
            text.Append("s")
          Case "Š"c
            text.Append("S")
          Case "ď"c
            text.Append("d")
          Case "Ď"c
            text.Append("D")
          Case "ž"c
            text.Append("z")
          Case "Ž"c
            text.Append("Z")
          Case "č"c
            text.Append("c")
          Case "Č"c
            text.Append("C")
          Case "ň"c
            text.Append("n")
          Case "Ň"c
            text.Append("N")
          Case Else
            text.Append(sourceString.Chars(i))
        End Select
      Next

      Return text.ToString()
    End Function

    <Extension()> _
    Public Function Repeat(ByVal source As String, ByVal count As Integer) As String
      Dim ret As New System.Text.StringBuilder()


      For index As Integer = 1 To count
        ret.Append(source)
      Next

      Return ret.ToString()

    End Function

  End Module

End Namespace
