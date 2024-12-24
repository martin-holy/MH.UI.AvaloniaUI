- Define DataTemplate in DataTemplates and not in ResourceDictionary
- Change xmlns http://schemas.microsoft.com/winfx/2006/xaml/presentation to https://github.com/avaloniaui
- Set Build action to AvaloniaResource

## Style
- definition:
	- WPF: <Style x:Key="MH.S.IconTextBlockItemsControl" TargetType="{x:Type c:IconTextBlockItemsControl}">
	- Avalonia: <Style Selector="c|IconTextBlockItemsControl">
	- Styles needs to be in Styles and not in ResourceDictionary
- Avalonia doesn't have style inheritance so try to use multiple Styles in Classes
- include styles in App.axaml using StyleInclude

## BasedOn Styles
- MH.S.IconTextBlockItemsControl
	- MH.S.IconTextBlockItemsControl.Borders.RoundDark