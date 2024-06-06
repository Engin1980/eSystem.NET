﻿using EXmlLib2;
using EXmlLib2.Implementations.Deserializers;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2Test.Model
{
  internal class SimpleClass
  {
    public enum YesNoEnum
    {
      Yes,
      No
    }

    public YesNoEnum? YesNoNullable { get; set; } = YesNoEnum.Yes;
    public YesNoEnum YesNo { get; set; }
    public bool RenamedBool { get; set; }
    public int Int { get; set; }
    public double Double { get; set; }
    public double DoubleAttribute { get; set; }
    public double? NullDouble { get; set; }
    public string String { get; set; } = string.Empty;
    public string? StringOptional { get; set; } = null;
    public string? StringIgnored { get; set; } = null;
    public static void AdjustEXml(EXml exml)
    {
      XmlTypeInfo<SimpleClass> xti = new();
      xti.ForProperty(q => q.StringOptional, q =>
      {
        q.Representation = EXmlLib2.Types.XmlRepresentation.Element;
        q.Obligation = EXmlLib2.Types.XmlObligation.Optional;
      })
        .ForProperty(q => q.StringIgnored, q => q.Obligation = EXmlLib2.Types.XmlObligation.Ignored)
        .ForProperty(q => q.DoubleAttribute, q => q.Representation = EXmlLib2.Types.XmlRepresentation.Attribute)
        .ForProperty(q => q.RenamedBool, q => q.XmlName = "CustomBoolName");

      TypeElementSerializer<SimpleClass> tes = new(xti);
      exml.InsertSerializer(0, tes);

      TypeElementDeserializer<SimpleClass> tds = new(xti);
      exml.InsertDeserializer(0, tds);
    }

    public override bool Equals(object? obj)
    {
      return obj is SimpleClass @class &&
        YesNoNullable == @class.YesNoNullable &&
        YesNo == @class.YesNo &&
        RenamedBool == @class.RenamedBool &&
        Int == @class.Int &&
        Double == @class.Double &&
        DoubleAttribute == @class.DoubleAttribute &&
        NullDouble == @class.NullDouble &&
        String == @class.String &&
        StringOptional == @class.StringOptional &&
        StringIgnored == @class.StringIgnored;
    }
  }
}
