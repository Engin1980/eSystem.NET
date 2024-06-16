using EXmlLib2;
using EXmlLib2Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2Test.SerializationTests
{
  internal class EnumerablesTests
  {
    [Test]
    public void Array()
    {
      int[] obj = 
      {
        1,2,3
      };

      EXml exml = EXml.CreateDefault();
      XElement root = new XElement(XName.Get("root"));
      exml.Serialize(obj, root);

      string exp = "<root>\r\n  <item>1</item>\r\n  <item>2</item>\r\n  <item>3</item>\r\n</root>";
      string act = root.ToString();

      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void List()
    {
      List<int> obj = new List<int>()
      {
        1,2,3
      };

      EXml exml = EXml.CreateDefault();
      XElement root = new XElement(XName.Get("root"));
      exml.Serialize(obj, root);

      string exp = "<root>\r\n  <item>1</item>\r\n  <item>2</item>\r\n  <item>3</item>\r\n</root>";
      string act = root.ToString();

      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void PropertyEnumerates()
    {
      SimpleListBox obj = new();
      obj.Numbers = new List<int> { 1, 2, 3 };
      obj.Texts = new List<string> { "a", "b", "c" };

      EXml exml = EXml.CreateDefault();
      XElement root = new XElement(XName.Get("root"));
      exml.Serialize(obj, root);

      string exp = "<root>\r\n  <Numbers>\r\n    <item>1</item>\r\n    <item>2</item>\r\n    <item>3</item>\r\n  </Numbers>\r\n  <Texts>\r\n    <item>a</item>\r\n    <item>b</item>\r\n    <item>c</item>\r\n  </Texts>\r\n</root>";
      string act = root.ToString();

      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void PropertyInheritesEnumerates()
    {
      InheriteListBox obj = new();
      obj.Values = new();
      obj.Values.Add(new B() { PropertyA = 8, PropertyB = 10 });
      obj.Values.Add(new C() { PropertyA = 8, PropertyC = 33 });

      EXml exml = EXml.CreateDefault();
      XElement root = new XElement(XName.Get("root"));
      exml.Serialize(obj, root);

      string exp = "<root>\r\n  <Values>\r\n    <item __type=\"EXmlLib2Test.Model.B, EXmlLib2Test\">\r\n      <PropertyB>10</PropertyB>\r\n      <PropertyA>8</PropertyA>\r\n    </item>\r\n    <item __type=\"EXmlLib2Test.Model.C, EXmlLib2Test\">\r\n      <PropertyC>33</PropertyC>\r\n      <PropertyA>8</PropertyA>\r\n    </item>\r\n  </Values>\r\n</root>";
      string act = root.ToString();

      Assert.That(act, Is.EqualTo(exp));
    }
  }
}
