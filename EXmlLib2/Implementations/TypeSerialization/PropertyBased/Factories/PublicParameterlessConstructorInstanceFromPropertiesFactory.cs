using EXmlLib2.Implementations.TypeSerialization.Factories;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Reflection;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Factories;

public class PublicParameterlessConstructorInstanceFromPropertiesFactory : IInstanceFactory<PropertyInfo>
{
  public object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> deserializedValues)
  {
    object ret = Activator.CreateInstance(targetType)
      ?? throw new InvalidOperationException($"Cannot create instance of type '{targetType.FullName}' using parameterless constructor.");
    foreach (var kvp in deserializedValues)
    {
      kvp.Key.SetValue(ret, kvp.Value);
    }
    return ret;
  }
}




