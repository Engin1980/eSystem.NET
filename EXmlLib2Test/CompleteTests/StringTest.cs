using EXmlLib2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2Test.CompleteTests
{
  [TestFixture]
  internal class StringTest
  {
    [Test]
    public void StringSerializationTest()
    {
      string source = "Hello, World!";

      EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      string dest = exml.Deserialize<string>(root) ?? "?-><>";

      Assert.That(source, Is.EqualTo(dest));
    }

    [Test]
    public void NullableStringSerializationTest()
    {
      string? source = "Hello, World!";

      EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      string? dest = exml.Deserialize<string?>(root);

      Assert.That(source == null && dest == null || source == dest);
    }

    [Test]
    public void NullableNullStringSerializationTest()
    {
      string? source = null;

      EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      System.Diagnostics.Debug.WriteLine(root);

      string? dest = exml.Deserialize<string?>(root);

      Assert.Multiple(() =>
      {
        Assert.That(source, Is.Null);
        Assert.That(dest, Is.Null);
      });
    }
  }
}
