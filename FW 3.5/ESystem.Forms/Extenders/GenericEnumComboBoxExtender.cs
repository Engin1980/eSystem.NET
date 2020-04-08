using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESystem.Extensions;

namespace ESystem.Forms.Extenders
{
  public class GenericEnumComboBoxExtender<T> where T : struct, IConvertible
  {
    static GenericEnumComboBoxExtender()
    {
      if (typeof(T).IsEnum == false)
        throw new ArgumentException("Used generic parameter (" + typeof(T).FullName + ") is not enumeration.");
    }

    public static void FillComboBox<K>(ComboBox comboBox) where K : struct, IConvertible
    {
      GenericEnumComboBoxExtender<K> pom = new GenericEnumComboBoxExtender<K>(comboBox);
    }

    public static GenericEnumComboBoxExtender<K> Create<K>(ComboBox comboBox, bool immediateFill) where K : struct, IConvertible
    {
      GenericEnumComboBoxExtender<K> ret = new GenericEnumComboBoxExtender<K>(comboBox, immediateFill);

      return ret;
    }

    private Dictionary<string, T> dctEn = new Dictionary<string, T>();
    public ComboBox ComboBox { get; private set; }
    public Type EnumType { get { return typeof(T); } }

    public GenericEnumComboBoxExtender(ComboBox comboBox, bool immediateClearAndFill = true)
    {
      this.ComboBox = comboBox;
      this.Fill(true);
    }

    public void Fill(bool deleteExistingItems)
    {
      ComboBox.SuspendLayout();
      if (deleteExistingItems)
        ComboBox.Items.Clear();

      Array values = Enum.GetValues(typeof(T));
      foreach (T fItem in values)
      {
        string displ = GetDisplayValue(fItem);

        dctEn.Add(displ, fItem);
        ComboBox.Items.Add(displ);
      }

      ComboBox.ResumeLayout();
    }
    public void Refresh()
    {
      Fill(true);
    }

    public bool HasSelectedValue()
    {
      return ComboBox.SelectedIndex > -1;
    }

    public T SelectedValue
    {
      get
      {
        if (HasSelectedValue())
        {
          T ret = dctEn[(string)ComboBox.SelectedItem];
          return ret;
        }
        else
          throw new IndexOutOfRangeException("No value selected in combo box.");
      }
      set
      {
        string disp = GetDisplayValue(value);
        ComboBox.SelectedItem = disp;
      }
    }

    private string GetDisplayValue(T value)
    {
      Enum e = value as Enum;

      string ret = e.ToDisplay();

      return ret;
    }

  }
}
