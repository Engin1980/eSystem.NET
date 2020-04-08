using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESystem.Extensions;

namespace ESystem.Forms
{
  [System.ComponentModel.DefaultEvent("FileNameChanged")]
  [System.ComponentModel.DefaultProperty("FileName")]
  public partial class FileBrowser : UserControl
  {
    #region Events

    public event System.EventHandler FileNameChanged;

    #endregion Events

    #region Constants

    public const string FILTER_ALL_FILES = "All files (*.*)|*.*";
    public const string FILTER_XML_FILES = "XML files (*.xml)|*.xml";
    public const string FILTER_TXT_FILES = "Text files (*.txt)|*.txt";
    public const string FILTER_DAT_FILES = "Data files (*.dat)|*.dat";
    public const string FILTER_BIN_FILES = "Data files (*.bin)|*.bin";

    #endregion Constants

    #region Enums

    public enum eOpenFileType
    {
      Open,
      SaveAs
    }

    public enum eExistCheck
    {
      Allways,
      OnlyForOpen,
      Never
    }

    #endregion Enums

    #region Properties

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
        string old = FileName;
        _RelativePath = _Browser.NormalizeRelativePath(value);
        UpdateViewedFileName(old);
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _FileName = "";
    ///<summary>
    /// Sets/gets FileName value.
    ///</summary>
    [Bindable(true)]
    [System.ComponentModel.SettingsBindable(true)]
    public string FileName
    {
      get
      {
        string val = txtFileName.Text;
        string ret = _Browser.GetName(RelativePath, val);
        return (ret);
      }
      set
      {
        _FileName = value;
        UpdateViewedFileName(value);
        Console.WriteLine(_FileName);
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private eOpenFileType _DialogType = eOpenFileType.Open;
    ///<summary>
    /// Sets/gets DialogType value.
    ///</summary>
    public eOpenFileType DialogType
    {
      get
      {
        return (_DialogType);
      }
      set
      {
        _DialogType = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _Title = "Select file to open";
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

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _Filter = FILTER_ALL_FILES;
    ///<summary>
    /// Sets/gets Filter value.
    ///</summary>
    public string Filter
    {
      get
      {
        return (_Filter);
      }
      set
      {
        _Filter = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private eExistCheck _CheckFileExists = eExistCheck.OnlyForOpen;
    ///<summary>
    /// Sets/gets CheckFilterExists value.
    ///</summary>
    public eExistCheck CheckFileExists
    {
      get
      {
        return (_CheckFileExists);
      }
      set
      {
        _CheckFileExists = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private eExistCheck _CheckPathExists = eExistCheck.OnlyForOpen;
    ///<summary>
    /// Sets/gets CheckFolderExists value.
    ///</summary>
    public eExistCheck CheckPathExists
    {
      get
      {
        return (_CheckPathExists);
      }
      set
      {
        _CheckPathExists = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _CurrentDirectory = null;
    ///<summary>
    /// Sets/gets CurrentDirectory value.
    ///</summary>
    public string CurrentDirectory
    {
      get
      {
        return (_CurrentDirectory);
      }
      set
      {
        _CurrentDirectory = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private string _DialogDirectory;
    ///<summary>
    /// Sets/gets Directory value.
    ///</summary>
    public string DialogDirectory
    {
      get
      {
        return (_DialogDirectory);
      }
      set
      {
        _DialogDirectory = value;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private int _FilterIndex;
    ///<summary>
    /// Sets/gets FilterIndex value.
    ///</summary>
    public int FilterIndex
    {
      get
      {
        return (_FilterIndex);
      }
      set
      {
        _FilterIndex = value;
      }
    }

    ///<summary>
    /// Sets/gets EmptyText value.
    ///</summary>
    public string EmptyText
    {
      get
      {
        return (txtFileName.EmptyText);
      }
      set
      {
        txtFileName.EmptyText = value;
      }
    }

    ///<summary>
    /// Sets/gets Text value.
    ///</summary>
    public override string Text
    {
      get
      {
        return (txtFileName.Text);
      }
      set
      {
        this.FileName = Text;
      }
    }

    #endregion Properties

    #region .ctor

    public FileBrowser()
    {
      InitializeComponent();

      this.txtFileName.Resize += new EventHandler(txtFileName_Resize);
    }

    #endregion .ctor

    #region Public methods

    public DialogResult ShowDialog()
    {
      FileDialog fd = null;
      if (DialogType == eOpenFileType.Open)
        fd = ofd;
      else if (DialogType == eOpenFileType.SaveAs)
        fd = sfd;

      SetDialog(ref fd);

      DialogResult ret = ofd.ShowDialog();

      if (ret == DialogResult.OK)
      {
        GetDialog(fd);
      }

      return ret;
    }

    /// <summary>
    /// Creates new filter string, e.g. "XML files (*.xml)|*.XML"
    /// </summary>
    /// <param name="fileTypeDescription">Description, e.g. "XML files"</param>
    /// <param name="pattern">File match pattern, e.g. "*.xml"</param>
    /// <returns></returns>
    public static string CreateFilterString(string fileTypeDescription, string pattern)
    {
      string ret = string.Format(
        "{0} ({1})|{1}",
        fileTypeDescription, pattern);
      return ret;
    }

    #endregion Public methods

    #region Private methods

    /// <summary>
    /// Sets multiple filter.
    /// </summary>
    /// <param name="filter"></param>
    private void SetFilters(params string[] filter)
    {
      string val = filter.ToString(";");

      this.Filter = val;
    }

    private void UpdateViewedFileName()
    {
      UpdateViewedFileName(FileName);
    }
    private void UpdateViewedFileName(string fileName)
    {
      string parent = "";
      if (!string.IsNullOrEmpty(fileName))
      {
        parent = System.IO.Path.GetDirectoryName(fileName).ToLower() + "\\";
        if (parent.StartsWith(this.RelativePath.ToLower()))
          txtFileName.Text = fileName.Substring(RelativePath.Length);
        else
          txtFileName.Text = fileName;
      }
      else
        txtFileName.Text = "";
    }

    private void SetDialog(ref FileDialog fd)
    {
      fd.Title = this.Title;
      fd.Filter = this.Filter;
      fd.FilterIndex = this.FilterIndex;
      fd.InitialDirectory = this.DialogDirectory;
      fd.FileName = this.FileName;
      fd.CheckFileExists = ToBool(this.CheckFileExists);
      fd.CheckPathExists = ToBool(this.CheckPathExists);
    }

    private bool ToBool(eExistCheck value)
    {
      if (value == eExistCheck.Allways)
        return true;
      else if (value == eExistCheck.Never)
        return false;
      else if (DialogType == eOpenFileType.Open)
        return true;
      else
        return false;
    }

    private void GetDialog(FileDialog fd)
    {
      FileName = fd.FileName;
      this.FilterIndex = fd.FilterIndex;
      this.CurrentDirectory = System.IO.Path.GetDirectoryName(fd.FileName);
    }


    #endregion Private methods

    #region Event handlers

    void txtFileName_Resize(object sender, EventArgs e)
    {
      this.Height = System.Math.Max(txtFileName.Height + 2, 23);
      this.btnBrowse.Height = this.Height;
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      if (ShowDialog() == DialogResult.OK)
      {
        if (FileNameChanged != null)
          FileNameChanged(this, new EventArgs());
      }
    }

    private void txtFileName_TextChanged(object sender, EventArgs e)
    {
      FileName = txtFileName.Text;
      if (FileNameChanged != null)
        FileNameChanged(this, new EventArgs());
    }

    #endregion Event handlers
  }
}
