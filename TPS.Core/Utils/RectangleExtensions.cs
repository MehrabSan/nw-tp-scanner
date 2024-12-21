using System.Drawing;

namespace TPS.Core.Utils;

public static class RectangleExtensions
{
    public static Rectangle Multiply(this Rectangle rectangle, double scale)
    {
        return new Rectangle(
            (int)(rectangle.X * scale),
            (int)(rectangle.Y * scale),
            (int)(rectangle.Width * scale),
            (int)(rectangle.Height * scale)
        );
    }

    public static Point GetCenter(this Rectangle rectangle)
    {
        return new Point((rectangle.Left + rectangle.Right) / 2, (rectangle.Top + rectangle.Bottom) / 2);
    }

    public static Point GetCenterRight(this Rectangle rectangle, int moveRight = 0)
    {
        return new Point(rectangle.Right + moveRight, (rectangle.Top + rectangle.Bottom) / 2);
    }

    public static Rectangle PadTop(this Rectangle rectangle, int pad)
    {
        pad = (int)Math.Ceiling(pad * (float)GameWindow.WindowRect.Width / GameWindow.BaseRect.Width);
        return rectangle with { Y = rectangle.Y + pad, Height = rectangle.Height - (pad/2) };
    }

    public static Rectangle Translate(this Rectangle rectangle) => Coords.TranslateFromBase(rectangle);


}

public static class StringExtensions
{

    public static string SafeSubString(this string value, int length)
    {
        if (value == null) return null;
        return value.Length < length ? value : value[..length];
    }


}