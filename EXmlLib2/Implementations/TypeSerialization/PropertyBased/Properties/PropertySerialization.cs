using ESystem.Asserting;
using ESystem.Miscelaneous;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties
{
  public class PropertySerialization : IPropertyDeserializer, IPropertySerializer
  {
    private readonly BiDictionary<Type, string> typeToXmlNameMapping = [];
    private readonly Dictionary<Type, Func<object?>> defaultValueFactories = [];
    private MissingPropertyXmlSourceBehavior missingXmlSourceBehavior = MissingPropertyXmlSourceBehavior.Ignore;
    private XmlSourceOrder xmlSourceOrder = XmlSourceOrder.AttributeFirst;
    private NameCaseMatching nameCaseMatching = NameCaseMatching.Exact;

    public PropertySerialization With(Type type, string xmlName)
    {
      EAssert.Argument.IsNotNull(type, nameof(type));
      EAssert.Argument.IsNonEmptyString(xmlName, nameof(xmlName));
      typeToXmlNameMapping.Set(type, xmlName);
      return this;
    }
    public PropertySerialization With<T>(string xmlName)
    {
      EAssert.Argument.IsNonEmptyString(xmlName, nameof(xmlName));
      return With(typeof(T), xmlName);
    }
    public PropertySerialization WithXmlSourceOrder(XmlSourceOrder order)
    {
      xmlSourceOrder = order;
      return this;
    }
    public PropertySerialization WithNameCaseMatching(NameCaseMatching matching)
    {
      nameCaseMatching = matching;
      return this;
    }
    public PropertySerialization WithMissingXmlSourceBehavior(MissingPropertyXmlSourceBehavior behavior)
    {
      missingXmlSourceBehavior = behavior;
      return this;
    }
    public PropertySerialization WithDefaultValue(Type type, Func<object?> defaultValueFactory)
    {
      EAssert.Argument.IsNotNull(type, nameof(type));
      EAssert.Argument.IsNotNull(defaultValueFactory, nameof(defaultValueFactory));
      defaultValueFactories[type] = defaultValueFactory;
      return this;
    }
    public PropertySerialization WithDefaultValue<T>(Func<T?> defaultValueFactory)
    {
      EAssert.Argument.IsNotNull(defaultValueFactory, nameof(defaultValueFactory));
      return WithDefaultValue(typeof(T), () => defaultValueFactory());
    }

    public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
    {
      DeserializationResult ret;
      object? propertySource = TryGetPropertySource(element, propertyInfo.Name);

      if (propertySource == null)
      {
        ret = missingXmlSourceBehavior switch
        {
          MissingPropertyXmlSourceBehavior.ThrowException => throw new InvalidOperationException($"Cannot find xml source for property '{propertyInfo.Name}' in element '{element.Name}' (mode {this.xmlSourceOrder}, casing: {this.nameCaseMatching})."),
          MissingPropertyXmlSourceBehavior.Ignore => DeserializationResult.FromNoResult(),
          MissingPropertyXmlSourceBehavior.ReturnDefault => DeserializationResult.From(CreateDefaultValueOfType(propertyInfo.PropertyType)),
          _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(missingXmlSourceBehavior),
        };
      }
      else
      {
        if (propertySource is XAttribute propertyAttribute)
        {
          Type targetType = typeToXmlNameMapping.TryGetKeyOrDefault(propertyAttribute.Name.LocalName, propertyInfo.PropertyType);
          IAttributeDeserializer deserializer = ctx.AttributeDeserializers.GetByType(targetType);
          object? tmp = deserializer.Deserialize(propertyAttribute.Value, targetType, ctx);
          ret = DeserializationResult.From(tmp);
          return ret;
        }
        else if (propertySource is XElement propertyElement)
        {
          Type targetType = typeToXmlNameMapping.TryGetKeyOrDefault(propertyElement.Name.LocalName, propertyInfo.PropertyType);
          IElementDeserializer deserializer = ctx.ElementDeserializers.GetByType(targetType);
          object? tmp = ctx.DeserializeFromElement(propertyElement, targetType, deserializer);
          ret = DeserializationResult.From(tmp);
        }
        else
          throw new ApplicationException("Unreachable code reached. Source must be XElement or XAttribute.");
      }
      return ret;
    }

    private object? CreateDefaultValueOfType(Type propertyType)
    {
      if (defaultValueFactories.TryGetValue(propertyType, out var factory))
        return factory();
      else if (propertyType.IsValueType)
        return Activator.CreateInstance(propertyType);
      else
        return null;
    }

    private object? TryGetPropertySource(XElement element, string name)
    {
      XElement? elm = TryGetPropertyElement(element, name);
      XAttribute? att = TryGetPropertyAttribute(element, name);
      object? ret = this.xmlSourceOrder switch
      {
        XmlSourceOrder.ElementFirst => elm != null ? (object?)elm : att,
        XmlSourceOrder.AttributeFirst => att != null ? (object?)att : elm,
        XmlSourceOrder.ElementOnly => elm,
        XmlSourceOrder.AttributeOnly => att,
        _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(this.xmlSourceOrder),
      };
      return ret;
    }

    private XAttribute? TryGetPropertyAttribute(XElement element, string name)
    {
      XAttribute? ret = null;
      var potentialNames = typeToXmlNameMapping.Select(q => q.Value).Append(name).ToList();
      ret = this.nameCaseMatching switch
      {
        NameCaseMatching.Exact => element.Attributes().FirstOrDefault(q => potentialNames.Contains(q.Name.LocalName)),
        NameCaseMatching.IgnoreCase => element.Attributes().FirstOrDefault(q => potentialNames.Any(pn => string.Equals(pn, q.Name.LocalName, StringComparison.OrdinalIgnoreCase))),
        _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(this.nameCaseMatching),
      };
      return ret;
    }

    private XElement? TryGetPropertyElement(XElement element, string name)
    {
      XElement? ret = null;
      var potentialNames = typeToXmlNameMapping.Select(q => q.Value).Append(name).ToList();
      ret = this.nameCaseMatching switch
      {
        NameCaseMatching.Exact => element.Descendants().FirstOrDefault(q => potentialNames.Contains(q.Name.LocalName)),
        NameCaseMatching.IgnoreCase => element.Descendants().FirstOrDefault(q => potentialNames.Any(pn => string.Equals(pn, q.Name.LocalName, StringComparison.OrdinalIgnoreCase))),
        _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(this.nameCaseMatching),
      };
      return ret;
    }

    public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
    {
      string elementName = propertyValue == null
        ? propertyInfo.Name
        : typeToXmlNameMapping.TryGetValueOrDefault(propertyValue.GetType(), propertyInfo.Name);


      var xso = this.xmlSourceOrder;

      if (xso == XmlSourceOrder.ElementOnly)
        SerializePropertyToElement(propertyInfo, propertyValue, element, ctx, elementName);
      else if (xso == XmlSourceOrder.AttributeOnly)
        SerializePropertyToAttribute(propertyInfo, propertyValue, element, ctx, elementName);
      else
      {
        IElementSerializer? elementSerializer = ctx.ElementSerializers.TryGetByType(propertyValue?.GetType() ?? propertyInfo.PropertyType);
        IAttributeSerializer? attributeSerializer = ctx.AttributeSerializers.TryGetByType(propertyValue?.GetType() ?? propertyInfo.PropertyType);

        if (xso == XmlSourceOrder.ElementFirst && elementSerializer != null)
          SerializePropertyToElement(propertyInfo, propertyValue, element, ctx, elementName);
        else if (xso == XmlSourceOrder.ElementFirst && attributeSerializer != null)
          SerializePropertyToAttribute(propertyInfo, propertyValue, element, ctx, elementName);
        else if (xso == XmlSourceOrder.AttributeFirst && attributeSerializer != null)
          SerializePropertyToAttribute(propertyInfo, propertyValue, element, ctx, elementName);
        else if (xso == XmlSourceOrder.AttributeFirst && elementSerializer != null)
          SerializePropertyToElement(propertyInfo, propertyValue, element, ctx, elementName);
        else
          throw new EXmlException($"Unable to find any serializer for " +
            $"type '{propertyValue?.GetType().FullName ?? "null"}' " +
            $"of property '{propertyInfo.Name}'.");
      }
    }

    private static void SerializePropertyToAttribute(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx, string elementName)
    {
      //if (propertyValue != null && propertyValue.GetType() != propertyInfo.PropertyType)
      //  throw new InvalidOperationException($"Cannot serialize property '{propertyInfo.Name}' as attribute " +
      //    $"because its runtime value type '{propertyValue.GetType().FullName}' differs from declared property " +
      //    $"type '{propertyInfo.PropertyType.FullName}' (type ${propertyInfo.DeclaringType}).");

      var attrSer = ctx.AttributeSerializers.GetByType(propertyValue?.GetType() ?? propertyInfo.PropertyType);
      XAttribute propAttr = ctx.SerializeToAttribute(elementName, propertyValue, propertyInfo.PropertyType, attrSer);
      element.Add(propAttr);
    }

    private static void SerializePropertyToElement(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx, string elementName)
    {
      var ser = ctx.ElementSerializers.GetByType(propertyValue?.GetType() ?? propertyInfo.PropertyType);
      XElement propElement = new(elementName);
      ctx.SerializeToElement(propertyValue, propertyInfo.PropertyType, propElement, ser);
      element.Add(propElement);
    }
  }
}
