using EXmlLib2;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2Test.Model;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;

namespace EXmlLib2Test.SerializationTests
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
    public void IntToElement()
    {
      int value = 1;

      XElement root = new("Root");
      exml.Serialize(value, root);

      string exp = "<Root>1</Root>";
      string act = root.ToString();
      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void NullIntToElement()
    {
      int? value = null;

      XElement root = new("Root");
      exml.Serialize(value, root);

      string exp = $"<Root>{exml.DefaultNullString}</Root>";
      string act = root.ToString();
      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void StringToElement()
    {
      string value = "Liška";

      XElement root = new("Root");
      exml.Serialize(value, root);

      string exp = "<Root>Liška</Root>";
      string act = root.ToString();
      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void SimpleClassToElement()
    {
      SimpleClass c = new()
      {
        Double = 123,
        DoubleNan = double.NaN,
        DoubleNull = null,
        Int = 1,
        TrueFalse = true,
        TrueFalseNull = null,
        YesNo = SimpleClass.YesNoEnum.No,
        YesNoNull = null
      };

      XElement root = new XElement("Root");
      exml.Serialize(c, root);

      string exp = "<Root>\r\n  <YesNoNull>(# null #)</YesNoNull>\r\n  <YesNo>No</YesNo>\r\n  <Int>1</Int>\r\n  <Double>123</Double>\r\n  <DoubleNull>(# null #)</DoubleNull>\r\n  <DoubleNan>NaN</DoubleNan>\r\n  <TrueFalse>True</TrueFalse>\r\n  <TrueFalseNull>(# null #)</TrueFalseNull>\r\n</Root>";
      string act = root.ToString();
      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void InheritedPropertyTest()
    {
      exml.AddSerializer(new TypeElementSerializer<InheritedPropertyTest>());

      XElement root = new XElement("Root");
      exml.Serialize(r
    }

    [Test]
    public void PropertyPlayClassToElement()
    {
      PropertyPlayClass c = new()
      {
        String = "string",
        StringAttribute = "stringAttribute",
        StringOptional = "stringOptional",
        StringRenamed = "stringRenamed"
      };

      XElement root = new XElement("Root");
      exml.Serialize(c, root);

      string exp = "<Root StringAttribute=\"stringAttribute\">\r\n  <CustomStringName>stringRenamed</CustomStringName>\r\n  <String>string</String>\r\n  <StringOptional>stringOptional</StringOptional>\r\n  <StringOptionalTwo>OptionalTwo</StringOptionalTwo>\r\n</Root>";
      string act = root.ToString();
      Assert.That(act, Is.EqualTo(exp));
    }
  }
}