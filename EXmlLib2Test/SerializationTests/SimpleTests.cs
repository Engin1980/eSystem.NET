using EXmlLib2;
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
    public void ObjectToElement()
    {
      SimpleClass c = new()
      {
        Double = 5,
        DoubleAttribute = 8.4,
        Int = -44,
        NullDouble = null,
        String = "str",
        StringIgnored = "strIgnored",
        StringOptional = "strOptional"
      };

      XElement root = new XElement("Root");
      exml.Serialize(c, root);

      string exp = "<Root DoubleAttribute=\"8.4\">\r\n  <Int>-44</Int>\r\n  <Double>5</Double>\r\n  <NullDouble />\r\n  <String>str</String>\r\n  <StringOptional>strOptional</StringOptional>\r\n</Root>";
      string act = root.ToString();
      Assert.That(act, Is.EqualTo(exp));
    }
  }
}