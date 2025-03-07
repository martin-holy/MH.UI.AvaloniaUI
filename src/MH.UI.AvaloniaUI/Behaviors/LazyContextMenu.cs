using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace MH.UI.AvaloniaUI.Behaviors;

public class LazyContextMenu : Behavior<Control> {
  public static readonly StyledProperty<object> MenuValueProperty =
    AvaloniaProperty.Register<LazyContextMenu, object>(nameof(MenuValue));

  public object MenuValue { get => GetValue(MenuValueProperty); set => SetValue(MenuValueProperty, value); }

  protected override void OnAttached() {
    base.OnAttached();
    if (AssociatedObject == null) return;
    AssociatedObject.ContextRequested += _onContextRequested;
    AssociatedObject.ContextMenu = null;
  }

  protected override void OnDetaching() {
    if (AssociatedObject != null)
      AssociatedObject.ContextRequested -= _onContextRequested;
    base.OnDetaching();
  }

  private void _onContextRequested(object? sender, ContextRequestedEventArgs e) {
    if (AssociatedObject is not { ContextMenu: null }) return;
    AssociatedObject.ContextMenu = MenuValue as ContextMenu;
    if (AssociatedObject.ContextMenu == null) return;
    AssociatedObject.ContextMenu.Open(AssociatedObject);
    e.Handled = true;
  }
}