using Avalonia;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace MH.UI.AvaloniaUI.Extensions;

public static class BitmapExtensions {
  public static Bitmap Create(string filePath, Rect rect) {
    using var bitmap = SKBitmap.Decode(filePath);
    if (bitmap == null) throw new ArgumentException("Failed to decode image", nameof(filePath));

    var skRect = new SKRectI((int)rect.X, (int)rect.Y, (int)(rect.X + rect.Width), (int)(rect.Y + rect.Height));
    using var croppedBitmap = new SKBitmap(skRect.Width, skRect.Height);
    if (!bitmap.ExtractSubset(croppedBitmap, skRect))
      throw new InvalidOperationException("Failed to extract subset of image");

    using var data = croppedBitmap.Encode(SKEncodedImageFormat.Png, 100);
    return new(data.AsStream());
  }

  public static void GetScale(this Bitmap bmp, int size, out double x, out double y) {
    var pxW = bmp.PixelSize.Width;
    var pxH = bmp.PixelSize.Height;
    var sizeW = pxW > pxH ? size : size * (pxW / (pxH / 100.0)) / 100;
    var sizeH = pxH > pxW ? size : size * (pxH / (pxW / 100.0)) / 100;
    x = sizeW / pxW;
    y = sizeH / pxH;
  }

  public static RenderTargetBitmap Resize(this Bitmap bmp, double scaleX, double scaleY) {
    var newWidth = (int)(bmp.PixelSize.Width * scaleX);
    var newHeight = (int)(bmp.PixelSize.Height * scaleY);
    var resized = new RenderTargetBitmap(new(newWidth, newHeight));
    using var context = resized.CreateDrawingContext();
    context.DrawImage(bmp, new(0, 0, newWidth, newHeight));

    return resized;
  }

  public static RenderTargetBitmap Resize(this Bitmap bmp, int size) {
    bmp.GetScale(size, out var scaleX, out var scaleY);
    return bmp.Resize(scaleX, scaleY);
  }

  public static void SaveAsJpeg(this Bitmap bmp, string filePath, int quality) {
    Directory.CreateDirectory(filePath[..filePath.LastIndexOf(Path.DirectorySeparatorChar)]);
    using var stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite);
    bmp.Save(stream, quality);
  }
}