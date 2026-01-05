using EXmlLib2;
using EXmlLib2.Implementations.Deserializers;
using EXmlLib2.Implementations.Serializers;
using EXmlLib2.Implementations.Wrappers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2Test.CompleteTests;

[TestFixture]
internal class ObjectByReferenceTest
{
  public class Location
  {
    public string Description { get; set; } = string.Empty;
  }
  public class Item
  {
    public string Title { get; set; } = string.Empty;
    public Location? LocationA { get; set; }
    public Location? LocationB { get; set; }
  }

  [Test]
  public void TestSimpleSave()
  {
    Item source = new()
    {
      Title = "Item 1",
      LocationA = new()
      {
        Description = "Location A"
      },
      LocationB = new()
      {
        Description = "Location B"
      }
    };
    Item? target;

    XElement elm = new("root");

    {
      EXml xml = EXml.Create().WithPrimitiveTypesAndStringSerialization();

      ObjectByReferenceSerializerWrapper<Location> locationSerializer = new(new SpecificTypeElementSerializer<Location>());
      ObjectByReferenceSerializerWrapper<Item> itemSerializer = new(new SpecificTypeElementSerializer<Item>());
      xml.ElementSerializers.AddFirst(locationSerializer);
      xml.ElementSerializers.AddFirst(itemSerializer);

      xml.Serialize(source, elm);
    }

    {
      EXml xml = EXml.Create().WithPrimitiveTypesAndStringSerialization();

      ObjectByReferenceDeserializerWrapper<Location> locationDeserializer = new(new SpecificTypeElementDeserializer<Location>());
      ObjectByReferenceDeserializerWrapper<Item> itemDeserializer = new(new SpecificTypeElementDeserializer<Item>());
      xml.ElementDeserializers.AddFirst(locationDeserializer);
      xml.ElementDeserializers.AddFirst(itemDeserializer);

      target = xml.Deserialize<Item>(elm);
    }

    Assert.That(target, Is.Not.Null);
    Assert.That(target!.Title, Is.EqualTo(source.Title));
  }

  [Test]
  public void TestReferenceSave()
  {
    Location location = new Location() { Description = "Location A" };
    Item source = new() { Title = "Item 1", LocationA = location, LocationB = location };
    Item? target;

    XElement elm = new("root");

    {
      EXml xml = EXml.Create().WithPrimitiveTypesAndStringSerialization();

      ObjectByReferenceSerializerWrapper<Location> locationSerializer = new(new SpecificTypeElementSerializer<Location>());
      ObjectByReferenceSerializerWrapper<Item> itemSerializer = new(new SpecificTypeElementSerializer<Item>());
      xml.ElementSerializers.AddFirst(locationSerializer);
      xml.ElementSerializers.AddFirst(itemSerializer);

      xml.Serialize(source, elm);
    }

    {
      EXml xml = EXml.Create().WithPrimitiveTypesAndStringSerialization();

      ObjectByReferenceDeserializerWrapper<Location> locationDeserializer = new(new SpecificTypeElementDeserializer<Location>());
      ObjectByReferenceDeserializerWrapper<Item> itemDeserializer = new(new SpecificTypeElementDeserializer<Item>());
      xml.ElementDeserializers.AddFirst(locationDeserializer);
      xml.ElementDeserializers.AddFirst(itemDeserializer);

      target = xml.Deserialize<Item>(elm);
    }

    Assert.That(target, Is.Not.Null);
    Assert.That(target.LocationA, Is.SameAs(target.LocationB));
    source.Should().BeEquivalentTo(target);
  }
}