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
  public class ObjectByReferenceSerializerWrapper : IElementSerializer
  {
    public const string REFERENCE_ID_ATTRIBUTE = "__referenceId";
    public const string OBJECT_ID_ATTRIBUTE = "__objectId";
    public const string CONTEXT_REFERENCES_KEY = "ObjectByReferenceSerializerWrapper_References";
    private readonly IElementSerializer innerSerializer;

    public ObjectByReferenceSerializerWrapper(IElementSerializer innerSerializer)
    {
      EAssert.Argument.IsNotNull(innerSerializer, nameof(innerSerializer));
      this.innerSerializer = innerSerializer;
    }

    public bool AcceptsType(Type type) => innerSerializer.AcceptsType(type);

    public void Serialize(object? value, Type expectedType, XElement element, IXmlContext ctx)
    {
      EAssert.Argument.IsNotNull(value, nameof(value));

      Dictionary<object, int> references = GetReferencesFromXmlContext(ctx);

      if (references.TryGetValue(value, out var id))
        element.SetAttributeValue(REFERENCE_ID_ATTRIBUTE, id);
      else
      {
        innerSerializer.Serialize(value, expectedType, element, ctx);
        id = references.Count > 0 ? references.Values.Max() + 1 : 1;
        references[value] = id;
        element.SetAttributeValue(OBJECT_ID_ATTRIBUTE, id);
      }
    }

    private Dictionary<object, int> GetReferencesFromXmlContext(IXmlContext ctx)
    {
      Dictionary<object, int> ret = ctx.GetOrSetCustomData<Dictionary<object, int>>(CONTEXT_REFERENCES_KEY, () => []);
      return ret;
    }
  }
}