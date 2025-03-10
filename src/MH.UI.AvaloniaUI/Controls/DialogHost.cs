using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using MH.UI.Controls;
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;

namespace MH.UI.AvaloniaUI.Controls;

// TODO PORT DialogHost as layer in single window app
public class DialogHost : ObservableObject {
  public Dialog Content { get; }
  public CustomWindow Window { get; }

  private DialogHost(Dialog content) {
    Content = content;
    
    Window = new() {
      Content = this,
      WindowStartupLocation = WindowStartupLocation.CenterOwner,
      ShowInTaskbar = false,
      CanResize = false,
      SizeToContent = SizeToContent.WidthAndHeight
    };

    content.PropertyChanged += (_, e) => {
      if (e.Is(nameof(content.Result)))
        Window.Close();
    };
  }

  public static int Show(Dialog content) {
    var dh = new DialogHost(content);
    var owner = _getOwner();

    if (owner == null) {
      dh.Window.Show();
      return content.Result;
    }

    // TODO PORT this works only on desktop
    var task = dh.Window.ShowDialog(owner);
    if (!task.IsCompleted) {
      var frame = new DispatcherFrame();
      task.ContinueWith(static (_, s) => ((DispatcherFrame)s).Continue = false, frame);
      Dispatcher.UIThread.PushFrame(frame);
    }
    task.GetAwaiter().GetResult();

    return content.Result;
  }

  private static Window? _getOwner() {
    if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
      return null;

    return desktop.Windows[^1].Content is DialogHost dh
      ? dh.Window
      : desktop.MainWindow!;
  }
}