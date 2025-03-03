﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MH.UI.AvaloniaUI.Converters;

namespace MH.UI.AvaloniaUI.AttachedProperties;

public class Icon : AvaloniaObject {
  public static readonly AttachedProperty<Geometry?> DataProperty = AvaloniaProperty.RegisterAttached<Icon, Control, Geometry?>("Data");
  public static readonly AttachedProperty<IBrush?> FillProperty = AvaloniaProperty.RegisterAttached<Icon, Control, IBrush?>("Fill");
  public static readonly AttachedProperty<string?> ResProperty = AvaloniaProperty.RegisterAttached<Icon, Control, string?>("Res");
  public static readonly AttachedProperty<double?> SizeProperty = AvaloniaProperty.RegisterAttached<Icon, Control, double?>("Size");

  public static Geometry? GetData(AvaloniaObject element) => element.GetValue(DataProperty);
  public static void SetData(AvaloniaObject element, Geometry value) => element.SetValue(DataProperty, value);
  public static IBrush? GetFill(AvaloniaObject element) => element.GetValue(FillProperty);
  public static void SetFill(AvaloniaObject element, IBrush value) => element.SetValue(FillProperty, value);
  public static string? GetRes(AvaloniaObject element) => element.GetValue(ResProperty);
  public static void SetRes(AvaloniaObject element, string value) => element.SetValue(ResProperty, value);
  public static double? GetSize(AvaloniaObject element) => element.GetValue(SizeProperty);
  public static void SetSize(AvaloniaObject element, double value) => element.SetValue(SizeProperty, value);

  static Icon() {
    ResProperty.Changed.AddClassHandler<Control>(_onResChanged);
  }

  private static void _onResChanged(Control c, AvaloniaPropertyChangedEventArgs e) {
    if (e.NewValue is not string resName || resName.Length == 0) return;

    if (!c.IsSet(DataProperty))
      c.SetValue(DataProperty, ResourceConverter.Inst.Convert(resName, null));

    if (!c.IsSet(FillProperty))
      c.SetValue(FillProperty, ResourceConverter.Inst.Convert(resName, Resources.Dictionaries.IconToBrush));
  }
}