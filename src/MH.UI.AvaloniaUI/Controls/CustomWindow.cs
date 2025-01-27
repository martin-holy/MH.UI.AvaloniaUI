using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using MH.Utils.BaseClasses;

namespace MH.UI.AvaloniaUI.Controls;

public class CustomWindow : Window {
  private Border? _partResizeBorder;

  public static readonly StyledProperty<bool> CanFullScreenProperty = AvaloniaProperty.Register<CustomWindow, bool>(nameof(CanFullScreen));
  public static readonly StyledProperty<bool> IsFullScreenProperty = AvaloniaProperty.Register<CustomWindow, bool>(nameof(IsFullScreen));
  public static readonly StyledProperty<Window> IsDragAreaForProperty = AvaloniaProperty.RegisterAttached<CustomWindow, Control, Window>("IsDragAreaFor");

  public bool CanFullScreen { get => GetValue(CanFullScreenProperty); set => SetValue(CanFullScreenProperty, value); }
  public bool IsFullScreen { get => GetValue(IsFullScreenProperty); set => SetValue(IsFullScreenProperty, value); }

  public static Window GetIsDragAreaFor(AvaloniaObject o) => o.GetValue(IsDragAreaForProperty);
  public static void SetIsDragAreaFor(AvaloniaObject o, Window value) => o.SetValue(IsDragAreaForProperty, value);

  private const int _resizeCornerSize = 10;
  private const int _resizeBorderSize = 4;

  public static RelayCommand<Window> MinimizeWindowCommand { get; } = new(
    x => x!.WindowState = WindowState.Minimized, x => x != null);

  public static RelayCommand<Window> MaximizeWindowCommand { get; } = new(
    x => {
      x!.MaxHeight = double.PositiveInfinity;
      x.WindowState = WindowState.Maximized;
    }, x => x != null);

  public static RelayCommand<Window> RestoreWindowCommand { get; } = new(
    x => x!.WindowState = WindowState.Normal, x => x != null);

  public static RelayCommand<Window> CloseWindowCommand { get; } = new(
    x => x!.Close(), x => x != null);

  public static RelayCommand<CustomWindow> ToggleFullScreenCommand { get; } = new(
    x => x!.IsFullScreen = !x.IsFullScreen, x => x != null);

  static CustomWindow() {
    IsFullScreenProperty.Changed.AddClassHandler<CustomWindow>(_onIsFullScreenChanged);
    IsDragAreaForProperty.Changed.AddClassHandler<Control>(_onIsDragAreaChanged);
    WindowStateProperty.Changed.AddClassHandler<CustomWindow>(_onWindowStateChanged);
  }

  public CustomWindow() {
    Loaded += delegate {
      if (WindowState != WindowState.Maximized) return;
      var isFullScreen = IsFullScreen;
      WindowState = WindowState.Normal;
      if (isFullScreen) IsFullScreen = true;
      WindowState = WindowState.Maximized;
    };
  }

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);
    _partResizeBorder = e.NameScope.Find<Border>("PART_ResizeBorder");
    if (_partResizeBorder is null) return;
    _partResizeBorder.PointerEntered += _onResizePointerEntered;
    _partResizeBorder.PointerExited += _onResizePointerExited;
    _partResizeBorder.PointerPressed += _onResizePointerPressed;
  }

  protected override Type StyleKeyOverride => typeof(CustomWindow);

  private void _onResizePointerEntered(object? o, PointerEventArgs e) =>
    _setCursor(e.GetCurrentPoint(this).Position);

  private void _onResizePointerExited(object? o, PointerEventArgs e) =>
    _resetCursor(e.GetCurrentPoint(this));

  private void _onResizePointerPressed(object? o, PointerPressedEventArgs e) {
    var point = e.GetCurrentPoint(this);
    if (!point.Properties.IsLeftButtonPressed) return;
    BeginResizeDrag(_getResizeEdge(point.Position), e);
  }

  private static void _onIsFullScreenChanged(CustomWindow o, AvaloniaPropertyChangedEventArgs e) =>
    o._onIsFullScreenChanged();

  private static void _onIsDragAreaChanged(Control o, AvaloniaPropertyChangedEventArgs e) {
    if (e.NewValue is Window window)
      window.PointerPressed += (po, pe) => {
        if (po is Window w && pe.GetCurrentPoint(w).Properties.IsLeftButtonPressed)
          w.BeginMoveDrag(pe);
      };
  }

  private static void _onWindowStateChanged(CustomWindow o, AvaloniaPropertyChangedEventArgs e) =>
    o._onStateChanged();

  private void _onStateChanged() {
    if (WindowState == WindowState.Normal && IsFullScreen)
      IsFullScreen = false;

    if (WindowState == WindowState.Maximized && !IsFullScreen)
      MaxHeight = Screens.Primary?.WorkingArea.Height ?? double.PositiveInfinity; // TODO PORT test it
  }

  private void _onIsFullScreenChanged() {
    MaxHeight = IsFullScreen
      ? double.PositiveInfinity
      : Screens.Primary?.WorkingArea.Height ?? double.PositiveInfinity; // TODO PORT test it

    if (IsFullScreen && WindowState != WindowState.Maximized)
      WindowState = WindowState.Maximized;
  }

  private void _setCursor(Point position) =>
    Cursor = _windowEdgeToCursor(_getResizeEdge(position));

  private void _resetCursor(PointerPoint point) {
    if (!point.Properties.IsLeftButtonPressed)
      Cursor = new(StandardCursorType.Arrow);
  }

  private WindowEdge _getResizeEdge(Point position) {
    if (position.X > _resizeCornerSize && position.X < Bounds.Width - _resizeCornerSize) {
      if (position.Y < _resizeBorderSize) return WindowEdge.North;
      if (position.Y > Bounds.Height - _resizeBorderSize) return WindowEdge.South;
    }
    else if (position.Y > _resizeCornerSize && position.Y < Bounds.Height - _resizeCornerSize) {
      if (position.X < _resizeBorderSize) return WindowEdge.West;
      if (position.X > Bounds.Width - _resizeBorderSize) return WindowEdge.East;
    }
    else if (position.X < _resizeCornerSize) {
      if (position.Y < _resizeCornerSize) return WindowEdge.NorthWest;
      if (position.Y > Bounds.Height - _resizeCornerSize) return WindowEdge.SouthWest;
    }
    else if (position.X > Bounds.Width - _resizeCornerSize) {
      if (position.Y < _resizeCornerSize) return WindowEdge.NorthEast;
      if (position.Y > Bounds.Height - _resizeCornerSize) return WindowEdge.SouthEast;
    }

    return WindowEdge.NorthWest;
  }

  private static Cursor _windowEdgeToCursor(WindowEdge edge) =>
    new(edge switch {
      WindowEdge.West => StandardCursorType.SizeWestEast,
      WindowEdge.East => StandardCursorType.SizeWestEast,
      WindowEdge.North => StandardCursorType.SizeNorthSouth,
      WindowEdge.NorthWest => StandardCursorType.TopLeftCorner,
      WindowEdge.NorthEast => StandardCursorType.TopRightCorner,
      WindowEdge.South => StandardCursorType.SizeNorthSouth,
      WindowEdge.SouthWest => StandardCursorType.BottomLeftCorner,
      WindowEdge.SouthEast => StandardCursorType.BottomRightCorner,
      _ => StandardCursorType.Arrow,
    });
}