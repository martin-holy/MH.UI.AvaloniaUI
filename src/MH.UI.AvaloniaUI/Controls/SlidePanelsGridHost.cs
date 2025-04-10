using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using MH.UI.AvaloniaUI.Extensions;
using MH.UI.Controls;
using MH.Utils.Types;
using Dock = Avalonia.Controls.Dock;

namespace MH.UI.AvaloniaUI.Controls;

public class SlidePanelsGridHost : TemplatedControl, ISlidePanelsGridHost {
  private Point _swipeStart;
  private SlidePanelHost? _leftPanel;
  private SlidePanelHost? _topPanel;
  private SlidePanelHost? _rightPanel;
  private SlidePanelHost? _bottomPanel;

  public static readonly StyledProperty<SlidePanelsGrid?> ViewModelProperty =
    AvaloniaProperty.Register<SlidePanelsGridHost, SlidePanelsGrid?>(nameof(ViewModel));

  public SlidePanelsGrid? ViewModel { get => GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

  private static void _onViewModelChanged(SlidePanelsGridHost o, AvaloniaPropertyChangedEventArgs e) {
    if (o.ViewModel != null) o.ViewModel.Host = o;
  }

  static SlidePanelsGridHost() {
    ViewModelProperty.Changed.AddClassHandler<SlidePanelsGridHost>(_onViewModelChanged);
  }

  public event EventHandler<(PointD Position, double Width, double Height)>? HostMouseMoveEvent;

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);
    _leftPanel = e.NameScope.Find<SlidePanelHost>("PART_LeftPanel");
    _topPanel = e.NameScope.Find<SlidePanelHost>("PART_TopPanel");
    _rightPanel = e.NameScope.Find<SlidePanelHost>("PART_RightPanel");
    _bottomPanel = e.NameScope.Find<SlidePanelHost>("PART_BottomPanel");
  }

  private void _panelSwipeStart(object? sender, PointerPressedEventArgs e) {
    _swipeStart = e.GetPosition(this);
  }

  private void _panelSwipeRecognize(object? sender, PointerReleasedEventArgs e) {
    var edge = _isSwipeFromEdge(Bounds, _swipeStart, e.GetPosition(this));
    if (edge == null) return;

    var panel = (e.Source as Control)?.FindAncestorOfType<SlidePanelHost>();
    var isOpening = panel == null;

    var targetPanel = edge switch {
      Dock.Left => isOpening ? _leftPanel : _rightPanel,
      Dock.Right => isOpening ? _rightPanel : _leftPanel,
      Dock.Top => isOpening ? _topPanel : _bottomPanel,
      Dock.Bottom => isOpening ? _bottomPanel : _topPanel,
      _ => null
    };

    _setPinned(targetPanel, isOpening);
  }

  private static void _setPinned(SlidePanelHost? panel, bool value) {
    if (panel?.ViewModel == null) return;
    panel.ViewModel.IsPinned = value;
  }

  private static Dock? _isSwipeFromEdge(Rect bounds, Point start, Point end) {
    const double edgeThreshold = 50;
    const double minSwipeDistance = 50;

    var deltaX = end.X - start.X;
    var deltaY = end.Y - start.Y;
    var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

    if (distance < minSwipeDistance) return null;

    if (start.X < edgeThreshold && deltaX > 0)
      return Dock.Left;
    if (start.X > bounds.Width - edgeThreshold && deltaX < 0)
      return Dock.Right;
    if (start.Y < edgeThreshold && deltaY > 0)
      return Dock.Top;
    if (start.Y > bounds.Height - edgeThreshold && deltaY < 0)
      return Dock.Bottom;

    return null;
  }

  protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
    base.OnAttachedToVisualTree(e);
    AddHandler(PointerMovedEvent, _onPointerMoved, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    AddHandler(PointerPressedEvent, _panelSwipeStart, RoutingStrategies.Tunnel);
    AddHandler(PointerReleasedEvent, _panelSwipeRecognize, RoutingStrategies.Tunnel);
  }

  protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
    base.OnDetachedFromVisualTree(e);
    RemoveHandler(PointerMovedEvent, _onPointerMoved);
    RemoveHandler(PointerPressedEvent, _panelSwipeStart);
    RemoveHandler(PointerReleasedEvent, _panelSwipeRecognize);
  }

  private void _onPointerMoved(object? sender, PointerEventArgs e) =>
    HostMouseMoveEvent?.Invoke(this, new(e.GetPosition(this).ToPointD(), Bounds.Width, Bounds.Height));
}