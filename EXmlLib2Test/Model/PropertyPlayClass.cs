﻿using EXmlLib2;
using EXmlLib2.Implementations.Deserializers;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EXmlLib2Test.Model.SimpleClass;

namespace EXmlLib2Test.Model
{
  internal class PropertyPlayClass
  {
    public string StringAttribute { get; set; } = null!;
    public string StringRenamed { get; set; } = null!;
    public string String { get; set; } = null!;
    public string? StringOptional { get; set; } = "Optional";
    public string? StringOptionalTwo { get; set; } = "OptionalTwo";
    public string? StringIgnored { get; set; } = "Ignored";

    public static void AdjustEXml(EXml exml)
    {
      var xti = XmlTypeInfo<PropertyPlayClass>.Create()
        .ForProperty(q => q.StringOptional, q => q.Obligation = XmlObligation.Optional)
        .ForProperty(q => q.StringOptionalTwo, q => q.Obligation = XmlObligation.Optional)
        .ForProperty(q => q.StringIgnored, q => q.Obligation = XmlObligation.Ignored)
        .ForProperty(q => q.StringRenamed, q => q.XmlName = "CustomStringName")
        .ForProperty(q => q.StringAttribute, q => q.Representation = XmlRepresentation.Attribute);

      TypeElementSerializer<PropertyPlayClass> tes = new(xti);
      exml.InsertSerializer(0, tes);

      TypeElementDeserializer<PropertyPlayClass> tds = new(xti);
      exml.InsertDeserializer(0, tds);
    }

    public override bool Equals(object? obj)
    {
      return obj is PropertyPlayClass @class &&
             StringAttribute == @class.StringAttribute &&
             StringRenamed == @class.StringRenamed &&
             String == @class.String &&
             StringOptional == @class.StringOptional &&
             StringIgnored == @class.StringIgnored;
    }
  }
}
