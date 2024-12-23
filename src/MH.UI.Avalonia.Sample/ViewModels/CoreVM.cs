using MH.Utils.BaseClasses;

namespace MH.UI.Avalonia.Sample.ViewModels;

public class CoreVM : ObservableObject {
  public MainWindowVM MainWindow { get; } = new();
}