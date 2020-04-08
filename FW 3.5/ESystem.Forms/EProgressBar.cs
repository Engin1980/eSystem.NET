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
  public partial class EProgressBar : UserControl
  {
    public enum eOrientation
    {
      TopDown,
      BottomUp,
      LeftRight,
      RightLeft
    }

    public EProgressBar()
    {
      InitializeComponent();

      this.ForeColorChanged += new EventHandler(EngProgressBar_ForeColorChanged);
      this.BackColorChanged += new EventHandler(EngProgressBar_BackColorChanged);
    }

    void EngProgressBar_BackColorChanged(object sender, EventArgs e)
    {
      pnlOuter.BackColor = this.BackColor;
    }

    void EngProgressBar_ForeColorChanged(object sender, EventArgs e)
    {
      pnlInner.BackColor = this.ForeColor;
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private double _Minimum  = 0;
    ///<summary>
    /// Sets/gets Minimum value.
    ///</summary>
    public double Minimum
    {
      get
      {
        return (_Minimum);
      }
      set
      {
        _Minimum = value;
        if (_Minimum > _Maximum)
          _Minimum = _Maximum;
      }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private double _Maximum = 100;
    ///<summary>
    /// Sets/gets Maximum value.
    ///</summary>
    public double Maximum
    {
      get
      {
        return (_Maximum);
      }
      set
      {
        _Maximum = value;
        if (_Maximum < _Minimum)
          _Maximum = _Minimum;
        Adjust();
      }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private double _Value = 70;
    ///<summary>
    /// Sets/gets Value value.
    ///</summary>
    public double Value
    {
      get
      {
        return (_Value);
      }
      set
      {
        _Value = value;
        if (_Value < _Minimum)
          _Value = _Minimum;
        else if (_Value > _Maximum)
          _Value = _Maximum;
        Adjust();
      }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private eOrientation _Orientation = eOrientation.LeftRight;
    ///<summary>
    /// Sets/gets Orientation value.
    ///</summary>
    public eOrientation Orientation
    {
      get
      {
        return (_Orientation);
      }
      set
      {
        _Orientation = value;
      }
    }

    private void Adjust()
    {
      double perc = Value / (Maximum - Minimum);
      int pom;

      switch (Orientation)
      {
        case eOrientation.BottomUp:
          pnlInner.Left = 0;
          pnlInner.Width = pnlOuter.Width;
          pom = (int) (pnlOuter.Height * perc);
          pnlInner.Top = pom;
          pnlInner.Height = pnlOuter.Height - pnlInner.Top;
          break;
        case eOrientation.TopDown:
          pnlInner.Left = 0;
          pnlInner.Width = pnlOuter.Width;
          pom = (int)(pnlOuter.Height * perc);
          pnlInner.Top = 0;
          pnlInner.Height = pom;
          break;
        case eOrientation.LeftRight:
          pnlInner.Top = 0;
          pnlInner.Height= pnlOuter.Height;
          pom = (int)(pnlOuter.Width * perc);
          pnlInner.Left = 0;
          pnlInner.Width = pom;
          break;
        case eOrientation.RightLeft:
          pnlInner.Top = 0;
          pnlInner.Height = pnlOuter.Height;
          pom = (int)(pnlOuter.Width * perc);
          pnlInner.Left = 0;
          pnlInner.Width = pom;
          break;
        default:
          throw new NotSupportedException();
      }
    }

    private void EngProgressBar_Resize(object sender, EventArgs e)
    {
      Adjust();
    }
  }
}
