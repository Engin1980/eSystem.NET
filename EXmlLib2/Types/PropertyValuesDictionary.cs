using System.Linq.Expressions;
using System.Reflection;

namespace EXmlLib2.Types;

public class PropertyValuesDictionary<T> : Dictionary<PropertyInfo, object?>
{
  public TProp? GetPropertyValue<TProp>(string propertyName)
  {
    object? tmp = this.FirstOrDefault(kv => kv.Key.Name == propertyName).Value;
    TProp? ret = (TProp?)tmp;
    return ret;
  }

  public TProp? GetPropertyValue<TProp>(Expression<Func<T, TProp>> propertySelector)
  {
    if (propertySelector.Body is not MemberExpression member ||
        member.Member is not PropertyInfo prop)
      throw new ArgumentException("Expression must be a property access.", nameof(propertySelector));

    object? tmp = this.FirstOrDefault(kv => kv.Key == prop).Value;
    TProp? ret = (TProp?)tmp;
    return ret;
  }

}