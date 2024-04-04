using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace ESystem.Forms.Extenders
{
  public partial class EnumComboBoxExtender : Component
  {
    #region Nested

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DisplayAttribute : Attribute
    {
      [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
      private string _DisplayText;
      ///<summary>
      /// Sets/gets DisplayText value.
      ///</summary>
      public string DisplayText
      {
        get
        {
          return (_DisplayText);
        }
        set
        {
          _DisplayText = value;
        }
      }

      public DisplayAttribute(string displayText)
      {
        this.DisplayText = displayText;
      }

    }

    #endregion Nested

    #region Events

    public event CancelEventHandler ComboContentChanging;
    public event EventHandler ComboContentChanged;

    #endregion Events

    #region Fields & Properties

    private Dictionary<string, object> dctEn = new Dictionary<string, object>();

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    ///<summary>
    /// Sets/gets ComboBoxName value.
    ///</summary>
    public string ComboBoxName
    {
      get
      {
        if (_ComboBox == null)
          return null;
        else
          return _ComboBox.Name;
      }
      set
      {
        if (value == null)
          ComboBox = null;
        else
        {
          var obj = this.Container.Components[value];
          if (obj == null)
            ThrowError("Control " + value + "does not exist.");
          else if (obj is ComboBox)
            this.ComboBox = obj as ComboBox;
          else
            ThrowError("Control " + value + " is not ComboBox.");
        }
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private System.Windows.Forms.ComboBox _ComboBox;
    ///<summary>
    /// Sets/gets ComboBox value.
    ///</summary>
    public System.Windows.Forms.ComboBox ComboBox
    {
      get
      {
        return (_ComboBox);
      }
      set
      {
        _ComboBox = value;
        if (value != null)
        {
          RefreshComboContent();
        }
      }
    }

    ///<summary>
    /// Sets/gets EnumTypeName value.
    ///</summary>
    public string EnumTypeName
    {
      get
      {
        if (EnumType == null)
          return "(null)";
        else
          return EnumType.FullName;
      }
      set
      {
        if (value == null || value == "null" || value == "(null)")
          EnumType = null;
        else
        {
          Type t = FindType(value);

          if (t == null)
            ThrowError("Unable load type");
          else
            EnumType = t;
        }
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    private Type _EnumType = null;
    ///<summary>
    /// Sets/gets EnumType value.
    ///</summary>
    public Type EnumType
    {
      get
      {
        return (_EnumType);
      }
      set
      {

        if (value == null)
          _EnumType = null;
        else if (value.IsEnum == false)
        {
          ThrowError("Type " + value + " is not enum type.");
          return;
        }
        else
          _EnumType = value;

        RefreshComboContent();
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    ///<summary>
    /// Sets/gets SelectedInt value.
    ///</summary>
    public int ValueAsInt
    {
      get
      {
        object val = Value;
        if (val == null)
          return int.MinValue;
        else
        {
          int ret = (int)val;
          return ret;
        }
      }
      set
      {
        if (EnumType == null)
          return;

        var obj = Enum.ToObject(EnumType, value);

        Value = obj;

      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    ///<summary>
    /// Sets/gets SelectedValue value.
    ///</summary>
    public object Value
    {
      get
      {
        if (HasSelectedValue())
        {
          object ret = dctEn[ComboBox.Text];
          return ret;
        }
        else
          return null;
      }
      set
      {
        if (EnumType == null)
          return;
        if (ComboBox == null)
          return;

        if (value == null)
          ComboBox.SelectedIndex = -1;
        else
        {
          var txt = Enum.GetName(EnumType, value);
          string displ = GetDisplayText(EnumType, txt);
          ComboBox.Text = displ;
        }
      }
    }

    #endregion Fields & Properties

    #region .ctor

    public EnumComboBoxExtender()
    {
      InitializeComponent();
    }

    public EnumComboBoxExtender(IContainer container)
    {
      container.Add(this);

      InitializeComponent();
    }

    #endregion .ctor

    #region Methods

    public bool HasSelectedValue()
    {
      if (ComboBox == null || ComboBox.SelectedIndex < 0 || dctEn.Keys.Contains(ComboBox.Text) == false)
        return false;
      else
        return true;
    }

    public T GetValue<T>()
    {
      T ret = default(T);
      object val = Value;

      if (val != null)
      {
        ret = (T)val;
      }

      return ret;
    }

    #endregion Methods

    #region Privates

    private void ThrowError(string message)
    {
      Console.WriteLine("EnumComboBoxExtender error: " + message);
    }

    public void Refresh()
    {
      RefreshComboContent();
    }

    private void RefreshComboContent()
    {
      if (ComboBox == null)
        return;


      if (ComboContentChanging != null)
      {
        CancelEventArgs e = new CancelEventArgs();
        ComboContentChanging(this, e);
        if (e.Cancel)
          return;
      }

      ComboBox.SuspendLayout();
      ComboBox.Items.Clear();
      dctEn.Clear();
      if (EnumType != null)
      {
        Array values = Enum.GetValues(EnumType);
        foreach (var fItem in values)
        {
          string txt = Enum.GetName(EnumType, fItem);
          string displ = GetDisplayText(EnumType, txt);
          dctEn.Add(displ, fItem);
          ComboBox.Items.Add(displ);
        }
      }
      ComboBox.ResumeLayout();

      if (ComboContentChanged != null)
        ComboContentChanged(this, new EventArgs());
    }

    private Type FindType(string fullTypeName)
    {
      string pom = fullTypeName;
      if (pom.Contains('+') == false)
      {
        int index = fullTypeName.LastIndexOf('.');
        StringBuilder sb = new StringBuilder(pom);
        sb[index] = '+';
        pom = sb.ToString();
      }

      List<Assembly> alreadySearched = new List<Assembly>();

      Type ret =
        FindTypeIn(pom, Assembly.GetEntryAssembly(), ref alreadySearched);

      return ret;
    }

    private Type FindTypeIn(string fullTypeName, Assembly ass, ref List<Assembly> alreadySearched)
    {
      Type t = ass.GetType(fullTypeName);

      alreadySearched.Add(ass);

      if (t == null)
      {
        AssemblyName[] refNames = ass.GetReferencedAssemblies();
        foreach (var fItem in refNames)
        {
          Assembly refAss = Assembly.Load(fItem);
          if (alreadySearched.Contains(refAss))
            continue;

          t = FindTypeIn(fullTypeName, refAss, ref alreadySearched);

          if (t != null)
            break;
        } // foreach (var fItem in refNames)
      }

      return t;
    }

    private string GetDisplayText(Type enumType, string stringValue)
    {
      string ret = stringValue;

      FieldInfo fi = enumType.GetField((string)stringValue);

      object[] attributes = fi.GetCustomAttributes(false);
      foreach (var fItem in attributes)
      {
        if (fItem is DisplayAttribute)
        { 
          ret = (fItem as DisplayAttribute).DisplayText;
          break;
        }
      } // foreach (var fItem in attributes)

      return ret;
    }

    #endregion Privates
  }
}
