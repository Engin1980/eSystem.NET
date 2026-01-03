using ESystem.Asserting;
using ESystem.Exceptions;
using EXmlLib2.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXmlLib2.Abstractions.Abstracts;

public abstract class TypedBase<T>(DerivedTypesBehavior derivedTypesBehavior = DerivedTypesBehavior.ExactTypeOnly) : ISelectableByType
{
  protected readonly DerivedTypesBehavior derivedTypesBehavior = derivedTypesBehavior;

  public bool AcceptsType(Type type) => derivedTypesBehavior switch
  {
    DerivedTypesBehavior.AllowDerivedTypes => typeof(T).IsAssignableFrom(type),
    DerivedTypesBehavior.ExactTypeOnly => type == typeof(T),
    _ => throw new NotSupportedException($"DerivedTypesBehavior value {derivedTypesBehavior} is not supported."),
  };

  protected void CheckTypeSanity(Type? type)
  {
    EAssert.Argument.IsNotNull(type, nameof(type));
    switch (this.derivedTypesBehavior)
    {
      case DerivedTypesBehavior.AllowDerivedTypes:
        EAssert.Argument.IsTrue(type.IsAssignableTo(typeof(T)), nameof(type), $"Value must be of type '{typeof(T)}' or a derived type. Provided: '{type}'");
        break;
      case DerivedTypesBehavior.ExactTypeOnly:
        EAssert.Argument.IsTrue(type == typeof(T), nameof(type), $"Value must be of exact type '{typeof(T)}'. Provided: '{type}'");
        break;
      default:
        throw new UnexpectedEnumValueException(this.derivedTypesBehavior);
    }
  }
}
