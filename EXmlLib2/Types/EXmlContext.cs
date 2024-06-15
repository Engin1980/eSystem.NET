using ELogging;
using ESystem.Asserting;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Types
{
  internal class EXmlContext : IXmlContext
  {
    private readonly Logger logger = Logger.Create(typeof(EXmlContext), "EXml+Ctx");
    private readonly List<IElementSerializer> elementSerializers = new();
    private readonly List<IAttributeSerializer> attributeSerializers = new();
    private readonly List<IElementDeserializer> elementDeserializers = new();
    private readonly List<IAttributeDeserializer> attributeDeserializers = new();

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

    public void AddSerializer(IElementSerializer serializer) => this.InsertSerializer(0, serializer);

    public void AddDeserializer(IElementDeserializer deserializer) => this.InsertDeserializer(0, deserializer);

    public void AddDeserializer<T>(IElementDeserializer<T> deserializer) => this.InsertDeserializer(0, deserializer);

    public void InsertSerializer(int index, IElementSerializer serializer)
    {
      try
      {
        this.elementSerializers.Insert(index, serializer);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Unable to insert element serializer at position {index}.", ex);
        logger.LogException(eex);
        throw eex;
      }
    }

    public void InsertDeserializer<T>(int index, IElementDeserializer<T> deserializer)
    {
      var w = new TypedElementDeserializerWrapper<T>(deserializer);
      this.elementDeserializers.Insert(index, w);
    }

    public void InsertDeserializer(int index, IElementDeserializer deserializer)
    {
      try
      {
        this.elementDeserializers.Insert(index, deserializer);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Unable to insert element deserializer at position {index}.", ex);
        logger.LogException(eex);
        throw eex;
      }
    }

    public void AddSerializer(IAttributeSerializer serializer) => this.InsertSerializer(0, serializer);

    public void AddDeserializer(IAttributeDeserializer deserializer) => this.InsertDeserializer(0, deserializer);

    public void AddDeserializer<T>(IAttributeDeserializer<T> deserializer) => this.InsertDeserializer(0, deserializer);

    public void InsertSerializer(int index, IAttributeSerializer serializer)
    {
      try
      {
        this.attributeSerializers.Insert(index, serializer);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Unable to insert attribute serializer at position {index}.", ex);
        logger.LogException(eex);
        throw eex;
      }
    }

    public void InsertDeserializer(int index, IAttributeDeserializer deserializer)
    {
      try
      {
        this.attributeDeserializers.Insert(index, deserializer);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Unable to insert attribute deserializer at position {index}.", ex);
        logger.LogException(eex);
        throw eex;
      }
    }

    public void InsertDeserializer<T>(int index, IAttributeDeserializer<T> deserializer)
    {
      TypedAttributeDeserializerWrapper<T> w = new(deserializer);
      InsertDeserializer(index, w);
    }

    public IElementSerializer<T> GetElementSerializer<T>()
    {
      IElementSerializer<T> ret;
      try
      {
        ret = (IElementSerializer<T>)elementSerializers.First(q => q is IElementSerializer<T>);
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to find element serializer for type {typeof(T).Name}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
    }

    public IElementSerializer GetElementSerializer(object? value)
    {
      IElementSerializer ret;
      try
      {
        ret = elementSerializers.First(q => q.AcceptsValue(value));
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to find element serializer for value {value}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
    }

    public IElementDeserializer GetElementDeserializer(Type type)
    {
      IElementDeserializer ret;
      try
      {
        ret = elementDeserializers.First(q => q.AcceptsType(type));
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to find element deserializer for type {type}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
    }

    public void SerializeToElement(object? value, XElement element, IElementSerializer serializer)
    {
      EAssert.Argument.IsNotNull(element, nameof(element));
      EAssert.Argument.IsNotNull(serializer, nameof(serializer));
      logger.Log(LogLevel.INFO, $"Serializing {value} to {element} using {serializer}.");
      try
      {
        EAssert.IsTrue(serializer.AcceptsValue(value));
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

    internal void AddSerializer<T>(IElementSerializer<T> elementSerializer)
    {
      TypedElementSerializerWrapper<T> w = new(elementSerializer);
      AddSerializer(w);
    }

    internal void AddSerializer<T>(IAttributeSerializer<T> attributeSerializer)
    {
      TypedAttributeSerializerWrapper<T> w = new(attributeSerializer);
      AddSerializer(w);
    }

    public IAttributeSerializer<T> GetAttributeSerializer<T>(T? value)
    {
      IAttributeSerializer<T> ret;
      try
      {
        ret = (IAttributeSerializer<T>)attributeSerializers.First(q => q is IAttributeSerializer<T>);
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to find attribute serializer for type {typeof(T).Name}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
    }

    public IAttributeSerializer GetAttributeSerializer(object? value)
    {
      IAttributeSerializer ret;
      try
      {
        ret = attributeSerializers.First(q => q.AcceptsValue(value));
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to find attribute serializer for value {value}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
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

    public IAttributeDeserializer GetAttributeDeserializer(Type targetType)
    {
      IAttributeDeserializer ret;
      try
      {
        ret = attributeDeserializers.First(q => q.AcceptsType(targetType));
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to find attribute deserializer for type {targetType}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
    }

    public IAttributeDeserializer<T> GetAttributeDeserializer<T>()
    {
      IAttributeDeserializer<T> ret;
      try
      {
        ret = (IAttributeDeserializer<T>)attributeDeserializers.First(q => q is IAttributeDeserializer<T>);
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to find attribute deserializer for type {typeof(T).Name}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
    }

    public IElementDeserializer<T> GetElementDeserializer<T>()
    {
      IElementDeserializer<T> ret;
      try
      {
        ret = (IElementDeserializer<T>)elementDeserializers.First(q => q is IElementDeserializer<T>);
      }
      catch (Exception ex)
      {
        EXmlException eex = new($"Failed to find element deserializer for type {typeof(T).Name}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
    }
  }
}
