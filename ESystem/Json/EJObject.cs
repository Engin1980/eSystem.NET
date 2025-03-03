using ESystem.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Json
{
  public class EJObject
  {
    private readonly JToken inner;
    public EJObject(JToken inner)
    {
      this.inner = inner;
    }

    public static EJDict Parse(string jsonText)
    {
      EJDict ret;
      try
      {
        JObject obj = JObject.Parse(jsonText);
        ret = EJDict.Create(obj);
      }
      catch (Exception ex)
      {
        throw new Exception("Error parsing EJObject", ex);
      }
      return ret;
    }
    public static EJDict Load(string fileName)
    {
      EJDict ret;
      try
      {
        string s = System.IO.File.ReadAllText(fileName);
        ret = Parse(s);
      }
      catch (Exception ex)
      {
        throw new Exception("Error loading EJObject from file", ex);
      }
      return ret;
    }

    public EJDict AsDict()
    {
      EJDict ret = EJDict.Create(inner);
      return ret;
    }

    public int AsInt()
    {
      int ret = inner.Value<int>();
      return ret;
    }

    public string AsString()
    {
      string ret = inner.Value<string>() ?? throw new UnexpectedNullException();
      return ret;
    }

    public byte AsByte()
    {
      return (byte)AsInt();
    }

    public List<EJObject> AsList()
    {
      List<EJObject> ret = new();
      JArray jArray = inner.Value<JArray>() ?? throw new UnexpectedNullException();
      foreach (var jItem in jArray)
      {
        ret.Add(new EJObject(jItem));
      }
      return ret;
    }

    public List<EJDict> AsListOfDicts()
    {
      var lst = AsList();
      List<EJDict> ret = lst.Select(q => q.AsDict()).ToList();
      return ret;
    }
  }
}
