using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESystem
{
  public static class Enums
  {
    public static string GetDisplayName(Enum value) =>
        value.GetType()
             .GetMember(value.ToString())[0]
             .GetCustomAttribute<DisplayAttribute>()?
             .Name ?? value.ToString();

    public static TEnum FromDisplayName<TEnum>(string displayName, TEnum defaultValue) where TEnum : struct, Enum
    {
      TEnum? result = TryFromDisplayName<TEnum>(displayName);
      if (result != null)
        return result.Value;
      else
        return defaultValue;
    }

    public static TEnum? FromDisplayName<TEnum>(string displayName) where TEnum : struct, Enum
    {
      TEnum? result = TryFromDisplayName<TEnum>(displayName);

      if (result != null)
        return result;

      throw new ArgumentException($"No enum value found for display name '{displayName}' in enum type '{typeof(TEnum).Name}'.", nameof(displayName));
    }

    private static TEnum? TryFromDisplayName<TEnum>(string displayName) where TEnum : struct, Enum
    {
      foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
      {
        var displayAttr = field.GetCustomAttribute<DisplayAttribute>();
        if (displayAttr == null && field.Name == displayName)
          return (TEnum)field.GetValue(null)!;
        else if (displayAttr?.Name == displayName)
          return (TEnum)field.GetValue(null)!;
      }

      return null;
    }
  }
}
