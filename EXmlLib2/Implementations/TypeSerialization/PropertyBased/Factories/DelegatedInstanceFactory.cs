using ESystem.Asserting;
using EXmlLib2.Implementations.TypeSerialization.Factories;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using EXmlLib2.Types;
using System.Reflection;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Factories;

public class DelegatedInstanceFactory : IInstanceFactory
{
  private readonly Func<Type, Dictionary<string, object?>, object> factoryMethod;
  public DelegatedInstanceFactory(Func<Type, Dictionary<string, object?>, object> factoryMethod)
  {
    this.factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
  }
  public object CreateInstance(Type targetType, Dictionary<string, object?> deserializedValues)
  {
    object ret = factoryMethod(targetType, deserializedValues);
    return ret;
  }
}

public class DelegatedInstanceFactory<T> : IInstanceFactory
{
  private readonly Func<PropertyValuesDictionary<T>, object> factoryMethod;
  public DelegatedInstanceFactory(Func<PropertyValuesDictionary<T>, object> factoryMethod)
  {
    this.factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
  }
  public object CreateInstance(Type targetType, Dictionary<string, object?> deserializedValues)
  {
    EAssert.IsTrue(typeof(T) == targetType, $"DelegatedInstanceFactory<{typeof(T)}> unable to create instance of '{targetType}'.");
    PropertyValuesDictionary<T> tmp = [];

    Type type = typeof(T);
    foreach (var item in deserializedValues)
    {
      PropertyInfo pi = type.GetProperty(item.Key)!;
      tmp[pi] = item.Value;
    }

    object ret = factoryMethod(tmp);
    return ret;
  }
}

