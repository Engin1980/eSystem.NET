using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESystem.SimpleXmlSerialization.Attributes;

namespace ESystem.SimpleXmlSerialization.Behavior
{
  static class Extensions
  {
    public static XmlBehaviorAttribute GetXmlBehavior(this System.Reflection.PropertyInfo property)
    {
      var attrs = property.GetCustomAttributes(false);
      XmlBehaviorAttribute ret = (XmlBehaviorAttribute)
        attrs.FirstOrDefault(i => i is XmlBehaviorAttribute);

      if (ret == null)
        ret = new XmlBehaviorAttribute();
      else
        ret.DoValidityTest();

      return ret;
    }

    public static XmlTypeBehaviorAttribute GetXmlTypeBehavior(this Type type)
    {
      var attrs = type.GetCustomAttributes(false);
      XmlTypeBehaviorAttribute ret = (XmlTypeBehaviorAttribute)
        attrs.FirstOrDefault(i => i is XmlTypeBehaviorAttribute);

      if (ret == null)
        ret = new XmlTypeBehaviorAttribute();
      else
        ret.DoValidityTest();

      return ret;
    }

    public static bool InheritsOrImplements(this Type child, Type parent)
    {
      parent = ResolveGenericTypeDefinition(parent);

      var currentChild = child.IsGenericType
                             ? child.GetGenericTypeDefinition()
                             : child;

      while (currentChild != typeof(object))
      {
        if (parent == currentChild || HasAnyInterfaces(parent, currentChild))
          return true;

        currentChild = currentChild.BaseType != null
                       && currentChild.BaseType.IsGenericType
                           ? currentChild.BaseType.GetGenericTypeDefinition()
                           : currentChild.BaseType;

        if (currentChild == null)
          return false;
      }
      return false;
    }

    private static bool HasAnyInterfaces(Type parent, Type child)
    {
      return child.GetInterfaces()
          .Any(childInterface =>
          {
            var currentInterface = childInterface.IsGenericType
                ? childInterface.GetGenericTypeDefinition()
                : childInterface;

            return currentInterface == parent;
          });
    }

    private static Type ResolveGenericTypeDefinition(Type parent)
    {
      var shouldUseGenericType = true;
      if (parent.IsGenericType && parent.GetGenericTypeDefinition() != parent)
        shouldUseGenericType = false;

      if (parent.IsGenericType && shouldUseGenericType)
        parent = parent.GetGenericTypeDefinition();
      return parent;
    }
  }
}
