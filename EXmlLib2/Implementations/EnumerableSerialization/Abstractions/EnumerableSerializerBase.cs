using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.EnumerableSerialization.Abstractions
{
  public abstract class EnumerableSerializerBase : IElementSerializer
  {
    public abstract bool AcceptsType(Type type);

    protected abstract Type ExtractItemType(Type iterableType);
    protected abstract IEnumerable<object> ExtractItems(object value);
    protected abstract void SerializeItem(object item, Type itemType, XElement element, IXmlContext ctx);
    public void Serialize(object? value, Type expectedType, XElement element, IXmlContext ctx)
    {
      //TODO tady je to divne, tkery typ se ma brat, ten z value nebo ten expected?
      EAssert.Argument.IsNotNull(value, nameof(value));
      Type itemType = ExtractItemType(expectedType);
      IEnumerable<object> items = ExtractItems(value);
      SerializeItems(items, itemType, element, ctx);
    }

    public virtual void SerializeItems(IEnumerable<object> items, Type itemType, XElement element, IXmlContext ctx)
    {
      foreach (object item in items)
        SerializeItem(item, itemType, element, ctx);
    }
  }
}
