using EXmlLib2.Implementations.TypeSerialization.Helpers;

namespace EXmlLib2.Implementations.TypeSerialization.Factories;

public interface IInstanceFactory
{
  object CreateInstance(Type targetType, Dictionary<string, object?> deserializedValues);
}




