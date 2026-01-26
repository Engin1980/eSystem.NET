using ESystem.Asserting;
using ESystem.Exceptions;
using ESystem.Miscelaneous;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.EnumerableSerialization.Internal;
using EXmlLib2.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.EnumerableSerialization
{
  public class ListAndArrayDeserializer : EnumerableDeserializer
  {
    private Func<Type, bool> typeAccepter = q => EnumerableTypeUtils.IsListOrArray(q);

    public enum UnknownElementHandling
    {
      Skip,
      UseDefaultItemType,
      ThrowException
    }

    public class DefaultOptions(ListAndArrayDeserializer parent)
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
      public DefaultOptions WithUnknownElementHandling(UnknownElementHandling handling)
      {
        parent.unknownElementHandling = handling;
        return this;
      }
    }

    public class TypeMappingOptions(ListAndArrayDeserializer parent)
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

    private const string DEFAULT_ITEM_ELEMENT_NAME = "Item";
    private string defaultItemElementName = DEFAULT_ITEM_ELEMENT_NAME;
    private XmlRepresentation itemXmlRepresentation = XmlRepresentation.Element;
    private readonly BiDictionary<Type, string> typeToXmlNameMapping = new();
    private UnknownElementHandling unknownElementHandling = UnknownElementHandling.Skip;

    public ListAndArrayDeserializer WithAcceptedType<T>()
    {
      this.typeAccepter = q => EnumerableTypeUtils.IsListOrArray(typeof(T)) && q == typeof(T);
      return this;
    }

    public ListAndArrayDeserializer WithDefaultOptions(Action<DefaultOptions> opts)
    {
      EAssert.Argument.IsNotNull(opts, nameof(opts));
      opts(new DefaultOptions(this));
      return this;
    }

    public ListAndArrayDeserializer WithTypeMappingOptions(Action<TypeMappingOptions> opts)
    {
      EAssert.Argument.IsNotNull(opts, nameof(opts));
      opts(new TypeMappingOptions(this));
      return this;
    }

    public override bool AcceptsType(Type type) => typeAccepter(type);

    protected override object CreateInstance(Type enumerableType, List<object?> items)
    {
      if (enumerableType.IsArray)
      {
        Type elementType = enumerableType.GetElementType() ?? throw new UnexpectedNullException();
        Array array = Array.CreateInstance(elementType, items.Count);

        for (int i = 0; i < items.Count; i++)
        {
          array.SetValue(items[i], i);
        }
        return array;
      }

      if (enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(List<>))
      {
        // Vytvoříme instanci List<T>
        // Použijeme rozhraní IList, které není generické a umožní nám volat metodu .Add()
        IList list = (IList)(Activator.CreateInstance(enumerableType) ?? throw new UnexpectedNullException());

        foreach (var item in items)
        {
          list.Add(item);
        }
        return list;
      }

      throw new NotSupportedException($"Typ {enumerableType.FullName} není podporován. Očekává se pole nebo List<T>.");
    }

    protected override List<object?> DeserializeItems(XElement element, Type itemType, IXmlContext ctx)
    {
      var childElements = element.Elements();
      List<object?> ret = [];
      foreach (var childElement in childElements)
      {
        Type? childType;
        object? item;
        switch (this.itemXmlRepresentation)
        {
          case XmlRepresentation.Element:
            childType = TryEstimateItemTypeByElementName(childElement, itemType);
            if (childType == null)
            {
              switch (this.unknownElementHandling)
              {
                case UnknownElementHandling.Skip:
                  continue;
                case UnknownElementHandling.UseDefaultItemType:
                  childType = itemType;
                  break;
                case UnknownElementHandling.ThrowException:
                  throw new InvalidOperationException($"Cannot determine item type for element '{childElement.Name.LocalName}'");
              }
            }
            EAssert.IsNotNull(childType);
            IElementDeserializer elementDeserializer = ctx.ElementDeserializers.GetByType(childType);
            item = elementDeserializer.Deserialize(childElement, childType, ctx);
            break;
          case XmlRepresentation.Attribute:
            if (childElement.Name.LocalName != this.defaultItemElementName)
              if (this.unknownElementHandling == UnknownElementHandling.Skip)
                continue;
              else
                throw new InvalidOperationException($"Unexpected element name '{childElement.Name.LocalName}' when expecting attribute representation of items.");

            string childAttributeName;
            (childType, childAttributeName) = TryEstimateItemTypeByAttributeName(childElement, itemType);
            if (childType == null)
              if (this.unknownElementHandling == UnknownElementHandling.Skip)
                continue;
              else
                throw new InvalidOperationException($"Cannot determine item type for element '{childElement.Name.LocalName}'");
            IAttributeDeserializer attributeDeserializer = ctx.AttributeDeserializers.GetByType(childType);
            XAttribute? attr = childElement.Attribute(XName.Get(this.typeToXmlNameMapping[childType]));
            if (attr == null)
              if (this.unknownElementHandling == UnknownElementHandling.Skip)
                continue;
              else
                throw new InvalidOperationException($"Cannot determine item type from attribute(s) of element '{childElement.Name.LocalName}'");
            item = attributeDeserializer.Deserialize(attr.Value, childType, ctx);
            break;
          default:
            throw new ESystem.Exceptions.UnexpectedEnumValueException(this.itemXmlRepresentation);
        }
        ret.Add(item);
      }
      return ret;
    }

    private Type? TryEstimateItemTypeByElementName(XElement childElement, Type itemType)
    {
      Type? ret = typeToXmlNameMapping.TryGetKeyOrDefault(childElement.Name.LocalName);
      if (ret == null && childElement.Name.LocalName == this.defaultItemElementName)
        ret = itemType;
      return ret;
    }

    private (Type?, string) TryEstimateItemTypeByAttributeName(XElement childElement, Type itemType)
    {
      Type? ret = null;
      string childAttributeName = string.Empty;

      var attributes = childElement.Attributes();
      foreach (var attribute in attributes)
      {
        Type? tmp = typeToXmlNameMapping.TryGetKeyOrDefault(attribute.Name.LocalName);
        if (tmp == null && childElement.Name.LocalName == this.defaultItemElementName)
          tmp = itemType;
        if (tmp != null)
        {
          childAttributeName = attribute.Name.LocalName;
          ret = tmp;
          break;
        }
      }

      return (ret, childAttributeName);
    }

    protected override Type ExtractItemType(Type targetType)
    {
      EAssert.Argument.IsNotNull(targetType, nameof(targetType));
      Type ret;
      if (targetType.IsArray)
        ret = targetType.GetElementType()!;
      else
        ret = Internal.EnumerableTypeUtils.GetItemTypeForIEnumerable(targetType);
      return ret;
    }
  }
}
