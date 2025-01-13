using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;

namespace MH.UI.AvaloniaUI.Utils;

public static class Init {
  public static void ControlsExtensions() {
    TabControlHelper.Init();
  }

  // TODO PORT
  public static void SetDelegates() {
    /*MH.Utils.Keyboard.IsCtrlOn = () => (Keyboard.Modifiers & ModifierKeys.Control) > 0;
    MH.Utils.Keyboard.IsAltOn = () => (Keyboard.Modifiers & ModifierKeys.Alt) > 0;
    MH.Utils.Keyboard.IsShiftOn = () => (Keyboard.Modifiers & ModifierKeys.Shift) > 0;

    MH.Utils.Clipboard.SetText = Clipboard.SetText;

    MH.Utils.Imaging.GetBitmapHashPixels = Imaging.GetBitmapHashPixels;
    MH.Utils.Imaging.ResizeJpg = Imaging.ResizeJpg;

    MH.UI.Controls.Dialog.Show = DialogHost.Show;

    MH.Utils.Tasks.Dispatch = action => Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, action);

    CommandManager.RequerySuggested += RelayCommandBase.RaiseCanExecuteChanged;*/
  }

  public static void LoadDataTemplates(DataTemplates dataTemplates) {
    var files = new[] {
      "SlidePanelPinButton.axaml"
    };

    foreach (var file in files) {
      var uri = new Uri($"avares://MH.UI.AvaloniaUI/Resources/DataTemplates/{file}");
      if (AvaloniaXamlLoader.Load(uri) is DataTemplates dts)
        dataTemplates.AddRange(dts);
    }
  }

  public static void UseRelayCommandIconAndText() =>
    RelayCommandHelper.Init();
}