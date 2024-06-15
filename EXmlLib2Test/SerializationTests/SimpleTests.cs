using EXmlLib2;
using EXmlLib2.Implementations.Deserializers;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Types;
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
    public void InheritedPropertyAsAttributeTest()
    {
      exml.AddSerializer(new TypeElementSerializer<InheritedPropertyTest>());
      exml.AddSerializer(new TypeElementSerializer<PropertyParent>());
      exml.AddSerializer(new TypeElementSerializer<PropertyChild>());

      var obj = new InheritedPropertyTest()
      {
        ParentParent = new PropertyParent(),
        ParentChild = new PropertyChild(),
        PropertyParentNull = null
      };

      XElement root = new XElement("Root");
      exml.Serialize(obj, root);

      string exp = "<Root>\r\n  <ParentParent>\r\n    <Int>22</Int>\r\n  </ParentParent>\r\n  <ParentChild __type=\"EXmlLib2Test.Model.PropertyChild, EXmlLib2Test\">\r\n    <OtherInt>33</OtherInt>\r\n    <Int>22</Int>\r\n  </ParentChild>\r\n  <PropertyParentNull>(# null #)</PropertyParentNull>\r\n</Root>";
      string act = root.ToString();

      Assert.That(act, Is.EqualTo(exp));
    }

    [Test]
    public void InheritedPropertyAsDifferentNameElementTest()
    {
      EXml exml = EXml.CreateDefault();

      XmlTypeInfo<InheritedPropertyTest> xti = new XmlTypeInfo<InheritedPropertyTest>()
        .ForProperty(q => q.ParentChild, q =>
        {
          q.XmlNameByType[typeof(PropertyParent)] = nameof(PropertyParent);
          q.XmlNameByType[typeof(PropertyChild)] = nameof(PropertyChild);
        });
      exml.AddSerializer(new TypeElementSerializer<InheritedPropertyTest>(xti));
      exml.AddSerializer(new TypeElementSerializer<PropertyParent>());
      exml.AddSerializer(new TypeElementSerializer<PropertyChild>());

      var obj = new InheritedPropertyTest()
      {
        ParentParent = new PropertyParent(),
        ParentChild = new PropertyChild(),
        PropertyParentNull = null
      };

      XElement root = new XElement("Root");
      exml.Serialize(obj, root);

      string exp = "<Root>\r\n  <ParentParent>\r\n    <Int>22</Int>\r\n  </ParentParent>\r\n  <PropertyChild>\r\n    <OtherInt>33</OtherInt>\r\n    <Int>22</Int>\r\n  </PropertyChild>\r\n  <PropertyParentNull>(# null #)</PropertyParentNull>\r\n</Root>";
      string act = root.ToString();

      Assert.That(act, Is.EqualTo(exp));
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

    [Test]
    public void PropertyEnumeratesToElement()
    {
      SimpleListBox obj = new();
      obj.Numbers = new List<int> { 1, 2, 3 };
      obj.Texts = new List<string> { "a", "b", "c" };

      EXml exml = EXml.CreateDefault();
      XElement root = new XElement(XName.Get("root"));
      exml.Serialize(obj, root);

      string exp = "";
      string act = root.ToString();

      Assert.That(act, Is.EqualTo(exp));
    }
  }
}