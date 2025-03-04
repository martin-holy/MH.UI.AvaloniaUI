using Avalonia.Controls;
using Avalonia.Input;

namespace MH.UI.AvaloniaUI.Converters;

public class MouseWheelConverter : BaseConverter {
  private static readonly object _lock = new();
  private static MouseWheelConverter? _inst;
  public static MouseWheelConverter Inst { get { lock (_lock) { return _inst ??= new(); } } }

  public override object? Convert(object? value, object? parameter) {
    if (value is not PointerWheelEventArgs e) return null;

    return new MH.Utils.EventsArgs.MouseWheelEventArgs {
      OriginalSource = e.Source,
      DataContext = (e.Source as Control)?.DataContext,
      Delta = (int)e.Delta.Y
    };
  }
}