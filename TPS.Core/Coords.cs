using System.Drawing;
using TPS.Core.Utils;

namespace TPS.Core;

public static class Coords
{
    public static Rectangle PricesTable = new(690, 320, 1100, 700);
    public static Rectangle CurrentPage = new(1670, 200, 65, 50);
    public static Rectangle PageCount = new(1750, 200, 30, 50);
    public static Rectangle PageInfo = new(1680, 200, 125, 50);
    public static Rectangle NextPageButton = new(1790, 220, 20, 20);
    public static Rectangle WorldName = new(1130, 865, 200, 30);
    public static Rectangle Categories = new(170, 342, 330, 700);
    public static Rectangle Wheel = new(1464, 24, 30, 30);
    public static Rectangle WheelDetect = new(480, 20, 110, 40);
    public static Rectangle SearchBox = new(120, 210, 200, 36);
    public static Rectangle RefreshButton = new(1170, 655, 110, 35);
    public static Rectangle SortByAvailabilityButton = new(1690, 295, 70, 20);
    public static Rectangle SortByPriceButton = new(990, 295, 50, 20);

    public static int CategoryHeight = 72;
    public static int SubCategoryHeight = 55;

    public static Rectangle TranslateFromBase(Rectangle baseRect)
    {
        var scale = (float)GameWindow.Resolution.PixelHeight / GameWindow.BaseRect.Height;
        var scaledRect = baseRect.Multiply(scale);
        if (GameWindow.WindowRect.X > 0)
        {
            scaledRect.X += GameWindow.WindowRect.X;
        }
        return scaledRect;
    }

    public static Rectangle GetCategoryRect(int order, bool subCategory = false)
    {
        var categoryHeight = TranslateFromBase(subCategory ? SubCategoryHeight : CategoryHeight);
        var categoriesRect = Categories.Translate();
        var categoryTop = categoryHeight * order;
        return categoriesRect with { Y = categoryTop + categoriesRect.Y, Height = categoryHeight };
    }

    public static int TranslateFromBase(int distance)
    {
        var scale = (float)GameWindow.Resolution.PixelHeight / GameWindow.BaseRect.Height;
        return (int)(distance * scale);
    }

    public static Rectangle GetCenterRegion(Rectangle rect)
    {
        const float aspectRatio = 16f / 9f;

        var centerWidth = rect.Width;
        var centerHeight = (int)(centerWidth / aspectRatio);

        if (centerHeight > rect.Height)
        {
            centerHeight = rect.Height;
            centerWidth = (int)(centerHeight * aspectRatio);
        }

        var centerX = rect.X + (rect.Width - centerWidth) / 2;
        var centerY = rect.Y + (rect.Height - centerHeight) / 2;

        return new Rectangle(centerX, centerY, centerWidth, centerHeight);
    }


}