using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace MH.UI.AvaloniaUI.Controls;

public class FlatTreeItemHost : ContentControl {
  private static readonly Point _invalidPoint = new(double.NaN, double.NaN);
  private Point _pointerDownPoint = _invalidPoint;

  protected override void OnPointerPressed(PointerPressedEventArgs e) {
    base.OnPointerPressed(e);
    _pointerDownPoint = _invalidPoint;
    if (e.Handled) return;
    var p = e.GetCurrentPoint(this);
    if (p.Properties.PointerUpdateKind is PointerUpdateKind.LeftButtonPressed or PointerUpdateKind.RightButtonPressed)
      _pointerDownPoint = p.Position;
  }

  protected override void OnPointerReleased(PointerReleasedEventArgs e) {
    base.OnPointerReleased(e);

    if (!e.Handled &&
        !double.IsNaN(_pointerDownPoint.X) &&
        e.InitialPressMouseButton is MouseButton.Left or MouseButton.Right) {

      var point = e.GetCurrentPoint(this);
      var settings = TopLevel.GetTopLevel(e.Source as Visual)?.PlatformSettings;
      var tapSize = settings?.GetTapSize(point.Pointer.Type) ?? new Size(4, 4);
      var tapRect = new Rect(_pointerDownPoint, new Size())
        .Inflate(new Thickness(tapSize.Width, tapSize.Height));

      if (new Rect(Bounds.Size).ContainsExclusive(point.Position) &&
          tapRect.ContainsExclusive(point.Position) &&
          ItemsControl.ItemsControlFromItemContainer(this) is TreeViewHost2 owner) {
        if (owner.UpdateSelectionFromPointerEvent(this, e)) {
          if (e.Pointer.Type != PointerType.Mouse) {
            var sourceBackup = e.Source;
            owner.RaiseEvent(e);
            e.Source = sourceBackup;
          }

          e.Handled = true;
        }
      }
    }

    _pointerDownPoint = _invalidPoint;
  }
}