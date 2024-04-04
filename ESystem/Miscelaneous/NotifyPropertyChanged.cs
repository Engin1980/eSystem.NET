using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESystem.Miscelaneous
{
  public abstract class NotifyPropertyChanged : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly Dictionary<string, object?> inner = new();

    protected void UpdateProperty<T>(string key, T? value)
    {
      // TODO check for changing to the same value
      inner[key] = value;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
    }

    protected T? GetProperty<T>(string key)
    {
      T? ret;
      if (inner.ContainsKey(key))
      {
        ret = (T?)inner[key];
      }
      else
      {
        ret = default;
      }

      return ret;
    }
  }
}
