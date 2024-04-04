using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace ESystem.Collections
{

  /// <summary>
  /// Represents collection of strings.
  /// </summary>
  /// <seealso cref="T:ESystem.Collections.EList(Of T)"/>
  [Serializable()]
  public class StringCollection : EList<string>, ICloneable<StringCollection>
  {

    /// <summary>
    /// Initializes a new Instance of ESystem.Collections.StringCollection
    /// </summary>
    /// <param name="Params"></param>
    public StringCollection(params string[] @params)
    {
      int j = 0;
      int i = 0;

      j = @params.GetUpperBound(0);
      for (i = 0; i <= j; i++)
      {
        base.Add(@params[i].Trim());
      }
    }

    /// <summary>
    /// Initializes a new Instance of ESystem.Collections.StringCollection
    /// </summary>
    /// <param name="text">Text to split.</param>
    /// <param name="separator">Separator</param>
    /// <param name="detectSubStrings">If true, function will not split string in quotes.</param>
    /// <remarks>
    /// Konstruktor tvo��c� novou instanci a napln� ji polo�kami vytvo�en�mi rozsek�n�m _
    /// vstupn�ho �et�zce "Text" podle znaku "Separator". Pokud "IngorovatRetezce" = True, _
    /// tak se znak "Separator" bude ignorovat v �et�zci uzav�en�ch v uvozovk�ch.
    /// </remarks>
    public StringCollection(string text, char separator, bool detectSubStrings)
    {
      string[] temp = null;
      string s = null;
      if (!detectSubStrings)
      {
        temp = text.Split(separator);
        foreach (string s_loopVariable in temp)
        {
          s = s_loopVariable;
          this.Add(s.Trim());
        }
      }
      else
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Adds new empty string (not null) into collection.
    /// </summary>
    /// <remarks></remarks>
    public void AddEmpty()
    {
      base.Add("");
    }

    /// <summary>
    /// Adds new null item into collection.
    /// </summary>
    /// <remarks></remarks>
    public void AddNull()
    {
      base.Add("");
    }

    /// <summary>
    /// Add trimmed item into collection. Is the same as Add (text.Trim()).
    /// </summary>
    /// <param name="Text"></param>
    /// <remarks></remarks>
    public void AddTrimmed(string text)
    {
      base.Add(text.Trim());
    }

    /// <summary>
    /// Returns item by index.
    /// </summary>
    /// <param name="index"></param>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public new string this[int index]
    {
      get
      {
        try
        {
          return Convert.ToString(base[index]);
        }
        catch (IndexOutOfRangeException e)
        {
          throw new IndexOutOfRangeException("Index is out of range (index: " + index.ToString() + ", items: " + base.Count + ")", e);
        }
      }
    }

    [Obsolete("Use ToString(\", \") instead.")]
    public string ToStringAll()
    {
      //vrati vsechny polozky v jednom retezci oddeleny carkou a mezerou
      string ret = "";
      string tStr = null;

      foreach (string tStr_loopVariable in this)
      {
        tStr = tStr_loopVariable;
        ret += tStr + ", ";
      }

      if (ret.Length > 2)
        ret = ret.Substring(0, ret.Length - 2);

      return ret;

    }

    //Vr�t� v�echny polo�ky pole jako pole string�
    public string[] ToStringArray()
    {
      string[] ret = null;

      ret = base.ToArray();

      return ret;
    }

    //Zap�e obsah pole do streamu "stream" co z�znam to ��dek.
    public void WriteLineToStream(ref StreamWriter stream)
    {
      string str = null;
      foreach (string str_loopVariable in this)
      {
        str = str_loopVariable;
        stream.WriteLine(str);
      }
    }

    //Zap�e obsah do souboru "FileName" s dan�m k�dov�n�m "CodePage" (implicitn� 1250) _
    //a uzav�e soubor. Co z�znam to ��dek.
    public void WriteLineToFile(string fileName, int CodePage = 1250)
    {
      StreamWriter stream = null;

      stream = new StreamWriter(fileName, false, System.Text.Encoding.GetEncoding(CodePage));
      WriteLineToStream(ref stream);
      stream.Close();
    }

    /// <summary>
    /// Creates clone of object.
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public StringCollection Clone()
    {
      StringCollection ret = new StringCollection();

      ret.AddRange(this);

      return ret;
    }

  }

}
