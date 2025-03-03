﻿using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls.Primitives;
using MH.UI.AvaloniaUI.Extensions;
using MH.UI.Controls;
using MH.Utils.Types;

namespace MH.UI.AvaloniaUI.Controls;

public class SlidePanelHost : TemplatedControl, ISlidePanelHost {
  private Thickness _openTo;
  private TimeSpan _openDuration;

  private Thickness _closeTo;
  private TimeSpan _closeDuration;

  public static readonly StyledProperty<SlidePanel?> ViewModelProperty = AvaloniaProperty.Register<SlidePanelHost, SlidePanel?>(nameof(ViewModel));

  public SlidePanel? ViewModel { get => GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

  private static void _onViewModelChanged(SlidePanelHost o, AvaloniaPropertyChangedEventArgs e) {
    if (o.ViewModel != null) o.ViewModel.Host = o;
  }

  static SlidePanelHost() {
    BoundsProperty.Changed.AddClassHandler<SlidePanelHost>(_onBoundsChanged);
    ViewModelProperty.Changed.AddClassHandler<SlidePanelHost>(_onViewModelChanged);
  }

  public event EventHandler<MH.Utils.EventsArgs.SizeChangedEventArgs>? HostSizeChangedEvent;

  private void _raiseHostSizeChanged(MH.Utils.EventsArgs.SizeChangedEventArgs e) =>
    HostSizeChangedEvent?.Invoke(this, e);

  private static void _onBoundsChanged(SlidePanelHost o, AvaloniaPropertyChangedEventArgs e) {
    if (!ReferenceEquals(o, e.Sender)) return;
    var oldSize = (e.OldValue is Rect oldRect ? oldRect : default).Size.ToSizeD();
    var newSize = (e.NewValue is Rect newRect ? newRect : default).Size.ToSizeD();
    var widthChanged = Math.Abs(newSize.Width - oldSize.Width) > 1;
    var heightChanged = Math.Abs(newSize.Height - oldSize.Height) > 1;
    if (!widthChanged && !heightChanged) return;
    o._raiseHostSizeChanged(new(oldSize, newSize, widthChanged, heightChanged));
  }

  public void OpenAnimation() =>
    AnimateMargin(_openTo, _openDuration);

  public void CloseAnimation() =>
    AnimateMargin(_closeTo, _closeDuration);

  public void UpdateOpenAnimation(ThicknessD from, ThicknessD to, TimeSpan duration) {
    _openTo = to.FromThicknessD();
    _openDuration = duration;
  }

  public void UpdateCloseAnimation(ThicknessD from, ThicknessD to, TimeSpan duration) {
    _closeTo = to.FromThicknessD();
    _closeDuration = duration;
  }

  private void AnimateMargin(Thickness to, TimeSpan duration) {
    var transition = new ThicknessTransition {
      Duration = duration,
      Property = MarginProperty,
      Easing = new CubicEaseInOut()
    };

    Transitions = [transition];
    Margin = to;
  }
}