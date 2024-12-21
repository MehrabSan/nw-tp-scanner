using System.Drawing;
using Emgu.CV;
using TPS.Core.Models;
using TPS.Core.Utils;

namespace TPS.Core;

public static class GameWindow
{
    public delegate void DrawRectEventHandler(Rectangle[] rectangles);

    public static DrawRectEventHandler OnDrawRect;

    public static nint Window;
    public static ScreenResolution Resolution;
    public static Rectangle WindowRect;
    public static Rectangle WindowRectScreen;

    public static readonly Rectangle BaseRect = new(0, 0, 1920, 1080);
    public static bool SlowDownMode = false;


    public static void Initialize()
    {
        ApiClient.GetServers();
        ItemResolver.Initialize();
        if (!WinApi.TryFindWindow("New World", out Window))
        {
            throw new TPSCoreException("Could not find the game window");
        }

        Resolution = WinApi.GetWindowsScreenResolution(Window);
        if (Resolution.PixelHeight < 1080 || Resolution.PixelWidth < 1920 || Resolution.AspectRatio < 16 / 9d)
        {
            throw new TPSCoreException($"Resolution {Resolution.PixelWidth}x{Resolution.PixelHeight} is not supported");
        }
        if (!WinApi.GetWindowRect(Window, out var windowRect))
        {
            throw new TPSCoreException("Could not find get the game window dimensions");
        }

        var y = windowRect.Top;


        WindowRect = Coords.GetCenterRegion(windowRect.ToRectangle().Multiply(Resolution.Scale));
        WindowRectScreen = windowRect.ToRectangle();

    }


    public static async Task SortByPrice()
    {

        var center = Coords.SortByPriceButton.Translate().GetCenter();
        await ControlModule2.Click(center);
        await Task.Delay(2000);
        //await ControlModule.Click(center);
        //await Task.Delay(2000);
    }

    public static bool IsFocused()
    {
        return WinApi.GetForegroundWindow() == Window;
    }

    public static async Task BringToFront()
    {
        var foregroundWindow = WinApi.GetForegroundWindow();
        if (foregroundWindow != Window) WinApi.SetForegroundWindow(Window);
        await Task.Delay(200);
    }

    public static async Task ClickToCenter()
    {
        await ControlModule2.Click(WindowRect.GetCenter());
        await Task.Delay(500);
    }

    public static async Task MoveToCenter()
    {
        await ControlModule2.MoveMouse(WindowRect.GetCenter());
        await Task.Delay(500);
    }

    public static async Task MoveToScrollbar()
    {
        var right = 32 * WindowRect.Width / BaseRect.Width;
        var point = Coords.PricesTable.Translate().GetCenterRight(right);
        await ControlModule2.MoveMouse(point);
        await Task.Delay(150);
    }

    public static async Task OpenTradingPost()
    {
        var retries = 0;
        while (!IsTradingPostWindow())
        {
            Logger.Log("Opening trading post");
            await ControlModule2.Send(Scanner.Settings.InteractionKey.ToAhkKey());
            await Task.Delay(1500);
            retries++;
            if (retries > 5) break;

        }
        if (!IsTradingPostWindow()) throw new TPSCoreException("Could not open the trading post");
    }

    public static async Task Reset()
    {
        await ControlModule2.SendEsc();
        await MoveCharacter();
        await HideWheel();

    }

    public static async Task HideWheel()
    {
        Logger.Log("Hiding the menu", true);
        while (IsWheelVisible())
        {
            await ControlModule2.SendEsc();
            await Task.Delay(500);
        }
    }

    public static bool IsTradingPostWindow()
    {
        Logger.Log("Checking if Trading Post is open", true);

        var cap = WindowCapture.CaptureWindow(Window, Coords.SearchBox.Translate());
        var mat = ImageProcessing.PrepareImageForOcr(cap.ToMat());
        var text = CharacterRecognition.Text(mat);
        OnDrawRect?.Invoke([Coords.SearchBox.Translate()]);
        return text == "Search";

    }

    public static async Task<string> GetServerName()
    {
        Logger.Log("Getting Server Name", true);
        await ControlModule2.SendEsc();
        await Task.Delay(300);
        ServerModel server = null;
        string text = null;
        while (!IsWheelVisible())
        {
            await ControlModule2.SendEsc();
            await Task.Delay(300);
        }
        await ControlModule2.Click(Coords.Wheel.Translate().GetCenter());
        OnDrawRect?.Invoke([Coords.Wheel.Translate()]);

        await Task.Delay(1000);
        OnDrawRect?.Invoke([Coords.WorldName.Translate()]);
        for (var i = 0; i < 5; i++)
        {
            Logger.Log($"Capturing Server Name ({i + 1})", true);
            var capture = WindowCapture.CaptureWindow(Window, Coords.WorldName.Translate());
            var mat = ImageProcessing.PrepareImageForOcr(capture.ToMat());
            text = CharacterRecognition.Text(mat);
            server = ApiClient.GetServer(text);
            if (server == null)
            {
                await Task.Delay(500);
                continue;
            }
            Logger.Log($"Server: {server.Name} {server.Type} {server.Region}");
            return server.Name;
        }
        OnDrawRect?.Invoke([]);
        throw new TPSCoreException($"Invalid Server Name: {text}, true");
    }

    private static Random rng = new();
    public static async Task MoveCharacter()
    {
        Logger.Log("Moving Character", true);
        ControlModule2.Press("s");
        await Task.Delay(rng.Next(120, 150));
        ControlModule2.Release("s");
        await Task.Delay(10);
        ControlModule2.Press("w");
        await Task.Delay(rng.Next(200, 250));
        ControlModule2.Release("w");
        await Task.Delay(50);
    }

    public static bool IsRefresh()
    {
        var capture = WindowCapture.CaptureWindow(Window, Coords.RefreshButton.Translate());
        var prepared = ImageProcessing.PrepareImageForOcr(capture.ToMat());
        var text = CharacterRecognition.Text(prepared);
        return text == "Refresh";
    }

    public static async Task NextPage(bool waitLonger)
    {
        Logger.Debug("Clicking to next page button");

        await ControlModule2.Click(Coords.NextPageButton.Translate().GetCenter());
        await Task.Delay(waitLonger ? SlowDownMode ? 2500 : 1500 : 100);
    }

    public static async Task SwitchToCategory(Category category)
    {
        Logger.Log($"Changing category to {category.Name}", true);
        var rect = Coords.GetCategoryRect(category.Order, category.Parent != null);
        await ControlModule2.Click(rect.GetCenter());
        await Task.Delay(1500);
    }

    internal static int maskThreshold = 10;
    internal static int threshold = 10;
    internal static float RequiredConfidence = 0.96f;

    public static bool TryGetPageInfo(out PageInfo pageInfo)
    {

        pageInfo = null;
        for (var i = 0; i < 10; i++)
        {
            var pageCountCap = WindowCapture.CaptureWindow(Window, Coords.PageCount.Translate());
            var pageCountPrep = ImageProcessing.PreparePageCountCellForOcr(pageCountCap.ToMat(), Color.FromArgb(255, 95, 87, 68), 60, 70);

            //var confidence = CharacterRecognition.NumberConfidence(pageCountPrep);
            //while (confidence<RequiredConfidence)
            //{
            //    maskThreshold++;
            //    //threshold++;
            //    pageCountPrep = ImageProcessing.PreparePageCountCellForOcr(pageCountCap.ToMat(), Color.FromArgb(255, 95, 87, 68), maskThreshold, threshold);
            //    confidence = CharacterRecognition.NumberConfidence(pageCountPrep);
            //    if (maskThreshold <= 150) continue;
            //    maskThreshold = 10;
            //    threshold = 40;
            //    RequiredConfidence -= 0.01f;
            //    if (RequiredConfidence < 0.85f)
            //    {
            //        throw new TPSCoreException("Could not detect the page count");
            //    }

            //}

            var pageCount = CharacterRecognition.Number(pageCountPrep);
            if (pageCount == -1) continue;
            var coords = Coords.CurrentPage;
            switch (pageCount)
            {
                case < 10:
                    coords.X += 20;
                    break;
                case < 100:
                    coords.X += 10;
                    break;
            }
            var currentPageCap = WindowCapture.CaptureWindow(Window, coords.Translate());
            var currentPagePrep = ImageProcessing.PreparePageCountCellForOcr(currentPageCap.ToMat(), Color.FromArgb(255, 240, 202, 67));
            var currentPage = CharacterRecognition.Number(currentPagePrep);
            if (currentPage == -1) continue;
            if (currentPage > pageCount) continue;
            pageInfo = new PageInfo(currentPage, pageCount);
            Logger.Log($"Detected page {pageInfo.CurrentPage} / {pageInfo.PageCount}");
            return true;
        }

        return false;
    }

    public static bool IsWheelVisible(bool debug = false)
    {
        Logger.Log($"Checking if menu is open", true);
        var capture = WindowCapture.CaptureWindow(Window, Coords.WheelDetect.Translate());
        var processed = ImageProcessing.PreparePageCountForOcr(capture);
        var text = CharacterRecognition.Text(processed);
        OnDrawRect?.Invoke([Coords.WheelDetect.Translate()]);
        return string.Equals(text, "Mount", StringComparison.OrdinalIgnoreCase);
    }


    public static async Task<List<TableRow>> ScanPage(bool lastPage = false, bool isRetry = false)
    {
        ControlModule2.SendFast("{F2}");
        await MoveToScrollbar();
        var part1Results = ScanScreen(1, isRetry);
        if (part1Results.Count == 0)
        {
            part1Results = ScanScreen(1, !isRetry);
            if (part1Results.Count == 0)
            {
                Logger.Log("Failed to scan the page part 1", true);
                return [];
            }
        }
        ControlModule2.SendFast("{F2}");
        await ControlModule2.WheelDown(11);
        await Task.Delay(150);
        if (part1Results.Count < 8)
        {
            return part1Results;
        }
        var part2Results = ScanScreen(2, isRetry);
        if (part2Results.Count == 0)
        {
            part2Results = ScanScreen(2, !isRetry);
            if (part2Results.Count == 0)
            {
                Logger.Log("Failed to scan the page part 2", true);
                return part1Results;
            }
        }
        ControlModule2.SendFast("{F2}");
        await ControlModule2.WheelDown(2);
        await Task.Delay(150);
        var part3Results = ScanScreen(3, isRetry);
        if (part3Results.Count < 9)
        {
            part3Results = ScanScreen(3, !isRetry);
            if (part3Results.Count < 9)
            {
                Logger.Log("Failed to scan the page part 3", true);
                return [.. part1Results, .. part2Results];
            }
        }

        List<TableRow> results = [.. part1Results, .. part2Results, .. part3Results[^2..]];

        if (lastPage)
        {
            results = results.Distinct(new TableRowComparer()).ToList();
        }
        return results;
    }

    public static List<TableRow> ScanScreen(int part, bool isRetry)
    {

        var capture = WindowCapture.CaptureWindow(Window, Coords.PricesTable.Translate());
        OnDrawRect?.Invoke([Coords.PricesTable.Translate()]);

        var cells = ImageProcessing.GetCells(capture, part, isRetry);
        //for (var i = 0; i < cells[0].Count; i++)
        //{
        //    var cell = cells[0][i];
        //    cell.Save($"images\\c0-{part}-{i}-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.png");
        //}
        //for (var i = 0; i < cells[1].Count; i++)
        //{
        //    var cell = cells[1][i];
        //    cell.Save($"images\\c1-{part}-{i}-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.png");
        //}
        //for (var i = 0; i < cells[2].Count; i++)
        //{
        //    var cell = cells[2][i];
        //    cell.Save($"images\\c2-{part}-{i}-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.png");
        //}

        var names = cells[0].Select(CharacterRecognition.ItemName).ToList();
        var prices = cells[1].Select(CharacterRecognition.Number).ToList();
        var availabilities = cells[2].Select(CharacterRecognition.Number).ToList();

        return TextToTable.GetTable(names, prices, availabilities);
    }

}