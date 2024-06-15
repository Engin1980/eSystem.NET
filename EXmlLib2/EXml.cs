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
    #region Private Fields

    private readonly EXmlContext ctx = new();
    private readonly Logger logger = Logger.Create(typeof(EXml), "EXml");

    #endregion Private Fields

    #region Public Properties

    public string DefaultNullString { get => ctx.DefaultNullString; set => ctx.DefaultNullString = value; }

    #endregion Public Properties

    #region Private Constructors

    private EXml()
    {
    }

    #endregion Private Constructors

    #region Public Methods

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

        ret.ctx.AddSerializer((IElementSerializer)new EnumSerializer());
        ret.ctx.AddSerializer((IAttributeSerializer)new EnumSerializer());

        ret.ctx.AddSerializer(new TypeElementSerializer<object>());
        ret.ctx.AddSerializer(new EnumerableSerializer());
      }
      if (addDefaultDeserializers)
      {
        ret.ctx.AddDeserializer((IElementDeserializer)new NumberDeserializer());
        ret.ctx.AddDeserializer((IAttributeDeserializer)new NumberDeserializer());

        //ret.ctx.AddDeserializer((IElementDeserializer)new NullableNumberDeserializer());
        //ret.ctx.AddDeserializer((IAttributeDeserializer)new NullableNumberDeserializer());

        ret.ctx.AddDeserializer((IElementDeserializer<char>)new CharDeserializer());
        ret.ctx.AddDeserializer((IAttributeDeserializer<char>)new CharDeserializer());

        //ret.ctx.AddDeserializer((IElementDeserializer<char?>)new NullableCharDeserializer());
        //ret.ctx.AddDeserializer((IAttributeDeserializer<char?>)new NullableCharDeserializer());

        ret.ctx.AddDeserializer((IElementDeserializer<bool>)new BoolDeserializer(true));
        ret.ctx.AddDeserializer((IAttributeDeserializer<bool>)new BoolDeserializer(true));

        //ret.ctx.AddDeserializer((IElementDeserializer<bool?>)new NullableBoolDeserializer(true));
        //ret.ctx.AddDeserializer((IAttributeDeserializer<bool?>)new NullableBoolDeserializer(true));

        ret.ctx.AddDeserializer((IElementDeserializer<string?>)new StringDeserializer());
        ret.ctx.AddDeserializer((IAttributeDeserializer<string?>)new StringDeserializer());

        ret.ctx.AddDeserializer((IElementDeserializer)new EnumDeserializer());
        ret.ctx.AddDeserializer((IAttributeDeserializer)new EnumDeserializer());

        ret.ctx.AddDeserializer((IElementDeserializer)new NullableDeserializer());
        ret.ctx.AddDeserializer((IAttributeDeserializer)new NullableDeserializer());
      }
      return ret;
    }

    public static EXml CreateEmpty() => new();

    public void AddSerializer<T>(IElementSerializer<T> serializer)
    {
      var w = new TypedElementSerializerWrapper<T>(serializer);
      this.ctx.AddSerializer(w);
    }

    public void AddSerializer(IAttributeSerializer serializer)
    {
      this.ctx.AddSerializer(serializer);
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

    public void InsertDeserializer<T>(int index, IElementDeserializer<T> deserializer)
    {
      TypedElementDeserializerWrapper<T> w = new(deserializer);
      this.InsertDeserializer(index, w);
    }

    public void InsertDeserializer(int index, IElementDeserializer deserializer)
    {
      this.ctx.InsertDeserializer(index, deserializer);
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

    public void InsertSerializer(int index, IAttributeSerializer serializer)
    {
      this.ctx.InsertSerializer(index, serializer);
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

    public void AddSerializer<T>(TypeElementSerializer<T> serializer) where T : notnull
    {
      this.ctx.AddSerializer(serializer);
    }

    public void AddDeserializer<T>(TypeElementDeserializer<T> typeElementDeserializer)
    {
      this.ctx.AddDeserializer(typeElementDeserializer);
    }

    #endregion Public Methods
  }
}
