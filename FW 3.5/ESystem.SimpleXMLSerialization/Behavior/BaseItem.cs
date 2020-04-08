using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using ESystem.SimpleXmlSerialization.Attributes;

namespace ESystem.SimpleXmlSerialization.Behavior
{
  class BaseItem
  {

    public readonly static string[] ILIST_IGNORED_PROPERTIES = new string[] { "Capacity", "Count", "Item" };
    public readonly static string[] BOOLEAN_FALSE_VALUES = new string[] { "false", "0", "no", "ne", "null", "" };

    public const string DESIRED_TYPE_ATTRIBUTE = "__type";

    #region Culture stack stuff


    private CultureBehavior.CultureStack cultureStack = new CultureBehavior.CultureStack();

    protected void RemoveCulture()
    {
      cultureStack.Pop();
    }

    protected bool AddCultureIfRequired(XmlBehaviorAttribute beh)
    {
      bool ret = false;
      string c = beh.Culture;
      if (string.IsNullOrEmpty(c) == false)
      {
        cultureStack.Push(c);
        ret = true;
      }
      return ret;
    }

    protected CultureInfo GetCurrentCulture()
    {
      return cultureStack.Current;
    }

    #endregion Culture stack stuff

    #region Get-Xml-Behavior

    protected XmlTypeBehaviorAttribute GetXmlTypeBehaviorOf(object instance)
    {
      Debug.Assert(instance != null);

      Type type = instance.GetType();
      XmlTypeBehaviorAttribute ret = type.GetXmlTypeBehavior();
      return ret;
    }

    protected XmlBehaviorAttribute GetXmlBehaviorOf(PropertyInfo item)
    {
      XmlBehaviorAttribute ret = item.GetXmlBehavior();
      return ret;
    }

    #endregion Get-Xml-Behavior

    #region GetFullName()

    protected string GetFullName(Type type)
    {
      return type.FullName;
    }
    protected string GetFullName(PropertyInfo item)
    {
      return item.DeclaringType.FullName + "." + item.Name;
    }
    protected string GetFullName(MethodInfo item)
    {
      return item.DeclaringType.FullName + "." + item.Name + "()";
    }
    #endregion GetFullName()

    protected Type GetTypeOfListItem(System.Collections.IEnumerable lst)
    {
      Type ret;
      var beh = GetXmlTypeBehaviorOf(lst.GetType());
      if (beh.DefaultIListItemType != null)
        ret = beh.DefaultIListItemType;
      else
        ret = GetGenericParameterOfList(lst);

      return ret;
    }
    protected Type GetTypeOfListItem(System.Collections.IList lst)
    {
      Type ret = GetTypeOfListItem(lst as System.Collections.IEnumerable);
      return ret;
    }

    private Type GetGenericParameterOfList(System.Collections.IEnumerable lst)
    {
      Type listType = lst.GetType();
      Type ret = null;

      try
      {
        ret = GetGenericArgumentOfType(listType);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to get generic type of IList of type " + GetFullName(lst.GetType()) +
          ". List is probably not a generic, and non-generic lists are not supported.", ex);
      } // end catch

      return ret;
    }

    private Type GetGenericArgumentOfType(Type listType)
    {
      Type ret;
      var gargs = listType.GetGenericArguments();
      if (gargs.Length == 0)
      {
        if (listType.BaseType != null)
          ret = GetGenericArgumentOfType(listType.BaseType);
        else
          throw new Exception("Failed to get generic type. Current type has no base type and no generic type in inheritance sequence found.");
      }
      else
        ret = gargs[0];

      return ret;
    }


  }
}
