using MH.Utils.BaseClasses;

namespace MH.UI.AvaloniaUI.Sample.ViewModels;

public class CoreVM : ObservableObject {
  public MainWindowVM MainWindow { get; } = new();
}