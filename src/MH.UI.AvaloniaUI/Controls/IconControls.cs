using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace MH.UI.AvaloniaUI.Controls;

public enum IconTextBlockShadow { None, Icon, Text, Both }

public class IconTextBlock : ContentControl {
  private Grid? _partGrid;
  private Border? _partTextBorder;

  public static readonly StyledProperty<bool> CompactProperty = AvaloniaProperty.Register<IconTextBlock, bool>(nameof(Compact));
  public static readonly StyledProperty<string> TextBorderClassesProperty = AvaloniaProperty.Register<IconTextBlock, string>(nameof(TextBorderClasses));
  public static readonly StyledProperty<IconTextBlockShadow> ShadowProperty = AvaloniaProperty.Register<IconTextBlock, IconTextBlockShadow>(nameof(Shadow));

  public bool Compact { get => GetValue(CompactProperty); set => SetValue(CompactProperty, value); }
  public string TextBorderClasses { get => GetValue(TextBorderClassesProperty); set => SetValue(TextBorderClassesProperty, value); }
  public IconTextBlockShadow Shadow { get => GetValue(ShadowProperty); set => SetValue(ShadowProperty, value); }

  static IconTextBlock() {
    CompactProperty.Changed.AddClassHandler<IconTextBlock>(_onCompactChanged);
    TextBorderClassesProperty.Changed.AddClassHandler<IconTextBlock>(_onTextBorderClassesChanged);
  }

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);
    _partGrid = e.NameScope.Find<Grid>("PART_Grid");
    _partTextBorder = e.NameScope.Find<Border>("PART_TextBorder");
    SetCompactGrid();
    SetTextBorderClasses();
  }

  private static void _onCompactChanged(IconTextBlock o, AvaloniaPropertyChangedEventArgs e) =>
    o.SetCompactGrid();

  private static void _onTextBorderClassesChanged(IconTextBlock o, AvaloniaPropertyChangedEventArgs e) =>
    o.SetTextBorderClasses();

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

  internal void SetTextBorderClasses() {
    if (_partTextBorder == null || string.IsNullOrWhiteSpace(TextBorderClasses)) return;
    _partTextBorder.Classes.Clear();
    foreach (var cls in TextBorderClasses.Split(' ', StringSplitOptions.RemoveEmptyEntries))
      _partTextBorder.Classes.Add(cls);
  }
}

public class IconButton : Button;
public class IconTextButton : Button;
public class SlimButton : Button;
public class IconToggleButton : ToggleButton;

public class IconTextBlockItemsControl : ItemsControl;