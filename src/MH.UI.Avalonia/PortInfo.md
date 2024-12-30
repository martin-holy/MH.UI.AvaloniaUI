## DataTemplate
- Change ResourceDictionary to DataTemplates
- Change xmlns http://schemas.microsoft.com/winfx/2006/xaml/presentation to https://github.com/avaloniaui
- Include file in csproj <AvaloniaResource Include="Views\Layout\LeftContentV.axaml" />
- Load file in App._loadDataTemplates()

## Style
- Change ResourceDictionary to Styles
- Change xmlns http://schemas.microsoft.com/winfx/2006/xaml/presentation to https://github.com/avaloniaui
- definition:
	- WPF: <Style x:Key="MH.S.IconTextBlockItemsControl" TargetType="{x:Type c:IconTextBlockItemsControl}">
	- Avalonia: <Style Selector="c|IconTextBlockItemsControl">
- Avalonia doesn't have style inheritance so try to use multiple Styles in Classes
- include styles in App.axaml using StyleInclude

## BasedOn Styles
- MH.S.IconTextBlockItemsControl
	- MH.S.IconTextBlockItemsControl.Borders.RoundDark

- Binding.DoNothing => AvaloniaProperty.UnsetValue
- LayoutTransform is RenderTransform


```cs
public class SlidePanelHost : ContentControl {
  private IDisposable? _boundsSubscription;

  public override void OnApplyTemplate() {
    base.OnApplyTemplate();

    var canvas = this.FindControl<Canvas>("Canvas");
    if (canvas != null && _boundsSubscription == null) {
      _boundsSubscription = canvas.GetObservable(BoundsProperty).Subscribe(OnBoundsChanged);
    }
  }

  protected override void OnDetachedFromVisualTree(global::Avalonia.VisualTree.VisualTreeAttachmentEventArgs e) {
    base.OnDetachedFromVisualTree(e);
    _boundsSubscription?.Dispose();
    _boundsSubscription = null;
  }

  private void OnBoundsChanged(Rect bounds) {
    // Handle bounds change
  }
}
```