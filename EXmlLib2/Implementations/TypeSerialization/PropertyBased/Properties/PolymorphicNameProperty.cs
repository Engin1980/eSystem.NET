//using ESystem.Asserting;
//using ESystem.Miscelaneous;
//using EXmlLib2.Abstractions;
//using EXmlLib2.Abstractions.Interfaces;
//using EXmlLib2.Implementations.TypeSerialization.Helpers;
//using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties.Abstractions;
//using System.Reflection;
//using System.Xml.Linq;

//namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

//public class PolymorphicNameProperty : IPropertySerializer, IPropertyDeserializer
//{
//  private readonly BiDictionary<Type, string> typeToXmlNameMapping = [];
//  private NameCaseMatching nameCaseMatching = NameCaseMatching.Exact;
//  private MissingPropertyXmlSourceBehavior missingDefinitionBehavior = MissingPropertyXmlSourceBehavior.Ignore;
//  private XmlSourceOrder xmlSourceOrder = XmlSourceOrder.ElementFirst;

//  public PolymorphicNameProperty WithNameCaseMatching(NameCaseMatching caseMatching)
//  {
//    nameCaseMatching = caseMatching;
//    return this;
//  }
//  public PolymorphicNameProperty WithMissingDefinitionBehavior(MissingPropertyXmlSourceBehavior behavior)
//  {
//    missingDefinitionBehavior = behavior;
//    return this;
//  }
//  public PolymorphicNameProperty WithXmlSourceOrder(XmlSourceOrder order)
//  {
//    xmlSourceOrder = order;
//    return this;
//  }


//  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
//  {
//    string elementName = propertyValue == null
//      ? propertyInfo.Name
//      : typeToXmlNameMapping.TryGetValueOrDefault(propertyValue.GetType(), propertyInfo.Name);

//    switch (this.xmlSourceOrder)
//    {
//      case XmlSourceOrder.ElementFirst:
//      case XmlSourceOrder.ElementOnly:
//        var ser = ctx.ElementSerializers.GetByType(propertyValue?.GetType() ?? propertyInfo.PropertyType);
//        XElement propElement = new(elementName);
//        ctx.SerializeToElement(propertyValue, propertyInfo.PropertyType, propElement, ser);
//        element.Add(propElement);
//        break;
//      case XmlSourceOrder.AttributeFirst:
//      case XmlSourceOrder.AttributeOnly:
//        var attrSer = ctx.AttributeSerializers.GetByType(propertyValue?.GetType() ?? propertyInfo.PropertyType);
//        XAttribute propAttr = ctx.SerializeToAttribute(elementName, propertyValue, propertyInfo.PropertyType, attrSer);
//        element.Add(propAttr);
//        break;
//      default:
//        throw new ESystem.Exceptions.UnexpectedEnumValueException(this.xmlSourceOrder);
//    }
//  }

//  public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
//  {
//    DeserializationResult ret;
//    object? propertySource = TryGetPropertySource(element, propertyInfo.Name);

//    if (propertySource == null)
//    {
//      ret = missingDefinitionBehavior switch
//      {
//        MissingPropertyXmlSourceBehavior.ThrowException => throw new InvalidOperationException($"Cannot find xml source for property '{propertyInfo.Name}' in element '{element.Name}' (mode {this.xmlSourceOrder}, casing: {this.nameCaseMatching})."),
//        MissingPropertyXmlSourceBehavior.Ignore => DeserializationResult.FromNoResult(),
//        MissingPropertyXmlSourceBehavior.ReturnNull => DeserializationResult.FromNull(),
//        _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(missingDefinitionBehavior),
//      };
//    }
//    else
//    {
//      if (propertySource is XAttribute propertyAttribute)
//      {
//        Type targetType = typeToXmlNameMapping.TryGetKeyOrDefault(propertyAttribute.Name.LocalName, propertyInfo.PropertyType);
//        IAttributeDeserializer deserializer = ctx.AttributeDeserializers.GetByType(targetType);
//        object? tmp = deserializer.Deserialize(propertyAttribute.Value, targetType, ctx);
//        ret = DeserializationResult.From(tmp);
//        return ret;
//      }
//      else if (propertySource is XElement propertyElement)
//      {
//        Type targetType = typeToXmlNameMapping.TryGetKeyOrDefault(propertyElement.Name.LocalName, propertyInfo.PropertyType);
//        IElementDeserializer deserializer = ctx.ElementDeserializers.GetByType(targetType);
//        object? tmp = ctx.DeserializeFromElement(propertyElement, targetType, deserializer);
//        ret = DeserializationResult.From(tmp);
//      }
//      else
//        throw new ApplicationException("Unreachable code reached. Source must be XElement or XAttribute.");
//    }
//    return ret;
//  }

//  private object? TryGetPropertySource(XElement element, string name)
//  {
//    XElement? elm = TryGetPropertyElement(element, name);
//    XAttribute? att = TryGetPropertyAttribute(element, name);
//    object? ret = this.xmlSourceOrder switch
//    {
//      XmlSourceOrder.ElementFirst => elm != null ? (object?)elm : att,
//      XmlSourceOrder.AttributeFirst => att != null ? (object?)att : elm,
//      XmlSourceOrder.ElementOnly => elm,
//      XmlSourceOrder.AttributeOnly => att,
//      _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(this.xmlSourceOrder),
//    };
//    return ret;
//  }

//  private XAttribute? TryGetPropertyAttribute(XElement element, string name)
//  {
//    XAttribute? ret = null;
//    var potentialNames = typeToXmlNameMapping.Select(q => q.Value).Append(name).ToList();
//    ret = this.nameCaseMatching switch
//    {
//      NameCaseMatching.Exact => element.Attributes().FirstOrDefault(q => potentialNames.Contains(q.Name.LocalName)),
//      NameCaseMatching.IgnoreCase => element.Attributes().FirstOrDefault(q => potentialNames.Any(pn => string.Equals(pn, q.Name.LocalName, StringComparison.OrdinalIgnoreCase))),
//      _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(this.nameCaseMatching),
//    };
//    return ret;
//  }

//  private XElement? TryGetPropertyElement(XElement element, string name)
//  {
//    XElement? ret = null;
//    var potentialNames = typeToXmlNameMapping.Select(q => q.Value).Append(name).ToList();
//    ret = this.nameCaseMatching switch
//    {
//      NameCaseMatching.Exact => element.Descendants().FirstOrDefault(q => potentialNames.Contains(q.Name.LocalName)),
//      NameCaseMatching.IgnoreCase => element.Descendants().FirstOrDefault(q => potentialNames.Any(pn => string.Equals(pn, q.Name.LocalName, StringComparison.OrdinalIgnoreCase))),
//      _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(this.nameCaseMatching),
//    };
//    return ret;
//  }
//}




