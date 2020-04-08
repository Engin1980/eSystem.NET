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
  public partial class OutputTextBox : UserControl
  {
    public OutputTextBox()
    {
      InitializeComponent();
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private bool _AutoScrollCarret = true;
    ///<summary>
    /// Sets/gets Autoscroll value.
    ///</summary>
    public bool AutoScrollCarret
    {
      get
      {
        return (_AutoScrollCarret);
      }
      set
      {
        _AutoScrollCarret = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private bool _AutoTime;
    ///<summary>
    /// Sets/gets Autotime value.
    ///</summary>
    public bool AutoTime
    {
      get
      {
        return (_AutoTime);
      }
      set
      {
        _AutoTime = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _AutoTimeSeparator = " ";
    ///<summary>
    /// Sets/gets AutoTimeSeparator value. Default value is " ".
    ///</summary>
    public string AutoTimeSeparator
    {
      get
      {
        return (_AutoTimeSeparator);
      }
      set
      {
        _AutoTimeSeparator = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private bool _AutoNewLine;
    ///<summary>
    /// Sets/gets AutoNewLine value.
    ///</summary>
    public bool AutoNewLine
    {
      get
      {
        return (_AutoNewLine);
      }
      set
      {
        _AutoNewLine = value;
      }
    }

    public bool ReadOnly
    {
      get
      {
        return txt.ReadOnly;
      }
      set
      {
        txt.ReadOnly = value;
      }
    }

    public override System.Drawing.Font Font
    {
      get
      {
        return base.Font;
      }
      set
      {
        base.Font = value;
      }
    }

    public void Clear()
    {
      txt.Clear();
    }


    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Bindable(true)]
    public override string Text
    {
      get
      {
        return txt.Text;
      }
      set
      {
        txt.Text = value;
      }
    }

    public void AddText(string text)
    {
      StringBuilder sb = new StringBuilder(text);

      if (AutoTime)
        sb.Insert(0, DateTime.Now.ToString() + AutoTimeSeparator);

      if (AutoNewLine)
        sb.Append(Environment.NewLine);

      txt.Text += sb.ToString();

      if ((AutoScrollCarret) && (txt.Text.Length > 0))
      {
        txt.Select(txt.Text.Length, 0);
        txt.ScrollToCaret();
      }
    }

    public void SelectAll()
    {
      txt.SelectAll();
    }

  }
}
