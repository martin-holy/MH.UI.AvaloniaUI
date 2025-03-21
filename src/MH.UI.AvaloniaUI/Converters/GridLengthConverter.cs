using Avalonia.Controls;

namespace MH.UI.AvaloniaUI.Converters;

public class GridLengthConverter : BaseConverter {
  private static readonly object _lock = new();
  private static GridLengthConverter? _inst;
  public static GridLengthConverter Inst { get { lock (_lock) { return _inst ??= new(); } } }

  public override object Convert(object? value, object? parameter) =>
    value switch {
      int i => new(i),
      double d => new(d),
      _ => GridLength.Auto
    };

  public override object? ConvertBack(object? value, object? parameter) =>
    ((GridLength?)value)?.Value;
}