using ESystem.Logging;
using ESystem;
using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Abstractions.Abstracts;

namespace EXmlLib2.Implementations.Deserializers
{
  public class TypeElementDeserializer<T> : TypedElementDeserializer<T>
  {
    enum TypeKind
    {
      Class,
      Struct,
      RecordClass,
      RecordStruct
    }

    private readonly Logger logger = Logger.Create(typeof(T));
    public XmlTypeInfo<T> XmlTypeInfo { get; private set; }

    public TypeElementDeserializer()
    {
      this.XmlTypeInfo = new();
    }

    public TypeElementDeserializer(XmlTypeInfo<T> xmlTypeInfo)
    {
      EAssert.Argument.IsNotNull(xmlTypeInfo, nameof(xmlTypeInfo));
      this.XmlTypeInfo = xmlTypeInfo;
    }

    protected virtual PropertyInfo[] GetProperties()
    {
      PropertyInfo[] ret = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
      return ret;
    }

    public override T Deserialize(XElement element, IXmlContext ctx)
    {
      PropertyValuesDictionary<T> propertyValues;
      T? ret;
      EAssert.Argument.IsNotNull(element, nameof(element));

      EAssert.IsFalse(element.Value == ctx.DefaultNullString, $"Element value cannot be '{ctx.DefaultNullString}' (that is 'null') for deserialization of type {typeof(T)}.");

      var props = GetProperties();
      try
      {
        propertyValues = DeserializeProperties(props, element, ctx);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Failed to deserialize {typeof(T)} from element {element} - failed to read properties.", ex);
        this.logger.LogException(eex);
        throw eex;
      }

      try
      {
        ret = CreateAndFillObject(propertyValues);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Failed to deserialize {typeof(T)} from element {element} - failed to create & fill the instance.", ex);
        this.logger.LogException(eex);
        throw eex;
      }
      return ret;
    }

    private T CreateAndFillObject(PropertyValuesDictionary<T> propertyValues)
    {
      T ret;

      if (XmlTypeInfo.FactoryMethod != null)
      {
        ret = CreateUsingFactoryMethod(propertyValues);
      }
      else
      {
        TypeKind typeKind = GetTypeKind<T>();
        ret = typeKind switch
        {
          TypeKind.RecordClass => CreateAndInitStructOrRecord(true, propertyValues),
          TypeKind.RecordStruct => CreateAndInitStructOrRecord(true, propertyValues),
          TypeKind.Struct => CreateAndInitStructOrRecord(false, propertyValues),
          TypeKind.Class => CreateAndInitClass(propertyValues),
          _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(typeKind)
        };
      }
      return ret;
    }

    private T CreateAndInitStructOrRecord(bool isForRecords, Dictionary<PropertyInfo, object?> propertyValues)
    {
      var type = typeof(T);
      if (!isForRecords)
        EAssert.IsTrue(type.IsValueType, $"Generic type T must be struct (provided '{typeof(T)}').");

      var ctors = type.GetConstructors();
      EAssert.IsTrue(ctors.Length > 0, $"Generic type '{typeof(T)}' must have public constructor.");

      // here we are looking for .ctor with best match of parameters against properties
      ConstructorInfo? bestCtor = null;
      object?[]? bestArgs = null;
      int bestScore = -1;

      foreach (var ctor in ctors)
      {
        var parameters = ctor.GetParameters();
        var args = new object?[parameters.Length];
        int score = 0;
        bool usable = true;

        for (int i = 0; i < parameters.Length; i++)
        {
          var param = parameters[i];
          var prop = propertyValues.Keys.FirstOrDefault(p => string.Equals(p.Name, param.Name, StringComparison.OrdinalIgnoreCase));

          if (prop != null)
          {
            var value = propertyValues[prop];

            if (value != null && !param.ParameterType.IsInstanceOfType(value))
            {
              try
              {
                value = Convert.ChangeType(value, param.ParameterType);
              }
              catch
              {
                usable = false;
                break;
              }
            }

            args[i] = value;
            score++;
          }
          else if (param.HasDefaultValue)
            args[i] = param.DefaultValue;
          else if (param.IsOptional)
            args[i] = Type.Missing;
          else
          {
            usable = false;
            break;
          }
        }

        if (!usable) continue;

        if (score > bestScore)
        {
          bestScore = score;
          bestCtor = ctor;
          bestArgs = args;
        }
      }

      if (bestCtor == null || bestArgs == null)
        throw new InvalidOperationException($"No available constructor found to create an instance of {typeof(T)}.");

      T ret = (T)bestCtor.Invoke(bestArgs);
      return ret;
    }

    private T CreateAndInitClass(Dictionary<PropertyInfo, object?> propertyValues)
    {
      T ret;
      if (XmlTypeInfo.Constructor != null)
      {
        try
        {
          ret = XmlTypeInfo.Constructor.Invoke();
        }
        catch (Exception ex)
        {
          var eex = new EXmlException($"Failed to create an instance of {typeof(T)} using custom {nameof(XmlTypeInfo<T>.Constructor)}.", ex);
          logger.LogException(eex);
          throw eex;
        }
      }
      else
      {
        try
        {
          ret = CreateInstanceUsingDefaultConstructor();
        }
        catch (Exception ex)
        {
          var eex = new EXmlException($"Failed to create an instance of {typeof(T)} using default implementation.", ex);
          logger.LogException(eex);
          throw eex;
        }
      }
      FillObject(ret, propertyValues);
      return ret;
    }

    static TypeKind GetTypeKind<T>()
    {
      var type = typeof(T);

      bool isRecord =
          type.GetProperty(
              "EqualityContract",
              BindingFlags.Instance | BindingFlags.NonPublic
          ) != null;

      if (isRecord)
        return type.IsValueType ? TypeKind.RecordStruct : TypeKind.RecordClass;

      return type.IsValueType ? TypeKind.Struct : TypeKind.Class;
    }
    private T CreateUsingFactoryMethod(PropertyValuesDictionary<T> propertyValues)
    {
      EAssert.IsNotNull(XmlTypeInfo.FactoryMethod);
      T ret;
      try
      {
        ret = XmlTypeInfo.FactoryMethod.Invoke(propertyValues);
      }
      catch (Exception ex)
      {
        var eex = new EXmlException($"Failed to create an instance of {typeof(T)} using custom {nameof(XmlTypeInfo<T>.FactoryMethod)}.", ex);
        logger.LogException(eex);
        throw eex;
      }

      return ret;
    }

    private void FillObject(T target, Dictionary<PropertyInfo, object?> propertyValues)
    {
      foreach (var kvp in propertyValues)
      {
        PropertyInfo propertyInfo = kvp.Key;
        object? propertyValue = kvp.Value;

        if (propertyInfo.CanWrite == false) throw new EXmlException($"Unable to fill value of property {typeof(T).Name}.{propertyInfo.Name}. Cannot write.");

        try
        {
          propertyInfo.SetValue(target, propertyValue);
        }
        catch (Exception ex)
        {
          var eex = new EXmlException($"Failed to create an instance of {typeof(T)} using default implementation.", ex);
          logger.LogException(eex);
          throw eex;
        }
      }
    }

    private T CreateInstanceUsingDefaultConstructor()
    {
      T ret;
      //TODO remove if not used
      //ConstructorInfo ci;
      //ci = typeof(T).GetConstructor(Array.Empty<Type>()) ?? throw new EXmlException($"Failed to find public parameter-less constructor for type {typeof(T)}.");
      try
      {
        ret = Activator.CreateInstance<T>();
      }
      catch (Exception ex)
      {
        var eex = new EXmlException("Failed to create an instance.", ex);
        logger.LogException(eex);
        throw eex;
      }
      return ret;
    }

    private PropertyValuesDictionary<T> DeserializeProperties(PropertyInfo[] properties, XElement element, IXmlContext ctx)
    {
      PropertyValuesDictionary<T> ret = [];

      foreach (var property in properties)
      {
        object? propertyValue = DeserializeProperty(property, element, ctx);
        if (propertyValue == IGNORED_PROPERTY) continue;
        ret[property] = propertyValue;
      }

      return ret;
    }

    private static readonly object IGNORED_PROPERTY = new();
    private object? DeserializeProperty(PropertyInfo property, XElement element, IXmlContext ctx)
    {
      object? ret;
      XmlPropertyInfo xpi = XmlTypeInfo.PropertyInfos.TryGet(property) ?? this.XmlTypeInfo.DefaultXmlPropertyInfo;
      if (xpi.Obligation == XmlObligation.Ignored)
        ret = IGNORED_PROPERTY;
      else
      {
        string xmlName = xpi.XmlName ?? property.Name;
        object? propertySource = ResolvePropertySource(element, xpi.Representation, xmlName);
        if (propertySource == null)
        {
          if (xpi.Obligation == XmlObligation.Mandatory)
            throw new EXmlException($"Unable to find source for property {typeof(T).Name}.{property.Name} (xmlName={xmlName}) in element {element}.");
          else
            ret = IGNORED_PROPERTY;
        }
        else if (propertySource is XElement childElement)
        {
          ret = DeserializePropertyFromElement(childElement, property.PropertyType, ctx);
        }
        else if (propertySource is XAttribute attribute)
        {
          ret = DeserializePropertyFromAttribute(attribute, property.PropertyType, ctx);
        }
        else
          throw new NotSupportedException();
      }

      return ret;
    }

    private object? DeserializePropertyFromAttribute(XAttribute attribute, Type targetType, IXmlContext ctx)
    {
      IAttributeDeserializer deserializer = ctx.AttributeDeserializers.GetByType(targetType);
      object? ret = deserializer.Deserialize(attribute.Value, targetType, ctx);
      return ret;
    }

    private object? DeserializePropertyFromElement(XElement element, Type targetType, IXmlContext ctx)
    {
      if (element.Attributes().FirstOrDefault(q => q.Name.Equals(XName.Get(ctx.TypeNameAttribute))) is XAttribute typeAttribute)
      {
        string typeName = typeAttribute.Value;
        targetType = global::System.Type.GetType(typeName)
          ?? throw new EXmlException($"Custom type is defined as {typeName}, but cannot be loaded.");
      }
      IElementDeserializer deserializer = ctx.ElementDeserializers.GetByType(targetType);
      object? ret = deserializer.Deserialize(element, targetType, ctx);
      return ret;
    }

    private object? ResolvePropertySource(XElement element, XmlRepresentation? representation, string xmlName)
    {
      object? ret;
      XAttribute? findInAttributes() => element.Attributes().FirstOrDefault(q => q.Name == xmlName);
      XElement? findInElements() => element.Elements().FirstOrDefault(q => q.Name == xmlName);
      if (representation == XmlRepresentation.Attribute)
        ret = findInAttributes();
      else if (representation == XmlRepresentation.Element)
        ret = findInElements();
      else
      {
        ret = findInAttributes();
        ret ??= findInElements();
      }
      return ret;
    }
  }
}
