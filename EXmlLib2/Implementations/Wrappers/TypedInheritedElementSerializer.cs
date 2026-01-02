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
//  public class TypedInheritedElementSerializer<T> : IElementSerializer
//  {
//    private readonly TypedElementSerializer<T> innerSerializer;

//    public TypedInheritedElementSerializer(TypedElementSerializer<T> innerSerializer)
//    {
//      this.innerSerializer = innerSerializer;
//    }

//    public bool AcceptsType(Type type) => typeof(T).IsAssignableFrom(type);

//    public void Serialize(object? value, XElement element, IXmlContext ctx)
//    {
//      EAssert.Argument.IsNotNull(value, nameof(value));

//      innerSerializer.Serialize(value, element, ctx);
//      if (value.GetType() != typeof(T))
//      {
//        string typeName = value.GetType().AssemblyQualifiedName!;
//        element.Add(new XAttribute(ctx.TypeNameAttribute, typeName));
//      }
//    }
//  }
//}
