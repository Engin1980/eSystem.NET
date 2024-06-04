using EXmlLib2;
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
    public void StringToElement()
    {
      string value = "Liška";

      XElement root = new("Root");
      exml.Serialize(value, root);

      string exp = "<Root>Liška</Root>";
      string act = root.ToString();
      Assert.That(act, Is.EqualTo(exp));
    }
  }
}