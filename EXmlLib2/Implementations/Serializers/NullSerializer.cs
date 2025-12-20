using ESystem.Asserting;
using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class NullSerializer : IAttributeSerializer, IElementSerializer
  {
    public bool AcceptsValue(object? value) => value == null;

    public void Serialize(object? value, XElement element, IXmlContext ctx) => element.Value = Serialize(value, ctx);

    public string Serialize(object? value, IXmlContext ctx)
    {
      EAssert.Argument.IsTrue(value == null, nameof(value), "Null value expected only.");
      return ctx.DefaultNullString;
    }
  }
}
