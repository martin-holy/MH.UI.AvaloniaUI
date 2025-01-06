using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace MH.UI.AvaloniaUI.Controls;

public enum IconTextBlockShadow { None, Icon, Text, Both }

public class IconTextBlock : ContentControl {
  private Grid? _partGrid;

  public static readonly StyledProperty<bool> CompactProperty = AvaloniaProperty.Register<IconTextBlock, bool>(nameof(Compact));
  public static readonly StyledProperty<IStyle> TextBorderStyleProperty = AvaloniaProperty.Register<IconTextBlock, IStyle>(nameof(TextBorderStyle));
  public static readonly StyledProperty<IconTextBlockShadow> ShadowProperty = AvaloniaProperty.Register<IconTextBlock, IconTextBlockShadow>(nameof(Shadow));

  public bool Compact { get => GetValue(CompactProperty); set => SetValue(CompactProperty, value); }
  public IStyle TextBorderStyle { get => GetValue(TextBorderStyleProperty); set => SetValue(TextBorderStyleProperty, value); }
  public IconTextBlockShadow Shadow { get => GetValue(ShadowProperty); set => SetValue(ShadowProperty, value); }

  static IconTextBlock() {
    CompactProperty.Changed.AddClassHandler<IconTextBlock>(_onCompactChanged);
  }

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);
    _partGrid = e.NameScope.Find<Grid>("PART_Grid");
    SetCompactGrid();
  }

  private static void _onCompactChanged(IconTextBlock o, AvaloniaPropertyChangedEventArgs e) =>
    o.SetCompactGrid();

  internal void SetCompactGrid() {
    if (_partGrid == null) return;

    _partGrid.ColumnDefinitions.Clear();
    if (Compact) {
      _partGrid.ColumnDefinitions.Add(new(GridLength.Star));
      _partGrid.ColumnDefinitions.Add(new(GridLength.Auto));
    }
    else {
      _partGrid.ColumnDefinitions.Add(new(GridLength.Auto));
      _partGrid.ColumnDefinitions.Add(new(GridLength.Star));
    }
  }
}

public class IconButton : Button;
public class IconTextButton : Button;
public class SlimButton : Button;
public class IconToggleButton : ToggleButton;

public class IconTextBlockItemsControl : ItemsControl;