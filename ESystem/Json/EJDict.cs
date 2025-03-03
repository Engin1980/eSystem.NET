using ESystem.Exceptions;
using ESystem.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ESystem.Json
{
  public class EJDict : Dictionary<string, EJObject>
  {
    public static EJDict Create(JToken jToken)
    {
      JObject jObject = jToken.Value<JObject>() ?? throw new UnexpectedNullException();
      EJDict ret = new();
      var enumerator = jObject.GetEnumerator();
      while (enumerator.MoveNext())
      {
        var t = enumerator.Current;
        string itemKey = t.Key;
        JToken itemJToken = t.Value ?? throw new UnexpectedNullException();
        ret[itemKey] = new EJObject(itemJToken);
      }
      return ret;
    }
  }
}
