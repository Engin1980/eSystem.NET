using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.EnumerableSerialization.Abstractions
{
  public abstract class EnumerableDeserializerBase : IElementDeserializer
  {
    public abstract bool AcceptsType(Type type);

    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
    {
      Type itemType = ExtractItemType(targetType);
      List<object?> items = DeserializeItems(element, itemType, ctx).ToList();
      object ret = CreateInstance(targetType, items);
      return ret;
    }

    protected abstract List<object?> DeserializeItems(XElement element, Type itemType, IXmlContext ctx);
    protected abstract object CreateInstance(Type enumerableType, List<object?> items);

    protected abstract Type ExtractItemType(Type targetType);
  }
}