using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESystem.WPF.Converters
{
  public class Int2VisibilityCollapsedConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int number = (int)value;
      return number == 0
        ? Visibility.Collapsed
        : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  [Obsolete("Use Int2VisibilityCollapsedConverter instead.")]
  public class Int2VisibilityConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int number = (int)value;
      return number == 0
        ? Visibility.Collapsed
        : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
