using ESystem.Asserting;
using ESystem.Exceptions;
using EXmlLib2.Implementations.TypeSerialization.Factories;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Reflection;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Factories;

public class UniversalTypeFactory : IInstanceFactory<PropertyInfo>
{
  private enum TypeKind
  {
    Class,
    Struct,
    RecordClass,
    RecordStruct
  }

  public object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> propertyValues)
  {
    TypeKind typeKind = GetTypeKind(targetType);
    object ret = typeKind switch
    {
      TypeKind.RecordClass => CreateAndInitStructOrRecord(targetType, true, propertyValues),
      TypeKind.RecordStruct => CreateAndInitStructOrRecord(targetType, true, propertyValues),
      TypeKind.Struct => CreateAndInitStructOrRecord(targetType, false, propertyValues),
      TypeKind.Class => CreateAndInitClass(targetType, propertyValues),
      _ => throw new UnexpectedEnumValueException(typeKind)
    };
    return ret;
  }

  private object CreateAndInitStructOrRecord(Type type, bool isForRecords, DataFieldValueDictionary<PropertyInfo> propertyValues)
  {
    if (!isForRecords)
      EAssert.IsTrue(type.IsValueType, $"Generic type T must be struct (provided '{type}').");

    var ctors = type.GetConstructors();
    EAssert.IsTrue(ctors.Length > 0, $"Generic type '{type}' must have public constructor.");

    // here we are looking for .ctor with best match of parameters against properties
    ConstructorInfo? bestCtor = null;
    object?[]? bestArgs = null;
    int bestScore = -1;

    foreach (var ctor in ctors)
    {
      var parameters = ctor.GetParameters();
      var args = new object?[parameters.Length];
      int score = 0;
      bool usable = true;

      for (int i = 0; i < parameters.Length; i++)
      {
        var param = parameters[i];
        var prop = propertyValues.Keys.FirstOrDefault(p => string.Equals(p.Name, param.Name, StringComparison.OrdinalIgnoreCase));

        if (prop != null)
        {
          var value = propertyValues[prop];

          if (value != null && !param.ParameterType.IsInstanceOfType(value))
          {
            try
            {
              value = Convert.ChangeType(value, param.ParameterType);
            }
            catch
            {
              usable = false;
              break;
            }
          }

          args[i] = value;
          score++;
        }
        else if (param.HasDefaultValue)
          args[i] = param.DefaultValue;
        else if (param.IsOptional)
          args[i] = Type.Missing;
        else
        {
          usable = false;
          break;
        }
      }

      if (!usable) continue;

      if (score > bestScore)
      {
        bestScore = score;
        bestCtor = ctor;
        bestArgs = args;
      }
    }

    if (bestCtor == null || bestArgs == null)
      throw new InvalidOperationException($"No available constructor found to create an instance of {type}.");

    object ret = bestCtor.Invoke(bestArgs);
    return ret;
  }

  private object CreateAndInitClass(Type instanceType, DataFieldValueDictionary<PropertyInfo> propertyValues)
  {
    var pom = new PublicParameterlessConstructorInstanceFromPropertiesFactory();
    object ret = pom.CreateInstance(instanceType, propertyValues);
    return ret;
  }

  private static TypeKind GetTypeKind(Type type)
  {
    bool isRecord =
        type.GetProperty(
            "EqualityContract",
            BindingFlags.Instance | BindingFlags.NonPublic
        ) != null;

    if (isRecord)
      return type.IsValueType ? TypeKind.RecordStruct : TypeKind.RecordClass;

    return type.IsValueType ? TypeKind.Struct : TypeKind.Class;
  }
}




