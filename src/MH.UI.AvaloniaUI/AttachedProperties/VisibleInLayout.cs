using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace MH.UI.AvaloniaUI.AttachedProperties;

public class VisibleInLayout {
  public static readonly AttachedProperty<bool> WhenProperty =
    AvaloniaProperty.RegisterAttached<VisibleInLayout, Control, bool>(
    "When", false, false, BindingMode.OneWay, null, _onWhenCoerced);

  public static bool GetWhen(Control control) => control.GetValue(WhenProperty);
  public static void SetWhen(Control control, bool value) => control.SetValue(WhenProperty, value);

  private static bool _onWhenCoerced(AvaloniaObject control, bool value) {
    if (control is Control c) {
      c.IsHitTestVisible = value;
      c.Opacity = value ? 1 : 0;
    }

    return value;
  }
}