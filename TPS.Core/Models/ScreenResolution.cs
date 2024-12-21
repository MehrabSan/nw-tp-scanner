namespace TPS.Core.Models;

public class ScreenResolution
{
    public int LogicalWidth { get; set; }
    public int PixelWidth { get; set; }
    public int LogicalHeight { get; set; }
    public int PixelHeight { get; set; }
    public double Scale { get; set; }
    public double AspectRatio => (double)PixelWidth / PixelHeight;
}