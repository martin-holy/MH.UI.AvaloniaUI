﻿using Avalonia;
using Avalonia.Media;

namespace MH.UI.AvaloniaUI.Utils;

public static class ColorHelper {
  public static void AddGradients(string name, Color color, int samples = 9) {
    if (Application.Current == null) return;
    var colorName = $"MH.Color.{name}";
    var brushName = $"MH.B.{name}";

    Application.Current.Resources[colorName] = color;
    Application.Current.Resources[brushName] = CreateBrush(color);

    samples++; // to skip color without transparency
    var size = 255.0 / samples;
    for (var i = 1; i < samples; i++) {
      var gColor = Color.FromArgb((byte)(size * i), color.R, color.G, color.B);
      Application.Current.Resources[$"{colorName}{i}"] = gColor;
      Application.Current.Resources[$"{brushName}{i}"] = CreateBrush(gColor);
    }
  }

  public static void AddVariants(string name, Color color) {
    if (Application.Current == null) return;
    var colorName = $"MH.Color.{name}";
    var brushName = $"MH.B.{name}";

    MH.Utils.Imaging.RgbToHsl(color.R, color.G, color.B, out var h, out var s, out var l);
    l = 50;
    MH.Utils.Imaging.HslToRgb(h, s, l, out var r, out var g, out var b);
    var darkColor = Color.FromRgb(r, g, b);
    Application.Current.Resources[$"{colorName}-Dark"] = darkColor;
    Application.Current.Resources[$"{brushName}-Dark"] = CreateBrush(darkColor);
  }

  private static Brush CreateBrush(Color color) => new SolidColorBrush(color);

  public static void AddColorsToResources() {
    var accent = _getSystemAccentColor();
    AddVariants("Accent", accent);
    AddGradients("Accent", accent);
    AddGradients("Black", Color.FromRgb(0, 0, 0));
    AddGradients("White", Color.FromRgb(255, 255, 255));
  }

  // TODO PORT get SystemParameters.WindowGlassColor on Windows
  private static Color _getSystemAccentColor() =>
    Color.FromRgb(45, 125, 154);
}