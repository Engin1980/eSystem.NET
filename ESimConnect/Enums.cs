using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESimConnect
{
  public enum SimConnectSimObjectType
  {
    USER,
    ALL,
    AIRCRAFT,
    HELICOPTER,
    BOAT,
    GROUND
  }

  public enum SimConnectPeriod
  {
    NEVER,
    ONCE,
    VISUAL_FRAME,
    SIM_FRAME,
    SECOND
  }

  public class EnumConverter
  {
    public static TTargetEnum ConvertEnum<TSourceEnum, TTargetEnum>(TSourceEnum sourceEnum)
        where TSourceEnum : Enum
        where TTargetEnum : Enum
    {
      // Get the name (key) of the source enum
      string sourceName = sourceEnum.ToString();

      // Check if the target enum contains a matching name
      if (Enum.IsDefined(typeof(TTargetEnum), sourceName))
      {
        // If it does, convert the name to the target enum type
        return (TTargetEnum)Enum.Parse(typeof(TTargetEnum), sourceName);
      }
      else
      {
        throw new ArgumentException($"No matching key found in target enum for key '{sourceName}'.");
      }
    }

    public static TTargetEnum ConvertEnum2<TSourceEnum, TTargetEnum>(TSourceEnum sourceEnum)
        where TSourceEnum : Enum
        where TTargetEnum : Enum
    {
      // Get the type of the source and target enums
      Type sourceEnumType = typeof(TSourceEnum);
      Type targetEnumType = typeof(TTargetEnum);

      // Get the field representing the source enum value
      FieldInfo? sourceField = sourceEnumType.GetField(sourceEnum.ToString());

      if (sourceField == null)
      {
        throw new ArgumentException($"Source enum key '{sourceEnum}' not found.");
      }

      // Get all fields for the target enum
      FieldInfo[] targetFields = targetEnumType.GetFields(BindingFlags.Public | BindingFlags.Static);

      // Find a matching field in the target enum based on key
      FieldInfo? matchingTargetField = targetFields.FirstOrDefault(f => f.Name == sourceField.Name);

      if (matchingTargetField == null)
      {
        throw new ArgumentException($"No matching key found in target enum for key '{sourceField.Name}'.");
      }

      // Convert the matching field to the target enum
      return (TTargetEnum)matchingTargetField.GetValue(null);
    }

    public static TEnum ParseEnum<TEnum>(string value, bool ignoreCase = true) where TEnum : Enum 
    {
      if (Enum.TryParse(typeof(TEnum), value, ignoreCase, out object? result) && result != null)
      {
        TEnum ret = (TEnum)result;
        return ret;
      }
      else
        throw new ArgumentException($"Cannot parse '{value}' into {typeof(TEnum).Name}.");
    }
  }
}
