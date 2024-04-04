using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESystem.Forms.Extenders.ObjectEditor.Types
{
  public class TypeInfo
  {
    public string FullTypeName { get; set; }
    public List<DeclarationInfo> Declarations { get; set; }
  }
}
