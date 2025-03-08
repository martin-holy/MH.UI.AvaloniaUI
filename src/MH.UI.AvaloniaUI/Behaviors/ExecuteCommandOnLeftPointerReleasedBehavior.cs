using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
using System.Reactive.Disposables;

namespace MH.UI.AvaloniaUI.Behaviors;

public class ExecuteCommandOnLeftPointerReleasedBehavior : ExecuteCommandRoutedEventBehaviorBase {
  protected override void OnAttachedToVisualTree(CompositeDisposable disposable) {
    var control = SourceControl ?? AssociatedObject;
    if (control == null) return;
    disposable.Add(control.AddDisposableHandler(InputElement.PointerReleasedEvent, _onPointerReleased, EventRoutingStrategy));
  }

  private void _onPointerReleased(object? sender, PointerReleasedEventArgs e) {
    if (e.Handled || e.InitialPressMouseButton != MouseButton.Left) return;
    if (ExecuteCommand()) e.Handled = MarkAsHandled;
  }
}