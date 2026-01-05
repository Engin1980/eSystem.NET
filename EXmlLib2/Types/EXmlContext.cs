using ESystem.Asserting;
using ESystem.Logging;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Types.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static EXmlLib2.Abstractions.IXmlContext;

namespace EXmlLib2.Types
{
  internal class EXmlContext : IXmlContext
  {
    private readonly Logger logger = Logger.Create(typeof(EXmlContext), "EXml+Ctx");
    public SerializerDeserializerRegistry<IElementSerializer> ElementSerializers { get; private init; } = new();
    public SerializerDeserializerRegistry<IAttributeSerializer> AttributeSerializers { get; private init; } = new();
    public SerializerDeserializerRegistry<IElementDeserializer> ElementDeserializers { get; private init; } = new();
    public SerializerDeserializerRegistry<IAttributeDeserializer> AttributeDeserializers { get; private init; } = new();
    public XmlPropertyInfo DefaultXmlPropertyInfo { get; private init; } = new();

    private CultureInfo _DefaultCultureInfo = CultureInfo.GetCultureInfo("en-US");
    public CultureInfo DefaultCultureInfo
    {
      get => _DefaultCultureInfo;
      set => _DefaultCultureInfo = value ?? throw new ArgumentNullException();
    }

    private string _DefaultNullString = "(# null #)";
    public string DefaultNullString
    {
      get => _DefaultNullString;
      set => _DefaultNullString = value ?? throw new ArgumentNullException();
    }

    private string _DefaultTrueString = "True";
    public string DefaultTrueString
    {
      get => _DefaultTrueString;
      set => _DefaultTrueString = value ?? throw new ArgumentNullException();
    }

    private string _DefaultFalseString = "False";
    public string DefaultFalseString
    {
      get => _DefaultFalseString;
      set => _DefaultFalseString = value ?? throw new ArgumentNullException();
    }

    private string _TypeNameAttribute = "__type";
    public string TypeNameAttribute { get => _TypeNameAttribute; set => _TypeNameAttribute = value ?? throw new ArgumentNullException(); }

    private string _DefaultItemXmlName = "item";
    public string? DefaultItemXmlName
    {
      get => _DefaultItemXmlName;
      set => _DefaultItemXmlName = value ?? throw new ArgumentNullException();
    }


    public void SerializeToElement(object? value, XElement element, IElementSerializer serializer)
    {
      EAssert.Argument.IsNotNull(element, nameof(element));
      EAssert.Argument.IsNotNull(serializer, nameof(serializer));
      logger.Log(LogLevel.INFO, $"Serializing {value} to {element} using {serializer}.");
      try
      {
        serializer.Serialize(value, element, this);
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to serialize value {value} using serializer {serializer}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      logger.Log(LogLevel.INFO, $"Serialized {value} to {element} using {serializer}.");
    }

    internal object? DeserializeFromElement(XElement element, Type targetType, IElementDeserializer deserializer)
    {
      object? ret;
      EAssert.Argument.IsNotNull(element, nameof(element));
      EAssert.Argument.IsNotNull(deserializer, nameof(deserializer));
      logger.Log(LogLevel.INFO, $"Deserializing {targetType} from {element} using {deserializer}.");
      try
      {
        EAssert.IsTrue(deserializer.AcceptsType(targetType));
        ret = deserializer.Deserialize(element, targetType, this);
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to deserialize type {targetType} using serializer {deserializer}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      logger.Log(LogLevel.INFO, $"Deserialized {targetType} from {element} using {deserializer} to {ret}.");
      return ret;
    }

    private readonly Dictionary<string, object> customDataStore = [];
    public void SetCustomData<T>(string key, T data) => customDataStore[key] = data!;
    public T? GetCustomData<T>(string key) => (T?)customDataStore[key];
    public T GetOrSetCustomData<T>(string key, Func<T> newDataProvider)
    {
      T ret;
      if (customDataStore.TryGetValue(key, out object? tmp))
        ret = (T)tmp!;
      else
      {
        ret = newDataProvider();
        customDataStore[key] = ret!;
      }
      return ret;
    }

    public XAttribute SerializeToAttribute(string name, object? value, IAttributeSerializer serializer)
    {
      XAttribute ret;
      EAssert.Argument.IsNotNull(name, nameof(name));
      EAssert.Argument.IsNotNull(serializer, nameof(serializer));
      logger.Log(LogLevel.INFO, $"Serializing {value} to attribute {name} using {serializer}.");
      try
      {
        ret = new XAttribute(XName.Get(name), serializer.Serialize(value, this));
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to serialize value {value} using serializer {serializer}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      logger.Log(LogLevel.INFO, $"Serializing {value} to attribute {name} using {serializer}.");
      return ret;
    }
  }
}
