using ESystem;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.Abstractions;

public abstract class TypeDeserializerBase : IElementDeserializer
{
  public const string TYPE_NAME_ATTRIBUTE = TypeSerializerBase.TYPE_NAME_ATTRIBUTE;
  public abstract bool AcceptsType(Type type);

  protected abstract DeserializationResult DeserializeDataMember(Type targetType, string dataMemberName, XElement element, IXmlContext ctx);
  protected abstract IEnumerable<string> GetDataMemberNames(Type type);
  protected abstract object CreateInstance(Type targetType, Dictionary<string, object?> deserializedValues);

  public delegate void BeforeXmlReadoutDelegate(XElement element);
  public event BeforeXmlReadoutDelegate? BeforeXmlReadout;

  public delegate void BeforeInstanceCreationDelegate(Dictionary<string, object?> deserializedValues);
  public event BeforeInstanceCreationDelegate? BeforeInstanceCreation;

  public delegate void AfterInstanceCreationDelegate(object createdInstance);
  public event AfterInstanceCreationDelegate? AfterInstanceCreation;

  public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
  {
    BeforeXmlReadout?.Invoke(element);
    IEnumerable<string> dataMemberNames = GetDataMemberNames(targetType);
    Dictionary<string, object?> deserializedValues = [];
    foreach (string dataMemberName in dataMemberNames)
    {
      DeserializationResult tmp = DeserializeDataMember(targetType, dataMemberName, element, ctx);
      if (tmp.HasResult)
        deserializedValues[dataMemberName] = tmp.Value;
    }

    Type realTargetType = EvaluateRealTargetType(element, targetType);

    BeforeInstanceCreation?.Invoke(deserializedValues);
    object ret;
    try
    {
      ret = CreateInstance(realTargetType, deserializedValues);
    }
    catch (Exception ex)
    {
      throw new EXmlException($"Failed to create an instance of {realTargetType.Name}.", ex);
    }

    AfterInstanceCreation?.Invoke(ret);
    return ret;
  }

  private static Type EvaluateRealTargetType(XElement element, Type targetType)
  {
    Type ret;
    XAttribute? typeAttribute = element.Attribute(TYPE_NAME_ATTRIBUTE);
    if (typeAttribute != null)
    {
      string typeName = typeAttribute.Value;
      Type? realType = ResolveType(typeName);
      ret = realType ?? targetType;
    }
    else
      ret = targetType;
    return ret;
  }

  private static Type? ResolveType(string typeName)
  {
    // direct type load
    var type = Type.GetType(typeName, throwOnError: false);
    if (type != null)
      return type;

    // searching in loaded/referenced assemblies
    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
    {
      type = asm.GetType(typeName, throwOnError: false);
      if (type != null)
        return type;
    }

    return null;
  }
}
