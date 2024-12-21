using System.Drawing;
using System.Drawing.Imaging;

namespace TPS.Core;

public static class WindowCapture
{
    public static Bitmap CaptureWindow(IntPtr hWnd, Rectangle rect)
    {
        var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bmp);
        Logger.Debug($"Capturing {rect.Left} {rect.Top} {rect.Width} {rect.Height}");
        graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(rect.Width, rect.Height), CopyPixelOperation.SourceCopy);
        return bmp;
    }
}