using ESystem.Asserting;
using EXmlLib2.Abstractions;
using EXmlLib2.Implementations.TypeSerialization.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EXmlLib2.Implementations.TypeSerialization.PropertyBased.Properties
{
  public class PropertyFromAnyDeserializer : IPropertyDeserializer
  {
    private MissingPropertyElementBehavior behavior = MissingPropertyElementBehavior.Ignore;
    private readonly SimplePropertyAsElement elementDeserializer = new SimplePropertyAsElement().WithMissingPropertyElementBehavior(MissingPropertyElementBehavior.Ignore);
    private readonly SimplePropertyAsAttribute attributeDeserializer = new SimplePropertyAsAttribute().WithMissingPropertyElementBehavior(MissingPropertyElementBehavior.Ignore);
    private readonly List<IPropertyDeserializer> deserializers = [];
    private XmlSourceOrder sourceOrder = XmlSourceOrder.AttributesFirst;

    private void AdjustDeserializersOrder()
    {
      deserializers.Clear();
      if (sourceOrder == XmlSourceOrder.AttributesFirst)
      {
        deserializers.Add(attributeDeserializer);
        deserializers.Add(elementDeserializer);
      }
      else
      {
        deserializers.Add(elementDeserializer);
        deserializers.Add(attributeDeserializer);
      }
    }

    public PropertyFromAnyDeserializer()
    {
      AdjustDeserializersOrder();
    }

    public PropertyFromAnyDeserializer WithSourceOrder(XmlSourceOrder order)
    {
      sourceOrder = order;
      this.AdjustDeserializersOrder();
      return this;
    }

    public PropertyFromAnyDeserializer WithMissingPropertyElementBehavior(MissingPropertyElementBehavior behavior)
    {
      this.behavior = behavior;
      return this;
    }

    public PropertyFromAnyDeserializer WithNameCaseMatching(NameCaseMatching matching)
    {
      this.elementDeserializer.WithNameCaseMatching(matching);
      this.attributeDeserializer.WithNameCaseMatching(matching);
      return this;
    }

    public DeserializationResult DeserializeProperty(PropertyInfo propertyInfo, XElement element, IXmlContext ctx)
    {
      DeserializationResult ret = DeserializationResult.NoResult();

      foreach (var deserializer in deserializers)
      {
        ret = deserializer.DeserializeProperty(propertyInfo, element, ctx);
        EAssert.IsNotNull(ret);

        if (ret.HasResult)
          break;
      }

      if (ret.HasResult == false)
      {
        ret = behavior switch
        {
          MissingPropertyElementBehavior.Ignore => DeserializationResult.NoResult(),
          MissingPropertyElementBehavior.ThrowException => throw new InvalidOperationException($"Cannot find element or attribute for property '{propertyInfo.Name}' in element '{element.Name}'."),
          MissingPropertyElementBehavior.ReturnNull => DeserializationResult.Null(),
          _ => throw new ESystem.Exceptions.UnexpectedEnumValueException(behavior),
        };
      }

      return ret;
    }
  }
}
