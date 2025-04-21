using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using MH.UI.AvaloniaUI.Controls;
using MH.UI.AvaloniaUI.Converters;
using MH.Utils.BaseClasses;
using AP = MH.UI.AvaloniaUI.AttachedProperties;

namespace MH.UI.AvaloniaUI.Utils;

public static class RelayCommandHelper {
  public static void Init() {
    Button.CommandProperty.Changed.AddClassHandler<Control>(_onCommandChanged);
    MenuItem.CommandProperty.Changed.AddClassHandler<Control>(_onCommandChanged);
  }

  private static void _onCommandChanged(Control o, AvaloniaPropertyChangedEventArgs e) {
    if (e.NewValue is not RelayCommandBase rc) return;
    _setIconData(o, rc);
    _setText(o, rc);
  }

  private static void _setIconData(Control c, RelayCommandBase rc) {
    if (string.IsNullOrEmpty(rc.Icon)) return;

    switch (c) {
      case Button:
        if (!c.IsSet(AP.Icon.DataProperty) || c.GetValue(AP.Icon.DataProperty) == null)
          c.SetValue(AP.Icon.DataProperty, ResourceConverter.Inst.Convert(rc.Icon, null));
        if (!c.IsSet(AP.Icon.FillProperty) || c.GetValue(AP.Icon.FillProperty) == null)
          c.SetValue(AP.Icon.FillProperty, ResourceConverter.Inst.Convert(rc.Icon, Resources.Dictionaries.IconToBrush));
        break;
      case MenuItem mi:
        mi.Icon ??= _getIcon(rc.Icon);
        break;
    }
  }

  private static void _setText(Control c, RelayCommandBase rc) {
    if (string.IsNullOrEmpty(rc.Text)) return;

    switch (c) {
      case IconButton:
      case IconTextButton:
      case SlimButton:
      case IconToggleButton:
        if (!c.IsSet(ToolTip.TipProperty) || c.GetValue(ToolTip.TipProperty) == null)
          c.SetValue(ToolTip.TipProperty, rc.Text);
        break;
      case Button:
        c.SetValue(AP.Text.TextProperty, rc.Text);
        break;
      case MenuItem mi: mi.Header ??= rc.Text; break;
    }
  }

  private static Avalonia.Controls.Shapes.Path _getIcon(string icon) {
    var path = new Avalonia.Controls.Shapes.Path {
      Classes = { "icon", "shadow" }
    };

    path.SetValue(AP.Icon.DataProperty, ResourceConverter.Inst.Convert(icon, null) as Geometry);
    path.SetValue(AP.Icon.FillProperty, ResourceConverter.Inst.Convert(icon, Resources.Dictionaries.IconToBrush) as Brush);

    return path;
  }
}