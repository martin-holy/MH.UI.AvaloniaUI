using Avalonia;
using Avalonia.Controls;

namespace MH.UI.Avalonia.AttachedProperties; 

public class Text {
  public static readonly AttachedProperty<string> TextProperty = AvaloniaProperty.RegisterAttached<Text, Control, string>("Text");
  public static readonly AttachedProperty<bool> ShadowProperty = AvaloniaProperty.RegisterAttached<Text, Control, bool>("Shadow");

  public static string GetText(AvaloniaObject element) => element.GetValue(TextProperty);
  public static void SetText(AvaloniaObject element, string value) => element.SetValue(TextProperty, value);
  public static bool GetShadow(AvaloniaObject element) => element.GetValue(ShadowProperty);
  public static void SetShadow(AvaloniaObject element, bool value) => element.SetValue(ShadowProperty, value);
}
