using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.BasicSerialization.Serializers
{
  public class NumberSerializer : IElementSerializer, IAttributeSerializer
  {
    public bool AcceptsType(Type type)
    {
      if (type == typeof(double)) return true;
      if (type == typeof(int)) return true;
      if (type == typeof(long)) return true;
      if (type == typeof(float)) return true;
      if (type == typeof(decimal)) return true;
      if (type == typeof(short)) return true;
      if (type == typeof(byte)) return true;
      if (type == typeof(uint)) return true;
      if (type == typeof(ulong)) return true;
      if (type == typeof(ushort)) return true;
      if (type == typeof(sbyte)) return true;
      return false;
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
