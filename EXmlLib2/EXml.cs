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
        ret.ctx.ElementSerializers.Push(new NullableSerializer());
        ret.ctx.AttributeSerializers.Push(new NullableSerializer());

        ret.ctx.ElementSerializers.Push(new NumberSerializer());
        ret.ctx.AttributeSerializers.Push(new NumberSerializer());

        ret.ctx.ElementSerializers.Push(new StringSerializer().AsNullableElementWrapper());
        ret.ctx.AttributeSerializers.Push(new StringSerializer().AsNullableAttributeWrapper());

        ret.ctx.ElementSerializers.Push(new BoolSerializer());
        ret.ctx.AttributeSerializers.Push(new BoolSerializer());

        ret.ctx.ElementSerializers.Push(new CharSerializer());
        ret.ctx.AttributeSerializers.Push(new CharSerializer());

        ret.ctx.ElementSerializers.Push(new EnumSerializer());
        ret.ctx.AttributeSerializers.Push(new EnumSerializer());

        ret.ctx.ElementSerializers.Push(new DateTimeSerializer().AsNullableElementWrapper());
        ret.ctx.AttributeSerializers.Push(new DateTimeSerializer().AsNullableAttributeWrapper());

        ret.ctx.ElementSerializers.Push(new EnumerableSerializer());
      }
      if (addDefaultDeserializers)
      {
        ret.ctx.ElementDeserializers.Push(new NumberDeserializer());
        ret.ctx.AttributeDeserializers.Push(new NumberDeserializer());

        //ret.ctx.AddDeserializer((IElementDeserializer)new NullableNumberDeserializer());
        //ret.ctx.AddDeserializer((IAttributeDeserializer)new NullableNumberDeserializer());

        ret.ctx.ElementDeserializers.Push(new CharDeserializer());
        ret.ctx.AttributeDeserializers.Push(new CharDeserializer());

        //ret.ctx.AddDeserializer((IElementDeserializer<char?>)new NullableCharDeserializer());
        //ret.ctx.AddDeserializer((IAttributeDeserializer<char?>)new NullableCharDeserializer());

        ret.ctx.ElementDeserializers.Push(new BoolDeserializer(true));
        ret.ctx.AttributeDeserializers.Push(new BoolDeserializer(true));

        //ret.ctx.AddDeserializer((IElementDeserializer<bool?>)new NullableBoolDeserializer(true));
        //ret.ctx.AddDeserializer((IAttributeDeserializer<bool?>)new NullableBoolDeserializer(true));

        ret.ctx.ElementDeserializers.Push(new StringDeserializer());
        ret.ctx.AttributeDeserializers.Push(new StringDeserializer());

        ret.ctx.ElementDeserializers.Push(new EnumDeserializer());
        ret.ctx.AttributeDeserializers.Push(new EnumDeserializer());

        ret.ctx.ElementDeserializers.Push(new NullableDeserializer());
        ret.ctx.AttributeDeserializers.Push(new NullableDeserializer());

        ret.ctx.ElementDeserializers.Push(new DateTimeDeserializer().AsNullableElementWrapper());
        ret.ctx.AttributeDeserializers.Push(new DateTimeDeserializer().AsNullableAttributeWrapper());
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
