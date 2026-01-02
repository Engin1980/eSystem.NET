using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2Test.CompleteTests;

[TestFixture]
internal class NumberTest
{
  [Test]
  public void IntegerSerializationTest()
  {
    int source = 67;

    var exml = EXmlLib2.EXml.CreateDefault();
    XElement root = new XElement("Root");
    exml.Serialize(source, root);

    int dest = exml.Deserialize<int>(root);

    Assert.That(source, Is.EqualTo(dest));
  }

  [Test]
  public void NullableIntSerializationTest()
  {
    int? source = 68;

    var exml = EXmlLib2.EXml.CreateDefault();
    XElement root = new XElement("Root");
    exml.Serialize(source, root);

    int? dest = exml.Deserialize<int?>(root);

    Assert.That(source == null && dest == null || source == dest);
  }

  [Test]
  public void NullableNullIntSerializationTest()
  {
    int? source = null;

    var exml = EXmlLib2.EXml.CreateDefault();
    XElement root = new XElement("Root");
    exml.Serialize(source, root);

    System.Diagnostics.Debug.WriteLine(root);

    int? dest = exml.Deserialize<int?>(root);

    Assert.Multiple(() =>
    {
      Assert.That(source, Is.Null);
      Assert.That(dest, Is.Null);
    });
  }

  [Test]
  public void DoubleSerializationTest()
  {
    double source = Math.PI;

    var exml = EXmlLib2.EXml.CreateDefault();
    XElement root = new XElement("Root");
    exml.Serialize(source, root);

    double dest = exml.Deserialize<double>(root);

    Assert.That(source, Is.EqualTo(dest));
  }

  [Test]
  public void NullableDoubleSerializationTest()
  {
    double? source = Math.E;

    var exml = EXmlLib2.EXml.CreateDefault();
    XElement root = new XElement("Root");

    exml.Serialize(source, root);

    double? dest = exml.Deserialize<double?>(root);

    Assert.That(source == null && dest == null || source == dest);
  }

  [Test]
  public void NullableNullDoubleSerializationTest()
  {
    double? source = null;

    var exml = EXmlLib2.EXml.CreateDefault();
    XElement root = new XElement("Root");
    exml.Serialize(source, root);

    double? dest = exml.Deserialize<double?>(root);

    Assert.Multiple(() =>
    {
      Assert.That(source, Is.Null);
      Assert.That(dest, Is.Null);
    });
  }
}
