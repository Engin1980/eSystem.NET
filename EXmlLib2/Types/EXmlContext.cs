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

    public void AddSerializer(IElementSerializer serializer) => this.InsertSerializer(0, serializer);

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

    public void AddSerializer(IAttributeSerializer serializer) => this.InsertSerializer(0, serializer);

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
  }
}
