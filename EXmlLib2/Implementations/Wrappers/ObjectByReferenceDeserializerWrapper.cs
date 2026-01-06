using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Abstracts;
using EXmlLib2.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Wrappers;

public class ObjectByReferenceDeserializerWrapper : IElementDeserializer
{
  private const string REFERENCE_ID_ATTRIBUTE = ObjectByReferenceSerializerWrapper.REFERENCE_ID_ATTRIBUTE;
  public const string OBJECT_ID_ATTRIBUTE = ObjectByReferenceSerializerWrapper.OBJECT_ID_ATTRIBUTE;
  private const string CONTEXT_REFERENCES_KEY = "ObjectByReferenceDeserializerWrapper_References";
  private readonly IElementDeserializer innerDeserializer;

  public bool AcceptsType(Type type) => innerDeserializer.AcceptsType(type);

  public ObjectByReferenceDeserializerWrapper(IElementDeserializer innerSerializer)
  {
    EAssert.Argument.IsNotNull(innerSerializer, nameof(innerSerializer));
    innerDeserializer = innerSerializer;
  }

  public object? Deserialize(XElement element, Type type, IXmlContext ctx)
  {
    object? ret;

    Dictionary<int, object> references = GetContextReferences(ctx);

    var attr = element.Attribute(REFERENCE_ID_ATTRIBUTE);
    if (attr != null)
    {
      var id = int.Parse(attr.Value);
      if (references.TryGetValue(id, out ret))
      {
        EAssert.IsNotNull(ret, $"Referenced object with ID {0} is represented by 'null' in already deserialized objects.");
      }
      else
        throw new Exception("Referenced object with ID {0} not found within already deserialized objects.");
    }
    else
    {
      attr = element.Attribute(OBJECT_ID_ATTRIBUTE);
      ret = innerDeserializer.Deserialize(element, type, ctx);
      if (attr != null)
      {
        EAssert.IsNotNull(ret, "Deserialized object cannot be null.");
        var id = int.Parse(attr.Value);
        references[id] = ret;
      }
    }
    return ret;
  }

  private Dictionary<int, object> GetContextReferences(IXmlContext ctx)
  {
    return ctx.GetOrSetCustomData<Dictionary<int, object>>(CONTEXT_REFERENCES_KEY, () => []);
  }
}