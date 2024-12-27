using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using MH.UI.Avalonia.Extensions;
using MH.UI.Controls;
using MH.Utils.Types;

namespace MH.UI.Avalonia.Controls;

public class SlidePanelsGridHost : TemplatedControl, ISlidePanelsGridHost {
  public static readonly StyledProperty<SlidePanelsGrid?> ViewModelProperty = AvaloniaProperty.Register<SlidePanelsGridHost, SlidePanelsGrid?>(nameof(ViewModel));

  public SlidePanelsGrid? ViewModel { get => GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

  private static void _onViewModelChanged(SlidePanelsGridHost o, AvaloniaPropertyChangedEventArgs e) {
    if (o.ViewModel != null) o.ViewModel.Host = o;
  }

  static SlidePanelsGridHost() {
    ViewModelProperty.Changed.AddClassHandler<SlidePanelsGridHost>(_onViewModelChanged);
  }

  public event EventHandler<(PointD Position, double Width, double Height)>? HostMouseMoveEvent;

  protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
    base.OnAttachedToVisualTree(e);
    AddHandler(PointerMovedEvent, _onPointerMoved, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
  }

  protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
    base.OnDetachedFromVisualTree(e);
    RemoveHandler(PointerMovedEvent, _onPointerMoved);
  }

  private void _onPointerMoved(object? sender, PointerEventArgs e) =>
    HostMouseMoveEvent?.Invoke(this, new(e.GetPosition(this).ToPointD(), Width, Height));
}