//using ESystem.Asserting;
//using EXmlLib2.Abstractions.Abstracts;
//using EXmlLib2.Abstractions.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Linq;

//namespace EXmlLib2.Abstractions.Wrappers
//{
//  public class TypedInheritedElementDeserializer<T> : IElementDeserializer
//  {
//    private readonly TypedElementDeserializer<T> innerDeserializer;

//    public TypedInheritedElementDeserializer(TypedElementDeserializer<T> innerDeserializer)
//    {
//      EAssert.Argument.IsNotNull(innerDeserializer, nameof(innerDeserializer));
//      this.innerDeserializer = innerDeserializer;
//    }

//    public bool AcceptsType(Type type) => typeof(T).IsAssignableFrom(type);

//    public object Deserialize(XElement element, Type targetType, IXmlContext ctx)
//    {
//      object? ret;
//      string? attributeValue = element.Attribute(ctx.TypeNameAttribute)?.Value;
//      if (attributeValue != null)
//      {
//        Type actualType = Type.GetType(attributeValue, throwOnError: true)!;
//        IElementDeserializer deserializer = ctx.ElementDeserializers.GetByType(actualType);
//        ret = deserializer.Deserialize(element, actualType, ctx);// ctx.DeserializeFromElement(element, actualType, deserializer);
//      }
//      else
//      {
//        ret = innerDeserializer.Deserialize(element, targetType, ctx);
//      }
//      EAssert.IsNotNull(ret, "Deserialized value is null.");
//      EAssert.IsTrue(ret is T, "Deserialized value is not of expected type.");
//      return ret;
//    }
//  }
//}
