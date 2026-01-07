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
  public const string TYPE_NAME_ATTRIBUTE = "__instanceType";
  public abstract bool AcceptsType(Type type);

  public void Serialize(object? value, Type expectedType, XElement element, IXmlContext ctx)
  {
    EAssert.Argument.IsNotNull(value, nameof(value));

    IEnumerable<string> dataMembers = GetDataMemberNames(value.GetType());
    foreach (string dataMember in dataMembers)
    {
      SerializeDataMember(value, dataMember, element, ctx);
    }

    if (value != null && value.GetType() != expectedType)
    {
      string typeName = value.GetType().FullName!;
      if (typeName.StartsWith("System.") == false)  // if not mscorlib type, assembly name is required
        typeName += ", " + value.GetType().Assembly.GetName().Name;
      element.SetAttributeValue(XName.Get(TYPE_NAME_ATTRIBUTE), typeName);
    }
  }

  protected abstract void SerializeDataMember(object value, string dataMemberName, XElement element, IXmlContext ctx);
  protected abstract IEnumerable<string> GetDataMemberNames(Type type);
}
