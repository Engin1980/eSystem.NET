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
    public void ObjectToElement()
    {
      string s = "<Root DoubleAttribute=\"8.4\">\r\n  <Int>-44</Int>\r\n  <Double>5</Double>\r\n  <NullDouble />\r\n  <String>str</String>\r\n  <StringOptional>strOptional</StringOptional>\r\n</Root>";

      SimpleClass exp = new()
      {
        Double = 5,
        DoubleAttribute = 8.4,
        Int = -44,
        NullDouble = null,
        String = "str",
        StringIgnored = "strIgnored",
        StringOptional = "strOptional"
      };
      XElement root = XElement.Parse(s);

      SimpleClass? act = (SimpleClass?)exml.Deserialize(root, typeof(SimpleClass));
      Assert.That(act, Is.EqualTo(exp));
    }
  }
}
