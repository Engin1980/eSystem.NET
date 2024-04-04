using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ESystem.Forms.Extenders
{
  public class TypeListViewExtender<T>
  {

    #region Nested

    private class PropertyData : IComparable<PropertyData>
    {
      public PropertyInfo PropertyInfo { get; set; }
      public int DisplayIndex { get; set; }
      public string DisplayText { get; set; }
      public bool IsHidden { get; set; }

      public PropertyData(PropertyInfo pi)
      {
        if (pi == null) throw new ArgumentNullException();

        this.PropertyInfo = pi;

        var atts = pi.GetCustomAttributes(typeof(DisplayAttribute), false);
        if (atts.Length == 0)
        {
          DisplayText = pi.Name;
          DisplayIndex = 0;
          IsHidden = false;
        }
        else
        {
          var att = (atts[0] as DisplayAttribute);
          DisplayText = att.DisplayText;
          DisplayIndex = att.Index;
          IsHidden = att.IsHidden;
        }
      }

      public static PropertyData[] CreateForTypeT()
      {
        List<PropertyData> ret = new List<PropertyData>();
        Type t = typeof(T);

        var pis = typeof(T).GetProperties(
          System.Reflection.BindingFlags.Public |
          System.Reflection.BindingFlags.GetProperty |
          System.Reflection.BindingFlags.Instance
          );

        foreach (var item in pis)
        {
          var newItem = new PropertyData(item);
          if (newItem.IsHidden == false)
            ret.Add(newItem);
        }

        ret.Sort();

        return ret.ToArray();
      }

      public int CompareTo(PropertyData other)
      {
        int ret = this.DisplayIndex.CompareTo(other.DisplayIndex);
        if (ret == 0)
          ret = this.DisplayText.CompareTo(other.DisplayText);

        return ret;
      }
    }
    #endregion Nested

    public ListView ListView { get; private set; }
    public IList<T> DataSource { get; private set; }
    private PropertyData[] PropertyDatas { get; set; }

    public TypeListViewExtender(IList<T> dataSource, System.Windows.Forms.ListView listView, bool immediateFill)
    {
      if (listView == null)
        throw new ArgumentNullException("ListView");

      this.ListView = listView;
      this.DataSource = dataSource;

      if (immediateFill)
        Fill();
    }

    public void Refresh()
    {
      Fill();
    }

    public void Fill()
    {
      if (PropertyDatas == null) CreatePropertyData();
      ListView.View = View.Details;

      ListView.Items.Clear();
      ListView.Columns.Clear();

      CreateColumns();

      FillData();

      AdjustColumnWidth();
    }

    private void AdjustColumnWidth()
    {
      foreach (ColumnHeader item in ListView.Columns)
      {
        item.Width = -1; // sirka podle nejdelsiho retezce
      }
    }

    private void CreatePropertyData()
    {
      this.PropertyDatas = PropertyData.CreateForTypeT();
    }

    private void CreateColumns()
    {
      foreach (var item in PropertyDatas)
      {
        ListView.Columns.Add(item.DisplayText);
      }
    }

    private void FillData()
    {
      if (DataSource == null) return;

      foreach (var item in DataSource)
      {
        FillDataItem(item);
      }
    }

    private void FillDataItem(T item)
    {
      bool isFirst = true;
      ListViewItem lwi = new ListViewItem();
      lwi.Tag = item;

      foreach (var fProp in PropertyDatas)
      {
        object val = fProp.PropertyInfo.GetValue(item, null);
        if (isFirst)
        {
          lwi.Text = val.ToString();
          isFirst = false;
        }
        else
          lwi.SubItems.Add(val.ToString());
      }

      ListView.Items.Add(lwi);
    }
  }
}
