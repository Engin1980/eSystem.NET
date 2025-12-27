//using ESystem.Asserting;
//using EXmlLib2.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EXmlLib2.Types
//{
//  /// <summary>
//  /// Used to wrap IAttributeSerializer<T> into non-generic IAttributeSerializer
//  /// </summary>
//  /// <typeparam name="T"></typeparam>
//  internal class TypedAttributeSerializerWrapper<T> : IAttributeSerializer
//  {
//    private readonly IAttributeSerializer<T> inner;

//    public TypedAttributeSerializerWrapper(IAttributeSerializer<T> inner)
//    {
//      EAssert.Argument.IsNotNull(inner, nameof(inner));
//      this.inner = inner;
//    }

//    public bool AcceptsType(Type type) => type == typeof(T);

//    public string Serialize(object? value, IXmlContext ctx)
//    {
//      EAssert.Argument.IsTrue(
//        value != null && value.GetType() == typeof(T),
//        nameof(value),
//        $"Value type ({value?.GetType()?.Name ?? "null"}) does not match attribute serializer type ({typeof(T).Name}).");
//      T typedValue = (T)value!;
//      string ret = inner.Serialize(typedValue, ctx);
//      return ret;
//    }
//  }
//}
