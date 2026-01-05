using EXmlLib2.Implementations.TypeSerialization.Factories;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Reflection;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Factories;

public class PublicParameterlessConstructorInstanceFromPropertiesFactory : IInstanceFactory
{
  public object CreateInstance(Type targetType, Dictionary<string, object?> deserializedValues)
  {
    object ret = Activator.CreateInstance(targetType)
      ?? throw new InvalidOperationException($"Cannot create instance of type '{targetType.FullName}' using parameterless constructor.");
    foreach (var kvp in deserializedValues)
    {
      PropertyInfo pi = targetType.GetProperty(kvp.Key)
        ?? throw new InvalidOperationException($"Type '{targetType.FullName}' does not contain property '{kvp.Key}'.");
      pi.SetValue(ret, kvp.Value);
    }
    return ret;
  }
}




