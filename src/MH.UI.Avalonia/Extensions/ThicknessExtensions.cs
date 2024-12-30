using Avalonia;
using MH.Utils.Types;

namespace MH.UI.AvaloniaUI.Extensions;

public static class ThicknessExtensions {
  public static Thickness FromThicknessD(this ThicknessD thickness) =>
    new(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
}