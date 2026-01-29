using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased;

public static class PropertyValueReaders
{
  public static readonly Func<object, System.Reflection.PropertyInfo, object?> DefaultPropertyReader =
    (obj, prop) => prop.GetValue(obj);
}
