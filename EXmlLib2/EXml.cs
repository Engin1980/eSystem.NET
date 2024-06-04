using System.Xml.Linq;
using ELogging;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Interfaces;
using EXmlLib2.Types;

namespace EXmlLib2
{
  public class EXml
  {
    private readonly EXmlContext ctx = new();
    private readonly Logger logger = Logger.Create(typeof(EXml), "EXml");

    public static EXml CreateDefault(bool addDefaultSerializers = true)
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
      return ret;
    }
    public static EXml CreateEmpty() => new();

    private EXml()
    {
    }

    public void Serialize<T>(T? value, XElement elm) => Serialize((object?)value, elm);

    public void Serialize(object? value, XElement elm)
    {
      logger.Log(LogLevel.INFO, $"Serializing {value} to {elm}");
      try
      {
        IElementSerializer serializer = ctx.GetElementSerializer(value);
        ctx.SerializeToElement(value, elm, serializer);
      }
      catch (Exception ex)
      {
        throw new EXmlException($"Failed to serialize {value} to {elm}.", ex);
      }
      logger.Log(LogLevel.INFO, $"Serialized {value} to {elm}.");
    }

    public T? Deserialize<T>(XElement root)
    {
      throw new NotImplementedException();
    }
  }
}
