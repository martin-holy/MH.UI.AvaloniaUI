using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using MH.UI.AvaloniaUI.Extensions;
using MH.Utils.Types;
using UIC = MH.UI.Controls;

namespace MH.UI.AvaloniaUI.Controls;

public class ZoomAndPanHost : ContentControl, UIC.IZoomAndPanHost {
  private Canvas _canvas = null!;
  private Control _content = null!;

  public static readonly StyledProperty<UIC.ZoomAndPan?> ViewModelProperty =
    AvaloniaProperty.Register<ZoomAndPanHost, UIC.ZoomAndPan?>(nameof(ViewModel));

  public UIC.ZoomAndPan? ViewModel { get => GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

  private static void _onViewModelChanged(ZoomAndPanHost o, AvaloniaPropertyChangedEventArgs e) {
    if (o.ViewModel != null) o.ViewModel.Host = o;
  }

  static ZoomAndPanHost() {
    BoundsProperty.Changed.AddClassHandler<ZoomAndPanHost>(_onBoundsChanged);
    ViewModelProperty.Changed.AddClassHandler<ZoomAndPanHost>(_onViewModelChanged);
  }

  double UIC.IZoomAndPanHost.Width => Bounds.Width;
  double UIC.IZoomAndPanHost.Height => Bounds.Height;

  public event EventHandler? HostSizeChangedEvent;
  public event EventHandler<PointD>? HostMouseMoveEvent;
  public event EventHandler<(PointD, PointD)>? HostMouseDownEvent;
  public event EventHandler? HostMouseUpEvent;
  public event EventHandler<(int, PointD)>? HostMouseWheelEvent;

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);

    _canvas = e.NameScope.Find<Canvas>("PART_Canvas")!;
    _canvas.PointerMoved += _onCanvasPointerMoved;

    _content = e.NameScope.Find<Control>("PART_Content")!;
    _content.PointerPressed += _onContentPointerPressed;
    _content.PointerReleased += _onContentPointerReleased;
    _content.PointerWheelChanged += _onContentPointerWheel;
  }

  // TODO PORT
  public void StartAnimation(double toValue, double duration, bool horizontal, Action onCompleted) {
    throw new NotImplementedException();
  }

  public void StopAnimation() {
    throw new NotImplementedException();
  }

  private static void _onBoundsChanged(ZoomAndPanHost o, AvaloniaPropertyChangedEventArgs e) =>
    o._raiseHostSizeChanged();

  private void _raiseHostSizeChanged() =>
    HostSizeChangedEvent?.Invoke(this, EventArgs.Empty);

  private void _onCanvasPointerMoved(object? sender, PointerEventArgs e) {
    if (!ReferenceEquals(e.Pointer.Captured, _content)) return;
    HostMouseMoveEvent?.Invoke(this, e.GetPosition(_canvas).ToPointD());
  }

  private void _onContentPointerPressed(object? sender, PointerPressedEventArgs e) {
    var canvasPoint = e.GetCurrentPoint(_canvas);
    if (!canvasPoint.Properties.IsLeftButtonPressed) return;
    HostMouseDownEvent?.Invoke(this, new(canvasPoint.Position.ToPointD(), e.GetPosition(_content).ToPointD()));
    _canvas.Cursor = new(StandardCursorType.Hand);
    e.Pointer.Capture(_content);
  }

  private void _onContentPointerReleased(object? sender, PointerReleasedEventArgs e) {
    _canvas.Cursor = new(StandardCursorType.Arrow);
    e.Pointer.Capture(null);
    HostMouseUpEvent?.Invoke(this, EventArgs.Empty);
  }

  private void _onContentPointerWheel(object? sender, PointerWheelEventArgs e) =>
    HostMouseWheelEvent?.Invoke(this, ((int)e.Delta.Y, e.GetPosition(_content).ToPointD()));
}