using ESystem.Miscelaneous;
using ESystem.WPF.KeyHooking;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfKeyHookTesting
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public class KeyHookVM : NotifyPropertyChanged
    {
      public KeyHook KeyHook { get => base.GetProperty<KeyHook>(nameof(KeyHook))!; set => base.UpdateProperty(nameof(KeyHook), value); }
      public BindingList<KeyRegistrationVM> Registereds { get => base.GetProperty<BindingList<KeyRegistrationVM>>(nameof(Registereds))!; set => base.UpdateProperty(nameof(Registereds), value); }
      public KeyHookVM()
      {
        this.Registereds = [];
      }
    }

    public class KeyRegistrationVM : NotifyPropertyChanged
    {
      public KeyChord? RegisteredChord { get => base.GetProperty<KeyChord?>(nameof(RegisteredChord)); set => base.UpdateProperty(nameof(RegisteredChord), value); }
      public KeyShortcut? RegisteredShortcut { get => base.GetProperty<KeyShortcut?>(nameof(RegisteredShortcut)); set => base.UpdateProperty(nameof(RegisteredShortcut), value); }
      public DateTime? LastActivated { get => base.GetProperty<DateTime?>(nameof(LastActivated)); set => base.UpdateProperty(nameof(LastActivated), value); }
      public string DisplayText { get => base.GetProperty<string>(nameof(DisplayText)) ?? string.Empty; set => base.UpdateProperty(nameof(DisplayText), value); }
    }

    public class VM : NotifyPropertyChanged
    {
      public BindingList<KeyHookVM> KeyHooksVM { get => base.GetProperty<BindingList<KeyHookVM>>(nameof(KeyHooksVM))!; set => base.UpdateProperty(nameof(KeyHooksVM), value); }
      public KeyHookVM? SelectedKeyHookVM { get => base.GetProperty<KeyHookVM?>(nameof(SelectedKeyHookVM)); set => base.UpdateProperty(nameof(SelectedKeyHookVM), value); }
      public KeyRegistrationVM? SelectedKeyRegistrationVM { get => base.GetProperty<KeyRegistrationVM?>(nameof(SelectedKeyRegistrationVM)); set => base.UpdateProperty(nameof(SelectedKeyRegistrationVM), value); }
      public VM()
      {
        this.KeyHooksVM = [];
      }
    }

    private readonly VM vm;
    private KeyHook kh = new();
    public MainWindow()
    {
      InitializeComponent();
      this.DataContext = this.vm = new();

      kh.RegisterKeyShortcut(new(Key.F5), () =>
      {
        MessageBox.Show("Global Shortcut F5 pressed!");
      });
    }

    private async void btnAddShortcut_Click(object sender, RoutedEventArgs e)
    {
      FrmWaiting frm = new FrmWaiting();

      if (this.vm.SelectedKeyHookVM == null)
      {
        MessageBox.Show("Create/Select a KeyHook first.");
        return;
      }

      KeyHookVM kvm = this.vm.SelectedKeyHookVM;

      frm.Show();
      KeyShortcut ks = await kvm.KeyHook.DetectKeyShortcutAsync();
      frm.Close();


      var krvm = new KeyRegistrationVM
      {
        RegisteredShortcut = ks,
        DisplayText = $"{ks.Modifiers} + {ks.Key} //key-shortcut"
      };
      kvm.Registereds.Add(krvm);
      kvm.KeyHook.RegisterKeyShortcut(ks, () =>
      {
        krvm.LastActivated = DateTime.Now;
      });
    }

    private void btnAddHook_Click(object sender, RoutedEventArgs e)
    {
      this.vm.SelectedKeyHookVM = new KeyHookVM { KeyHook = new KeyHook() };
      this.vm.KeyHooksVM.Add(this.vm.SelectedKeyHookVM);
    }

    private void tvwRegistered_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      // Reset selected registration
      this.vm.SelectedKeyRegistrationVM = null;

      if (e.NewValue is KeyRegistrationVM kr)
      {
        // A registration was selected
        this.vm.SelectedKeyRegistrationVM = kr;

        // Find parent KeyHookVM for this registration and select it
        foreach (var kh in this.vm.KeyHooksVM)
        {
          if (kh.Registereds.Contains(kr))
          {
            this.vm.SelectedKeyHookVM = kh;
            break;
          }
        }
      }
      else if (e.NewValue is KeyHookVM khvm)
      {
        // A hook was selected
        this.vm.SelectedKeyHookVM = khvm;
      }
      else
      {
        // Unknown type, clear selections
        this.vm.SelectedKeyHookVM = null;
        this.vm.SelectedKeyRegistrationVM = null;
      }
    }

    private void btnDeleteHook_Click(object sender, RoutedEventArgs e)
    {
      if (this.vm.SelectedKeyHookVM == null)
      {
        MessageBox.Show("Create/Select a KeyHook first.");
        return;
      }

      this.vm.SelectedKeyHookVM.KeyHook.UnregisterAll();
      this.vm.KeyHooksVM.Remove(this.vm.SelectedKeyHookVM);
    }

    private async void btnAddChord_Click(object sender, RoutedEventArgs e)
    {
      FrmWaiting frm = new();

      if (this.vm.SelectedKeyHookVM == null)
      {
        MessageBox.Show("Create/Select a KeyHook first.");
        return;
      }

      KeyHookVM kvm = this.vm.SelectedKeyHookVM;

      frm.Show();
      KeyChord kc = await kvm.KeyHook.DetectKeyChordAsync();
      frm.Close();


      var krvm = new KeyRegistrationVM
      {
        RegisteredChord = kc,
        DisplayText = $"{kc.ToString()} //key-shortcut"
      };
      kvm.Registereds.Add(krvm);
      kvm.KeyHook.RegisterKeyChord(kc, () =>
      {
        krvm.LastActivated = DateTime.Now;
      });
    }
  }
}