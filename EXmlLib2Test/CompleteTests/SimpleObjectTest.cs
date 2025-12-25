using ESystem.Logging;
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
    public void SimpleClassTest()
    {
      SimpleClass source = new SimpleClass()
      {
        CreatedAt = DateTime.Now,
        Id = 11,
        Name = "Test Object"
      };

      var exml = EXmlLib2.EXml.CreateDefault();
      XElement root = new XElement("Root");
      exml.Serialize(source, root);

      SimpleClass? dest = exml.Deserialize<SimpleClass>(root);

      source.Should().BeEquivalentTo(dest);
    }
  }
}
