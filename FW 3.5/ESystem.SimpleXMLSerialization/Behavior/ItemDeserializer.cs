using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

namespace ESystem.SimpleXmlSerialization.Behavior
{
  class ItemDeserializer : BaseItem
  {
    private enum eBehavior
    {
      Boolean,
      String,
      Enum,
      DataType,
      List
    }

    internal T Deserialize<T>(XElement elm)
    {
      T ret = default(T);

      ret = (T)Deserialize(typeof(T), elm);

      return ret;
    }

    internal object Deserialize(Type type, XElement elm)
    {
      object ret = null;

      eBehavior b = GetBehaviorOf(type);

      switch (b)
      {
        case eBehavior.String:
          ret = DeserializeString(elm);
          break;
        case eBehavior.Boolean:
          ret = DeserializeBoolean(elm);
          break;
        case eBehavior.Enum:
          ret = DeserializeEnum(type, elm);
          break;
        case eBehavior.List:
          ret = DeserializeList(type, elm);
          break;
        case eBehavior.DataType:
          ret = DeserializeObject(type, elm);
          break;
        default:
          throw new NotSupportedException();
      }

      return ret;
    }

    private object DeserializeEnum(Type type, XElement elm)
    {
      object ret;
      ret = GetEnumValueFromSomething(type, elm.Value);

      return ret;
    }

    private object GetEnumValueFromSomething(Type type, string value)
    {
      object ret;
      int pomI;
      if (int.TryParse(value, out pomI))
        ret = GetEnumValueFromInteger(type, pomI);
      else
        ret = GetEnumValueFromString(type, value);
      return ret;
    }

    private object GetEnumValueFromInteger(Type type, int pomI)
    {
      object ret;

      ret = pomI;

      return ret;
    }

    private object GetEnumValueFromString(Type type, string pom)
    {
      object ret;

      try
      {
        ret = Enum.Parse(type, pom);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to convert string " + pom + " into instance of enum " + GetFullName(type) + ".", ex);
      } // end catch

      return ret;
    }

    private object DeserializeBoolean(XElement elm)
    {
      string val = elm.Value;
      bool ret = GetBooleanValue(val);
      return ret;
    }

    #region Fill object

    private object DeserializeObject(Type type, XElement elm)
    {
      var beh = type.GetXmlTypeBehavior();
      object ret;

      ret = GetNewInstanceOf(type);
      var props = GetPropertiesOf(type);
      FillInstanceProperties(ret, props, elm);

      return ret;
    }

    #endregion Fill object

    #region Fill list

    private object DeserializeList(Type listType, XElement elm)
    {
      object ret = null;

      ret = GetNewInstanceOf(listType);
      FillInstance(ret, elm);

      System.Collections.IList lst = (System.Collections.IList)ret;
      FillListWithItems(lst, elm);

      return ret;
    }

    private void FillListWithItems(System.Collections.IList lst, XElement elm)
    {
      var beh = GetXmlTypeBehaviorOf(lst);
      string itemElement = beh.IListItemXmlElement;
      FillListWithItems(lst, itemElement, elm);
    }

    private void FillListWithItems(System.Collections.IList lst, string itemElement, XElement elm)
    {
      var elms = GetElementsFromList(itemElement, elm);

      if (elms.Count > 0)
      {
        Type itemType = GetTypeOfListItem(lst);
        elms.ForEach(i =>
        {
          FillListWithItem(lst, itemType, i);
        });
      }
    }

    private void FillListWithItem(System.Collections.IList lst, Type itemType, XElement elm)
    {
      object val = Deserialize(itemType, elm);

      lst.Add(val);
    }

    #endregion Fill list

    #region Fill instance properties

    private void FillInstance(object instance, XElement elm)
    {
      Type t = instance.GetType();
      List<PropertyInfo> props = GetPropertiesOf(t, ILIST_IGNORED_PROPERTIES);

      FillInstanceProperties(instance, props, elm);
    }

    private void FillInstanceProperties(object instance, List<PropertyInfo> props, XElement elm)
    {
      foreach (var item in props)
      {
        FillInstanceProperty(instance, item, elm);
      }
    }

    private void FillInstanceProperty(object instance, PropertyInfo property, XElement elm)
    {
      XElement valueElm = null;
      var beh = GetXmlBehaviorOf(property);
      if (beh.IsIgnored)
        return; // ignorovana vlastnost

      valueElm = GetElementForProperty(elm, property);

      if (valueElm != null)
      {
        if (string.IsNullOrEmpty(beh.Format) == false)
          FillInstanceByParsing(instance, property, valueElm);
        else
          FillInstancePropertyWithNewInstance(instance, property, valueElm);
      }
      else
      {
        string attributeValue = GetAttributeForProperty(elm, property);
        if (string.IsNullOrEmpty(beh.Format) == false)
          FillInstanceByParsingWithAttributeValue(instance, property, attributeValue);
        else
          FillInstancePropertyWithNewInstanceWithAttributeValue(instance, property, attributeValue);
      }
    }

    private void FillInstancePropertyWithNewInstanceWithAttributeValue(object instance, PropertyInfo property, string attributeValue)
    {
      object val;
      var b = GetBehaviorOf(property.PropertyType);

      switch (b)
      {
        case eBehavior.String:
          SetObjectProperty(instance, property, attributeValue);
          break;
        case eBehavior.Boolean:
          val = GetBooleanValue(attributeValue);
          SetObjectProperty(instance, property, val);
          break;
        case eBehavior.Enum:
          val = GetEnumValueFromSomething(property.PropertyType, attributeValue);
          SetObjectProperty(instance, property, val);
          break;
        default:
          try
          {
            FillInstanceByParsingWithAttributeValue(instance, property, attributeValue);
          }
          catch (Exception ex)
          {
            throw new Exception("Failed to get convert xml-attribute value " + attributeValue + " into property " + GetFullName(property) +
              ". Property must be type of string or have Parse() method. Or culture information info is incorrect or not specified when required.",
              ex);

          } // end catch

          break;
      }
    }

    private void FillInstanceByParsingWithAttributeValue(object instance, PropertyInfo property, string attributeValue)
    {
      object val = GetParsedValue(property, attributeValue);
      SetObjectProperty(instance, property, val);
    }

    private bool AreNamesEqual(string a, string b)
    {
      return string.Compare(a, b, true) == 0;
    }

    #region Fill by parsing

    private void FillInstanceByParsing(object instance, PropertyInfo item, XElement elm)
    {
      object val = GetParsedValue(item, elm.Value);
      SetObjectProperty(instance, item, val);
    }

    private object GetParsedValue(PropertyInfo item, string value)
    {
      object ret;

      bool hasParseExact = GetIfTypeHasParseExactMethod(item.PropertyType);
      bool hasParse = GetIfTypeHasParseMethod(item.PropertyType);

      if (hasParse || hasParseExact)
      {
        var beh = GetXmlBehaviorOf(item);
        bool addedCulture = AddCultureIfRequired(beh);

        if (hasParseExact && string.IsNullOrEmpty(beh.Format) == false)
          ret = ParseExact(item.PropertyType, beh.Format, value);
        else
          ret = Parse(item.PropertyType, value);

        if (addedCulture)
          RemoveCulture();
      }
      else
        throw new Exception("Property " + GetFullName(item) + " cannot be filled, because neither \"ParseExact()\" nor \"Parse()\" methods were found for type " + GetFullName(item.PropertyType) + ".");

      return ret;
    }

    private object ParseExact(Type type, string format, string elementValue)
    {
      CultureInfo provider = CultureInfo.InvariantCulture;
      object ret = null;

      try
      {
        ret =
          InvokeStaticMethod(type, "ParseExact", elementValue, format, provider);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to parse value via type " + GetFullName(type) + " for value " + elementValue + " and format " + format + ".", ex);

      } // end catch


      return ret;
    }
    private object Parse(Type type, string elementValue)
    {
      object ret = null;

      try
      {
        ret =
          InvokeStaticMethod(type, "Parse", elementValue, base.GetCurrentCulture());
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to parse value via type " + GetFullName(type) + " for value " + elementValue + ".", ex);

      } // end catch

      return ret;
    }


    private bool GetIfTypeHasParseExactMethod(Type type)
    {
      bool ret = GetIfTypeHasMethod(type, "ParseExact", true);
      return ret;
    }
    private bool GetIfTypeHasParseMethod(Type type)
    {
      bool ret = GetIfTypeHasMethod(type, "Parse", true);
      return ret;
    }
    private bool GetIfTypeHasMethod(Type type, string methodName, bool isStatic)
    {
      BindingFlags flags;
      if (isStatic)
        flags = BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static;
      else
        flags = BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance;

      MethodInfo[] mis = type.GetMethods(flags).Where(i => i.Name == methodName).ToArray();

      return mis.Length > 0;
    }
    private object InvokeStaticMethod(Type type, string methodName, params object[] parameters)
    {
      BindingFlags flags;
      flags = BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static;

      object ret = null;

      try
      {
        ret = type.InvokeMember(methodName, flags, null, null, parameters);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to invoke method " + methodName + " on type " + GetFullName(type) + " with " + parameters.Length + " parameters.", ex);
      } // end catch

      return ret;
    }

    #endregion Fill by parsing

    private void FillInstancePropertyWithNewInstance(object instance, PropertyInfo item, XElement elm)
    {
      object value = Deserialize(item.PropertyType, elm);
      SetObjectProperty(instance, item, value);
    }

    private void SetObjectProperty(object instance, PropertyInfo item, object value)
    {
      try
      {
        item.SetValue(instance, value, null);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to set into property " + instance.GetType().FullName + "." + item.Name + " value.", ex);
      } // end catch

    }

    private List<PropertyInfo> GetPropertiesOf(Type t, string[] ignoredProperties = null)
    {
      List<PropertyInfo> pom = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty).ToList();
      List<PropertyInfo> ret = new List<PropertyInfo>();

      if (ignoredProperties != null && ignoredProperties.Length > 0)
        pom.ForEach(i =>
        {
          if (ignoredProperties.Contains(i.Name) == false) ret.Add(i);
        });
      else
        ret.AddRange(pom);

      return ret;
    }

    #endregion Fill instance properties

    #region Create new instance

    private object GetNewInstanceOf(Type type)
    {
      object ret = null;
      try
      {
        var beh = type.GetXmlTypeBehavior();
        Type targetInstanceType = GetTargetInstanceType(type, beh);
        ret = CreateNewInstanceOf(targetInstanceType);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to get new instance.", ex);
      }

      return ret;
    }

    private object CreateNewInstanceOf(Type targetInstanceType)
    {
      object ret = null;

      try
      {
        ret = Activator.CreateInstance(targetInstanceType);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to create new instance of type " + targetInstanceType.FullName + ".", ex);
      } // end catch

      return ret;
    }

    private Type GetTargetInstanceType(Type type, Attributes.XmlTypeBehaviorAttribute beh)
    {
      Type ret = type;
      if (beh.DefaultInstanceType != null)
      {
        ret = beh.DefaultInstanceType;
      }

      return ret;
    }

    #endregion Create new instance

    private object DeserializeString(XElement elm)
    {
      string ret = elm.Value;
      return ret;
    }

    private bool GetBooleanValue(string attributeValue)
    {
      if (attributeValue == null)
        return false;

      attributeValue = attributeValue.ToLower();

      foreach (var item in BOOLEAN_FALSE_VALUES)
      {
        if (AreNamesEqual(item, attributeValue))
          return false;
      }

      return true;
    }

    private eBehavior GetBehaviorOf(Type type)
    {
      eBehavior ret;
      if (type == typeof(string))
        ret = eBehavior.String;
      else if (type == typeof(bool))
        ret = eBehavior.Boolean;
      else if (type.IsEnum)
        ret = eBehavior.Enum;
      else if (type.InheritsOrImplements(typeof(System.Collections.IList)))
        ret = eBehavior.List;
      else
        ret = eBehavior.DataType;

      return ret;
    }

    #region Get-XXX-from XElement
    private string GetAttributeForProperty(XElement elm, PropertyInfo property)
    {
      string ret = null;

      var attribs = elm.Attributes();
      foreach (var item in attribs)
      {
        if (AreNamesEqual(property.Name, item.Name.LocalName))
        {
          ret = item.Value;
          break;
        }
      }

      return ret;
    }
    private XElement GetElementForProperty(XElement elm, PropertyInfo property)
    {
      string propertyElementName = property.Name;
      var beh = property.GetXmlBehavior();
      XElement ret = null;

      if (beh.XmlMapping!=null)
      {
        if (beh.IsXmlMappingToSelf())
          ret = elm;
        else
          elm = GetMappedElementUsingRelativeMapping(elm, beh.XmlMapping, out propertyElementName);
      }

      var elms = elm.Elements();

      foreach (var item in elms)
      {
        if (AreNamesEqual(propertyElementName, item.Name.LocalName))
        {
          ret = item;
          break;
        }
      }

      return ret;
    }

    private static XElement GetMappedElementUsingRelativeMapping(XElement elm, string relativeMappingOrNull, out string newPropertyElementName)
    {
      XElement ret = elm;

      string[] inners = relativeMappingOrNull.Split('/');
      for (int i = 0; i < (inners.Length - 1); i++)
      {
        try
        {
          ret = ret.Element(XName.Get(inners[i], ""));
        }
        catch (Exception ex)
        {
          throw new Exception("Failed to find relative mapping path " + relativeMappingOrNull + ". Inner element " + inners[i] + " not found.", ex);
        } // end catch

      }
      newPropertyElementName = inners.Last();
      return ret;
    }
    private List<XElement> GetElementsFromList(string itemElementName, XElement elm)
    {
      var elms = elm.Elements();
      List<XElement> ret = new List<XElement>();

      foreach (XElement item in elms)
      {
        if (AreNamesEqual(itemElementName, item.Name.LocalName))
          ret.Add(item);
      }

      return ret;
    }

    #endregion Get-XXX-from XElement
  }
}
