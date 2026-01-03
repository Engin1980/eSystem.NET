using System.Xml.Linq;
using ESystem.Asserting;
using ESystem.Exceptions;
using ESystem.Logging;
using EXmlLib2.Implementations.Deserializers;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Abstractions;
using EXmlLib2.Types;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Types.Internal;

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
    public SerializerDeserializerRegistry<IElementSerializer> ElementSerializers => ctx.ElementSerializers;
    public SerializerDeserializerRegistry<IAttributeSerializer> AttributeSerializers => ctx.AttributeSerializers;
    public SerializerDeserializerRegistry<IElementDeserializer> ElementDeserializers => ctx.ElementDeserializers;
    public SerializerDeserializerRegistry<IAttributeDeserializer> AttributeDeserializers => ctx.AttributeDeserializers;

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
        List<IElementSerializer> elmSer = [];
        List<IAttributeSerializer> attSer = [];

        elmSer.Add(new NullableSerializer());
        attSer.Add(new NullableSerializer());

        elmSer.Add(new NumberSerializer());
        attSer.Add(new NumberSerializer());

        elmSer.Add(new StringSerializer().AsNullableElementWrapper());
        attSer.Add(new StringSerializer().AsNullableAttributeWrapper());

        elmSer.Add(new BoolSerializer());
        attSer.Add(new BoolSerializer());

        elmSer.Add(new CharSerializer());
        attSer.Add(new CharSerializer());

        elmSer.Add(new EnumSerializer());
        attSer.Add(new EnumSerializer());

        elmSer.Add(new DateTimeSerializer().AsNullableElementWrapper());
        attSer.Add(new DateTimeSerializer().AsNullableAttributeWrapper());

        elmSer.Add(new EnumerableSerializer());
        elmSer.Add(new SpecificTypeElementSerializer<object>(Abstractions.Abstracts.DerivedTypesBehavior.AllowDerivedTypes));

        ret.ctx.ElementSerializers.Set(elmSer);
        ret.ctx.AttributeSerializers.Set(attSer);
      }
      if (addDefaultDeserializers)
      {
        //ret.ctx.AddDeserializer((IElementDeserializer)new NullableNumberDeserializer());
        //ret.ctx.AddDeserializer((IAttributeDeserializer)new NullableNumberDeserializer());

        List<IElementDeserializer> elmDeser = [];
        List<IAttributeDeserializer> attDeser = [];

        elmDeser.Add(new NumberDeserializer());
        attDeser.Add(new NumberDeserializer());

        elmDeser.Add(new CharDeserializer());
        attDeser.Add(new CharDeserializer());

        elmDeser.Add(new BoolDeserializer(true));
        attDeser.Add(new BoolDeserializer(true));

        elmDeser.Add(new StringDeserializer());
        attDeser.Add(new StringDeserializer());

        elmDeser.Add(new EnumDeserializer());
        attDeser.Add(new EnumDeserializer());

        elmDeser.Add(new NullableDeserializer());
        attDeser.Add(new NullableDeserializer());

        elmDeser.Add(new DateTimeDeserializer().AsNullableElementWrapper());
        attDeser.Add(new DateTimeDeserializer().AsNullableAttributeWrapper());

        elmDeser.Add(new SpecificTypeElementDeserializer<object>(Abstractions.Abstracts.DerivedTypesBehavior.AllowDerivedTypes));

        ret.ctx.ElementDeserializers.Set(elmDeser);
        ret.ctx.AttributeDeserializers.Set(attDeser);
      }
      return ret;
    }

    public static EXml CreateEmpty() => new();

    public T? Deserialize<T>(XElement element) => (T)Deserialize(element, typeof(T))!;

    public object? Deserialize(XElement element, Type expectedType)
    {
      object? ret;
      logger.Log(LogLevel.INFO, $"Deserializing type {expectedType.Name} from {element}.");
      try
      {
        IElementDeserializer deserializer = ctx.ElementDeserializers.GetByType(expectedType);
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

    public void Serialize<T>(T? value, XElement element) => Serialize(value, typeof(T), element);

    public void Serialize(object? value, Type expectedType, XElement element)
    {
      EAssert.Argument.IsTrue(
        value == null || expectedType.IsAssignableFrom(value.GetType()),
        nameof(value),
        $"Value type '{value?.GetType().Name ?? "null"}' does not match expected type '{expectedType.Name}'.");

      logger.Log(LogLevel.INFO, $"Serializing {value} to {element}");
      try
      {
        IElementSerializer serializer = ctx.ElementSerializers.GetByType(expectedType);
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

    #endregion Public Methods
  }
}
