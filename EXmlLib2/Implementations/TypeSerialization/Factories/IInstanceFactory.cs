using EXmlLib2.Implementations.TypeSerialization.Helpers;

namespace EXmlLib2.Implementations.TypeSerialization.Factories;

public interface IInstanceFactory<TDataFieldInfo> where TDataFieldInfo : notnull
{
  object CreateInstance(Type targetType, DataFieldValueDictionary<TDataFieldInfo> deserializedValues);
}




