using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;
using System.ComponentModel;

namespace MH.UI.AvaloniaUI.Controls;

public class RangeSlider : TemplatedControl {
  private Control? _sliderContainer;
  private Control? _startArea;
  private Control? _selectedArea;
  private Control? _endArea;
  private Thumb? _startThumb;
  private Thumb? _endThumb;
  private bool _arrangeIsPending;

  public static readonly StyledProperty<SelectionRange?> RangeProperty =
    AvaloniaProperty.Register<RangeSlider, SelectionRange?>(nameof(Range));

  public static readonly StyledProperty<Orientation> OrientationProperty =
    AvaloniaProperty.Register<RangeSlider, Orientation>(nameof(Orientation));

  public static readonly StyledProperty<double> TickFrequencyProperty =
    AvaloniaProperty.Register<RangeSlider, double>(nameof(TickFrequency), 1.0);

  public SelectionRange? Range { get => GetValue(RangeProperty); set => SetValue(RangeProperty, value); }
  public Orientation Orientation { get => GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }
  public double TickFrequency { get => GetValue(TickFrequencyProperty); set => SetValue(TickFrequencyProperty, value); }

  static RangeSlider() {
    IsVisibleProperty.Changed.AddClassHandler<RangeSlider>((o, _) => o._onIsVisibleChanged());
    RangeProperty.Changed.AddClassHandler<RangeSlider>(_onRangePropertyChanged);
    Thumb.DragDeltaEvent.AddClassHandler<RangeSlider>((o, e) => o._onThumbDragDelta(e));
    Thumb.DragCompletedEvent.AddClassHandler<RangeSlider>((o, _) => o.Range?.RaiseChanged());
  }

  private void _onIsVisibleChanged() {
    if (!_arrangeIsPending) return;
    _arrangeIsPending = false;
    InvalidateArrange();
  }

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);

    _sliderContainer = e.NameScope.Find<Control>("PART_SliderContainer");
    _sliderContainer?.AddHandler(PointerPressedEvent, _onSliderPointerPressed, RoutingStrategies.Tunnel);
    _startArea = e.NameScope.Find<Control>("PART_StartArea");
    _selectedArea = e.NameScope.Find<Control>("PART_SelectedArea");
    _endArea = e.NameScope.Find<Control>("PART_EndArea");
    _startThumb = e.NameScope.Find<Thumb>("PART_StartThumb");
    _endThumb = e.NameScope.Find<Thumb>("PART_EndThumb");
  }

  // TODO PORT Vertical orientation
  protected override Size ArrangeOverride(Size bounds) {
    var arrangeSize = base.ArrangeOverride(bounds);
    if (!IsVisible) { _arrangeIsPending = true; return arrangeSize; }
    if (_startArea == null || _selectedArea == null || _endArea == null) return arrangeSize;

    var size = _sliderContainer?.Bounds.Width ?? bounds.Width;
    var start = Range == null || Range.Start == 0 ? 0 : (Range.Start - Range.Min) / (Range.Max - Range.Min) * size;
    var end = Range == null || Range.End == 0 ? size : (Range.End - Range.Min) / (Range.Max - Range.Min) * size;
    if (double.IsNaN(start) || start < 0) start = 0;
    if (double.IsNaN(end) || end > size) end = size;
    var rectStart = new Rect(0, 0, start, bounds.Height);
    var rectSelected = new Rect(start, 0, end - start, bounds.Height);
    var rectEnd = new Rect(end, 0, size - end, bounds.Height);

    _startArea.Arrange(rectStart);
    _selectedArea.Arrange(rectSelected);
    _endArea.Arrange(rectEnd);
    _startThumb?.Arrange(rectStart);
    _endThumb?.Arrange(rectEnd);

    return arrangeSize;
  }

  private void _onSliderPointerPressed(object? sender, PointerPressedEventArgs e) {
    if (Range == null || _startThumb?.IsPointerOver == true || _endThumb?.IsPointerOver == true) return;

    var point = e.GetPosition(_sliderContainer);
    if (e.GetCurrentPoint(_sliderContainer).Properties.IsLeftButtonPressed)
      _moveThumbTo(point.X, true);
    else if (e.GetCurrentPoint(_sliderContainer).Properties.IsRightButtonPressed)
      _moveThumbTo(point.X, false);

    e.Handled = true;
  }

  private void _moveThumbTo(double position, bool start) {
    var size = _sliderContainer?.Bounds.Width ?? double.NaN;
    if (double.IsNaN(size) || !(size > 0)) return;
    var value = Math.Min(Range!.Max, Range.Min + (position / size) * (Range.Max - Range.Min)).RoundTo(TickFrequency);

    if (start) Range.Start = Math.Min(Range.End, value);
    else Range.End = Math.Max(Range.Start, value);

    InvalidateArrange();
    Range.RaiseChanged();
  }

  private void _onThumbDragDelta(VectorEventArgs e) {
    if (Range == null || e.Source is not Thumb thumb || _sliderContainer == null) return;
    var change = e.Vector.X / _sliderContainer.Bounds.Width * (Range.Max - Range.Min);

    if (ReferenceEquals(thumb, _startThumb))
      Range.Start = Math.Max(Range.Min, Math.Min(Range.End, Range.Start + change)).RoundTo(TickFrequency);
    else if (ReferenceEquals(thumb, _endThumb))
      Range.End = Math.Min(Range.Max, Math.Max(Range.Start, Range.End + change)).RoundTo(TickFrequency);

    InvalidateArrange();
  }

  private static void _onRangePropertyChanged(RangeSlider o, AvaloniaPropertyChangedEventArgs e) {
    if (e.OldValue is SelectionRange oldRange)
      oldRange.PropertyChanged -= o._onAnyRangePropertyChanged;
    if (e.NewValue is SelectionRange newRange)
      newRange.PropertyChanged += o._onAnyRangePropertyChanged;
    o.InvalidateArrange();
  }

  private void _onAnyRangePropertyChanged(object? sender, PropertyChangedEventArgs e) {
    InvalidateArrange();
  }
}