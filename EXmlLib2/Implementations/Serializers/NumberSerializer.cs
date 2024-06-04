using ESystem.Asserting;
using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class NumberSerializer : IElementSerializer, IAttributeSerializer
  {

    public bool AcceptsValue(object? value)
    {
      if (value == null) return false;
      if (value is double) return true;
      if (value is not IConvertible) return false;
      try
      {
        Convert.ToDouble(value);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      string s = SerializeToString(value, ctx);
      element.Value = s;
    }

    private string SerializeToString(object? value, IXmlContext ctx)
    {
      EAssert.Argument.IsNotNull(value, nameof(value));
      CultureInfo ci = ctx.DefaultCultureInfo;
      string ret = Convert.ToString(value, ci) ?? throw new EXmlException(this, "Unexpected null returned from number->string conversion.");
      return ret;
    }

    public string Serialize(object? value, IXmlContext ctx)
    {
      string s = SerializeToString(value, ctx);
      return s;
    }
  }
}
