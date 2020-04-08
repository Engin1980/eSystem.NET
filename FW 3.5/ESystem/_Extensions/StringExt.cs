using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Extensions
{
  public static class StringExt
  {

    public static string[] Split2(this string source, string delimiter)
    {
      string[] ret = source.Split2(delimiter,  StringSplitOptions.None);

      return ret;
    }

    public static string[] Split2(this string source, string delimiter, StringSplitOptions options)
    {
      string[] ret = source.Split(new string[] { delimiter }, options);

      return ret;
    }

    public static string RemoveCzechDiakritics(this string sourceString)
    {
      int i = 0;
      System.Text.StringBuilder text = new System.Text.StringBuilder();

      for (i = 0; i <= sourceString.Length - 1; i++)
      {
        switch (sourceString[i])
        {
          case 'ě':
          case 'é':
            text.Append("e");
            break;
          case 'Ě':
          case 'É':
            text.Append("E");
            break;
          case 'ř':
            text.Append("r");
            break;
          case 'Ř':
            text.Append("R");
            break;
          case 'Ť':
            text.Append("T");
            break;
          case 'ť':
            text.Append("t");
            break;
          case 'ý':
            text.Append("y");
            break;
          case 'Ý':
            text.Append("Y");
            break;
          case 'ů':
          case 'ú':
            text.Append("u");
            break;
          case 'Ú':
            text.Append("U");
            break;
          case 'Í':
            text.Append("I");
            break;
          case 'í':
            text.Append("i");
            break;
          case 'ó':
            text.Append("o");
            break;
          case 'Ó':
            text.Append("O");
            break;
          case 'á':
            text.Append("a");
            break;
          case 'Á':
            text.Append("A");
            break;
          case 'š':
            text.Append("s");
            break;
          case 'Š':
            text.Append("S");
            break;
          case 'ď':
            text.Append("d");
            break;
          case 'Ď':
            text.Append("D");
            break;
          case 'ž':
            text.Append("z");
            break;
          case 'Ž':
            text.Append("Z");
            break;
          case 'č':
            text.Append("c");
            break;
          case 'Č':
            text.Append("C");
            break;
          case 'ň':
            text.Append("n");
            break;
          case 'Ň':
            text.Append("N");
            break;
          default:
            text.Append(sourceString[i]);
            break;
        }
      }

      return text.ToString();
    }

    /// <summary>
    /// Returns string containing "count" times the content of "source" parameter.
    /// </summary>
    /// <param name="source">String to repeat.</param>
    /// <param name="count">Repetition count.</param>
    /// <returns></returns>
    public static string Repeat(this string source, int count)
    {
      System.Text.StringBuilder ret = new System.Text.StringBuilder();


      for (int index = 1; index <= count; index++)
      {
        ret.Append(source);
      }

      return ret.ToString();

    }

  }
}
