//using ESystem.Exceptions;
//using EXmlLib2.Abstractions;
//using EXmlLib2.Implementations.TypeSerialization.Helpers;
//using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties.Abstractions;
//using System.Reflection;
//using System.Xml.Linq;

//namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;

//public class SimplePropertyAsElement : IPropertySerializer, IPropertyDeserializer
//{
//  private MissingPropertyXmlSourceBehavior missingPropertyElementBehavior = MissingPropertyXmlSourceBehavior.Ignore;
//  private NameCaseMatching nameCaseMatching = NameCaseMatching.IgnoreCase;

//  public SimplePropertyAsElement WithMissingPropertyElementBehavior(MissingPropertyXmlSourceBehavior behavior)
//  {
//    missingPropertyElementBehavior = behavior;
//    return this;
//  }

//  public SimplePropertyAsElement WithNameCaseMatching(NameCaseMatching matching)
//  {
//    nameCaseMatching = matching;
//    return this;
//  }

//  private XElement? GetElementByName(XElement parentElement, string name)
//  {
//    XElement? ret;
//    if (nameCaseMatching == NameCaseMatching.Exact)
//    {
//      ret = parentElement.Element(XName.Get(name));
//    }
//    else
//    {
//      ret = parentElement.Elements().FirstOrDefault(q => string.Equals(q.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));
//    }
//    return ret;
//  }

//  public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
//  {
//    DeserializationResult ret;

//    XElement? propertyElement = GetElementByName(element, propertyInfo.Name);
//    if (propertyElement == null)
//    {
//      ret = missingPropertyElementBehavior switch
//      {
//        MissingPropertyXmlSourceBehavior.Ignore => DeserializationResult.FromNoResult(),
//        MissingPropertyXmlSourceBehavior.ThrowException => throw new InvalidOperationException($"Cannot find element for property '{propertyInfo.Name}' in element '{element.Name}'."),
//        _ => throw new UnexpectedEnumValueException(missingPropertyElementBehavior),
//      };
//    }
//    else
//    {
//      var deserializer = ctx.ElementDeserializers.GetByType(propertyInfo.PropertyType);
//      object? tmp = deserializer.Deserialize(propertyElement, propertyInfo.PropertyType, ctx); //TODO: ctx.DeserializeFromElement(element, instanceType, deserializer);
//      ret = tmp == null ? DeserializationResult.FromNull() : DeserializationResult.FromValue(tmp);
//    }

//    return ret;
//  }

//  public void SerializeProperty(PropertyInfo propertyInfo, object? propertyValue, XElement element, IXmlContext ctx)
//  {
//    var ser = ctx.ElementSerializers.GetByType(propertyInfo.PropertyType);
//    XElement propElement = new(propertyInfo.Name);
//    ctx.SerializeToElement(propertyValue, propertyInfo.PropertyType, propElement, ser);
//    element.Add(propElement);
//  }
//}




