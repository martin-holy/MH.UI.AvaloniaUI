using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace MH.UI.AvaloniaUI.Controls;

public class PopupSlider : Slider {
  public static readonly StyledProperty<Button?> ContentProperty = AvaloniaProperty.Register<PopupSlider, Button?>(nameof(Content));
  public static readonly StyledProperty<ICommand?> PopupClosedCommandProperty = AvaloniaProperty.Register<PopupSlider, ICommand?>(nameof(PopupClosedCommand));

  public Button? Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }
  public ICommand? PopupClosedCommand { get => GetValue(PopupClosedCommandProperty); set => SetValue(PopupClosedCommandProperty, value); }

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);
    if (Content == null || e.NameScope.Find<Popup>("PART_Popup") is not { } popup) return;

    Content.Click += delegate { popup.IsOpen = true; };

    popup.AddHandler(PointerReleasedEvent, _onPopupPointerReleased, RoutingStrategies.Tunnel);
    popup.CustomPopupPlacementCallback +=_customPopupPlacementCallback;
  }

  private void _onPopupPointerReleased(object? sender, PointerReleasedEventArgs e) {
    if (sender is Popup popup)
      popup.IsOpen = false;

    if (PopupClosedCommand?.CanExecute(null) == true)
      PopupClosedCommand.Execute(null);
  }

  private static void _customPopupPlacementCallback(CustomPopupPlacement parameters) {
    var targetSize = parameters.Target.Bounds.Size;
    var offsetX = (targetSize.Width - parameters.PopupSize.Width) / 2;
    parameters.Anchor = PopupAnchor.TopLeft;
    parameters.Gravity = PopupGravity.BottomRight;
    parameters.Offset = new(offsetX, targetSize.Height);
  }
}