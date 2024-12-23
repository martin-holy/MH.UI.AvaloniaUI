using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace MH.UI.Avalonia.Controls;

public enum IconTextBlockShadow { None, Icon, Text, Both }

public class IconTextBlock : Control {
  public static readonly StyledProperty<bool> CompactProperty = AvaloniaProperty.Register<IconTextBlock, bool>(nameof(Compact));
  public static readonly StyledProperty<IStyle> TextBorderStyleProperty = AvaloniaProperty.Register<IconTextBlock, IStyle>(nameof(TextBorderStyle));
  public static readonly StyledProperty<IconTextBlockShadow> ShadowProperty = AvaloniaProperty.Register<IconTextBlock, IconTextBlockShadow>(nameof(Shadow));

  public bool Compact { get => GetValue(CompactProperty); set => SetValue(CompactProperty, value); }
  public IStyle TextBorderStyle { get => GetValue(TextBorderStyleProperty); set => SetValue(TextBorderStyleProperty, value); }
  public IconTextBlockShadow Shadow { get => GetValue(ShadowProperty); set => SetValue(ShadowProperty, value); }
}


public class IconButton : Button;
public class IconTextButton : Button;
public class SlimButton : Button;
public class IconToggleButton : ToggleButton;

public class IconTextBlockItemsControl : ItemsControl;