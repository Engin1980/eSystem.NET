using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESystem.WPF.Converters
{
  public class BoolToVisibilityConverter : TypedConverter<bool, Visibility>
  {
    protected override Visibility Convert(bool value, object parameter, CultureInfo culture)
    {
      return value ? Visibility.Visible : Visibility.Collapsed;
    }

    protected override bool ConvertBack(Visibility value, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
