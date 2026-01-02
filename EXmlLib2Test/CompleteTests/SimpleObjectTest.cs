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

  public struct SimpleStructNoCtor
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
  }

  public struct SimpleStructWithCtor
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public SimpleStructWithCtor(int id, string name, DateTime createdAt)
    {
      Id = id;
      Name = name;
      CreatedAt = createdAt;
    }

    public SimpleStructWithCtor()
    {
    }
  }

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
      exml.ElementSerializers.Push(new SpecificTypeElementSerializer<SimpleClass>());
      exml.ElementDeserializers.Push(new SpecificTypeElementDeserializer<SimpleClass>());

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleClass? dest = exml.Deserialize<SimpleClass>(root);

      source.Should().BeEquivalentTo(dest);
    }

    [Test]
    public void SimpleStructWithCtorToElementsTest()
    {
      SimpleStructWithCtor source = new SimpleStructWithCtor()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };

      var exml = EXmlLib2.EXml.CreateDefault();
      exml.ElementSerializers.Push(new SpecificTypeElementSerializer<SimpleStructWithCtor>());
      exml.ElementDeserializers.Push(new SpecificTypeElementDeserializer<SimpleStructWithCtor>());

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleStructWithCtor? dest = exml.Deserialize<SimpleStructWithCtor>(root);

      source.Should().BeEquivalentTo(dest);
    }

    [Test]
    public void SimpleStructNoCtorToElementsTest()
    {
      SimpleStructNoCtor source = new SimpleStructNoCtor()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };

      XmlTypeInfo<SimpleStructNoCtor> xti = new XmlTypeInfo<SimpleStructNoCtor>()
      .WithFactoryMethod(props =>
      {
        return new SimpleStructNoCtor()
        {
          Id = props.GetPropertyValue(q => q.Id)!,
          Name = props.GetPropertyValue(q => q.Name)!,
          CreatedAt = props.GetPropertyValue(q => q.CreatedAt)!
        };
      });

      var exml = EXmlLib2.EXml.CreateDefault();
      exml.ElementSerializers.Push(new SpecificTypeElementSerializer<SimpleStructNoCtor>(xti));
      exml.ElementDeserializers.Push(new SpecificTypeElementDeserializer<SimpleStructNoCtor>(xti));

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleStructNoCtor? dest = exml.Deserialize<SimpleStructNoCtor>(root);

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
      exml.ElementSerializers.Push(new SpecificTypeElementSerializer<SimpleClass>(xti));
      exml.ElementDeserializers.Push(new SpecificTypeElementDeserializer<SimpleClass>(xti));

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleClass? dest = exml.Deserialize<SimpleClass>(root);

      source.Should().BeEquivalentTo(dest);
    }
  }
}
