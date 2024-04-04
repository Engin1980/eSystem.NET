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
  public partial class ETextBox : System.Windows.Forms.TextBox
  {
    #region Nested

    /// <summary>
    /// Enum to define the behavior of "what is visible" when textbox looses focus.
    /// </summary>
    public enum eLostFocusBehavior
    {
      /// <summary>
      /// Default text-box behavior.
      /// </summary>
      DefaultAsTextbox,
      /// <summary>
      /// Beginning of the text is visible when no focus.
      /// </summary>
      ShowBeginning,
      /// <summary>
      /// End of the text is visible when no focus.
      /// </summary>
      ShowEnd
    }
    #endregion Nested

    #region Properties

    private string innerText;
    private int selectionStart = 0;
    private int selectionLength = 0;

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private eLostFocusBehavior _LostFocusBehavior;
    ///<summary>
    /// Sets/gets LostFocusBehavior value. This describes, which part of text is visible when textbox has not focus.
    ///</summary>
    public eLostFocusBehavior LostFocusBehavior
    {
      get
      {
        return (_LostFocusBehavior);
      }
      set
      {
        _LostFocusBehavior = value;
      }
    }

    /// <summary>
    /// Gets property IsEmpty. Is true when textbox is empty.
    /// </summary>
    public bool IsEmpty { get { return string.IsNullOrEmpty(innerText); } }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _EmptyText = "(enter value)";
    ///<summary>
    /// Sets/gets EmptyText value. This text is diplayed when textbox is empty and has no focus.
    ///</summary>
    public string EmptyText
    {
      get
      {
        return (_EmptyText);
      }
      set
      {
        _EmptyText = value;
        RefreshMe();
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private System.Drawing.Color _EmptyForeColor = System.Drawing.SystemColors.GrayText;
    ///<summary>
    /// Sets/gets EmptyForeColor value. Forecolor when textbox is empty and has no focus.
    ///</summary>
    public System.Drawing.Color EmptyForeColor
    {
      get
      {
        return (_EmptyForeColor);
      }
      set
      {
        _EmptyForeColor = value;
        RefreshMe();
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private System.Drawing.Color _EmptyBackColor = System.Drawing.SystemColors.ControlLightLight;
    ///<summary>
    /// Sets/gets EmptyBackColor value. Backcolor  when textbox is empty and has no focus.
    ///</summary>
    public System.Drawing.Color EmptyBackColor
    {
      get
      {
        return (_EmptyBackColor);
      }
      set
      {
        _EmptyBackColor = value;
        RefreshMe();
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private System.Drawing.Color _NonEmptyBackColor = System.Drawing.SystemColors.Window;
    ///<summary>
    /// Sets/gets NonEmptyBackColor value. Backcolor when textbox is not empty or has focus.
    ///</summary>
    public System.Drawing.Color NonEmptyBackColor
    {
      get
      {
        return (_NonEmptyBackColor);
      }
      set
      {
        _NonEmptyBackColor = value;
        RefreshMe();
      }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private System.Drawing.Color _NonEmptyForeColor = System.Drawing.SystemColors.WindowText;
    ///<summary>
    /// Sets/gets NonEmptyForeColor value. Forecolor  when textbox is not empty or has focus.
    ///</summary>
    public System.Drawing.Color NonEmptyForeColor
    {
      get
      {
        return (_NonEmptyForeColor);
      }
      set
      {
        _NonEmptyForeColor = value;
        RefreshMe();
      }
    }

    private string ParentText
    {
      get
      {
        forceToParent = true;
        string ret = Text;
        forceToParent = false;
        return ret;
      }
      set
      {
        forceToParent = true;
        this.Text = value;
        forceToParent = false;
      }
    }

    bool forceToParent = false;
    public override string Text
    {
      get
      {
        if (forceToParent)
          return base.Text;
        else
        {
          if (DesignMode)
          {
            if (IsEmpty)
              return "";
            else
              return ParentText;
          }
          else if (Focused)
            return ParentText;
          else
            return innerText;
        }
      }
      set
      {
        if (forceToParent)
          base.Text = value;
        else
        {
          ParentText = value;
          base_Leave(this, new EventArgs());

          //this.innerText = value;

          //if (Focused || DesignMode)
          //  ParentText = value;

          //if (Focused == false || DesignMode)
          //{
          //  RefreshMe();
          //}
        }
      }
    }

    private void RefreshMe()
    {
      if (IsEmpty)
        AdjustForEmpty();
      else
        AdjustForNonempty();
    }

    #endregion Properties

    #region .ctor

    public ETextBox() : this(null) { }

    public ETextBox(IContainer container)
    {
      if (container != null)
        container.Add(this);

      base.Enter += new EventHandler(base_Enter);
      base.Leave += new EventHandler(base_Leave);
      base.TextChanged += new EventHandler(base_TextChanged);
      base.KeyDown += new KeyEventHandler(base_KeyDown);

      AdjustForEmpty();

      InitializeComponent();
    }

    void ETextBox_Paint(object sender, PaintEventArgs e)
    {
      MessageBox.Show("P");
    }

    #endregion .ctor

    #region Private

    private void AdjustForNonempty()
    {
      base.BackColor = this.NonEmptyBackColor;
      base.ForeColor = this.NonEmptyForeColor;
    }

    private void AdjustForEmpty()
    {
      if (string.IsNullOrEmpty(EmptyText) == false)
      {
        ParentText = EmptyText;
        base.BackColor = this.EmptyBackColor;
        base.ForeColor = this.EmptyForeColor;
      }
    }

    //protected override void OnPaint(PaintEventArgs e)
    //{
    //  base.OnPaint(e);

    //  if (IsEmpty)
    //    return;

    //  if (base.Multiline)
    //    return;

    //  if (this.LostFocusBehavior != eLostFocusBehavior.ShowEnd)
    //    return;

    //  var g = e.Graphics;

    //  if (g.MeasureString(ParentText, Font).Width > this.Width)
    //  {
    //    Brush b = new SolidBrush(this.ForeColor);

    //    g.DrawString("<<", this.Font, b, 0, 0);
    //  }
    //}

    #endregion Private

    #region Event handlers

    void base_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Control && e.KeyCode == Keys.A)
      {
        base.SelectAll();
        e.Handled = true;
      }
    }

    void base_Leave(object sender, EventArgs e)
    {
      //Console.WriteLine("1 - Leave pre: " + base.Text + " (inner: " + innerText + ")");

      if (LostFocusBehavior != eLostFocusBehavior.DefaultAsTextbox)
      {
        try
        {
        this.selectionLength = SelectionLength;
        this.selectionStart = SelectionStart;

        this.SelectionLength = 0;

        if (LostFocusBehavior == eLostFocusBehavior.ShowBeginning)
          this.SelectionStart = 0;
        else
          this.SelectionStart = ParentText.Length;
        } // try
        catch (Exception)
        {
          this.selectionStart = 0;
          this.selectionLength = 0;
        } // catch (Exception ex)
      }

      this.innerText = ParentText;
      if (IsEmpty)
        AdjustForEmpty();
      else
        AdjustForNonempty();

      //Console.WriteLine("1 - Leave post: " + base.Text + " (inner: " + innerText + ")");
    }

    void base_Enter(object sender, EventArgs e)
    {
      //Console.WriteLine("1 - Enter pre: " + base.Text + " (inner: " + innerText + ")");

      if (IsEmpty)
        ParentText = "";

      AdjustForNonempty();

      if (LostFocusBehavior != eLostFocusBehavior.DefaultAsTextbox)
      {
        this.SelectionStart = selectionStart;
        this.SelectionLength = selectionLength;
      }

      //Console.WriteLine("1 - Enter post: " + base.Text + " (inner: " + innerText + ")");
    }

    void base_TextChanged(object sender, EventArgs e)
    {
      selectionStart = 0;
      selectionLength = 0;

    }

    #endregion Event handlers
  }
}
