using ESystem;
using ESystem.Asserting;
using ESystem.Logging;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Abstracts;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.BasicSerialization.Serializers;

public class SpecificTypeElementSerializer<T> : TypedElementSerializer<T>
{
  public const string TYPE_NAME_ATTRIBUTE = "__instanceType";
  private readonly Logger logger = Logger.Create(typeof(T));
  public XmlTypeInfo<T> XmlTypeInfo { get; private set; }

  public SpecificTypeElementSerializer() : this(new XmlTypeInfo<T>(), DerivedTypesBehavior.ExactTypeOnly) { }

  public SpecificTypeElementSerializer(XmlTypeInfo<T> xmlTypeInfo) : this(xmlTypeInfo, DerivedTypesBehavior.ExactTypeOnly) { }

  public SpecificTypeElementSerializer(DerivedTypesBehavior derivedTypesBehavior) : this(new XmlTypeInfo<T>(), derivedTypesBehavior) { }

  public SpecificTypeElementSerializer(XmlTypeInfo<T> xmlTypeInfo, DerivedTypesBehavior derivedTypesBehavior) : base(derivedTypesBehavior)
  {
    EAssert.Argument.IsNotNull(xmlTypeInfo, nameof(xmlTypeInfo));
    XmlTypeInfo = xmlTypeInfo;
  }

  public override void Serialize(T value, XElement element, IXmlContext ctx)
  {
    EAssert.Argument.IsNotNull(value, nameof(value));
    try
    {
      SerializeProperties(value, element, ctx);
      StoreValueTypeIfRequired(value, element);
    }
    catch (Exception ex)
    {
      var eex = new EXmlException($"Failed to serialize {value} to element {element}.", ex);
      logger.LogException(eex);
      throw eex;
    }
  }

  private void StoreValueTypeIfRequired(T value, XElement element)
  {
    if (value!.GetType() != typeof(T))
    {
      string typeName = value.GetType().FullName!;
      if (typeName.StartsWith("System.") == false)  // if not mscorlib type, assembly name is required
        typeName += ", " + value.GetType().Assembly.GetName().Name;
      element.SetAttributeValue(XName.Get(TYPE_NAME_ATTRIBUTE), typeName);
    }
  }

  protected virtual PropertyInfo[] GetProperties(Type type)
  {
    PropertyInfo[] ret = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    return ret;
  }

  protected virtual void SerializeProperties(T value, XElement element, IXmlContext ctx)
  {
    EAssert.IsNotNull(value);
    var properties = GetProperties(value.GetType());
    foreach (var property in properties)
    {
      SerializeProperty(property, value, element, ctx);
    }
  }

  protected virtual void SerializeProperty(PropertyInfo property, T value, XElement element, IXmlContext ctx)
  {
    XmlPropertyInfo xpi = XmlTypeInfo.PropertyInfos.TryGet(property) ?? XmlTypeInfo.DefaultXmlPropertyInfo;
    if (xpi.Obligation == XmlObligation.Ignored)
      return;
    object? propertyValue = GetPropertyValue(property, value);
    string xmlName = xpi.XmlName ?? property.Name;
    if (xpi.Representation == XmlRepresentation.Element)
      SerializePropertyAsElement(property, xpi, propertyValue, element, ctx);
    else if (xpi.Representation == XmlRepresentation.Attribute)
      SerializePropertyAsAttribute(xmlName, propertyValue, property.PropertyType, element, ctx);
    else
      SerializePropertyAsElement(property, xpi, propertyValue, element, ctx);
  }

  private void SerializePropertyAsAttribute(string xmlName, object? value, Type propertyExpectedType, XElement element, IXmlContext ctx)
  {
    IAttributeSerializer serializer = ctx.AttributeSerializers.GetByType(propertyExpectedType);
    string s = serializer.Serialize(value, ctx);
    element.SetAttributeValue(XName.Get(xmlName), s);

    //TODo resolve, if this must be here, or should be omitted
    if (value != null && value.GetType() != propertyExpectedType)
      throw new EXmlException(
        $"Unable to serialize inherited type into xml attribute. " +
        $"expected type {propertyExpectedType}, actual type {value.GetType()}.");
  }

  private void SerializePropertyAsElement(PropertyInfo property, XmlPropertyInfo xpi, object? value, XElement element, IXmlContext ctx)
  {
    IElementSerializer serializer = ctx.ElementSerializers.GetByType(property.PropertyType);
    XElement propertyElement;
    string xmlName;

    if (value != null && value.GetType() != property.PropertyType && value.GetType() != Nullable.GetUnderlyingType(property.PropertyType))
    {
      if (xpi.XmlNameByType.TryGetValue(value.GetType(), out xmlName!))
        propertyElement = new XElement(XName.Get(xmlName));
      else
      {
        propertyElement = new XElement(XName.Get(xpi.XmlName ?? property.Name));
        string typeName = value.GetType().FullName!;
        if (typeName.StartsWith("System.") == false)  // if not mscorlib type, assembly name is required
          typeName += ", " + value.GetType().Assembly.GetName().Name;
        propertyElement.SetAttributeValue(XName.Get(ctx.TypeNameAttribute), typeName);
      }
    }
    else
      propertyElement = new XElement(XName.Get(xpi.XmlName ?? property.Name));

    serializer.Serialize(value, propertyElement, ctx);

    element.Add(propertyElement);
  }

  private object? GetPropertyValue(PropertyInfo property, T? value)
  {
    try
    {
      object? ret = property.GetValue(value, null);
      return ret;
    }
    catch (Exception ex)
    {
      var eex = new EXmlException($"Failed to get value of property {property.Name} over object {value}.", ex);
      logger.LogException(eex);
      throw eex;
    }
  }
}
