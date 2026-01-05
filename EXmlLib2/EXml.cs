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
using EXmlLib2.Abstractions.Abstracts;
using System.Runtime.CompilerServices;

namespace EXmlLib2
{
  public class EXml
  {
    public enum XmlSupport
    {
      Attributes = 1,
      Elements = 2,
      AttributesAndElements = Attributes | Elements
    }
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

    public static EXml Create() => new();

    public static EXml CreateWithDefaultSerialization() => new EXml().WithDefaultSerialization();

    public EXml WithDefaultSerialization(XmlSupport xmlSupport = XmlSupport.AttributesAndElements) => this
      .WithPrimitiveTypesAndStringSerialization(xmlSupport)
      .WithCommonTypesSerialization(xmlSupport)
      .WithObjectSerialization();

    public EXml WithPrimitiveTypesAndStringSerialization(XmlSupport xmlSupport = XmlSupport.AttributesAndElements) => this
      .WithPrimitiveTypesAndStringSerializers(xmlSupport)
      .WithPrimitiveTypesAndStringDeserializers(xmlSupport);

    public EXml WithPrimitiveTypesAndStringSerializers(XmlSupport xmlSupport = XmlSupport.AttributesAndElements)
    {
      if (xmlSupport.HasFlag(XmlSupport.Elements))
      {
        List<IElementSerializer> lst = [
          new NullableSerializer(),
          new NumberSerializer(),
          new BoolSerializer(),
          new CharSerializer(),
          new EnumSerializer(),
          new StringSerializer().AsNullableElementWrapper()
        ];
        this.ctx.ElementSerializers.AddFirst(lst);
      }
      if (xmlSupport.HasFlag(XmlSupport.Attributes))
      {
        List<IAttributeSerializer> lst = [
          new NullableSerializer(),
          new NumberSerializer(),
          new BoolSerializer(),
          new CharSerializer(),
          new EnumSerializer(),
          new StringSerializer().AsNullableAttributeWrapper()
        ];
        this.ctx.AttributeSerializers.AddFirst(lst);
      }

      return this;
    }

    public EXml WithPrimitiveTypesAndStringDeserializers(XmlSupport xmlSupport = XmlSupport.AttributesAndElements)
    {
      if (xmlSupport.HasFlag(XmlSupport.Elements))
      {
        List<IElementDeserializer> lst = [
          new NullableDeserializer(),
          new NumberDeserializer(),
          new BoolDeserializer(true),
          new CharDeserializer(),
          new EnumDeserializer(),
          new StringDeserializer().AsNullableElementWrapper()
        ];
        this.ctx.ElementDeserializers.AddFirst(lst);
      }
      if (xmlSupport.HasFlag(XmlSupport.Attributes))
      {
        List<IAttributeDeserializer> lst = [
          new NullableDeserializer(),
          new NumberDeserializer(),
          new BoolDeserializer(true),
          new CharDeserializer(),
          new EnumDeserializer(),
          new StringDeserializer().AsNullableAttributeWrapper()
        ];
        this.ctx.AttributeDeserializers.AddFirst(lst);
      }

      return this;
    }

    public EXml WithCommonTypesSerialization(XmlSupport xmlSupport = XmlSupport.AttributesAndElements) => this
      .WithCommonTypesSerializers(xmlSupport)
      .WithCommonTypesDeserializers(xmlSupport);

    public EXml WithCommonTypesSerializers(XmlSupport xmlSupport = XmlSupport.AttributesAndElements)
    {
      if (xmlSupport.HasFlag(XmlSupport.Elements))
        this.ctx.ElementSerializers.AddFirst(new DateTimeSerializer().AsNullableElementWrapper());
      if (xmlSupport.HasFlag(XmlSupport.Attributes))
        this.ctx.AttributeSerializers.AddFirst(new DateTimeSerializer().AsNullableAttributeWrapper());
      return this;
    }

    public EXml WithCommonTypesDeserializers(XmlSupport xmlSupport = XmlSupport.AttributesAndElements)
    {
      if (xmlSupport.HasFlag(XmlSupport.Elements))
        this.ctx.ElementDeserializers.AddFirst(new DateTimeDeserializer().AsNullableElementWrapper());
      if (xmlSupport.HasFlag(XmlSupport.Attributes))
        this.ctx.AttributeDeserializers.AddFirst(new DateTimeDeserializer().AsNullableAttributeWrapper());
      return this;
    }

    public EXml WithEnumerableSerialization() => this.WithEnumerableSerializers().WithEnumerableDeserializers();

    private EXml WithEnumerableDeserializers()
    {
      //TODO implement
      return this;
    }

    private EXml WithEnumerableSerializers()
    {
      //TODO implement
      return this;
    }

    public EXml WithObjectSerialization() => this.WithObjectSerializers().WithObjectDeserializers();

    private EXml WithObjectDeserializers()
    {
      this.ElementDeserializers.AddLast(new NewTypeByPropertyDeserializer().WithAcceptedType<object>(true));
      return this;
    }

    private EXml WithObjectSerializers()
    {
      this.ElementSerializers.AddLast(new NewTypeByPropertySerializer().WithAcceptedType<object>(true));
      return this;
    }

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
