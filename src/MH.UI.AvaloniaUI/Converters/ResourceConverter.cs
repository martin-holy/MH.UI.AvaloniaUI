using Avalonia;
using Avalonia.Controls;

namespace MH.UI.AvaloniaUI.Converters;

public class ResourceConverter : BaseConverter {
  private static readonly object _lock = new();
  private static ResourceConverter? _inst;
  public static ResourceConverter Inst { get { lock (_lock) { return _inst ??= new(); } } }

  public override object Convert(object? value, object? parameter) {
    value = TryConvertValue(value, parameter);

    return value == null
      ? AvaloniaProperty.UnsetValue
      : TryFindResource(Application.Current?.Resources, value)
        ?? AvaloniaProperty.UnsetValue;
  }

  public static object? TryConvertValue(object? value, object? parameter) {
    if (value == null) return null;
    if (parameter is Dictionary<object, object> dict) {
      if (!dict.TryGetValue(value, out var dicValue))
        if (!dict.TryGetValue(value.GetType(), out dicValue))
          dict.TryGetValue("default", out dicValue);

      return dicValue;
    }

    return value;
  }

  public static object? TryFindResource(IResourceDictionary? dictionary, object value) {
    if (dictionary == null) return null;
    if (dictionary.TryGetValue(value, out var resource)) return resource;

    foreach (var md in dictionary.MergedDictionaries.OfType<ResourceDictionary>()) {
      var res = TryFindResource(md, value);
      if (res != null) return res;
    }

    return null;
  }
}