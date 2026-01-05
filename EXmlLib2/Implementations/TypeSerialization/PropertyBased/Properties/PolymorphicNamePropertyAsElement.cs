using ESystem.Asserting;
using ESystem.Miscelaneous;
using EXmlLib2.Abstractions;
using System.Reflection;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

public class PolymorphicNamePropertyAsElement : IPropertySerializer
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
    ctx.SerializeToElement(propertyValue, propElement, ser);
    element.Add(propElement);
  }
}




