using EXmlLib2;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2Test.CompleteTests
{
  [TestFixture]
  internal class SimpleArrayAndListTests
  {

    [Test]
    public void IntArrayTest()
    {
      int[] source = { 1, 2, 3 };
      int[]? target = null;

      XElement element = new XElement("Root");

      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
        exml.ElementSerializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArraySerializer());

        exml.Serialize(source, element);
      }
      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
        exml.ElementDeserializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArrayDeserializer());
        target = exml.Deserialize<int[]>(element);
      }

      Assert.That(target, Is.Not.Null);
      target.Should().BeEquivalentTo(source);
    }

    [Test]
    public void DateTimeArrayTest()
    {
      DateTime[] source = { DateTime.Now, DateTime.Now.AddMinutes(1), DateTime.Now.AddHours(1) };
      DateTime[]? target = null;

      XElement element = new XElement("Root");

      {
        EXml exml = EXml.Create().WithCommonTypesSerializers();
        exml.ElementSerializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArraySerializer());

        exml.Serialize(source, element);
      }
      {
        EXml exml = EXml.Create().WithCommonTypesDeserializers();
        exml.ElementDeserializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArrayDeserializer());
        target = exml.Deserialize<DateTime[]>(element);
      }

      Assert.That(target, Is.Not.Null);
      target.Should().BeEquivalentTo(source);
    }

    [Test]
    public void IntDoubleMixedArrayTest()
    {
      object[] source = { 1, 2.5, 3 };
      object[]? target = null;

      XElement element = new XElement("Root");

      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithObjectSerialization();
        exml.ElementSerializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArraySerializer()
          .WithTypeMappingOptions(opt => opt.With<int>("int").With<double>("double")));

        exml.Serialize(source, element);
      }
      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithObjectSerialization();
        exml.ElementDeserializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArrayDeserializer()
          .WithTypeMappingOptions(opt => opt.With<int>("int").With<double>("double")));
        target = exml.Deserialize<object[]>(element);
      }

      string expectedXml = @"
        <Root>
          <int>1</int>
          <double>2.5</double>
          <int>3</int>
        </Root>";

      // 3. Assert - Převedeme string na XDocument/XElement a porovnáme
      element.Should().BeEquivalentTo(XElement.Parse(expectedXml));
      Assert.That(target, Is.Not.Null);
      target.Should().BeEquivalentTo(source);
    }


    [Test]
    public void IntListTest()
    {
      List<int> source = [ 1, 2, 3 ];
      List<int>? target = null;

      XElement element = new XElement("Root");

      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
        exml.ElementSerializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArraySerializer());

        exml.Serialize(source, element);
      }
      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
        exml.ElementDeserializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArrayDeserializer());
        target = exml.Deserialize<List<int>>(element);
      }

      Assert.That(target, Is.Not.Null);
      target.Should().BeEquivalentTo(source);
    }

    [Test]
    public void DateTimeListTest()
    {
      List<DateTime> source = [ DateTime.Now, DateTime.Now.AddMinutes(1), DateTime.Now.AddHours(1) ];
      List<DateTime>? target = null;

      XElement element = new XElement("Root");

      {
        EXml exml = EXml.Create().WithCommonTypesSerializers();
        exml.ElementSerializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArraySerializer());

        exml.Serialize(source, element);
      }
      {
        EXml exml = EXml.Create().WithCommonTypesDeserializers();
        exml.ElementDeserializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArrayDeserializer());
        target = exml.Deserialize<List<DateTime>>(element);
      }

      Assert.That(target, Is.Not.Null);
      target.Should().BeEquivalentTo(source);
    }

    [Test]
    public void IntDoubleMixedListTest()
    {
      List<object> source = [ 1, 2.5, 3 ];
      List<object>? target = null;

      XElement element = new XElement("Root");

      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithObjectSerialization();
        exml.ElementSerializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArraySerializer()
          .WithTypeMappingOptions(opt => opt.With<int>("int").With<double>("double")));

        exml.Serialize(source, element);
      }
      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization().WithObjectSerialization();
        exml.ElementDeserializers.AddFirst(new EXmlLib2.Implementations.EnumerableSerialization.ListAndArrayDeserializer()
          .WithTypeMappingOptions(opt => opt.With<int>("int").With<double>("double")));
        target = exml.Deserialize<List<object>>(element);
      }

      string expectedXml = @"
        <Root>
          <int>1</int>
          <double>2.5</double>
          <int>3</int>
        </Root>";

      // 3. Assert - Převedeme string na XDocument/XElement a porovnáme
      element.Should().BeEquivalentTo(XElement.Parse(expectedXml));
      Assert.That(target, Is.Not.Null);
      target.Should().BeEquivalentTo(source);
    }
  }
}
