using Vanara.PInvoke;
using static Vanara.PInvoke.User32;
namespace TPS.Desktop;

public static class WindowHelper
{
    public static void SetTopMost(IntPtr hwnd)
    {
        SetWindowPos(hwnd, new HWND(-1), 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
    }
}

