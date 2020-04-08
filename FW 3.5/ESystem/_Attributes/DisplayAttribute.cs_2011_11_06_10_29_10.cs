using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem
{
  /// <summary>
  /// Defines how item can be shown in UI.
  /// </summary>
  /// <remarks>
  /// Typically used with Enum-eratins, with enum.ToDisplay() extension method, or with
  /// extenders for combobox or listview (see ESystem.Forms)
  /// </remarks>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public class DisplayAttribute : Attribute
  {
    /// <summary>
    /// Displayed text for item.
    /// </summary>
    public string DisplayText { get; private set; }

    /// <summary>
    /// Index of item. Used only if order request.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// If true, this item may not be displayed.
    /// </summary>
    public bool IsHidden { get; set; }

    public DisplayAttribute(string displayText)
    {
      Index = 0;
      this.DisplayText = displayText;
    }
  }
}
