Option Explicit On
Option Strict On

Namespace Extensions

  Public Module ExceptionExt
    ''' <summary>
    ''' Returns joined message from exception and all inner exceptions.
    ''' </summary>
    ''' <param name="exception"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension()> _
  Public Function Info(ByVal exception As Exception) As String
      Dim ret As New Text.StringBuilder()

      Dim ex As Exception = exception
      While ex IsNot Nothing
        ret.Append(ex.Message)
        ret.Append(" --> ")
        ex = ex.InnerException
      End While

      Return ret.ToString()
    End Function
  End Module

End Namespace
