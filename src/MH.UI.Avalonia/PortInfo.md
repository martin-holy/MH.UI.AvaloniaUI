- Define DataTemplate in DataTemplates and not in ResourceDictionary
- Change xmlns http://schemas.microsoft.com/winfx/2006/xaml/presentation to https://github.com/avaloniaui

## Style
- definition:
	- WPF: <Style x:Key="MH.S.IconTextBlockItemsControl" TargetType="{x:Type c:IconTextBlockItemsControl}">
	- Avalonia: <Style x:Key="MH.S.IconTextBlockItemsControl" Selector="c|IconTextBlockItemsControl">
- Avalonia doesn't have style inheritance so try to use multiple Styles in Classes