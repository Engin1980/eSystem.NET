using ESystem.Logging;
using ESystem;
using ESystem.Asserting;
using EXmlLib2.Interfaces;
using EXmlLib2.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class TypeElementSerializer<T> : IElementSerializer<T> where T : notnull
  {
    private readonly Logger logger = Logger.Create(typeof(T));
    public XmlTypeInfo<T> Type { get; private set; }

    public TypeElementSerializer()
    {
      this.Type = new();
    }

    public TypeElementSerializer(XmlTypeInfo<T> xmlTypeInfo)
    {
      EAssert.Argument.IsNotNull(xmlTypeInfo, nameof(xmlTypeInfo));
      this.Type = xmlTypeInfo;
    }


    public void Serialize(T value, XElement element, IXmlContext ctx)
    {
      EAssert.Argument.IsNotNull(value, nameof(value));
      try
      {
        SerializeProperties(value, element, ctx);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Failed to serialize {value} to element {element}.", ex);
        this.logger.LogException(eex);
        throw eex;
      }
    }

    private void SerializeProperties(T value, XElement element, IXmlContext ctx)
    {
      var properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (var property in properties)
      {
        SerializeProperty(property, value, element, ctx);
      }
    }

    private void SerializeProperty(PropertyInfo property, T value, XElement element, IXmlContext ctx)
    {
      XmlPropertyInfo xpi = Type.PropertyInfos.TryGet(property) ?? XmlTypeInfo<T>.DefaultXmlPropertyInfo;
      if (xpi.Obligation == XmlObligation.Ignored)
        return;
      object? propertyValue = GetPropertyValue(property, value);
      string xmlName = xpi.XmlName ?? property.Name;
      if (xpi.Representation == null || xpi.Representation == XmlRepresentation.Element)
        SerializePropertyAsElement(property, xpi, propertyValue, element, ctx);
      else
        SerializePropertyAsAttribute(xmlName, propertyValue, property.PropertyType, element, ctx);
    }

    private void SerializePropertyAsAttribute(string xmlName, object? value, Type propertyExpectedType, XElement element, IXmlContext ctx)
    {
      IAttributeSerializer serializer = ctx.GetAttributeSerializer(value);
      string s = serializer.Serialize(value, ctx);
      element.SetAttributeValue(XName.Get(xmlName), s);

      if (value != null && value.GetType() != propertyExpectedType)
        throw new EXmlException(
          $"Unable to serialize inherited type into xml attribute. " +
          $"expected type {propertyExpectedType}, actual type {value.GetType()}.");
    }

    private void SerializePropertyAsElement(PropertyInfo property, XmlPropertyInfo xpi, object? value, XElement element, IXmlContext ctx)
    {
      IElementSerializer serializer = ctx.GetElementSerializer(value);
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
}
