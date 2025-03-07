## DataTemplate
- Change xmlns http://schemas.microsoft.com/winfx/2006/xaml/presentation to https://github.com/avaloniaui
  - DataTemplate without x:Key
    - Change ResourceDictionary to DataTemplates
    - Include file in csproj <AvaloniaResource Include="Views\Layout\LeftContentV.axaml" />
    - Load file in App._loadDataTemplates()
  - DataTemplate with x:Key
    - Include resource in App.axaml <ResourceInclude Source="avares://MH.UI.AvaloniaUI/Resources/DataTemplates/IListItem.axaml"/>

## Style
- Change ResourceDictionary to Styles
- Change xmlns http://schemas.microsoft.com/winfx/2006/xaml/presentation to https://github.com/avaloniaui
- definition:
	- WPF: <Style x:Key="MH.S.IconTextBlockItemsControl" TargetType="{x:Type c:IconTextBlockItemsControl}">
	- Avalonia: <Style Selector="c|IconTextBlockItemsControl">
- Avalonia doesn't have style inheritance but ControlTheme have it
- include styles in App.axaml using StyleInclude

## BasedOn Styles
- MH.S.IconTextBlockItemsControl
	- MH.S.IconTextBlockItemsControl.Borders.RoundDark

- Binding.DoNothing => AvaloniaProperty.UnsetValue
- LayoutTransform is LayoutTransformControl
- AttachedProperties in ContntrolTemplate can't be bind like this: {Binding Path=(ap:Icon.Data)} but like this: {TemplateBinding (ap:Icon.Data)}
- FocusVisualStyle is FocusAdorner


## Info
- ResourceConverter doesn't look for resources in DataTemplate! so check if they are some resources in DataTemplates.
- There is problem with ContentPresenter.DataContext in DataTemplate when just Content is set. So do this:
  <ContentPresenter DataContext="{Binding SlidePanelPinButton}" Content="{Binding}"/>

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

```
<Style Selector="TickBar">
  <Setter Property="ReservedSpace" Value="{Binding #PART_Track.Thumb.Bounds}"/>
</Style>

DockPanel.Dock="{Binding $parent[TabControl].((uic:TabControl)DataContext).TabStrip.SlotPlacement, Converter={x:Static conv:DockConverter.Inst}}"

IsVisible="{Binding $parent[ToggleButton].IsChecked}"

Content="{Binding Path=(ap:Slot.TopContent), RelativeSource={RelativeSource TemplatedParent}}"

{Binding IsExpanded, DataType=uInt:ITreeItem}
```