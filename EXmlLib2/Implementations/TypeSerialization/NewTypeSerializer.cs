using ESystem.Asserting;
using ESystem.Logging;
using EXmlLib2.Abstractions;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization;

public abstract class NewTypeSerializer : IElementSerializer
{
  public abstract bool AcceptsType(Type type);

  public void Serialize(object? value, XElement element, IXmlContext ctx)
  {
    EAssert.Argument.IsNotNull(value, nameof(value));

    IEnumerable<string> dataMembers = GetDataMemberNames(value.GetType());
    foreach (string dataMember in dataMembers)
    {
      SerializeDataField(value, dataMember, element, ctx);
    }
  }

  protected abstract void SerializeDataField(object value, string dataMemberName, XElement element, IXmlContext ctx);
  protected abstract IEnumerable<string> GetDataMemberNames(Type type);
}
