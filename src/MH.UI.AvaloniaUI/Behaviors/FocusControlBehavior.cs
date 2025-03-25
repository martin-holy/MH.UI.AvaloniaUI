using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace MH.UI.AvaloniaUI.Behaviors;

public class FocusControlBehavior : Behavior<Control> {
  public static readonly StyledProperty<bool> WhenProperty =
    AvaloniaProperty.Register<FocusControlBehavior, bool>(nameof(When), coerce: _coerceWhen);

  public bool When { get => GetValue(WhenProperty); set => SetValue(WhenProperty, value); }

  private static bool _coerceWhen(AvaloniaObject sender, bool value) {
    if (value && sender is FocusControlBehavior { AssociatedObject: { } o })
      o.Focus();

    return value;
  }

  protected override void OnAttached() {
    base.OnAttached();
    if (AssociatedObject != null)
      AssociatedObject.AttachedToVisualTree += _onAttachedToVisualTree;
  }

  protected override void OnDetaching() {
    if (AssociatedObject != null)
      AssociatedObject.AttachedToVisualTree -= _onAttachedToVisualTree;
    base.OnDetaching();
  }

  private void _onAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e) {
    AssociatedObject?.Focus();
  }
}