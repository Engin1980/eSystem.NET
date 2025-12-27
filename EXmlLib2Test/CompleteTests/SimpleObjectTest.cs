using ESystem.Logging;
using EXmlLib2.Implementations.Deserializers;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Types;
using FluentAssertions;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2Test.CompleteTests
{
  public class SimpleClass
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
  }

  [TestFixture]
  internal class SimpleObjectTests
  {
    [SetUp]
    public void Setup()
    {
      ESystem.Logging.Logger.RegisterLogAction(li =>
      {
        System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} - {li.Level} - {li.Sender}: {li.Message}");
      },
      [new LogRule(".+", LogLevel.DEBUG)]);
    }

    [Test]
    public void SimpleClassToElementsTest()
    {
      SimpleClass source = new SimpleClass()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };

      var exml = EXmlLib2.EXml.CreateDefault();
      exml.ElementSerializers.Push(new TypeElementSerializer<SimpleClass>());
      exml.ElementDeserializers.Push(new TypeElementDeserializer<SimpleClass>());

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleClass? dest = exml.Deserialize<SimpleClass>(root);

      source.Should().BeEquivalentTo(dest);
    }

    [Test]
    public void SimpleClassToAttributesTest()
    {
      SimpleClass source = new SimpleClass()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };

      var exml = EXmlLib2.EXml.CreateDefault();
      var xti = new XmlTypeInfo<SimpleClass>();
      xti.DefaultXmlPropertyInfo.Representation = XmlRepresentation.Attribute;
      exml.ElementSerializers.Push(new TypeElementSerializer<SimpleClass>(xti));
      exml.ElementDeserializers.Push(new TypeElementDeserializer<SimpleClass>(xti));

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleClass? dest = exml.Deserialize<SimpleClass>(root);

      source.Should().BeEquivalentTo(dest);
    }
  }
}
