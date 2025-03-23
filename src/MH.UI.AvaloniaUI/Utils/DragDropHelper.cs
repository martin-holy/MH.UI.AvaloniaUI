using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using static MH.Utils.DragDropHelper;

namespace MH.UI.AvaloniaUI.Utils;

public class DragDropHelper : AvaloniaObject {
  private static Point _dragStartPosition;
  private static bool _isActive;
  private static object? _sourceControl;
  private static object? _data;
  private const double _dragThreshold = 3.0;

  public static DragEventArgs? DragEventArgs { get; set; }

  public static readonly AttachedProperty<bool> IsDragEnabledProperty =
    AvaloniaProperty.RegisterAttached<DragDropHelper, Control, bool>("IsDragEnabled", coerce: _onIsDragEnabledChanged);

  public static readonly AttachedProperty<bool> IsDropEnabledProperty =
    AvaloniaProperty.RegisterAttached<DragDropHelper, Control, bool>("IsDropEnabled", coerce: _onIsDropEnabledChanged);

  public static readonly AttachedProperty<Type?> DragDataTypeProperty =
    AvaloniaProperty.RegisterAttached<DragDropHelper, Control, Type?>("DragDataType");

  public static readonly AttachedProperty<CanDragFunc?> CanDragProperty =
    AvaloniaProperty.RegisterAttached<DragDropHelper, Control, CanDragFunc?>("CanDrag");

  public static readonly AttachedProperty<CanDropFunc?> CanDropProperty =
    AvaloniaProperty.RegisterAttached<DragDropHelper, Control, CanDropFunc?>("CanDrop");

  public static readonly AttachedProperty<DoDropAction?> DoDropProperty =
    AvaloniaProperty.RegisterAttached<DragDropHelper, Control, DoDropAction?>("DoDrop");

  public static readonly AttachedProperty<string?> DataFormatProperty =
    AvaloniaProperty.RegisterAttached<DragDropHelper, Control, string?>("DataFormat");

  public static bool GetIsDragEnabled(Control d) => d.GetValue(IsDragEnabledProperty);
  public static void SetIsDragEnabled(Control d, bool value) => d.SetValue(IsDragEnabledProperty, value);
  public static bool GetIsDropEnabled(Control d) => d.GetValue(IsDropEnabledProperty);
  public static void SetIsDropEnabled(Control d, bool value) => d.SetValue(IsDropEnabledProperty, value);
  public static Type? GetDragDataType(Control d) => d.GetValue(DragDataTypeProperty);
  public static void SetDragDataType(Control d, Type? value) => d.SetValue(DragDataTypeProperty, value);
  public static CanDragFunc? GetCanDrag(Control d) => d.GetValue(CanDragProperty);
  public static void SetCanDrag(Control d, CanDragFunc? value) => d.SetValue(CanDragProperty, value);
  public static CanDropFunc? GetCanDrop(Control d) => d.GetValue(CanDropProperty);
  public static void SetCanDrop(Control d, CanDropFunc? value) => d.SetValue(CanDropProperty, value);
  public static DoDropAction? GetDoDrop(Control d) => d.GetValue(DoDropProperty);
  public static void SetDoDrop(Control d, DoDropAction? value) => d.SetValue(DoDropProperty, value);
  public static string? GetDataFormat(Control d) => d.GetValue(DataFormatProperty);
  public static void SetDataFormat(Control d, string? value) => d.SetValue(DataFormatProperty, value);

  private static bool _onIsDragEnabledChanged(AvaloniaObject o, bool value) {
    if (o is not Control control)
      throw new InvalidOperationException();

    if (value) {
      control.AddHandler(InputElement.PointerPressedEvent, _startDrag, RoutingStrategies.Tunnel);
      control.AddHandler(InputElement.PointerMovedEvent, _dragging, RoutingStrategies.Tunnel);
    }
    else {
      control.RemoveHandler(InputElement.PointerPressedEvent, _startDrag);
      control.RemoveHandler(InputElement.PointerMovedEvent, _dragging);
    }
    return value;
  }

  private static bool _onIsDropEnabledChanged(AvaloniaObject o, bool value) {
    if (o is not Control control)
      throw new InvalidOperationException();

    if (value) {
      DragDrop.SetAllowDrop(control, true);
      control.AddHandler(DragDrop.DragEnterEvent, _allowDropCheck, RoutingStrategies.Bubble);
      control.AddHandler(DragDrop.DragLeaveEvent, _allowDropCheck, RoutingStrategies.Bubble);
      control.AddHandler(DragDrop.DragOverEvent, _allowDropCheck, RoutingStrategies.Bubble);
      control.AddHandler(DragDrop.DropEvent, _drop, RoutingStrategies.Bubble);
    }
    else {
      DragDrop.SetAllowDrop(control, false);
      control.RemoveHandler(DragDrop.DragEnterEvent, _allowDropCheck);
      control.RemoveHandler(DragDrop.DragLeaveEvent, _allowDropCheck);
      control.RemoveHandler(DragDrop.DragOverEvent, _allowDropCheck);
      control.RemoveHandler(DragDrop.DropEvent, _drop);
    }

    return value;
  }

  private static void _startDrag(object? sender, PointerPressedEventArgs e) {
    if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed || sender is not Control control) return;
    _dragStartPosition = e.GetPosition(control);
    _isActive = true;
  }

  private static void _dragging(object? sender, PointerEventArgs e) {
    if (!_isActive || !e.GetCurrentPoint(null).Properties.IsLeftButtonPressed || !_hasDragStarted(e) ||
        sender is not Control control) return;

    var dragData = (e.Source as Control)?.DataContext;
    var dragDataType = GetDragDataType(control);
    var canDrag = GetCanDrag(control);

    var data = dragData == null
      ? null
      : canDrag != null
        ? canDrag(dragData)
        : dragDataType == null
          ? dragData
          : dragDataType == dragData.GetType()
            ? dragData
            : null;

    if (data == null) {
      _reset();
      return;
    }

    _sourceControl = sender;
    _data = data;
    var dataFormat = GetDataFormat(control);
    var dataObject = new DataObject();
    dataObject.Set(dataFormat ?? string.Empty, data);
    DragDrop.DoDragDrop(e, dataObject, DragDropEffects.None | DragDropEffects.Copy | DragDropEffects.Move).ContinueWith(_ => _reset());
  }

  private static bool _hasDragStarted(PointerEventArgs e) {
    var diff = _dragStartPosition - e.GetPosition(null);
    return Math.Abs(diff.X) > _dragThreshold || Math.Abs(diff.Y) > _dragThreshold;
  }

  private static void _allowDropCheck(object? sender, DragEventArgs e) {
    DragEventArgs = e;
    var target = (e.Source as Control)?.DataContext;
    var effects = GetCanDrop((Control)sender!)?.Invoke(target, _data, ReferenceEquals(sender, _sourceControl));
    e.DragEffects = _convertEffects(effects ?? MH.Utils.DragDropEffects.None);
    e.Handled = true;
  }

  private static void _drop(object? sender, DragEventArgs e) {
    if (_data == null) return;
    GetDoDrop((Control)sender!)?.Invoke(_data, ReferenceEquals(sender, _sourceControl));
  }

  private static DragDropEffects _convertEffects(MH.Utils.DragDropEffects effects) =>
    effects switch {
      MH.Utils.DragDropEffects.All => DragDropEffects.None | DragDropEffects.Copy | DragDropEffects.Move,
      MH.Utils.DragDropEffects.None => DragDropEffects.None,
      MH.Utils.DragDropEffects.Copy => DragDropEffects.Copy,
      MH.Utils.DragDropEffects.Move => DragDropEffects.Move,
      MH.Utils.DragDropEffects.Link => DragDropEffects.Link,
      _ => throw new ArgumentOutOfRangeException(nameof(effects), effects, null)
    };

  private static void _reset() {
    _isActive = false;
    _sourceControl = null;
    _data = null;
  }
}