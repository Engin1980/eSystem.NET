using ESystem.Asserting;
using ESystem.Miscelaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ESystem.WPF.Windows
{
  /// <summary>
  /// Interaction logic for EnumInputBox.xaml
  /// </summary>
  public partial class EnumInputBox : Window
  {
    public delegate string DisplayStringSelector(object enumValue);

    public static class DisplayStringSelectors
    {
      public static string DisplayAttributeSelector(object enumValue)
      {
        EAssert.IsTrue(enumValue is Enum, "Provided value is not of Enum type. Provided: " + enumValue);
        string s = ESystem.Enums.GetDisplayName((Enum)enumValue);
        return s;
      }

      public static string ToStringSelector(object enumValue)
      {
        EAssert.IsNotNull(enumValue, "Provided value cannot be null.");
        return enumValue.ToString() ?? string.Empty;
      }
    }

    private record EnumValue(string Display, object Value)
    {
      public override string ToString() => this.Display;
    }
    private class ViewModel : NotifyPropertyChanged
    {
      public string Title
      {
        get => base.GetProperty<string>(nameof(Title))!;
        set => base.UpdateProperty(nameof(Title), value);
      }

      public string Prompt
      {
        get => base.GetProperty<string>(nameof(Prompt))!;
        set => base.UpdateProperty(nameof(Prompt), value);
      }

      public EnumValue? Input
      {
        get => base.GetProperty<EnumValue?>(nameof(Input))!;
        set => base.UpdateProperty(nameof(Input), value);
      }

      public List<EnumValue> Options
      {
        get => base.GetProperty<List<EnumValue>>(nameof(Options))!;
        set => base.UpdateProperty(nameof(Options), value);
      }
    }

    private readonly ViewModel vm;

    public object? Input => this.vm.Input?.Value;

    public EnumInputBox()
    {
      InitializeComponent();
      this.DataContext = this.vm = new ViewModel()
      {
        Title = "Not initialized",
        Prompt = "Not initialized",
        Input = default,
      };
    }

    public EnumInputBox(string prompt, string title,
      Type enumType,
      object? defaultInput = null,
      DisplayStringSelector? displayedValueSelector = null
      ) : this()
    {
      displayedValueSelector ??= DisplayStringSelectors.ToStringSelector;
      this.vm.Title = title;
      this.vm.Prompt = prompt;
      FillEnumValues(enumType, displayedValueSelector);
      SelectEnumValue(defaultInput);
    }

    private void SelectEnumValue(object? defaultInput)
    {
      var it = this.vm.Options.First(x => x.Value.Equals(defaultInput));
      this.vm.Input = it;
    }

    private void FillEnumValues(Type enumType, DisplayStringSelector displaySelector)
    {
      this.vm.Options = new List<EnumValue>();
      foreach (var enumValue in enumType.GetEnumValues())
      {
        string displayString = displaySelector(enumValue);
        EnumValue ev = new EnumValue(displayString, enumValue);
        this.vm.Options.Add(ev);
      }
    }

    private void btnOk_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
    }
  }
}
