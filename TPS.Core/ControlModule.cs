using System.Drawing;
using AHK.Interop;
using TPS.Core.Models;
using Vanara.PInvoke;

namespace TPS.Core;

public class ControlModule2
{
    private static readonly AutoHotkeyEngine AHK = AutoHotkeyEngine.Instance;
    private static bool initialized;
    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;
    }

    public static void Press(string key)
    {
        if (!GameWindow.IsFocused()) return;
        AHK.ExecRaw($"Send, {{{key} down}}");

    }

    public static void SendFast(string key)
    {
        if (!GameWindow.IsFocused()) return ;
        AHK.ExecRaw($"Send, {key}");
    }
    public static void Release(string key)
    {
        if (!GameWindow.IsFocused()) return;
        AHK.ExecRaw($"Send, {{{key} up}}");
    }

    public static async Task Send(string key)
    {
        if (!GameWindow.IsFocused()) return;

        Logger.Debug($"Sending: {key}");
        await Task.Delay(100);
        AHK.ExecRaw($"Send, {key}");
        await Task.Delay(100);
    }

    public static async Task WheelDown(int amount)
    {
        AHK.ExecRaw($"Send, {{WheelDown {amount}}}");
        await Task.Delay(50);
    }

    public static async Task WheelUp(int amount)
    {
        AHK.ExecRaw($"Send, {{WheelUp {amount}}}");
        await Task.Delay(100);
    }

    public static async Task MoveMouse(Point point)
    {
        if (!GameWindow.IsFocused()) return;
        AHK.ExecRaw($"CoordMode, Mouse, Screen\rMouseMove, {Math.Round(point.X / GameWindow.Resolution.Scale)}, {Math.Round(point.Y / GameWindow.Resolution.Scale)}");
        await Task.Delay(100);
    }

    public static async Task SendEsc()
    {
        if (!GameWindow.IsFocused()) return;
        Logger.Debug("Sending Esc");
        await Task.Delay(100);
        AHK.ExecRaw("Send, {Esc}");

        await Task.Delay(100);
    }

    public static async Task Click(Point point)
    {
        if (!GameWindow.IsFocused()) return;
        Logger.Debug($"Clicking to {point.X} {point.Y}");
        await MoveMouse(point);
        AHK.ExecRaw($"MouseClick");
        await Task.Delay(50);
    }


}