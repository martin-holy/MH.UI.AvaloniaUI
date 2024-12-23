using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MH.UI.Avalonia.Sample.ViewModels;
using MH.UI.Avalonia.Sample.Views;

namespace MH.UI.Avalonia.Sample;

public partial class App : Application {
  public static Core Core { get; private set; } = null!;
  public static CoreUI CoreUI { get; private set; } = null!;

  public override void Initialize() {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted() {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
      _onDesktopStartup(desktop);
      /*desktop.MainWindow = new MainWindow {
        DataContext = new MainViewModel()
      };*/
    }
    else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
      singleViewPlatform.MainView = new MainView {
        DataContext = new MainViewModel()
      };
    }

    base.OnFrameworkInitializationCompleted();
  }

  private static async void _onDesktopStartup(IClassicDesktopStyleApplicationLifetime desktop) {
    // TODO PORT
    //Utils.ColorHelper.AddColorsToResources();

    var splashScreen = new SplashScreenV();
    desktop.MainWindow = splashScreen;
    desktop.MainWindow.Show();

    await Core.Inst.InitAsync(splashScreen.ProgressMessage);

    Core = Core.Inst;
    CoreUI = new();
    Core.AfterInit();

    //ShutdownMode = ShutdownMode.OnMainWindowClose; // TODO PORT
    desktop.MainWindow = new MainWindowV();
    desktop.MainWindow.Show();

    splashScreen.Close();
  }
}
