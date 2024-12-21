using System.Drawing;
using System.Runtime.InteropServices;

namespace TPS.Core.Models;

[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public int Width => Right - Left;
    public int Height => Bottom - Top;

    public Rectangle ToRectangle()
    {
        return new Rectangle(Left, Top, Width, Height);
    }
}