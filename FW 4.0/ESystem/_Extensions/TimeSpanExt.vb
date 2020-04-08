Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module TimeSpanExt

    <Extension()> _
  Public Function DivideBy(ByVal a As TimeSpan, ByVal b As TimeSpan) As Double
      Return CDbl(a.Ticks) / b.Ticks
    End Function

    <Extension()> _
    Public Function Negate(ByVal value As TimeSpan) As TimeSpan

      Dim ret As New TimeSpan(-value.Ticks)
      Return ret

    End Function

  End Module

End Namespace
