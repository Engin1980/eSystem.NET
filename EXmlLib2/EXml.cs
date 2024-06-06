using System.Xml.Linq;
using ELogging;
using EXmlLib2.Implementations.Deserializers;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Interfaces;
using EXmlLib2.Types;

namespace EXmlLib2
{
  public class EXml
  {
    private readonly EXmlContext ctx = new();
    private readonly Logger logger = Logger.Create(typeof(EXml), "EXml");

    public string DefaultNullString { get => ctx.DefaultNullString; set => ctx.DefaultNullString = value; }

    public static EXml CreateDefault(bool addDefaultSerializers = true, bool addDefaultDeserializers = true)
    {
      EXml ret = new();
      if (addDefaultSerializers)
      {
        ret.ctx.AddSerializer((IElementSerializer)new NullSerializer());
        ret.ctx.AddSerializer((IAttributeSerializer)new NullSerializer());

        ret.ctx.AddSerializer((IElementSerializer)new NumberSerializer());
        ret.ctx.AddSerializer((IAttributeSerializer)new NumberSerializer());

        ret.ctx.AddSerializer((IElementSerializer<string>)new StringSerializer());
        ret.ctx.AddSerializer((IAttributeSerializer<string>)new StringSerializer());

        ret.ctx.AddSerializer((IElementSerializer<bool>)new BoolSerializer());
        ret.ctx.AddSerializer((IAttributeSerializer<bool>)new BoolSerializer());

        ret.ctx.AddSerializer((IElementSerializer<char>)new CharSerializer());
        ret.ctx.AddSerializer((IAttributeSerializer<char>)new CharSerializer());
      }
      if (addDefaultDeserializers)
      {
        ret.ctx.AddDeserializer((IElementDeserializer)new NumberDeserializer());
        ret.ctx.AddDeserializer((IAttributeDeserializer)new NumberDeserializer());

        ret.ctx.AddDeserializer((IElementDeserializer)new NullableNumberDeserializer());
        ret.ctx.AddDeserializer((IAttributeDeserializer)new NullableNumberDeserializer());
      }
      return ret;
    }
    public static EXml CreateEmpty() => new();

    private EXml()
    {
    }

    public void Serialize<T>(T? value, XElement element) => Serialize((object?)value, element);

    public void Serialize(object? value, XElement element)
    {
      logger.Log(LogLevel.INFO, $"Serializing {value} to {element}");
      try
      {
        IElementSerializer serializer = ctx.GetElementSerializer(value);
        ctx.SerializeToElement(value, element, serializer);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Failed to serialize {value} to {element}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      logger.Log(LogLevel.INFO, $"Serialized {value} to {element}.");
    }

    public T? Deserialize<T>(XElement element) => (T)Deserialize(element, typeof(T))!;

    public object? Deserialize(XElement element, Type expectedType)
    {
      object? ret;
      logger.Log(LogLevel.INFO, $"Deserializing type {expectedType.Name} from {element}.");
      try
      {
        IElementDeserializer deserializer = ctx.GetElementDeserializer(expectedType);
        ret = ctx.DeserializeFromElement(element, expectedType, deserializer);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Failed to deserialize {expectedType.Name} to {element}.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
    }

    public void AddSerializer(IElementSerializer serializer)
    {
      this.ctx.AddSerializer(serializer);
    }

    public void InsertSerializer<T>(int index, IElementSerializer<T> serializer)
    {
      TypedElementSerializerWrapper<T> w = new(serializer);
      this.InsertSerializer(index, w);
    }

    public void InsertSerializer(int index, IElementSerializer serializer)
    {
      this.ctx.InsertSerializer(index, serializer);
    }

    public void AddSerializer(IAttributeSerializer serializer)
    {
      this.ctx.AddSerializer(serializer);
    }

    public void InsertSerializer(int index, IAttributeSerializer serializer)
    {
      this.ctx.InsertSerializer(index, serializer);
    }
  }
}
