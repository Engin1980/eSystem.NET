using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.Xml.Linq;
using ESystem.SimpleXmlSerialization.Behavior;

namespace ESystem.SimpleXmlSerialization
{
  public class Serializer
  {
    public void Serialize(string fileName, object graph, string rootElement = "this", string dtdFile =null)
    {
      string tempFile = System.IO.Path.GetTempFileName();

      System.IO.FileStream fs = new System.IO.FileStream(tempFile, System.IO.FileMode.Open);
      this.Serialize(fs, graph, rootElement, dtdFile);
      fs.Close();

      System.IO.File.Copy(tempFile, fileName, true);
      System.IO.File.Delete(tempFile);
    }
    public void Serialize(System.IO.Stream stream, object graph, string rootElement = "this",string dtdFile =null)
    {      
      EXmlWriter wrt = new EXmlWriter(stream);

      ItemSerializer ser = new ItemSerializer();
      
      ser.Serialize(wrt, graph, rootElement, dtdFile);

      wrt.Flush();
      wrt.Close();
    }
    public T Deserialize<T>(string fileName)
    {
      System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
      T ret = this.Deserialize<T>(fs);
      fs.Close();
      return ret;
    }
    public T Deserialize<T>(System.IO.Stream stream)
    {
      XDocument doc = OpenXDocument(stream);
      XElement elm = doc.Root;
      
      ItemDeserializer des = new ItemDeserializer();
      T ret = des.Deserialize<T>(elm);
      return ret;
    }

    private static XDocument OpenXDocument(System.IO.Stream stream)
    {
      XmlReaderSettings sett = new XmlReaderSettings() { ProhibitDtd = false };
      XmlReader xmlReader = XmlReader.Create(stream, sett);

      XDocument doc = null;
      doc = XDocument.Load(xmlReader);

      return doc;
    }    
  }
}
