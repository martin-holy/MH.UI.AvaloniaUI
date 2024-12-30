using Avalonia;
using MH.Utils.Types;

namespace MH.UI.AvaloniaUI.Extensions;

public static class SizeExtensions {
  public static SizeD ToSizeD(this Size size) => new(size.Width, size.Height);
}