using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESystem;

namespace ESystem.Forms
{
  [System.ComponentModel.DefaultProperty("FolderName")]
  [System.ComponentModel.DefaultEvent("FolderNameChanged")]
  public partial class FolderBrowser : UserControl
  {
    //public delegate void FolderNameChangedDelegate(object sender, EventArgs e);
    
    public event System.EventHandler FolderNameChanged;

    public FolderBrowser()
    {
      InitializeComponent();
      this.txtFolderName.Resize += new EventHandler(txtFolderName_Resize);
    }

    void txtFolderName_Resize(object sender, EventArgs e)
    {
      this.Height = System.Math.Max(txtFolderName.Height + 2, 23);
      this.btnBrowse.Height = this.Height;
    }

    ///<summary>
    /// Sets/gets EmptyText value.
    ///</summary>
    public string EmptyText
    {
      get
      {
        return (txtFolderName.EmptyText);
      }
      set
      {
        txtFolderName.EmptyText = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _RelativePath = "";
    ///<summary>
    /// Sets/gets WorkingPath value.
    ///</summary>
    public string RelativePath
    {
      get
      {
        return (_RelativePath);
      }
      set
      {
        string old = FolderName;
        _RelativePath = _Browser.NormalizeRelativePath(value);
        UpdateViewedName(old);
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _Title = "Select folder";
    ///<summary>
    /// Sets/gets Title value.
    ///</summary>
    public string Title
    {
      get
      {
        return (_Title);
      }
      set
      {
        _Title = value;
      }
    }

    ///<summary>
    /// Sets/gets Folder value.
    ///</summary>
    public string FolderName
    {
      get
      {
        string val = txtFolderName.Text;
        string ret = _Browser.GetName(RelativePath, val);
        return (ret);
      }
      set
      {
        UpdateViewedName(value);
      }
    }

    ///<summary>
    /// Sets/gets Text value.
    ///</summary>
    public override string Text
    {
      get
      {
        return (txtFolderName.Text);
      }
      set
      {
        this.FolderName = Text;
      }
    }

    private void UpdateViewedName()
    {
      UpdateViewedName(FolderName);
    }
    private void UpdateViewedName(string name)
    {
      if (string.IsNullOrEmpty(name) == false)
      {
        if (name.StartsWith(this.RelativePath))
          txtFolderName.Text = name.Substring(RelativePath.Length);
        else
          txtFolderName.Text = name;
      }
      else
        txtFolderName.Text = null;
    }

    public DialogResult ShowDialog()
    {
      SetDialog(ref fbd);

      DialogResult ret = fbd.ShowDialog();

      if (ret == DialogResult.OK)
      {
        GetDialog(fbd);
      }

      return ret;
    }

    private void SetDialog(ref FolderBrowserDialog fd)
    {
      fd.Description = this.Title;
      fd.SelectedPath = this.FolderName;
    }

    private void GetDialog(FolderBrowserDialog fd)
    {
      this.FolderName = fd.SelectedPath;
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      if (ShowDialog() == DialogResult.OK)
      {
        FolderName = fbd.SelectedPath;
        if (FolderNameChanged != null)
          FolderNameChanged(this, new EventArgs());
      }
    }

    private void txtFolderName_TextChanged(object sender, EventArgs e)
    {
      if (FolderNameChanged != null)
        FolderNameChanged(this, new EventArgs());
    }

  }
}
