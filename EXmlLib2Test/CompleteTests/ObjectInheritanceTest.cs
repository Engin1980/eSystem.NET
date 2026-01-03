using EXmlLib2;
using EXmlLib2.Abstractions.Abstracts;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.Deserializers;
using EXmlLib2.Implementations.Serializers;
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
  internal class ObjectInheritanceTest
  {
    public class ParentClass
    {
      public int ParentProperty { get; set; } = 8;
    }
    public class ChildClass : ParentClass
    {
      public int ChildProperty { get; set; } = 16;
    }

    public class HolderClass
    {
      public ParentClass? HeldObjectA { get; set; }
      public ParentClass? HeldObjectB { get; set; }
    }

    [Test]
    public void SimpleInheritanceTest()
    {
      ParentClass source = new ChildClass()
      {
        ParentProperty = 10,
        ChildProperty = 20
      };
      ParentClass? target;
      XElement element = new XElement("Root");

      {
        var exml = EXml.CreateDefault();
        exml.ElementSerializers.Push(new SpecificTypeElementSerializer<ParentClass>(DerivedTypesBehavior.AllowDerivedTypes));
        exml.Serialize(source, element);
      }

      {
        var exml = EXml.CreateDefault();
        exml.ElementDeserializers.Push(new SpecificTypeElementDeserializer<ParentClass>(DerivedTypesBehavior.AllowDerivedTypes));
        target = exml.Deserialize<ParentClass>(element);
      }

      target.Should().BeOfType<ChildClass>();
      target.Should().BeEquivalentTo(source);
    }

    [Test]
    public void InheritanceInPropertyTest()
    {
      HolderClass source = new()
      {
        HeldObjectA = new ChildClass()
        {
          ParentProperty = 10,
          ChildProperty = 20
        },
        HeldObjectB = new ParentClass()
        {
          ParentProperty = 30
        }
      };
      HolderClass? target;
      XElement element = new XElement("Root");

      {
        var exml = EXml.CreateDefault();
        exml.ElementSerializers.Push(new SpecificTypeElementSerializer<ParentClass>(DerivedTypesBehavior.AllowDerivedTypes));
        exml.Serialize(source, element);
      }

      {
        var exml = EXml.CreateDefault();
        exml.ElementDeserializers.Push(new SpecificTypeElementDeserializer<ParentClass>(DerivedTypesBehavior.AllowDerivedTypes));
        target = exml.Deserialize<HolderClass>(element);
      }

      target!.HeldObjectA.Should().BeOfType<ChildClass>();
      target!.HeldObjectB.Should().BeOfType<ParentClass>();
      target.Should().BeEquivalentTo(source);
    }
  }
}
