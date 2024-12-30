using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MH.UI.AvaloniaUI.AttachedProperties;

public class Button : AvaloniaObject {
  public static readonly AttachedProperty<IBrush> OverLayerProperty = AvaloniaProperty.RegisterAttached<Button, Control, IBrush>("OverLayer");
  public static IBrush GetOverLayer(AvaloniaObject element) => element.GetValue(OverLayerProperty);
  public static void SetOverLayer(AvaloniaObject element, IBrush value) => element.SetValue(OverLayerProperty, value);
}