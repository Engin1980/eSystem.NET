using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ESystem.SimpleXmlSerialization.Attributes;

namespace ESystem.SimpleXmlSerialization.Behavior
{
  class ItemSerializer : BaseItem
  {
    enum eBehavior
    {
      Primitive,
      String,
      IEnumerable,
      DataType
    }

    public void Serialize(EXmlWriter wrt, object item, string elementName, string dtdFile)
    {
      wrt.WriteDocumentHeader();

      if (string.IsNullOrEmpty(dtdFile) == false)
        wrt.WriteDoctype(elementName, dtdFile);

      SerializeItem(wrt, elementName, item, null);
    }

    private void SerializeItem(EXmlWriter wrt, string elementName, object item, Type desiredType)
    {
      eBehavior beh = GetBehaviorOf(item.GetType());

      switch (beh)
      {
        case eBehavior.Primitive:
        case eBehavior.String:
          WriteToStringAsElement(wrt, elementName, item, null);
          break;
        case eBehavior.IEnumerable:
          SaveObject(wrt, item, elementName, desiredType, true);
          break;
        case eBehavior.DataType:
          SaveObject(wrt, item, elementName, desiredType, false);
          break;
        default:
          throw new NotImplementedException();
      }
    }

    private void WriteBooleanAsElement(EXmlWriter wrt, string element, object item, string explicitValues)
    {
      string t = true.ToString();
      string f = false.ToString();
      if (explicitValues != null)
      {
        string[] spl = explicitValues.Split(';');
        t = spl[0];
        f = spl[1];
      }
      bool value = (bool)item;
      string valueString;
      if (value)
        valueString = t;
      else
        valueString = f;
      wrt.WriteElement(element, valueString);
    }

    private void SaveObject(EXmlWriter wrt, object item, string elementName, Type desiredType, bool isObjectIEnumerable)
    {
      List<PropertyInfo> elmProps;
      List<PropertyInfo> attProps;

      AnalyseProperties(item, isObjectIEnumerable, out elmProps, out attProps);

      if (attProps.Count > 0 && elementName == null)
        throw new Exception("Unable to have xmlAttribute-properties in type which XmlMapping defines item to its subject xml element.");

      if (elementName != null)
        wrt.BeginOpeningTag(elementName);

      WriteDesiredTypeAttributeIfRequired(wrt, elementName == null, item, desiredType);

      WritePropertiesWhichAreAttributes(wrt, item, attProps);

      // this is here to build .. /> in XML
      if (elmProps.Count > 0 || isObjectIEnumerable)
      {
        if (elementName != null)
          wrt.EndOpeningTag();

        WritePropertiesWhichAreElements(wrt, item, elmProps);

        if (isObjectIEnumerable)
          SaveIEnumerableItems(wrt, item);
      }

      if (elementName != null)
        wrt.EndElement();
    }

    private void AnalyseProperties(object item, bool isObjectIEnumerable, out List<PropertyInfo> elmProps, out List<PropertyInfo> attProps)
    {
      string[] ignoredProperties = null;
      if (isObjectIEnumerable)
        ignoredProperties = ILIST_IGNORED_PROPERTIES;
      AnalyseProperties(item.GetType(), ignoredProperties, out elmProps, out attProps);
    }

    private void WriteDesiredTypeAttributeIfRequired(EXmlWriter wrt, bool isElementNameEmpty, object item, Type desiredType)
    {
      if (desiredType != null &&
        item.GetType() != desiredType)
      {
        if (isElementNameEmpty)
          throw new Exception("Cannot write desired-type attribute, because mapping defines that inner element is mapped to its parent.");
        wrt.AddAttribute(DESIRED_TYPE_ATTRIBUTE, item.GetType().FullName);
      }
    }

    private void WritePropertiesWhichAreElements(EXmlWriter wrt, object item, List<PropertyInfo> elmProps)
    {
      elmProps.ForEach(i =>
      {
        EvaluatePropertyFormatAndWriteIt(wrt, item, i, WritePropertyAsElement);
      });
    }

    private void WritePropertiesWhichAreAttributes(EXmlWriter wrt, object item, List<PropertyInfo> attProps)
    {
      attProps.ForEach(i =>
      {
        EvaluatePropertyFormatAndWriteIt(wrt, item, i, WritePropertyAsAttribute);
      });
    }

    private void EvaluatePropertyFormatAndWriteIt(EXmlWriter wrt, object item, PropertyInfo property,
      Action<EXmlWriter, object, PropertyInfo> actionForNonFormat)
    {
      var beh = property.GetXmlBehavior();

      if (string.IsNullOrEmpty(beh.Format))
        actionForNonFormat(wrt, item, property);
      else
      {
        string elementName = GetElementName(property);
        object val = GetPropertyValue(item, property);
        WriteToStringAsElement(wrt, elementName, val, beh.Format);
      }
    }

    private void SaveIEnumerableItems(EXmlWriter wrt, object item)
    {
      System.Collections.IEnumerable lst;

      lst = ConvertToIEnumerable(item);

      var beh = item.GetType().GetXmlTypeBehavior();
      string elementName = beh.IListItemXmlElement;
      Type itemType = base.GetTypeOfListItem(lst);

      foreach (var it in lst)
      {
        SerializeItem(wrt, elementName, it, itemType);
      }
    }

    private System.Collections.IEnumerable ConvertToIEnumerable(object item)
    {
      System.Collections.IEnumerable lst;
      try
      {
        lst = (System.Collections.IEnumerable)item;
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to cast instance of type " + GetFullName(item.GetType()) + " to IEnumerable.", ex);
      } // end catch
      return lst;
    }

    private void WritePropertyAsElement(EXmlWriter wrt, object item, PropertyInfo property)
    {
      var beh = property.GetXmlBehavior();
      object val = GetPropertyValue(item, property);
      string elementName = null;
      int hierarchyLevel = 0;

      if (beh.IsXmlMappingToSelf())
        elementName = null;
      else if (beh.IsXmlMappingWithHierarchy())
        hierarchyLevel = BuildHierarchy(wrt, beh.XmlMapping);
      else if (beh.XmlMapping != null)
        elementName = beh.XmlMapping;
      else
        elementName = GetElementName(property);

      SerializeItem(wrt, elementName, val, property.PropertyType);

      if (hierarchyLevel > 0)
        for (int i = 0; i < hierarchyLevel; i++) wrt.EndElement();
    }

    private int BuildHierarchy(EXmlWriter wrt, string mapping)
    {
      string[] elms = mapping.Split('/');
      for (int i = 0; i < elms.Length - 1; i++)
      {
        wrt.BeginElement(elms[i]);
      }
      return elms.Length - 1;
    }

    private string GetElementName(PropertyInfo i)
    {
      string ret;

      ret = char.ToUpper(i.Name[0]) + i.Name.Substring(1);

      return ret;
    }

    private void AnalyseProperties(Type type, string[] ignoredProperties, out List<PropertyInfo> elmProps, out List<PropertyInfo> attProps)
    {
      var props = type.GetProperties().Where(i => i.CanRead && i.CanWrite).ToArray();
      XmlBehaviorAttribute a;

      elmProps = new List<PropertyInfo>();
      attProps = new List<PropertyInfo>();

      foreach (var item in props)
      {
        if (ignoredProperties != null && ignoredProperties.Contains(item.Name))
          continue;

        a = item.GetXmlBehavior();
        if (a.IsIgnored)
          continue;

        if (a.IsXmlAttribute)
          attProps.Add(item);
        else
          elmProps.Add(item);
      }
    }

    private void WritePropertyAsAttribute(EXmlWriter wrt, object item, PropertyInfo property)
    {
      var beh = GetXmlBehaviorOf(property);
      bool cultureAdded = base.AddCultureIfRequired(beh);

      object objValue = GetPropertyValue(item, property);
      // pokud je hodnota NULL, tak se nic neuklada
      if (objValue != null)
      {
        string strValue ;
        if (objValue is bool && beh.ExplicitBoolTrueFalseValues != null)
          strValue = beh.GetExplicitBoolValueFor((bool)objValue);
        else
          strValue = GetToStringValue(objValue, beh.Format);
        string attribName = GetAttributeName(property.Name, beh);

        if (cultureAdded)
          base.RemoveCulture();

        wrt.AddAttribute(attribName, strValue);
      }
    }

    private string GetAttributeName(string p, XmlBehaviorAttribute behavior)
    {
      string ret;
      if (behavior.XmlMapping != null)
        ret = behavior.XmlMapping;
      else
        ret = char.ToLower(p[0]) + p.Substring(1);

      return ret;
    }

    private object GetPropertyValue(object item, PropertyInfo property)
    {
      object ret = null;

      try
      {
        ret = property.GetValue(item, null);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to get value of " + GetFullName(property) + ".", ex);
      } // end catch

      return ret;
    }

    private eBehavior GetBehaviorOf(Type type)
    {
      if (type.IsPrimitive)
        return eBehavior.Primitive;
      else if (type == typeof(string))
        return eBehavior.String;
      else if (type.InheritsOrImplements(typeof(System.Collections.IEnumerable)))
        return eBehavior.IEnumerable;
      else
        return eBehavior.DataType;
    }

    private bool IsPrimitiveItem(object item)
    {
      Type t = item.GetType();

      if (t.IsPrimitive)
        return true;
      else
        return false;
    }

    private void WriteToStringAsElement(EXmlWriter wrt, string element, object item, string formatIfAny)
    {
      string value = GetToStringValue(item, formatIfAny);
      wrt.WriteElement(element, value);
    }

    private string GetToStringValue(object item, string formatIfAny)
    {
      Type t = item.GetType();
      bool hasFormat = string.IsNullOrEmpty(formatIfAny) == false;
      bool useFormat;
      MethodInfo mi = GetBestToStringMethod(t, hasFormat, out useFormat);

      string ret = null;
      object[] pars = null;
      if (useFormat)
        pars = new object[] { formatIfAny, base.GetCurrentCulture() };
      else
        pars = new object[] { base.GetCurrentCulture() };

      try
      {
        ret = (string)mi.Invoke(item, pars);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to invoke " + GetFullName(mi) + " with " + pars.Length + " parameters.", ex);
      } // end catch


      return ret;
    }

    private MethodInfo GetBestToStringMethod(Type type, bool hasFormat, out bool useFormat)
    {
      var mis = type.GetMethods().Where(i => i.Name == "ToString").ToArray();

      if (hasFormat)
      {
        foreach (var item in mis)
        {
          var pars = item.GetParameters();
          if (pars.Length == 2 &&
            pars[0].ParameterType == typeof(string) &&
            pars[1].ParameterType == typeof(IFormatProvider))
          {
            useFormat = true;
            return item;
          }
        }
      }

      foreach (var item in mis)
      {
        var pars = item.GetParameters();
        if (pars.Length == 1 &&
          pars[0].ParameterType == typeof(IFormatProvider))
        {
          useFormat = false;
          return item;
        }
      }

      throw new Exception("Failed to find suitable ToString() method for type " + GetFullName(type));
    }
  }
}
