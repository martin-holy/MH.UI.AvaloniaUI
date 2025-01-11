using Avalonia.Controls;
using MH.UI.AvaloniaUI.Extensions;

namespace MH.UI.AvaloniaUI.Converters;

public class DockConverter : BaseConverter {
  private static readonly object _lock = new();
  private static DockConverter? _inst;
  public static DockConverter Inst { get { lock (_lock) { return _inst ??= new(); } } }

  public override object? Convert(object? value, object? parameter) =>
    value is UI.Controls.Dock dock ? dock.FromMhDock() : null;

  public override object? ConvertBack(object? value, object? parameter) =>
    value is Dock dock ? dock.ToMhDock() : null;
}