using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ESystem.Forms
{
  public partial class LabelPanel : UserControl
  {
    public LabelPanel()
    {
      InitializeComponent();
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Bindable(true)]
    public override string Text
    {
      get
      {
        return lblContent.Text;
      }
      set
      {
        lblContent.Text = value;
      }
    }
  }
}
