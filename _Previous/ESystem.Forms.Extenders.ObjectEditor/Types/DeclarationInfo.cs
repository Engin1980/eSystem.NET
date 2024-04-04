using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Forms.Extenders.ObjectEditor.Types
{
  public class DeclarationInfo
  {
    public enum eControl
    {
      TextBox,
      NumericUpDown
    }

    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public eControl Control { get; set; }
  }
}
