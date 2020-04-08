using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#if SILVERLIGHT == false
  using Microsoft.VisualBasic;
   using System.Data;
#endif

namespace ESystem.Extensions
{
  public static class StringBuilderExt
  {


    /// <summary>
    /// Adds space at the end of non-empty builder and then adds the text.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="text"></param>
    public static void AppendPreSpaced(this System.Text.StringBuilder builder, string text)
    {
      builder.AppendPreDelimited(text, " ");
    }


    /// <summary>
    /// Adds "preDelimiter" char/string at the end of non-empty builder and then adds the text.
    /// </summary>
    /// <param name="builder">Builder to alter</param>
    /// <param name="text">Text to add at the end of builder.</param>
    /// <param name="preDelimiter">Delimiter added at the end of builder before text is added.</param>
    /// <remarks>
    /// If builder currently ends with the "preDelimiter" char/string, only "text" parameter value is added.
    /// If builder is empty (that means contains no data, but is not null), only "text" parameter value is added.
    /// </remarks>
    public static void AppendPreDelimited(this System.Text.StringBuilder builder, string text, char preDelimiter)
    {
      builder.AppendPreDelimited(text, char.ToString(preDelimiter));
    }

    /// <summary>
    /// Adds "preDelimiter" char/string at the end of non-empty builder and then adds the text.
    /// </summary>
    /// <param name="builder">Builder to alter</param>
    /// <param name="text">Text to add at the end of builder.</param>
    /// <param name="preDelimiter">Delimiter added at the end of builder before text is added.</param>
    /// <remarks>
    /// If builder currently ends with the "preDelimiter" char/string, only "text" parameter value is added.
    /// If builder is empty (that means contains no data, but is not null), only "text" parameter value is added.
    /// </remarks>
    public static void AppendPreDelimited(this System.Text.StringBuilder builder, string text, string preDelimiter)
    {
      if (builder.Length > 0 && !builder.ToString().EndsWith(preDelimiter))
      {
        builder.Append(preDelimiter);
      }

      builder.Append(text);
    }

    /// <summary>
    /// Adds string into string builder and then empty space at the end, if there is none.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="text"></param>
    /// 
    public static void AppendPostSpaced(this System.Text.StringBuilder builder, string text)
    {
      builder.AppendPostDelimited(text, " ");
    }

    /// <summary>
    /// Adds text string at the end of stringbuilder and then adds "postDelimited" character/string
    /// at the end, if there is none.
    /// </summary>
    /// <param name="builder">Textbuilder to alter.</param>
    /// <param name="text">String to append.</param>
    /// <param name="postDelimiter">Delimiter added at the end of textBuilder, if is not already there. </param>
    /// <remarks>
    /// If added text already contains "postDelimited" char/string at its end, nothing will be added.
    /// </remarks>
    /// 
    public static void AppendPostDelimited(this System.Text.StringBuilder builder, string text, char postDelimiter)
    {
      builder.AppendPostDelimited(text, char.ToString(postDelimiter));
    }

    /// <summary>
    /// Adds text string at the end of stringbuilder and then adds "postDelimited" character/string
    /// at the end, if there is none.
    /// </summary>
    /// <param name="builder">Textbuilder to alter.</param>
    /// <param name="text">String to append.</param>
    /// <param name="postDelimiter">Delimiter added at the end of textBuilder, if is not already there. </param>
    /// <remarks>
    /// If added text already contains "postDelimited" char/string at its end, nothing will be added.
    /// </remarks>
    public static void AppendPostDelimited(this System.Text.StringBuilder builder, string text, string postDelimiter)
    {

      builder.Append(text);

      if (builder.Length > 0 && !builder.ToString().EndsWith(postDelimiter))
      {
        builder.Append(postDelimiter);
      }
    }

  }
}