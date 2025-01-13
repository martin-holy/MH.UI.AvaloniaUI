using Avalonia;
using Avalonia.Controls;

namespace MH.UI.AvaloniaUI.Utils;

public static class TabControlHelper {
  public static void Init() { }

  static TabControlHelper() {
    Visual.BoundsProperty.Changed.AddClassHandler<TabControl>(_onBoundsChanged);
  }

  private static void _onBoundsChanged(TabControl o, AvaloniaPropertyChangedEventArgs e) {
    if (e.Sender is not TabControl { DataContext: UI.Controls.TabControl tc } || e.NewValue is not Rect rect) return;
    tc.UpdateMaxTabSize(rect.Width, rect.Height);
  }
}