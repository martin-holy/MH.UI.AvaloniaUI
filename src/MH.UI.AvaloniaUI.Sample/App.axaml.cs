using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using MH.UI.AvaloniaUI.Sample.Views;
using System;
using System.Threading.Tasks;

namespace MH.UI.AvaloniaUI.Sample;

public partial class App : Application {
  public static Core Core { get; private set; } = null!;
  public static CoreUI CoreUI { get; private set; } = null!;

  public override void Initialize() {
    AvaloniaXamlLoader.Load(this);

    AvaloniaUI.Utils.Init.LoadDataTemplates(DataTemplates);
    AvaloniaUI.Utils.Init.UseRelayCommandIconAndText();
    AvaloniaUI.Utils.Init.ControlsExtensions();
    AvaloniaUI.Utils.ColorHelper.AddColorsToResources();
    _loadDataTemplates();
  }

  public override void OnFrameworkInitializationCompleted() {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      _onDesktopStartup(desktop);
    else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
      _onSingleViewStartup(singleViewPlatform);

    base.OnFrameworkInitializationCompleted();
  }

  private static async void _onDesktopStartup(IClassicDesktopStyleApplicationLifetime desktop) {
    var splashScreen = new SplashScreenV();
    desktop.MainWindow = splashScreen;
    desktop.MainWindow.Show();

    await _initCore(splashScreen.ProgressMessage);

    //ShutdownMode = ShutdownMode.OnMainWindowClose; // TODO PORT
    desktop.MainWindow = new MainWindowV();
    desktop.MainWindow.Show();

    splashScreen.Close();
  }

  private static async void _onSingleViewStartup(ISingleViewApplicationLifetime singleViewPlatform) {
    // TODO PORT progress
    var progress = new Progress<string>();
    await _initCore(progress);
    singleViewPlatform.MainView = new MainView { DataContext = Core.VM.MainWindow };
  }

  private static async Task _initCore(IProgress<string> progress) {
    await Core.Inst.InitAsync(progress);

    Core = Core.Inst;
    CoreUI = new();
    Core.AfterInit();
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
      "Controls/FolderTreeViewV.axaml",
      "Controls/IconTextV.axaml",
      "Controls/ListsV.axaml",
      "Controls/SlidersV.axaml",
      "Controls/TextsV.axaml",
      "FolderV.axaml"
    };

    foreach (var file in files) {
      var uri = new Uri($"avares://MH.UI.AvaloniaUI.Sample/Views/{file}");
      if (AvaloniaXamlLoader.Load(uri) is DataTemplates dts)
        DataTemplates.AddRange(dts);
    }
  }
}
