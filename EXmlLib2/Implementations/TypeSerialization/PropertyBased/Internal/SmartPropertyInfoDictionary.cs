using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Internal;

/// <summary>
/// Used to map PropertyInfo to property deserializer with smarter lookup (by name and declaring type as fallback - for inherited properties).
/// </summary>
/// <typeparam name="T"></typeparam>
internal class SmartPropertyInfoDictionary<T>
{
    private readonly Dictionary<PropertyInfo, T> inner = [];

    public void Put(PropertyInfo propertyInfo, T propertyDeserializer)
    {
      inner[propertyInfo] = propertyDeserializer;
    }

    public T? TryGet(PropertyInfo propertyInfo)
    {
      T? ret;
      if (!inner.TryGetValue(propertyInfo, out ret))
      {
        var kvp = inner.FirstOrDefault(q =>
          q.Key.Name == propertyInfo.Name && q.Key.DeclaringType == propertyInfo.DeclaringType);
        ret = kvp.Value;
      }
      return ret;
    }
  }
