using ESystem;
using ESystem.Asserting;
using EXmlLib2.Implementations.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Types
{
  public class XmlTypeInfo<T>
  {
    public static XmlTypeInfo<T> Create() => new();
    internal Dictionary<PropertyInfo, XmlPropertyInfo> PropertyInfos { get; } = new();
    internal static XmlPropertyInfo DefaultXmlPropertyInfo { get; } = new();
    internal Func<T>? Constructor { get; private set; }
    internal Func<Dictionary<PropertyInfo, object?>, T>? FactoryMethod { get; private set; }

    public XmlTypeInfo<T> ForProperty<V>(Expression<Func<T, V>> expression, Action<XmlPropertyInfo> action)
    {
      PropertyInfo propertyInfo = XmlTypeInfo<T>.ExtractPropertyInfoFromExpression(expression);
      XmlPropertyInfo xpi = PropertyInfos.GetOrAdd(propertyInfo, () => new XmlPropertyInfo());
      action(xpi);
      return this;
    }

    public XmlTypeInfo<T> WithFactoryMethod(Func<Dictionary<PropertyInfo, object?>, T> factoryMethod)
    {
      EAssert.Argument.IsNotNull(factoryMethod, nameof(factoryMethod));
      EAssert.IsNull(this.Constructor, $"Unable to set {nameof(FactoryMethod)} if {nameof(Constructor)} is not null.");
      this.FactoryMethod = factoryMethod;
      return this;
    }

    public XmlTypeInfo<T> WithConstructor(Func<T> constructor)
    {
      EAssert.Argument.IsNotNull(constructor, nameof(constructor));
      EAssert.IsNull(this.FactoryMethod, $"Unable to set {nameof(Constructor)} if {nameof(FactoryMethod)} is not null.");
      this.Constructor = constructor;
      return this;
    }

    private static PropertyInfo ExtractPropertyInfoFromExpression<V>(Expression<Func<T, V>> expression)
    {
      if (expression.Body is MemberExpression memberExpression && memberExpression.Member is PropertyInfo propertyInfo)
      {
        return propertyInfo;
      }
      else
        throw new ArgumentException("The provided expression is not a valid property expression.");
    }
  }
}
