using MH.UI.Avalonia.Sample.Resources;
using MH.Utils.BaseClasses;

namespace MH.UI.Avalonia.Sample.ViewModels.Controls;

public class ButtonsVM : ObservableObject {
  public RelayCommand DialogButtonWithIconCommand { get; }
  public RelayCommand DialogButtonWithoutIconCommand { get; }

  public ButtonsVM() {
    DialogButtonWithIconCommand = new(() => { }, Icons.Image, "Sample");
    DialogButtonWithoutIconCommand = new(() => { }, null, "Sample");
  }
}