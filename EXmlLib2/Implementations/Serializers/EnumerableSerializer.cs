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

namespace EXmlLib2.Implementations.Serializers
{
  public class EnumerableSerializer : IElementSerializer
  {
    internal readonly XmlIterableInfo xii;

    public EnumerableSerializer()
    {
      this.xii = new XmlIterableInfo();
    }

    public EnumerableSerializer(XmlIterableInfo xmlIterableInfo)
    {
      EAssert.Argument.IsNotNull(xmlIterableInfo, nameof(xmlIterableInfo));
      this.xii = xmlIterableInfo;
    }

    public bool AcceptsType(Type type)
    {
      if (type.IsArray)
        return true;
      if (type.IsGenericType && type.IsAssignableTo(typeof(System.Collections.IEnumerable)))
        return true;
      return false;
    }

    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      EAssert.Argument.IsNotNull(value, nameof(value));
      EAssert.Argument.IsTrue(
        value == null || AcceptsType(value.GetType()),
        nameof(value),
        $"This serializer {this.GetType().Name} does not accept provided value of type {value?.GetType()?.Name ?? "null"}.");

      Type itemType = ExtractItemType(value);
      IEnumerable<object> items = ExtractItems(value);
      SaveItemsToElement(items, itemType, element, ctx);
    }

    private void SaveItemsToElement(IEnumerable<object> items, Type itemType, XElement element, IXmlContext ctx)
    {
      foreach (object item in items)
      {
        XElement itemElement = SerializeItem(item, itemType, ctx);
        element.Add(itemElement);
      }
    }

    private XElement SerializeItem(object item, Type itemType, IXmlContext ctx)
    {
      IElementSerializer serializer = ctx.ElementSerializers.GetByType(item.GetType());
      XmlNameAndStoreFlag xnsf = GetItemElementName(item?.GetType(), itemType, ctx);
      XElement ret = new XElement(XName.Get(xnsf.XmlName));
      ctx.SerializeToElement(item, ret, serializer);
      if (xnsf.ForceStoreType)
        AddTypeInfoAttribute(ret, item!.GetType(), ctx);
      return ret;
    }

    private void AddTypeInfoAttribute(XElement element, Type type, IXmlContext ctx)
    {
      string typeName = type.FullName!;
      if (typeName.StartsWith("System.") == false)  // if not mscorlib type, assembly name is required
        typeName += ", " + type.Assembly.GetName().Name;
      element.SetAttributeValue(XName.Get(ctx.TypeNameAttribute), typeName);
    }

    private record XmlNameAndStoreFlag(string XmlName, bool ForceStoreType);
    private XmlNameAndStoreFlag GetItemElementName(Type? itemTypeOrNull, Type expectedType, IXmlContext ctx)
    {
      string? xmlName;
      bool storeType;
      if (itemTypeOrNull == null || itemTypeOrNull.GetType() == expectedType)
        xmlName = this.xii.XmlItemName.Get(null);
      else
        xmlName = this.xii.XmlItemName.Get(itemTypeOrNull.GetType());
      if (xmlName == null)
      {
        storeType = itemTypeOrNull != null && itemTypeOrNull != expectedType;
        xmlName = ctx.DefaultItemXmlName;
      }
      else
        storeType = false;

      EAssert.IsNotNull(xmlName);
      return new XmlNameAndStoreFlag(xmlName, storeType);
    }

    private Type ExtractItemType(object? value)
    {
      EAssert.Argument.IsNotNull(value, nameof(value));
      Type ret;
      if (value.GetType().IsArray)
        ret = value.GetType().GetElementType()!;
      else
        ret = GetItemTypeForIEnumerable(value.GetType());
      return ret;
    }

    private static Type GetItemTypeForIEnumerable(Type type)
    {
      Type ret;
      if (type.IsGenericType && type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
      {
        Type ienumerableInterface = type.GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        ret = ienumerableInterface.GetGenericArguments()[0];
      }
      else
        ret = typeof(object);
      return ret;
    }

    private IEnumerable<object> ExtractItems(object value)
    {
      IEnumerable<object> items;
      if (value.GetType().IsArray)
        items = GetItemsFromArray(value);
      else
        items = GetItemsFromList(value);
      return items;
    }

    private IEnumerable<object> GetItemsFromArray(object value)
    {
      List<object> ret = new();
      Array array = (Array)value;
      foreach (object item in array)
      {
        ret.Add(item);
      }
      return ret;
    }

    private IEnumerable<object> GetItemsFromList(object value)
    {
      List<object> ret = new();
      System.Collections.IEnumerable array = (System.Collections.IEnumerable)value;
      foreach (object item in array)
      {
        ret.Add(item);
      }
      return ret;
    }
  }
}
