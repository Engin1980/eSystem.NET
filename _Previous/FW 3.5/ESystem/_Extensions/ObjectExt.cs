using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace ESystem.Extensions
{
  public static class ObjectExt
  {
    private const string NULL = "(null)";

    #region "InLine"

    private static int callIndex = 0;
    public static string ToInlineInfoString(this object obj)
    {
      callIndex += 1;
      if (callIndex > 100)
        Console.WriteLine("stop");

      if ((obj == null))
        return NULL;
      else if (obj is string)
        return Convert.ToString(obj);

      StringBuilder cont = new StringBuilder();
      System.Text.StringBuilder ret = new System.Text.StringBuilder();
      Type t = obj.GetType();

      if (obj is ICollection)
      {
        ICollection ie = (ICollection)obj;

        foreach (object fItem in ie)
        {
          string valueString = ToInlineInfoString(fItem);
          cont.AppendPreDelimited(valueString, ';');
        }
      }
      else if (obj is IEnumerable)
      {
        IEnumerable ie = (IEnumerable)obj;

        foreach (object fItem in ie)
        {
          string valueString = ToInlineInfoString(fItem);
          cont.AppendPreDelimited(valueString, ';');
        }
      }
      else
      {
        PropertyInfo[] props = t.GetProperties();
        MemberInfo[] mems = props.ToArray();
        string loc = GenerateInlineContent(obj, mems);

        if ((loc.Length > 0))
          cont.AppendPreDelimited(loc.ToString(), ';');
      }

      if ((cont.Length == 0) && (obj is ICollection == false))
        ret.Append(obj.ToString());
      else
      {
        ret.Append("{" + cont.ToString() + "}");
        ret.Insert(0, "(" + t.Name + ")");
      }

      return ret.ToString();
    }

    private static string GenerateInlineContent(object obj, MemberInfo[] members)
    {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      foreach (MemberInfo Item in members)
      {
        
        //object fieldValue = obj.GetType().InvokeMember(Item.Name, BindingFlags.GetProperty, null, obj, null, null);
        object fieldValue = obj.GetType().InvokeMember(Item.Name, BindingFlags.GetProperty, null, obj, null);
        string fieldValueString = ToInlineInfoString(fieldValue);

        sb.AppendPreDelimited(Item.Name + "=" + fieldValueString, ',');
      }

      return sb.ToString();
    }
    #endregion


    #region "MultiLine"

    public static string ToMultilineInfoString(this object obj)
    {
      return ToMultilineInfoString(obj, 0);
    }


    private static string ToMultilineInfoString(object obj, int level)
    {
      string prefix = "  ".Repeat(level);

      if ((obj == null))
        return prefix + NULL;
      else if (obj is string)
        return prefix + Convert.ToString(obj);

      StringBuilder cont = new StringBuilder();
      System.Text.StringBuilder ret = new System.Text.StringBuilder();
      Type t = obj.GetType();

      if (obj is ICollection)
      {
        ICollection ie = (ICollection)obj;

        foreach (object fItem in ie)
        {
          string valueString = ToMultilineInfoString(fItem, level + 1);
          cont.AppendPreDelimited(prefix + "  " + valueString.TrimStart(), Environment.NewLine);
        }
      }
      else if (obj is IEnumerable)
      {
        IEnumerable ie = (IEnumerable)obj;

        foreach (object fItem in ie)
        {
          string valueString = ToMultilineInfoString(fItem, level + 1);
          cont.AppendPreDelimited(prefix + "  " + valueString.TrimStart(), Environment.NewLine);
        }
      }
      else
      {
        PropertyInfo[] props = t.GetProperties();
        MemberInfo[] mems = props.ToArray();

        string loc = GenerateMultilineContent(obj, mems, level);

        if ((loc.Length > 0))
        {
          cont.AppendPreDelimited(loc.ToString(), ';');
        }
      }

      if ((cont.Length == 0))
      {
        ret.Append(prefix + obj.ToString());
      }
      else
      {
        ret.Append(Environment.NewLine + cont.ToString());

        ret.Insert(0, prefix + "(" + t.Name + ")");
      }

      return ret.ToString();
    }

    private static string GenerateMultilineContent(object obj, MemberInfo[] members, int level)
    {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      string prefix = "  ".Repeat(level) + " > ";

      foreach (MemberInfo Item in members)
      {
        // object fieldValue = obj.GetType().InvokeMember(Item.Name, BindingFlags.GetProperty, null, obj, null, null);
        object fieldValue = obj.GetType().InvokeMember(Item.Name, BindingFlags.GetProperty, null, obj, null);
        string fieldValueString = ToMultilineInfoString(fieldValue, level + 1);

        sb.AppendPreDelimited(prefix + Item.Name + "=" + fieldValueString.TrimStart(), Environment.NewLine);
      }

      return sb.ToString();
    }

    #endregion


    #region "XML"

    #region "Nested"

    private const string INTENDER = "  ";
    private const string DEFAULT_ENCAPSULATING_TAG = "This";
    private const string DEFAULT_ENUMERATIONITEM_TAG = "Item";
    private const string DEFAULT_NULL_TAGITEM = "(null)";
    public class Settings
    {
      internal bool useLongTypeNames = false;
      internal bool expandValueTypes = false;
      internal string encapsulatingTag = ObjectExt.DEFAULT_ENCAPSULATING_TAG;
      internal string enumerationItemTag = ObjectExt.DEFAULT_ENUMERATIONITEM_TAG;
      internal int maxDepth = 3;
      internal int currentDepth = 0;
      internal bool sortByName = false;
      internal bool includeFields = true;
      internal bool includeProperties = true;
      internal bool expandEnumerations = true;
      internal string nullTagItem = ObjectExt.DEFAULT_NULL_TAGITEM;
      internal bool inlineValues = false;
    }

    private class MemberInfoByNameComparer : IComparer<MemberInfo>
    {

      public int Compare(System.Reflection.MemberInfo x, System.Reflection.MemberInfo y)
      {
        return x.Name.CompareTo(y.Name);
      }

    }

    #endregion


    #region "Public methods"

    public static string ToXml(this object item)
    {
      string ret = _ToXml(item, new Settings());
      return ret;
    }

    public static string ToXml(this object item, bool useLongTypeNames, string encapsulatingTag)
    {
      Settings sett = new Settings();
      sett.useLongTypeNames = useLongTypeNames;
      sett.encapsulatingTag = encapsulatingTag;

      string ret = _ToXml(item, sett);

      return ret;

    }

    public static string ToXml(this object item, bool useLongTypeNames, string encapsulatingTag, bool inlineValues)
    {
      Settings sett = new Settings();
      sett.useLongTypeNames = useLongTypeNames;
      sett.encapsulatingTag = encapsulatingTag;
      sett.inlineValues = inlineValues;

      string ret = _ToXml(item, sett);

      return ret;

    }

    public static string ToXml(this object item, 
      bool useLongTypeNames, bool expandValueTypes, 
      bool includeProperties, bool includeFields, 
      bool expandEnumerations, bool sortByName)
    {
      Settings sett = new Settings();
      sett.useLongTypeNames = useLongTypeNames;
      sett.expandValueTypes = expandValueTypes;
      sett.sortByName = sortByName;
      sett.includeFields = includeFields;
      sett.includeProperties = includeProperties;
      sett.expandEnumerations = expandEnumerations;

      string ret = _ToXml(item, sett);

      return ret;

    }

    public static string ToXml(this object item, 
      bool useLongTypeNames, bool expandValueTypes, 
      bool includeProperties, bool includeFields, 
      bool expandEnumerations, bool sortByName, int maxDepth)
    {
      Settings sett = new Settings();
      sett.useLongTypeNames = useLongTypeNames;
      sett.expandValueTypes = expandValueTypes;
      sett.maxDepth = maxDepth;
      sett.sortByName = sortByName;
      sett.includeFields = includeFields;
      sett.includeProperties = includeProperties;
      sett.expandEnumerations = expandEnumerations;

      string ret = _ToXml(item, sett);

      return ret;

    }

    public static string ToXml(this object item, 
      bool useLongTypeNames, bool expandValueTypes, 
      bool includeProperties, bool includeFields, 
      bool expandEnumerations, bool sortByName, 
      string encapsulatingTag, string enumerationItemTag, string nullTagItem,
      int maxDepth, bool inlineValues)
    {
      Settings sett = new Settings();
      sett.useLongTypeNames = useLongTypeNames;
      sett.expandValueTypes = expandValueTypes;
      sett.encapsulatingTag = encapsulatingTag;
      sett.enumerationItemTag = enumerationItemTag;
      sett.maxDepth = maxDepth;
      sett.sortByName = sortByName;
      sett.includeFields = includeFields;
      sett.includeProperties = includeProperties;
      sett.expandEnumerations = expandEnumerations;
      sett.nullTagItem = nullTagItem;
      sett.inlineValues = inlineValues;

      string ret = _ToXml(item, sett);

      return ret;

    }

    #endregion

    #region "Private methods"

    private static string _ToXml(object item, Settings sett)
    {
      if (item == null)
      {
        throw new ArgumentNullException("Cannot generate XML content from null/nothing value.");
      }

      string ret = CreateXmlByType(sett.encapsulatingTag, item, sett);

      ret = FormatXml(ret);

      return ret;

    }

    private static string CreateXml(string tag, ValueType value, Settings sett)
    {
      string ret = CreateTag(tag, value.GetType(), true, value.ToString(), sett);

      return ret;
    }

    private static string CreateXml(string tag, string value, Settings sett)
    {
      string ret = CreateTag(tag, value.GetType(), true, value, sett);

      return ret;
    }

    private static string CreateXml(string tag, Enum value, Settings sett)
    {
      string ret = CreateTag(tag, value.GetType(), true, value.ToString(), sett);

      return ret;
    }

    private static string CreateXml(string tag, IEnumerable value, Settings sett)
    {
      StringBuilder content = new StringBuilder();
      string pom = null;


      if (sett.expandEnumerations)
      {
        foreach (object fItem in value)
        {
          pom = CreateXmlByType(sett.enumerationItemTag, fItem, sett);
          content.AppendLine(pom);
        }

      }
      else
      {
        if ((value is ICollection))
        {
          content.AppendLine("Count: " + ((ICollection)value).Count.ToString());
        }
        else
        {
          content.AppendLine();
        }
      }

      string ret = CreateTag(tag, value.GetType(), false, content.ToString(), sett);

      return ret;

    }


    private static string CreateXml(string tag, object value, Settings sett)
    {
      StringBuilder content = new StringBuilder();
      string pom = null;
      object itemValue = null;
      Type valueType = value.GetType();
      PropertyInfo[] props = valueType.GetProperties();
      FieldInfo[] fields = valueType.GetFields();
      List<MemberInfo> members = new List<MemberInfo>();

      if (sett.includeFields)
        members.AddRange(valueType.GetFields());
      if (sett.includeProperties)
        members.AddRange(valueType.GetProperties());

      if (sett.sortByName)
      {
        members.Sort(new MemberInfoByNameComparer());
      }


      foreach (MemberInfo fItem in members)
      {

        if (fItem is PropertyInfo)
        {
          PropertyInfo pi = (PropertyInfo)fItem;
          if (!pi.CanRead)
            continue;

          //itemValue = valueType.InvokeMember(pi.Name, BindingFlags.GetProperty, null, value, null, null);
          itemValue = valueType.InvokeMember(pi.Name, BindingFlags.GetProperty, null, value, null);
        }
        else
        {
          //itemValue = valueType.InvokeMember(fItem.Name, BindingFlags.GetField, null, value, null, null);
          itemValue = valueType.InvokeMember(fItem.Name, BindingFlags.GetField, null, value, null);
        }

        if (itemValue == null)
        {
          Type t = null;
          if (fItem is PropertyInfo)
          {
            t = ((PropertyInfo)fItem).PropertyType;
          }
          else
          {
            t = ((FieldInfo)fItem).FieldType;
          }

          pom = CreateTag(fItem.Name, t, true, sett.nullTagItem, sett);
          content.AppendLine(pom);

        }
        else
        {
          pom = CreateXmlByType(fItem.Name, itemValue, sett);
          content.AppendLine(pom);
        }

      }

      string ret = CreateTag(tag, value.GetType(), false, content.ToString(), sett);

      return ret;

    }


    private static string CreateXmlByType(string tag, object value, Settings sett)
    {
      if (sett.maxDepth < sett.currentDepth)
        return "";

      sett.currentDepth += 1;

      string ret = null;

      if (value is ValueType)
      {
        Type t = value.GetType();
        if (t.IsPrimitive)
        {
          ret = CreateXml(tag, (ValueType)value, sett);
        }
        else
        {
          if (sett.expandValueTypes)
          {
            ret = CreateXml(tag, (object)value, sett);
          }
          else
          {
            ret = CreateXml(tag, (ValueType)value, sett);
          }
        }
      }
      else if (value is string)
      {
        ret = CreateXml(tag, Convert.ToString(value), sett);
      }
      else if (value is IEnumerable)
      {
        ret = CreateXml(tag, (IEnumerable)value, sett);
      }
      else
      {
        ret = CreateXml(tag, (object)value, sett);
      }

      sett.currentDepth -= 1;

      return ret;
    }

    private delegate StringBuilder AddingDelegate(string text);

    private static string CreateTag(string tag, Type type, bool isAtomic, string value, Settings sett)
    {
      StringBuilder sb = new StringBuilder();
      AddingDelegate d = null;

      if (sett.inlineValues && isAtomic)
      {
        d = sb.Append;
      }
      else
      {
        d = sb.AppendLine;
      }

      if (sett.useLongTypeNames)
      {
        d("<" + tag + " type=\"" + type.FullName + "\">");
      }
      else
      {
        d("<" + tag + " type=\"" + type.Name + "\">");
      }

      d(value);

      d("</" + tag + ">");

      return sb.ToString();
    }

    private static string FormatXml(string text)
    {
      System.IO.StringReader rdr = new System.IO.StringReader(text);
      int level = 0;
      StringBuilder ret = new StringBuilder();

      string line = rdr.ReadLine();

      while (line != null)
      {
        if (line.StartsWith("</"))
        {
          level = level - 1;
          ret.AppendLine(INTENDER.Repeat(level) + line);
        }
        else if (line.StartsWith("<"))
        {
          ret.AppendLine(INTENDER.Repeat(level) + line);
          if ((!line.Contains("</")))
            level = level + 1;
        }
        else if (line.Trim().Length > 0)
        {
          ret.AppendLine(INTENDER.Repeat(level) + line);
        }

        line = rdr.ReadLine();
      }

      return ret.ToString();
    }

    #endregion

    #endregion
  }

}
