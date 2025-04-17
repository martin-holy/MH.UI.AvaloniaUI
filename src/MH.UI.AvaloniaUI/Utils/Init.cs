using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using MH.UI.AvaloniaUI.Controls;

namespace MH.UI.AvaloniaUI.Utils;

public static class Init {
  public static void ControlsExtensions() {
    TabControlHelper.Init();
  }

  // TODO PORT
  public static void SetDelegates() {
    KeyboardHelper.Init();
    MH.Utils.Keyboard.IsCtrlOn = () => KeyboardHelper.IsCtrlOn;
    MH.Utils.Keyboard.IsAltOn = () => KeyboardHelper.IsAltOn;
    MH.Utils.Keyboard.IsShiftOn = () => KeyboardHelper.IsShiftOn;

    /*MH.Utils.Clipboard.SetText = Clipboard.SetText;

    MH.Utils.Imaging.GetBitmapHashPixels = Imaging.GetBitmapHashPixels;
    MH.Utils.Imaging.ResizeJpg = Imaging.ResizeJpg;*/

    MH.UI.Controls.Dialog.SetShowAsyncImplementation(DialogHost.ShowAsync);

    MH.Utils.Tasks.Dispatch = action => Dispatcher.UIThread.Post(action, DispatcherPriority.Render);

    //CommandManager.RequerySuggested += RelayCommandBase.RaiseCanExecuteChanged;
  }

  public static void LoadDataTemplates(DataTemplates dataTemplates) {
    var files = new[] {
      "DataTemplates/SlidePanelPinButton",
      "Dialogs/InputDialog",
      "Dialogs/MessageDialog",
      "Dialogs/SelectFromListDialog",
      "Dialogs/ToggleDialog"
    };

    foreach (var file in files) {
      var uri = new Uri($"avares://MH.UI.AvaloniaUI/Resources/{file}.axaml");
      if (AvaloniaXamlLoader.Load(uri) is DataTemplates dts)
        dataTemplates.AddRange(dts);
    }
  }

  public static void UseRelayCommandIconAndText() =>
    RelayCommandHelper.Init();
}