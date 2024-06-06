using ESystem.Asserting;
using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Deserializers
{
  public class EnumDeserializer : IAttributeDeserializer, IElementDeserializer
  {
    public bool IgnoreCase { get; private set; }
    public bool AcceptsType(Type type) => type.IsEnum;

    public EnumDeserializer(bool ignoreCase = true)
    {
      IgnoreCase = ignoreCase;
    }

    public object? Deserialize(XElement element, Type targetType, IXmlContext ctx)
    {
      EAssert.Argument.IsNotNull(element, nameof(element));
      EAssert.Argument.IsNotNull(targetType, nameof(targetType));
      string s = element.Value;
      object? ret = Deserialize(s, targetType, ctx);
      return ret;
    }

    public object? Deserialize(string value, Type targetType, IXmlContext ctx)
    {
      EAssert.Argument.IsTrue(targetType.IsEnum);
      object ret;
      try
      {
        ret = Enum.Parse(targetType, value, IgnoreCase);
      }
      catch (Exception ex)
      {
        throw new EXmlException($"Unable to parse {value} as value of enum {targetType}.", ex);
      }
      return ret;
    }
  }
}
