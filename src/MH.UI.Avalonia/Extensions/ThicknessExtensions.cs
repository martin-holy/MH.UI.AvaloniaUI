using Avalonia;
using MH.Utils.Types;

namespace MH.UI.Avalonia.Extensions;

public static class ThicknessExtensions {
  public static Thickness FromThicknessD(this ThicknessD thickness) =>
    new(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
}