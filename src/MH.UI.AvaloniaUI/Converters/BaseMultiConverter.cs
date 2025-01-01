using System.Globalization;
using Avalonia.Data.Converters;

namespace MH.UI.AvaloniaUI.Converters;

public class BaseMultiConverter : IMultiValueConverter {
  public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) =>
    Convert(values, parameter);

  public virtual object Convert(IList<object?> values, object? parameter) =>
    throw new NotImplementedException();
}