using ESystem.Asserting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Miscelaneous
{
  public abstract class Optional<T>
  {
    public static implicit operator Optional<T>(T value) => new Some<T>(value);

    public static implicit operator Optional<T>(None none) => new None<T>();

    public static Optional<T> Of(T value) => new Some<T>(value);
    public static Optional<T> Empty() => new None<T>();

    public abstract Optional<TResult> Map<TResult>(Func<T, TResult> map);
    public abstract Optional<TResult> MapOptional<TResult>(Func<T, Optional<TResult>> map);
    public abstract T Reduce(T whenNone);
    public abstract T Reduce(Func<T> whenNone);
  }

  public sealed class Some<T> : Optional<T>
  {
    public T Content { get; }

    public Some(T value)
    {
      EAssert.Argument.IsNotNull(value);
      this.Content = value;
    }

    public static implicit operator T(Some<T> some) => some.Content;

    public override Optional<TResult> Map<TResult>(Func<T, TResult> map) => map(this.Content);

    public override Optional<TResult> MapOptional<TResult>(Func<T, Optional<TResult>> map) => map(this.Content);

    public override T Reduce(T whenNone) => this.Content;

    public override T Reduce(Func<T> whenNone) => this.Content;
  }

  public sealed class None<T> : Optional<T>
  {
    public override Optional<TResult> Map<TResult>(Func<T, TResult> map) => None.Value;

    public override Optional<TResult> MapOptional<TResult>(Func<T, Optional<TResult>> map) => None.Value;

    public override T Reduce(T whenNone) => whenNone;

    public override T Reduce(Func<T> whenNone) => whenNone();
  }

  public sealed class None
  {
    public static None Value { get; } = new None();

    private None() { }
  }
}
