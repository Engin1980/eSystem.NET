using EXmlLib2;
using EXmlLib2.Implementations.BasicSerialization.Deserializers;
using EXmlLib2.Implementations.BasicSerialization.Serializers;
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

    public YesNoEnum? YesNoNull { get; set; } = YesNoEnum.Yes;
    public YesNoEnum YesNo { get; set; }
    public int Int { get; set; }
    public double Double { get; set; }
    public double? DoubleNull { get; set; } = double.MaxValue;
    public double? DoubleNan { get; set; } = double.NaN;
    public bool TrueFalse { get; set; }
    public bool? TrueFalseNull { get; set; } = false;

    public static void AdjustEXml(EXml exml)
    {
      var xti = XmlTypeInfo<SimpleClass>.Create();

      SpecificTypeElementSerializer<SimpleClass> tes = new(xti);
      exml.ElementSerializers.AddFirst(tes);

      SpecificTypeElementDeserializer<SimpleClass> tds = new(xti);
      exml.ElementDeserializers.AddFirst(tds);
    }

    public override bool Equals(object? obj)
    {
      return obj is SimpleClass @class &&
             YesNoNull == @class.YesNoNull &&
             YesNo == @class.YesNo &&
             Int == @class.Int &&
             Double == @class.Double &&
             DoubleNull == @class.DoubleNull &&
             DoubleNan == @class.DoubleNan &&
             TrueFalse == @class.TrueFalse &&
             TrueFalseNull == @class.TrueFalseNull;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
