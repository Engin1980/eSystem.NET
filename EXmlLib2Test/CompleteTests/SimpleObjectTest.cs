using ESystem.Logging;
using EXmlLib2;
using EXmlLib2.Abstractions.Abstracts;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties;
using EXmlLib2.Types;
using FluentAssertions;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

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

      EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithCommonTypesSerialization();
      exml.ElementSerializers.AddFirst(new ObjectSerializer().WithAcceptedType<SimpleClass>());
      exml.ElementDeserializers.AddFirst(new ObjectDeserializer().WithAcceptedType<SimpleClass>());

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

      EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithCommonTypesSerialization();
      exml.ElementSerializers.AddFirst(new ObjectSerializer().WithAcceptedType<SimpleStructWithCtor>());
      exml.ElementDeserializers.AddFirst(new ObjectDeserializer().WithAcceptedType<SimpleStructWithCtor>());

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

      EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithCommonTypesSerialization();
      exml.ElementDeserializers.AddFirst(new ObjectDeserializer()
        .WithAcceptedType<SimpleStructNoCtor>()
        .WithTypeOptions<SimpleStructNoCtor>(opt => opt.WithInstanceFactory(props =>
        {
          return new SimpleStructNoCtor()
          {
            Id = props.GetPropertyValue(q => q.Id)!,
            Name = props.GetPropertyValue(q => q.Name)!,
            CreatedAt = props.GetPropertyValue(q => q.CreatedAt)!
          };
        })));
      exml.ElementSerializers.AddFirst(new ObjectSerializer().WithAcceptedType<SimpleStructNoCtor>());

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleStructNoCtor? dest = exml.Deserialize<SimpleStructNoCtor>(root);

      source.Should().BeEquivalentTo(dest);
    }

    [Test]
    public void SimpleClassToElementsWithoutCustomSerializerTest()
    {
      SimpleClass source = new SimpleClass()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };

      EXml exml = EXml.Create()
        .WithPrimitiveTypesAndStringSerialization()
        .WithCommonTypesSerialization()
        .WithObjectSerialization();

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleClass? dest = exml.Deserialize<SimpleClass>(root);

      source.Should().BeEquivalentTo(dest);
    }

    [Test]
    public void SimpleStructWithCtorToElementsWithoutCustomSerializerTest()
    {
      SimpleStructWithCtor source = new SimpleStructWithCtor()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };

      EXml exml = EXml.Create().WithDefaultSerialization();

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleStructWithCtor? dest = exml.Deserialize<SimpleStructWithCtor>(root);

      source.Should().BeEquivalentTo(dest);
    }

    [Test]
    public void SimpleClassToPropetiesAsAttributesTest()
    {
      SimpleClass source = new SimpleClass()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };

      EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithCommonTypesSerialization();
      exml.ElementSerializers.AddFirst(new ObjectSerializer()
        .WithAcceptedType<SimpleClass>()
        .WithDefaultOptions(opts =>
          opts.WithPropertySerializer(new PropertySerialization().WithXmlSourceOrder(XmlSourceOrder.AttributeOnly))));
      exml.ElementDeserializers.AddFirst(new ObjectDeserializer()
        .WithAcceptedType<SimpleClass>()
        .WithDefaultOptions(opts =>
          opts.WithPropertyDeserializer(new PropertySerialization().WithXmlSourceOrder(XmlSourceOrder.AttributeOnly))));

      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleClass? dest = exml.Deserialize<SimpleClass>(root);

      root.Attribute("Id").Should().NotBeNull().And.Subject.Value.Should().Be(11.ToString());
      root.Attribute("Name").Should().NotBeNull().And.Subject.Value.Should().Be("Test Object");
      root.Attribute("CreatedAt").Should().NotBeNull();
      source.Should().BeEquivalentTo(dest);
    }

    [Test]
    public void IgnoredPropertyNotSerializedTest()
    {
      SimpleClass source = new SimpleClass()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };
      SimpleClass? target;

      XElement element = new("Root");

      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithCommonTypesSerialization();
        exml.ElementSerializers.AddFirst(new ObjectSerializer()
          .WithAcceptedType<SimpleClass>()
          .WithTypeOptions<SimpleClass>(opts => opts.WithIgnoredProperty(q => q.CreatedAt)));

        exml.Serialize(source, element);
      }
      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithCommonTypesSerialization();
        exml.ElementDeserializers.AddFirst(new ObjectDeserializer()
          .WithAcceptedType<SimpleClass>()
          .WithTypeOptions<SimpleClass>(opts => opts.WithIgnoredProperty(q => q.CreatedAt)));
        target = exml.Deserialize<SimpleClass>(element);
      }

      element.XPathSelectElement("./CreatedAt").Should().BeNull();
      target.Should().NotBeNull();
      target.Should().NotBeEquivalentTo(source);
      target.Id.Should().Be(source.Id);
      target.Name.Should().Be(source.Name);
      target.CreatedAt.Should().NotBe(source.CreatedAt);
    }

    [Test]
    public void IgnoredPropertyNotDeserializedTest()
    {
      SimpleClass source = new SimpleClass()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };
      SimpleClass? target;

      XElement element = new("Root");

      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithCommonTypesSerialization();
        exml.ElementSerializers.AddFirst(new ObjectSerializer()
          .WithAcceptedType<SimpleClass>());
        exml.Serialize(source, element);
      }
      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithCommonTypesSerialization();
        exml.ElementDeserializers.AddFirst(new ObjectDeserializer()
          .WithAcceptedType<SimpleClass>()
          .WithTypeOptions<SimpleClass>(opt => opt
            .WithIgnoredProperty(q => q.CreatedAt)));
        target = exml.Deserialize<SimpleClass>(element);
      }

      element.XPathSelectElement("./CreatedAt").Should().NotBeNull();
      target.Should().NotBeNull();
      target.Should().NotBeEquivalentTo(source);
      target.Id.Should().Be(source.Id);
      target.Name.Should().Be(source.Name);
      target.CreatedAt.Should().NotBe(source.CreatedAt);
    }
  }
}
