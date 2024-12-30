using Avalonia;
using MH.Utils.Types;

namespace MH.UI.AvaloniaUI.Extensions;

public static class PointExtensions {
  public static PointD ToPointD(this Point point) => new(point.X, point.Y);
}