using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased
{
  public static class PropertyProviders
  {
    public static Func<Type, PropertyInfo[]> PublicInstancePropertiesProvider
      => type => type.GetProperties(
        System.Reflection.BindingFlags.Public |
        System.Reflection.BindingFlags.Instance);
  }
}
