namespace EXmlLib2.Implementations.TypeSerialization.Helpers;

public readonly struct DeserializationResult
{
  public bool HasResult { get; }
  public bool IsNull { get => Value == null; }
  public object? Value { get; }

  private DeserializationResult(bool hasResult, object? value)
  {
    HasResult = hasResult;
    Value = value;
  }

  public static DeserializationResult NoResult() => new(false, null);

  public static DeserializationResult Null() => new(true, null);

  public static DeserializationResult ValueResult(object value) => new(true, value);

  public static DeserializationResult Result(object? value) => value == null ? Null() : ValueResult(value);
}
