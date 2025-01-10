using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using MH.UI.AvaloniaUI.Sample.ViewModels;
using MH.UI.AvaloniaUI.Sample.Views;
using System;

namespace MH.UI.AvaloniaUI.Sample;

public partial class App : Application {
  public static Core Core { get; private set; } = null!;
  public static CoreUI CoreUI { get; private set; } = null!;

  public override void Initialize() {
    AvaloniaXamlLoader.Load(this);

    AvaloniaUI.Utils.Init.LoadDataTemplates(DataTemplates);
    AvaloniaUI.Utils.Init.UseRelayCommandIconAndText();
    _loadDataTemplates();
  }

  public override void OnFrameworkInitializationCompleted() {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
      _onDesktopStartup(desktop);
    }
    else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
      singleViewPlatform.MainView = new MainView {
        DataContext = new MainViewModel()
      };
    }

    base.OnFrameworkInitializationCompleted();
  }

  private static async void _onDesktopStartup(IClassicDesktopStyleApplicationLifetime desktop) {
    AvaloniaUI.Utils.ColorHelper.AddColorsToResources();

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

  private void _loadDataTemplates() {
    // include files in *.csproj as AvaloniaResource too
    var files = new[] {
      "Layout/LeftContentV.axaml",
      "Layout/MiddleContentV.axaml",
      "Layout/RightContentV.axaml",
      "Layout/StatusBarV.axaml",
      "Layout/ToolBarV.axaml",
      "Controls/ButtonsV.axaml",
      "Controls/ColorsV.axaml",
      "Controls/ControlsV.axaml",
      "Controls/TextsV.axaml"
    };

    foreach (var file in files) {
      var uri = new Uri($"avares://MH.UI.AvaloniaUI.Sample/Views/{file}");
      if (AvaloniaXamlLoader.Load(uri) is DataTemplates dts)
        DataTemplates.AddRange(dts);
    }
  }
}
