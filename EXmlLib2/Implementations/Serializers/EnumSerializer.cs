﻿using EXmlLib2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.Serializers
{
  public class EnumSerializer : IAttributeSerializer, IElementSerializer
  {
    public bool AcceptsValue(object? value) => value != null && value.GetType().IsEnum;

    public void Serialize(object? value, XElement element, IXmlContext ctx)
    {
      element.Value = Serialize(value, ctx);
    }

    public string Serialize(object? value, IXmlContext ctx) => value!.ToString() ?? throw new EXmlException("Enum value must be not nul.");
  }
}
