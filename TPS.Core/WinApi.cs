using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using TPS.Core.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TPS.Core;

public static class WinApi
{
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern nint GetForegroundWindow();

    internal enum DeviceCap
    {
        HORZRES = 8,
        VERTRES = 10,
        DESKTOPVERTRES = 117,
        DESKTOPHORZRES = 118
    }

    internal static ScreenResolution GetWindowsScreenResolution(nint window)
    {
        //Create Graphics object from the current windows handle
        using var graphicsObject = Graphics.FromHwnd(window);
        //Get Handle to the device context associated with this Graphics object
        var deviceContextHandle = graphicsObject.GetHdc();

        var resolution = new ScreenResolution
        {
            LogicalHeight = GetDeviceCaps(deviceContextHandle, (int)DeviceCap.VERTRES),
            PixelHeight = GetDeviceCaps(deviceContextHandle, (int)DeviceCap.DESKTOPVERTRES),
            LogicalWidth = GetDeviceCaps(deviceContextHandle, (int)DeviceCap.HORZRES),
            PixelWidth = GetDeviceCaps(deviceContextHandle, (int)DeviceCap.DESKTOPHORZRES)
        };
        resolution.Scale = Math.Round(resolution.PixelHeight / (double)resolution.LogicalHeight, 2);
        graphicsObject.ReleaseHdc(deviceContextHandle);
        return resolution;
    }

    public static bool TryFindWindow(string title, out nint windowHWnd)
    {
        windowHWnd = FindWindow(null, title);

        if (windowHWnd != IntPtr.Zero) return true;
        var processes = Process.GetProcessesByName("NewWorld");
        if (processes.Length == 0) return false;
        windowHWnd = processes[0].MainWindowHandle;
        return true;
    }


}