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


namespace EXmlLib2.Implementations.Wrappers
{
  public class ObjectByReferenceSerializerWrapper<T> : TypedElementSerializer<T>
  {
    public const string REFERENCE_ID_ATTRIBUTE = "__REF_ID";
    public const string OBJECT_ID_ATTRIBUTE = "__OBJ_ID";
    public const string CONTEXT_REFERENCES_KEY = "ObjectByReferenceSerializerWrapper_References";
    private readonly TypedElementSerializer<T> innerSerializer;

    public ObjectByReferenceSerializerWrapper(TypedElementSerializer<T> innerSerializer)
    {
      EAssert.Argument.IsNotNull(innerSerializer, nameof(innerSerializer));
      this.innerSerializer = innerSerializer;
    }

    public override void Serialize(T value, XElement element, IXmlContext ctx)
    {
      EAssert.Argument.IsNotNull(value, nameof(value));

      Dictionary<object, int> references = GetReferencesFromXmlContext(ctx);

      if (references.TryGetValue(value, out var id))
        element.SetAttributeValue(REFERENCE_ID_ATTRIBUTE, id);
      else
      {
        innerSerializer.Serialize(value, element, ctx);
        id = references.Count > 0 ? references.Values.Max() + 1 : 1;
        references[value] = id;
        element.SetAttributeValue(OBJECT_ID_ATTRIBUTE, id);
      }
    }

    private Dictionary<object, int> GetReferencesFromXmlContext(IXmlContext ctx)
    {
      Dictionary<object, int> ret = ctx.GetOrSetData<Dictionary<object, int>>(CONTEXT_REFERENCES_KEY, () => []);
      return ret;
    }
  }
}