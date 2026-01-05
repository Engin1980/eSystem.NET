using ESystem.Asserting;
using EXmlLib2.Implementations.TypeSerialization.Factories;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using EXmlLib2.Types;
using System.Reflection;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Factories;

public class DelegatedInstanceFactory : IInstanceFactory<PropertyInfo>
{
  private readonly Func<Type, DataFieldValueDictionary<PropertyInfo>, object> factoryMethod;
  public DelegatedInstanceFactory(Func<Type, DataFieldValueDictionary<PropertyInfo>, object> factoryMethod)
  {
    this.factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
  }
  public object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> deserializedValues)
  {
    object ret = factoryMethod(targetType, deserializedValues);
    return ret;
  }
}

public class DelegatedInstanceFactory<T> : IInstanceFactory<PropertyInfo>
{
  private readonly Func<PropertyValuesDictionary<T>, object> factoryMethod;
  public DelegatedInstanceFactory(Func<PropertyValuesDictionary<T>, object> factoryMethod)
  {
    this.factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
  }
  public object CreateInstance(Type targetType, DataFieldValueDictionary<PropertyInfo> deserializedValues)
  {
    EAssert.IsTrue(typeof(T) == targetType, $"DelegatedInstanceFactory<{typeof(T)}> unable to create instance of '{targetType}'.");
    PropertyValuesDictionary<T> tmp = new();
    deserializedValues.ToList().ForEach(kv => tmp[kv.Key] = kv.Value);
    object ret = factoryMethod(tmp);
    return ret;
  }
}

