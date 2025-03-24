using Avalonia;
using Avalonia.Controls;

namespace MH.UI.AvaloniaUI.Controls;

public class TextLabel : ContentControl {
  public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<TextLabel, string?>(nameof(Text));
  public string? Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
}