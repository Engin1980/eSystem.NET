using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ESystem
{
  /// <summary>
  /// Interface to introduce generic clone method.
  /// </summary>
  /// <typeparam name="T">Type to clone.</typeparam>
  public interface ICloneable<T>
  {

    /// <summary>
    /// Clones object into new instance.
    /// </summary>
    /// <returns></returns>
    T Clone();

  }
}
