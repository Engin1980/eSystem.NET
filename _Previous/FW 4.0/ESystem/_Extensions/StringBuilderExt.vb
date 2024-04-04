Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module StringBuilderExt


    ''' <summary>
    ''' Adds space at the end of non-empty builder and then adds the text.
    ''' </summary>
    ''' <param name="builder"></param>
    ''' <param name="text"></param>
    <Extension()> _
    Public Sub AppendPreSpaced(ByVal builder As System.Text.StringBuilder, ByVal text As String)
      builder.AppendPreDelimited(text, " ")
    End Sub


    ''' <summary>
    ''' Adds "preDelimiter" char/string at the end of non-empty builder and then adds the text.
    ''' </summary>
    ''' <param name="builder">Builder to alter</param>
    ''' <param name="text">Text to add at the end of builder.</param>
    ''' <param name="preDelimiter">Delimiter added at the end of builder before text is added.</param>
    ''' <remarks>
    ''' If builder currently ends with the "preDelimiter" char/string, only "text" parameter value is added.
    ''' If builder is empty (that means contains no data, but is not null), only "text" parameter value is added.
    ''' </remarks>
    <Extension()> _
    Public Sub AppendPreDelimited(ByVal builder As System.Text.StringBuilder, ByVal text As String, ByVal preDelimiter As Char)
      builder.AppendPreDelimited(text, Char.ToString(preDelimiter))
    End Sub

    ''' <summary>
    ''' Adds "preDelimiter" char/string at the end of non-empty builder and then adds the text.
    ''' </summary>
    ''' <param name="builder">Builder to alter</param>
    ''' <param name="text">Text to add at the end of builder.</param>
    ''' <param name="preDelimiter">Delimiter added at the end of builder before text is added.</param>
    ''' <remarks>
    ''' If builder currently ends with the "preDelimiter" char/string, only "text" parameter value is added.
    ''' If builder is empty (that means contains no data, but is not null), only "text" parameter value is added.
    ''' </remarks>
    <Extension()> _
    Public Sub AppendPreDelimited(ByVal builder As System.Text.StringBuilder, ByVal text As String, ByVal preDelimiter As String)
      If builder Is Nothing Then
        Throw New ArgumentNullException("builder")
      End If

      If builder.Length > 0 AndAlso Not builder.ToString().EndsWith(preDelimiter) Then
        builder.Append(preDelimiter)
      End If

      builder.Append(text)
    End Sub

    ''' <summary>
    ''' Adds string into string builder and then empty space at the end, if there is none.
    ''' </summary>
    ''' <param name="builder"></param>
    ''' <param name="text"></param>
    ''' 
    <Extension()> _
    Public Sub AppendPostSpaced(ByVal builder As System.Text.StringBuilder, ByVal text As String)
      builder.AppendPostDelimited(text, " ")
    End Sub

    ''' <summary>
    ''' Adds text string at the end of stringbuilder and then adds "postDelimited" character/string
    ''' at the end, if there is none.
    ''' </summary>
    ''' <param name="builder">Textbuilder to alter.</param>
    ''' <param name="text">String to append.</param>
    ''' <param name="postDelimiter">Delimiter added at the end of textBuilder, if is not already there. </param>
    ''' <remarks>
    ''' If added text already contains "postDelimited" char/string at its end, nothing will be added.
    ''' </remarks>
    ''' 
    <Extension()> _
    <Extension()> _
    Public Sub AppendPostDelimited(ByVal builder As System.Text.StringBuilder, ByVal text As String, ByVal postDelimiter As Char)
      builder.AppendPostDelimited(text, Char.ToString(postDelimiter))
    End Sub

    ''' <summary>
    ''' Adds text string at the end of stringbuilder and then adds "postDelimited" character/string
    ''' at the end, if there is none.
    ''' </summary>
    ''' <param name="builder">Textbuilder to alter.</param>
    ''' <param name="text">String to append.</param>
    ''' <param name="postDelimiter">Delimiter added at the end of textBuilder, if is not already there. </param>
    ''' <remarks>
    ''' If added text already contains "postDelimited" char/string at its end, nothing will be added.
    ''' </remarks>
    <Extension()> _
    <Extension()> _
    Public Sub AppendPostDelimited(ByVal builder As System.Text.StringBuilder, ByVal text As String, ByVal postDelimiter As String)
      If builder Is Nothing Then
        Throw New ArgumentNullException("builder")
      End If

      builder.Append(text)

      If builder.Length > 0 AndAlso Not builder.ToString().EndsWith(postDelimiter) Then
        builder.Append(postDelimiter)
      End If
    End Sub

  End Module

End Namespace
