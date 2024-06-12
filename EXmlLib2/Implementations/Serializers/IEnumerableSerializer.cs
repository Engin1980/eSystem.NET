using ESystem.Asserting;
using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class IEnumerableSerializer : IElementSerializer
  {
    public bool AcceptsValue(object? value)
    {
      if (value == null) return false;
      Type type = value.GetType();
      if (type.IsArray)
        return true;
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        return true;
      return false;
    }

    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      EAssert.Argument.IsTrue(AcceptsValue(value));
      Type itemType = ExtractItemType(value);
      IEnumerable<object> items = ExtractItems(value);
      SaveItemsToElement(items, itemType, element, ctx);
    }

    private Type ExtractItemType(object? value)
    {
      EAssert.Argument.IsNotNull(value);
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

    private IEnumerable<object> ExtractItems(object? value)
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
