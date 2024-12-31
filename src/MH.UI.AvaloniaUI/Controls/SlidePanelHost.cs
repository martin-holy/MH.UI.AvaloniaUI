using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using MH.UI.AvaloniaUI.Extensions;
using MH.UI.Controls;
using MH.Utils.Types;

namespace MH.UI.AvaloniaUI.Controls;

public class SlidePanelHost : TemplatedControl, ISlidePanelHost {
  private IDisposable? _boundsSubscription;

  private Thickness _openFrom;
  private Thickness _openTo;
  private TimeSpan _openDuration;

  private Thickness _closeFrom;
  private Thickness _closeTo;
  private TimeSpan _closeDuration;

  public static readonly StyledProperty<SlidePanel?> ViewModelProperty = AvaloniaProperty.Register<SlidePanelHost, SlidePanel?>(nameof(ViewModel));

  public SlidePanel? ViewModel { get => GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

  private static void _onViewModelChanged(SlidePanelHost o, AvaloniaPropertyChangedEventArgs e) {
    if (o.ViewModel != null) o.ViewModel.Host = o;
  }

  static SlidePanelHost() {
    ViewModelProperty.Changed.AddClassHandler<SlidePanelHost>(_onViewModelChanged);
  }

  public event EventHandler<MH.Utils.EventsArgs.SizeChangedEventArgs>? HostSizeChangedEvent;

  protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
    base.OnAttachedToVisualTree(e);
    _boundsSubscription = BoundsProperty.Changed.AddClassHandler<SlidePanelHost>(_onBoundsChanged);
  }

  private void _onBoundsChanged(SlidePanelHost o, AvaloniaPropertyChangedEventArgs e) {
    var oldSize = (e.OldValue is Rect oldRect ? oldRect : default).Size.ToSizeD();
    var newSize = (e.NewValue is Rect newRect ? newRect : default).Size.ToSizeD();
    HostSizeChangedEvent?.Invoke(this, new(oldSize, newSize, newSize.Width != oldSize.Width, newSize.Height != oldSize.Height));
  }

  protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
    base.OnDetachedFromVisualTree(e);
    _boundsSubscription?.Dispose();
    _boundsSubscription = null;
  }

  public async void OpenAnimation() {
    await AnimateMarginAsync(_openFrom, _openTo, _openDuration);
  }

  public async void CloseAnimation() {
    await AnimateMarginAsync(_closeFrom, _closeTo, _closeDuration);
  }

  public void UpdateOpenAnimation(ThicknessD from, ThicknessD to, TimeSpan duration) {
    _openFrom = from.FromThicknessD();
    _openTo = to.FromThicknessD();
    _openDuration = duration;
  }

  public void UpdateCloseAnimation(ThicknessD from, ThicknessD to, TimeSpan duration) {
    _closeFrom = from.FromThicknessD();
    _closeTo = to.FromThicknessD();
    _closeDuration = duration;
  }

  private async Task AnimateMarginAsync(Thickness from, Thickness to, TimeSpan duration) {
    var animation = new Animation {
      Duration = duration,
      Easing = new CubicEaseInOut(),
      Children = {
        new() { Cue = new(0), Setters = { new Setter(MarginProperty, from) } },
        new() { Cue = new(1), Setters = { new Setter(MarginProperty, to) } } }
    };

    await animation.RunAsync(this);
  }
}