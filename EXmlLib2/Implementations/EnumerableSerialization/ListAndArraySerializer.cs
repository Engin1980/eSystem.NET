using ESystem.Asserting;
using ESystem.Exceptions;
using ESystem.Miscelaneous;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.EnumerableSerialization.Abstractions;
using EXmlLib2.Implementations.EnumerableSerialization.Internal;
using EXmlLib2.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.EnumerableSerialization
{
  public class ListAndArraySerializer : EnumerableSerializerBase
  {
    public class DefaultOptions(ListAndArraySerializer parent)
    {
      public DefaultOptions WithItemElementName(string itemElementName)
      {
        EAssert.Argument.IsNonEmptyString(itemElementName, nameof(itemElementName));
        parent.defaultItemElementName = itemElementName;
        return this;
      }
      public DefaultOptions WithItemXmlRepresentation(XmlRepresentation xmlRepresentation)
      {
        parent.itemXmlRepresentation = xmlRepresentation;
        return this;
      }
    }

    public class TypeMappingOptions(ListAndArraySerializer parent)
    {
      public TypeMappingOptions With<T>(string xmlElementName) => this.With(typeof(T), xmlElementName);

      public TypeMappingOptions With(Type type, string xmlElementName)
      {
        EAssert.Argument.IsNotNull(type, nameof(type));
        EAssert.Argument.IsNonEmptyString(xmlElementName, nameof(xmlElementName));
        parent.typeToXmlNameMapping.Add(type, xmlElementName);
        return this;
      }
    }

    private Func<Type, bool> typeAccepter = q => EnumerableTypeUtils.IsListOrArray(q);
    private const string DEFAULT_ITEM_ELEMENT_NAME = "Item";
    private string defaultItemElementName = DEFAULT_ITEM_ELEMENT_NAME;
    private XmlRepresentation itemXmlRepresentation = XmlRepresentation.Element;
    private readonly BiDictionary<Type, string> typeToXmlNameMapping = [];
    
    public ListAndArraySerializer WithAcceptedType<T>()
    {
      EAssert.IsTrue(EnumerableTypeUtils.IsListOrArray(typeof(T)),
        $"Typ {typeof(T).FullName} is not Array or IEnumerable.");
      this.typeAccepter = q => q == typeof(T);
      return this;
    }

    public ListAndArraySerializer WithDefaultOptions(Action<DefaultOptions> opts)
    {
      EAssert.Argument.IsNotNull(opts, nameof(opts));
      opts(new DefaultOptions(this));
      return this;
    }

    public ListAndArraySerializer WithTypeMappingOptions(Action<TypeMappingOptions> opts)
    {
      EAssert.Argument.IsNotNull(opts, nameof(opts));
      opts(new TypeMappingOptions(this));
      return this;
    }

    public override bool AcceptsType(Type type) => this.typeAccepter(type);

    protected override IEnumerable<object> ExtractItems(object value)
    {
      IEnumerable<object> items;
      if (value.GetType().IsArray)
        items = GetItemsFromArray(value);
      else
        items = GetItemsFromList(value);
      return items;
    }

    private static List<object> GetItemsFromArray(object value)
    {
      List<object> ret = [];
      Array array = (Array)value;
      foreach (object item in array)
        ret.Add(item);
      return ret;
    }

    private static List<object> GetItemsFromList(object value)
    {
      List<object> ret = [];
      System.Collections.IEnumerable array = (System.Collections.IEnumerable)value;
      foreach (object item in array)
        ret.Add(item);
      return ret;
    }

    protected override Type ExtractItemType(Type iterableType)
    {
      EAssert.Argument.IsNotNull(iterableType, nameof(iterableType));
      Type ret;
      if (iterableType.IsArray)
        ret = iterableType.GetElementType()!;
      else
        ret = Internal.EnumerableTypeUtils.GetItemTypeForIEnumerable(iterableType);
      return ret;
    }

    protected override void SerializeItem(object? item, Type itemType, XElement element, IXmlContext ctx)
    {
      var customXmlNameUsed = false;
      string itemXmlName;
      if (item != null)
      {
        customXmlNameUsed = typeToXmlNameMapping.TryGetValue(item.GetType(), out itemXmlName);
        if (!customXmlNameUsed)
          itemXmlName = defaultItemElementName;
      }
      else
        itemXmlName = defaultItemElementName;

      XElement itemElement;
      switch (this.itemXmlRepresentation)
      {
        case XmlRepresentation.Element:
          IElementSerializer elementSerializer = customXmlNameUsed
            ? ctx.ElementSerializers.GetByType(item!.GetType())
            : ctx.ElementSerializers.GetByType(itemType);
          itemElement = new XElement(XName.Get(itemXmlName));
          ctx.SerializeToElement(item, itemType, itemElement, elementSerializer);
          break;
        case XmlRepresentation.Attribute:
          IAttributeSerializer attributeSerializer = customXmlNameUsed
            ? ctx.AttributeSerializers.GetByType(item!.GetType())
            : ctx.AttributeSerializers.GetByType(itemType);
          itemElement = new XElement(XName.Get(defaultItemElementName));
          XAttribute itemAttribute = ctx.SerializeToAttribute(itemXmlName, item, itemType, attributeSerializer);
          itemElement.Add(itemAttribute);
          break;
        default:
          throw new UnexpectedEnumValueException(this.itemXmlRepresentation);
      }
      element.Add(itemElement);
    }
  }
}
