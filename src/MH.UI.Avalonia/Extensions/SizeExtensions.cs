using Avalonia;
using MH.Utils.Types;

namespace MH.UI.Avalonia.Extensions;

public static class SizeExtensions {
  public static SizeD ToSizeD(this Size size) => new(size.Width, size.Height);
}