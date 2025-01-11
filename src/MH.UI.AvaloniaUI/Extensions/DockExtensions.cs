using Avalonia.Controls;

namespace MH.UI.AvaloniaUI.Extensions;

public static class DockExtensions {
  public static Dock? FromMhDock(this MH.UI.Controls.Dock dock) =>
    dock switch {
      UI.Controls.Dock.Left => Dock.Left,
      UI.Controls.Dock.Top => Dock.Top,
      UI.Controls.Dock.Right => Dock.Right,
      UI.Controls.Dock.Bottom => Dock.Bottom,
      _ => null
    };

  public static UI.Controls.Dock? ToMhDock(this Dock dock) =>
    dock switch {
      Dock.Left => UI.Controls.Dock.Left,
      Dock.Bottom => UI.Controls.Dock.Bottom,
      Dock.Right => UI.Controls.Dock.Right,
      Dock.Top => UI.Controls.Dock.Top,
      _ => null
    };
}