using ESystem.Asserting;
using ESystem.Miscelaneous;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

public class PolymorphicNamePropertyAsElement : IPropertySerializer, IPropertyDeserializer
{
  public enum MissingDefinitionBehavior
  {
    UseDefault,
    ThrowException
  }
  private readonly BiDictionary<Type, string> typeToXmlNameMapping = new();
  private MissingDefinitionBehavior missingDefinitionBehavior = MissingDefinitionBehavior.UseDefault;

  public PolymorphicNamePropertyAsElement WithMissingDefinitionBehavior(MissingDefinitionBehavior behavior)
  {
    missingDefinitionBehavior = behavior;
    return this;
  }
  public PolymorphicNamePropertyAsElement With(Type type, string xmlName)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    EAssert.Argument.IsNonEmptyString(xmlName, nameof(xmlName));
    typeToXmlNameMapping.Set(type, xmlName);
    return this;
  }
  public PolymorphicNamePropertyAsElement With<T>(string xmlName)
  {
    EAssert.Argument.IsNonEmptyString(xmlName, nameof(xmlName));
    return With(typeof(T), xmlName);
  }

  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
  {
    string elementName = propertyValue == null
      ? propertyInfo.Name
      : typeToXmlNameMapping.TryGetValueOrDefault(propertyValue.GetType(), propertyInfo.Name);

    var ser = ctx.ElementSerializers.GetByType(propertyValue?.GetType() ?? propertyInfo.PropertyType);
    XElement propElement = new(elementName);
    ctx.SerializeToElement(propertyValue, propertyInfo.PropertyType, propElement, ser);
    element.Add(propElement);
  }

  public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
  {
    DeserializationResult ret;
    var potentialNames = typeToXmlNameMapping.Select(q => q.Value).ToList();
    Type targetType;
    XElement? propertyElement = null;
    foreach (var pontentialName in potentialNames)
    {
      propertyElement = element.Element(XName.Get(pontentialName));
      if (propertyElement != null)
        break;
    }

    if (propertyElement == null)
    {
      propertyElement = element.Element(XName.Get(propertyInfo.Name));
      targetType = propertyInfo.PropertyType;
    }
    else
      targetType = typeToXmlNameMapping.GetKey(propertyElement.Name.LocalName);

    if (propertyElement == null)
    {
      if (missingDefinitionBehavior == MissingDefinitionBehavior.ThrowException)
        throw new InvalidOperationException($"Cannot find element for property '{propertyInfo.Name}' in element '{element.Name}'.");
      ret = DeserializationResult.NoResult();
    }
    else
    {
      IElementDeserializer deserializer = ctx.ElementDeserializers.GetByType(targetType);
      object? tmp = deserializer.Deserialize(propertyElement, targetType, ctx); //TODO: ctx.DeserializeFromElement(element, instanceType, deserializer);
      ret = tmp == null ? DeserializationResult.Null() : DeserializationResult.ValueResult(tmp);
    }
    return ret;
  }
}




