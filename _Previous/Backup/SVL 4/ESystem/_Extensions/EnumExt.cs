using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ESystem.Extensions
{
  public static class EnumExt
  {
    /// <summary>
    /// Returns string displayed for enum item. See <see cref="ESystem.Attributes.DisplayAttribute"/>
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    /// <remarks>
    /// If enum item definition contains attribute <see cref="ESystem.Attributes.DisplayAttribute"/>,
    /// (ESystem.Attributes.DisplayAttribute), then display text from this attribute is extracted.
    /// If no specific definition is present, then the returned value is derived from 
    /// "camelCase" format from enum item name.
    /// </remarks>
    public static string ToDisplay(this Enum enumValue)
    {
      Type t = enumValue.GetType();

      var fis = t.GetFields();


      var fi = GetFieldInfoForValue(fis, enumValue);

      var display = GetDisplayTextForValue(fi);


      return display;
    }

    private static string GetDisplayTextForValue(FieldInfo fi)
    {
      string ret = "";
      var customAttributes = fi.GetCustomAttributes(typeof(DisplayAttribute), false);

      if (customAttributes.Length > 0)
        ret = (customAttributes[0] as DisplayAttribute).DisplayText;
      else
        ret = ConvertEnumFieldNameToDisplayString(fi);

      return ret;
    }

    private static string ConvertEnumFieldNameToDisplayString(FieldInfo fi)
    {
      StringBuilder ret = new StringBuilder();

      for (int i = 0; i < fi.Name.Length; i++)
      {
        char c = fi.Name[i];

        if (i == 0)
        {
          c = char.ToUpper(c);
          ret.Append(c);
        }
        else if (char.IsUpper(c))
        {
          ret.Append(" ");
          ret.Append(char.ToLower(c));
        }
        else
          ret.Append(char.ToLower(c));
      }

      return ret.ToString();
    }

    private static FieldInfo GetFieldInfoForValue(System.Reflection.FieldInfo[] fis, Enum enumValue)
    {
      foreach (var item in fis)
      {
        if (item.Name == enumValue.ToString())
          return item;
      }

      return null;
    }
  }
}
