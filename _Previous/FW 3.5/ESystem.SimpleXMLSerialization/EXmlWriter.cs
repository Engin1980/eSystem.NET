using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.SimpleXmlSerialization
{
  class EXmlWriter
  {
    public enum eState
    {
      OpenedOpeningTag,
      ClosedOpeningTag
    }

    public string IndentChar { get; set; }

    private Stack<String> openedElements;
    private int indentLevel = 0;
    private System.IO.StreamWriter wrt;
    private eState state;

    public EXmlWriter(System.IO.Stream stream, int initialIndentLevel = 0)
    {
      this.IndentChar = "  ";
      this.wrt = new System.IO.StreamWriter(stream, new UTF8Encoding());
      this.openedElements = new Stack<string>();
      this.indentLevel = initialIndentLevel;
      this.state = eState.ClosedOpeningTag;
    }

    public void WriteDocumentHeader()
    {
      wrt.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
    }
    public void WriteDoctype(string rootItem, string dtdFile)
    {
      wrt.WriteLine("<!DOCTYPE " + rootItem + " SYSTEM " + "\"" + dtdFile + "\">");
    }

    public void WriteElement(string element, string value)
    {
      BeginElement(element, false);
      wrt.Write(value);
      EndElement(false);
    }
    public void BeginElement(string element)
    {
      this.BeginElement(element, true);
    }
    private void BeginElement(string element, bool newLine)
    {
      BeginOpeningTag(element);
      EndOpeningTag(newLine);
    }
    public void EndElement()
    {
      this.EndElement(true);
    }
    private void EndElement(bool indent)
    {
      if (state == eState.OpenedOpeningTag)
      {
        wrt.Write(" />");
        wrt.WriteLine();

        string lastElement = openedElements.Pop();
        state = eState.ClosedOpeningTag;
      }
      else
      {
        string lastElement = openedElements.Pop();

        if (indent)
          wrt.Write(GetIndentString());
        wrt.Write("<");
        wrt.Write("/");
        wrt.Write(lastElement);
        wrt.Write(">");
        wrt.WriteLine();
      }
    }

    public void BeginOpeningTag(string element)
    {
      if (state == eState.OpenedOpeningTag)
        throw new XmlSerializeException("Cannot open new element when previous one is in attribute mode.");

      string pref = GetIndentString();

      wrt.Write(pref);
      wrt.Write("<");
      wrt.Write(element);

      state = eState.OpenedOpeningTag;
      openedElements.Push(element);
    }
    public void EndOpeningTag()
    {
      this.EndOpeningTag(true);
    }
    private void EndOpeningTag(bool newLine)
    {
      if (state != eState.OpenedOpeningTag)
        throw new XmlSerializeException("Cannot close begin-tag of element, cos it is no opening tag started.");

      wrt.Write(">");
      if (newLine)
        wrt.WriteLine();

      state = eState.ClosedOpeningTag;
    }

    public void AddAttribute(string attribute, string value)
    {
      char attributeValueDelimiter;
      if (value.Contains("\""))
        attributeValueDelimiter = '\'';
      else
        attributeValueDelimiter = '\"';

      if (value.Contains(attributeValueDelimiter))
        throw new XmlSerializeException("Cannot serialize attribute " + attribute + " with value " + value + ", cos it contains both chars \' and \" and therefore cannot be enclosed.", openedElements);

      wrt.Write(" ");
      wrt.Write(attribute);
      wrt.Write("=");
      wrt.Write(attributeValueDelimiter);
      wrt.Write(value);
      wrt.Write(attributeValueDelimiter);
    }

    public void Flush()
    {
      wrt.Flush();
    }

    public void Close()
    {
      this.Flush();
      wrt.Close();
    }

    private string GetIndentString()
    {
      StringBuilder ret = new StringBuilder();
      for (int i = 0; i < openedElements.Count; i++)
        ret.Append(IndentChar);
      return ret.ToString();
    }

    private void WriteIntended(string value)
    {
      string prefix = GetIndentString();

      wrt.Write(prefix);
      wrt.Write(value);
    }
  }
}
