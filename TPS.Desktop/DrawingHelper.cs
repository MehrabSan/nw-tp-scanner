namespace TPS.Desktop;

internal static class DrawingHelper
{
    internal static void DrawRectangle(this Graphics graphics, Rectangle rect, Color color)
    {
        using var myBrush = new SolidBrush(color);
        graphics.FillRectangle(myBrush, rect);
    }
}