using Avalonia;
using Avalonia.Controls;

namespace MH.UI.AvaloniaUI.Controls.Extensions;

public static class TabControlExtensions {
  static TabControlExtensions() {
    Visual.BoundsProperty.Changed.AddClassHandler<TabControl>(_onBoundsChanged);
  }

  private static void _onBoundsChanged(TabControl o, AvaloniaPropertyChangedEventArgs e) {
    if (e.Sender is not TabControl { DataContext: UI.Controls.TabControl tc } || e.NewValue is not Rect rect) return;
    tc.UpdateMaxTabSize(rect.Width, rect.Height);
  }

  public static void Init() { }
}