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

namespace EXmlLib2.Implementations.EnumerableSerialization
{
  public abstract class EnumerableDeserializer : IElementDeserializer
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
//    internal readonly XmlIterableInfo xii;

//    public EnumerableDeserializer()
//    {
//      this.xii = new XmlIterableInfo();
//    }

//    public EnumerableDeserializer(XmlIterableInfo xmlIterableInfo)
//    {
//      EAssert.Argument.IsNotNull(xmlIterableInfo, nameof(xmlIterableInfo));
//      this.xii = xmlIterableInfo;
//    }

//    public bool AcceptsType(Type type)
//    {
//      if (type.IsArray)
//        return true;
//      if (type.IsGenericType && type.IsAssignableTo(typeof(System.Collections.IEnumerable)))
//        return true;
//      return false;
//    }

//    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
//    {
//      Type itemType = ExtractItemType(targetType);
//      List<object> items = DeserializeItems(element, ctx);
//      object? ret = CreateEnumerableOfType(items, itemType, targetType);
//      return ret;
//    }

//    private List<object> DeserializeItems(XElement element, IXmlContext ctx)
//    {
//      List<object> ret = [];
//      var children = element.Elements();
//      for (XElement child in children)
//      {

//        object? item = ctx.DeserializeFromElement(child);
//        if (item != null)
//          ret.Add(item);
//      }
//      return ret;
//    }

//    private Type ExtractItemType(object? value)
//    {
//      EAssert.Argument.IsNotNull(value, nameof(value));
//      Type ret;
//      if (value.GetType().IsArray)
//        ret = value.GetType().GetElementType()!;
//      else
//        ret = GetItemTypeForIEnumerable(value.GetType());
//      return ret;
//    }

//    private static Type GetItemTypeForIEnumerable(Type type)
//    {
//      Type ret;
//      if (type.IsGenericType && type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
//      {
//        Type ienumerableInterface = type.GetInterfaces()
//            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
//        ret = ienumerableInterface.GetGenericArguments()[0];
//      }
//      else
//        ret = typeof(object);
//      return ret;
//    }
//  }
//}
