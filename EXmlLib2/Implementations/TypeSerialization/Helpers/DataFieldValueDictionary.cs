namespace EXmlLib2.Implementations.TypeSerialization.Helpers;

public abstract class DataFieldValueDictionary<TDataFieldInfo> : Dictionary<TDataFieldInfo, object?> where TDataFieldInfo : notnull
{
  public abstract object? this[string name] { get; }
  public T? Get<T>(string name)
  {
    object? tmp = this[name];
    T? ret = (T?)tmp;
    return ret;
  }
}


