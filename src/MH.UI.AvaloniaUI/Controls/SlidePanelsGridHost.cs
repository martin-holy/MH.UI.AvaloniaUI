using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using MH.UI.AvaloniaUI.Extensions;
using MH.UI.Controls;
using MH.Utils.Types;

namespace MH.UI.AvaloniaUI.Controls;

public class SlidePanelsGridHost : TemplatedControl, ISlidePanelsGridHost {
  private double _swipeStartX;
  private SlidePanelHost? _leftPanel;

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
    _leftPanel = e.NameScope.Find<SlidePanelHost>("PART_LeftSlidePanel");
  }

  private void _panelSwipeStart(object? sender, PointerPressedEventArgs e) {
    _swipeStartX = e.GetPosition(this).X;
  }

  // TODO PORT allow swipe only from edge of the screen
  private void _panelSwipeRecognize(object? sender, PointerReleasedEventArgs e) {
    var endX = e.GetPosition(this).X;

    var panel = (e.Source as Control)?.FindAncestorOfType<SlidePanelHost>();
    if (_swipeStartX - endX > 50 && ReferenceEquals(panel, _leftPanel) && _leftPanel?.ViewModel != null)
      _leftPanel.ViewModel.IsPinned = false;
    else if (endX - _swipeStartX > 50 && ReferenceEquals(sender, this) && _leftPanel?.ViewModel != null)
      _leftPanel.ViewModel.IsPinned = true;
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