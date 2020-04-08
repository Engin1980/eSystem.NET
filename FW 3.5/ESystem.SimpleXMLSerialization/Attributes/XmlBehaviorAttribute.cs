using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace ESystem.SimpleXmlSerialization.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class XmlBehaviorAttribute : Attribute
  {
    /// <summary>
    /// If true, property is saved as XML attribute. Property must be parsable or string type. This affects saving only! 
    /// </summary>
    public bool IsXmlAttribute { get; set; }
    /// <summary>
    /// If true, property is ignored and not saved/loaded from XML file.
    /// </summary>
    public bool IsIgnored { get; set; }
    /// <summary>
    /// Explicit format. If used, the type must have Parse/ParseExact methods and will be saved using Parse/ToString() methods.
    /// </summary>
    public string Format { get; set; }
    /// <summary>
    /// Type of which the instance will be created. Used when property is typed as interface, so the target instance type cannot 
    /// be inherited. This type must have public parameterless constructor.
    /// </summary>
    public Type DefaultInstanceType { get; set; }
    /// <summary>
    /// Explicit culture used for parsing/formatting. If empty, CurrentUICulture will be used.
    /// </summary>
    public string Culture { get; set; }
    /// <summary>
    /// Values for true/false delimited by semi-colon (";"). If empty, true/false strings will be used.
    /// </summary>
    public string ExplicitBoolTrueFalseValues { get; set; }
    /// <summary>
    /// Defines name of attribute (if is-xml-attribute == true) or name of element in which value is saved. Null if not used, empty string or dot (".") if
    /// mapped to parent element, hierarchy of elements delimited by forward-slash (/) for other mapping.
    /// </summary>
    public string XmlMapping { get; set; }

    internal bool IsXmlMappingToSelf()
    {
      return (XmlMapping != null && 
        (XmlMapping == "" || XmlMapping == "."));
    }
    internal bool IsXmlMappingWithHierarchy()
    {
      return (XmlMapping != null && XmlMapping.Contains('/'));
    }

    public XmlBehaviorAttribute()
    {
      this.IsXmlAttribute = false;
      this.IsIgnored = false;
      this.Format = null;
      this.DefaultInstanceType = null;
      this.Culture = null;
      this.ExplicitBoolTrueFalseValues = null;
    }

    public void DoValidityTest()
    {
      if (ExplicitBoolTrueFalseValues != null)
      {
        if (ExplicitBoolTrueFalseValues.Count(i =>i==';') != 1)
          throw new Exception("ExplicitBoolTrueFalseValues definition must containt exactly one \";\" delimiter.");
      }

      if (XmlMapping != null)
        if (XmlMapping.StartsWith("/") || XmlMapping.EndsWith("/"))
          throw new Exception("XmlMaping cannot start or end with slash.");

      if (IsXmlAttribute && (IsXmlMappingToSelf() || IsXmlMappingWithHierarchy()))
        throw new Exception("Cannot be IsXmlAttribute set to true and XmlMapping with self or hierarchy mapping.");
    }

    internal string GetExplicitBoolValueForFalse()
    {
      return GetExplicitBoolValueFor(false);
    }
    internal string GetExplicitBoolValueFor(bool value)
    {
      string[] split = ExplicitBoolTrueFalseValues.Split(';');
      if (value)
        return split[0];
      else
        return split[1];
    }
  }
}
