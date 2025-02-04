using Avalonia.Controls;
using Avalonia.Input;

namespace MH.UI.AvaloniaUI.Utils;

public static class KeyboardHelper {
  public static bool IsCtrlOn { get; private set; }
  public static bool IsAltOn { get; private set; }
  public static bool IsShiftOn { get; private set; }

  public static void Init() { }

  static KeyboardHelper() {
    InputElement.KeyDownEvent.AddClassHandler<TopLevel>(_updateKeyModifiers, handledEventsToo: true);
    InputElement.KeyUpEvent.AddClassHandler<TopLevel>(_updateKeyModifiers, handledEventsToo: true);
  }

  private static void _updateKeyModifiers(TopLevel o, KeyEventArgs e) {
    IsCtrlOn = e.KeyModifiers == KeyModifiers.Control;
    IsAltOn = e.KeyModifiers == KeyModifiers.Alt;
    IsShiftOn = e.KeyModifiers == KeyModifiers.Shift;
  }
}