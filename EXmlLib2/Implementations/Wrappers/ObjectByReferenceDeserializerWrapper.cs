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

public class ObjectByReferenceDeserializerWrapper<T> : TypedElementDeserializer<T>
{
  private const string REFERENCE_ID_ATTRIBUTE = ObjectByReferenceSerializerWrapper<T>.REFERENCE_ID_ATTRIBUTE;
  public const string OBJECT_ID_ATTRIBUTE = ObjectByReferenceSerializerWrapper<T>.OBJECT_ID_ATTRIBUTE;
  private const string CONTEXT_REFERENCES_KEY = "ObjectByReferenceDeserializerWrapper_References";
  private readonly TypedElementDeserializer<T> innerDeserializer;

  public ObjectByReferenceDeserializerWrapper(TypedElementDeserializer<T> innerSerializer)
  {
    EAssert.Argument.IsNotNull(innerSerializer, nameof(innerSerializer));
    innerDeserializer = innerSerializer;
  }

  public override T Deserialize(XElement element, IXmlContext ctx)
  {
    T ret;

    Dictionary<int, object> references = GetContextReferences(ctx);

    var attr = element.Attribute(REFERENCE_ID_ATTRIBUTE);
    if (attr != null)
    {
      var id = int.Parse(attr.Value);
      if (references.TryGetValue(id, out object? tmp))
      {
        EAssert.IsNotNull(tmp, $"Referenced object with ID {0} is represented by 'null' in already deserialized objects.");
        ret = (T)tmp;
      }
      else
        throw new Exception("Referenced object with ID {0} not found within already deserialized objects.");
    }
    else
    {
      attr = element.Attribute(OBJECT_ID_ATTRIBUTE);
      ret = innerDeserializer.Deserialize(element, ctx);
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
    return ctx.GetOrSetData<Dictionary<int, object>>(CONTEXT_REFERENCES_KEY, () => []);
  }
}