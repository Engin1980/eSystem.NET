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

  public static DeserializationResult FromNoResult() => new(false, null);

  public static DeserializationResult FromNull() => new(true, null);

  public static DeserializationResult FromValue(object value) => new(true, value);

  public static DeserializationResult From(object? value) => value == null ? FromNull() : FromValue(value);
}
