using ESystem.Asserting;
using ESystem.Exceptions;
using EXmlLib2.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Abstractions.Abstracts;

public abstract class TypedBase<T> : ISelectableByType
{
  public bool AcceptsType(Type type) => type == typeof(T);

  protected void CheckTypeSanity(Type? type)
  {
    EAssert.Argument.IsTrue(type != null, nameof(type), "Type cannot be null.");
    EAssert.Argument.IsTrue(type == typeof(T), nameof(type), $"Value should be of exact type '{typeof(T)}'. Provided: '{type}'");
  }
}
