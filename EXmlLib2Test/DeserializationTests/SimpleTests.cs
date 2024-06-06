using EXmlLib2;
using EXmlLib2Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2Test.DeserializationTests
{
  [TestFixture]
  public class SimpleTests
  {
    private EXml exml = null!;
    [SetUp]
    public void Setup()
    {
      exml = EXml.CreateDefault();
      SimpleClass.AdjustEXml(this.exml);
      PropertyPlayClass.AdjustEXml(this.exml);
    }

    [Test]
    public void ElementToInt()
    {
      string s = "<Root>1</Root>";
      XElement root = XElement.Parse(s);

      int act = exml.Deserialize<int>(root);
      int exp = 1;

      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void ElementToString()
    {
      string s = "<Root>Liška</Root>";


      XElement root = XElement.Parse(s);
      string? act = exml.Deserialize<string>(root);
      string exp = "Liška";
      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void ElementToSimpleClass()
    {
      string s = "<Root>\r\n  <YesNoNull>(# null #)</YesNoNull>\r\n  <YesNo>No</YesNo>\r\n  <Int>1</Int>\r\n  <Double>123</Double>\r\n  <DoubleNull>(# null #)</DoubleNull>\r\n  <DoubleNan>NaN</DoubleNan>\r\n  <TrueFalse>True</TrueFalse>\r\n  <TrueFalseNull>(# null #)</TrueFalseNull>\r\n</Root>";

      SimpleClass exp = new()
      {
        YesNoNull = null,
        YesNo = SimpleClass.YesNoEnum.No,
        Int = 1,
        Double = 123,
        DoubleNan = double.NaN,
        DoubleNull = null,
        TrueFalse = true,
        TrueFalseNull = null
      };
      XElement root = XElement.Parse(s);

      SimpleClass? act = (SimpleClass?)exml.Deserialize(root, typeof(SimpleClass));
      Utils.CompareProperties(exp, act);
    }

    [Test]
    public void ElementToPropertyPlayClass()
    {
      string s = "<Root StringAttribute=\"attrib\">\r\n  <CustomStringName>stringRenamed</CustomStringName>\r\n  <String>string</String>\r\n  <StringIgnored>nenene</StringIgnored>\r\n  <StringOptional>dudlajda</StringOptional>\r\n</Root>";

      PropertyPlayClass exp = new()
      {
        StringAttribute = "attrib",
        StringRenamed = "stringRenamed",
        String = "string",
        StringOptional = "dudlajda",
      };
      XElement root = XElement.Parse(s);

      PropertyPlayClass? act = exml.Deserialize<PropertyPlayClass>(root);
      Utils.CompareProperties(exp, act);
    }
  }
}
