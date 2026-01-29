using EXmlLib2;
using EXmlLib2.Abstractions.Abstracts;
using EXmlLib2.Abstractions.Interfaces;
using EXmlLib2.Implementations.TypeSerialization.PropertyBased;
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
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
        exml.ElementSerializers.AddFirst(new ObjectSerializer().WithAcceptedType<ParentClass>(true));
        exml.Serialize(source, typeof(ParentClass), element);
      }

      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
        exml.ElementDeserializers.AddFirst(new ObjectDeserializer().WithAcceptedType<ParentClass>(true));
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
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
        exml.ElementSerializers.AddFirst(new ObjectSerializer().WithAcceptedType<ParentClass>(true));
        exml.ElementSerializers.AddFirst(new ObjectSerializer().WithAcceptedType<HolderClass>(true));
        exml.Serialize(source, element);
      }

      {
        EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
        exml.ElementDeserializers.AddFirst(new ObjectDeserializer().WithAcceptedType<ParentClass>(true));
        exml.ElementDeserializers.AddFirst(new ObjectDeserializer().WithAcceptedType<HolderClass>(true));
        target = exml.Deserialize<HolderClass>(element);
      }

      target!.HeldObjectA.Should().BeOfType<ChildClass>();
      target!.HeldObjectB.Should().BeOfType<ParentClass>();
      target.Should().BeEquivalentTo(source);
    }


    [Test]
    public void InheritanceInPropertyTest2()
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
        EXml exml = EXml.Create().WithDefaultSerialization();
        exml.Serialize(source, element);
      }

      {
        EXml exml = EXml.Create().WithDefaultSerialization();
        target = exml.Deserialize<HolderClass>(element);
      }

      target!.HeldObjectA.Should().BeOfType<ChildClass>();
      target!.HeldObjectB.Should().BeOfType<ParentClass>();
      target.Should().BeEquivalentTo(source);
    }

    [Test]
    public void InheritanceInPropertyWithIgnoredTest()
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
        EXml exml = EXml.Create().WithDefaultSerialization();
        exml.Serialize(source, element);
      }

      {
        EXml exml = EXml.Create().WithDefaultSerialization();
        exml.ElementDeserializers.AddFirst(new ObjectDeserializer()
          .WithAcceptedType<ParentClass>(true)
          .WithTypeOptions<ParentClass>(opt => opt.WithIgnoredProperty(q => q.ParentProperty))
          );
        target = exml.Deserialize<HolderClass>(element);
      }

      target!.HeldObjectA.Should().BeOfType<ChildClass>();
      target!.HeldObjectB.Should().BeOfType<ParentClass>();
      target.HeldObjectA.ParentProperty.Should().Be(8);
      target.HeldObjectB.ParentProperty.Should().Be(8);
    }
  }
}
