using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XObjdetect;
using static System.Net.Mime.MediaTypeNames;

namespace TPS.Core;

public static class ImageProcessing
{
    private static readonly List<int> ColumnWidths = [312, 168, 584, 112];

    private static readonly List<float> ColumnWidthsPercentages =
        ColumnWidths.Select(w => (float)w / ColumnWidths.Sum()).ToList();

    public static List<List<Mat>> GetCells(Bitmap image, int part, bool isRetry)
    {
        var results = new List<List<Mat>> { new(), new(), new() };
        var cellHeight = image.Height / 9 * 3;
        //var mat = PrepareImageForOcr(image);
        var mat = image.ToMat();
        var columns = SplitImageIntoColumns(mat);
        //columns[0].Save("images/" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_o_0.png");
        //columns[1].Save("images/" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_o_1.png");
        //columns[3].Save("images/" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_o_3.png");
        columns[0] = PrepareItemNamesForOcr(columns[0], isRetry);
        columns[1] = PrepareAuctionCellForOcr(columns[1], !isRetry);
        columns[3] = PrepareAuctionCellForOcr(columns[3], !isRetry);

        //columns[0].Save("images/" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_0.png");
        //columns[1].Save("images/" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_1.png");
        //columns[3].Save("images/" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_3.png");

        var topCut = part switch
        {
            1 => cellHeight / 3,
            2 => cellHeight / 30,
            3 => cellHeight / 2,
            _ => cellHeight / 3
        };

        const int height = 100;

        for (var i = 0; i < 9; i++)
        {
            results[0].Add(new Mat(columns[0], new Rectangle(0, cellHeight * i, columns[0].Width, cellHeight)));
            results[1].Add(new Mat(columns[1], new Rectangle(0, cellHeight * i + topCut, columns[1].Width, height)));
            results[2].Add(new Mat(columns[3], new Rectangle(0, cellHeight * i + topCut, columns[3].Width, height)));
        }




        return results;
    }

    public static Mat Crop(Mat src, Rectangle rect)
    {
        return new Mat(src, rect);
    }

    public static Bitmap DrawRect(Bitmap image, Rectangle rect)
    {
        var mat = image.ToMat();
        CvInvoke.Rectangle(mat, rect, new MCvScalar(0, 0, 255), 2);
        return mat.ToBitmap();
    }

    public static void ShowImage(Bitmap image)
    {
        var mat = image.ToMat();
        CvInvoke.Imshow("Image", mat);

        CvInvoke.WaitKey(0);
    }

    private static List<Mat> SplitImageIntoColumns(Mat image)
    {
        var columnImages = new List<Mat>();
        var startX = 0;
        foreach (var widthPercentage in ColumnWidthsPercentages)
        {
            var width = (int)(image.Width * widthPercentage);
            // Define the rectangle for the column
            var rect = new Rectangle(startX, 0, width, image.Height);

            // Crop the image based on the rectangle
            var column = new Mat(image, rect);

            // Add the cropped column to the list
            columnImages.Add(column);

            //CvInvoke.Line(image, new Point(startX, 0), new Point(startX, image.Height), new MCvScalar(0, 0, 255), 2); // Red line

            // Update startX for the next column
            startX += width;
        }

        return columnImages;

        //CvInvoke.Line(image, new Point(startX, 0), new Point(startX, image.Height), new MCvScalar(0, 0, 255), 2); // Red line
        //CvInvoke.Imshow("Image with Column Lines", image);

        //CvInvoke.WaitKey();

        //return columnImages;
    }

    private static Mat Blur(Mat image, int radius)
    {
        var mat = new Mat();
        CvInvoke.GaussianBlur(image, mat, new Size(radius, 1), 0);
        return mat;

    }

    private static Mat Erode(Mat image)
    {
        var eroded = new Mat();
        CvInvoke.Erode(image, eroded, null, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0));
        return eroded;

    }

    public static Mat PreparePageCountCellForOcr(Mat mat, Color color, int maskThreshold = 90, int threshold = 30)
    {
        using var bgr = mat.ToImage<Bgr, byte>();
        //bgr._EqualizeHist();
        //bgr._GammaCorrect(0.5d);
        using var resized = bgr.Resize(bgr.Width * 3, bgr.Height * 3, Inter.Lanczos4);
        var colors = new List<Color>
        {
            color
        };

        var img = MaskImage(resized, colors, maskThreshold);
        CvInvoke.CvtColor(img, img, ColorConversion.Bgr2Gray);

        CvInvoke.BitwiseNot(img, img);
        CvInvoke.Threshold(img, img, threshold, 255, ThresholdType.Binary | ThresholdType.Otsu);
        CvInvoke.GaussianBlur(img, img, new Size(3, 1), 0);

        return img;
    }


    public static Mat PreparePageCountForOcr(Bitmap image)
    {
        using var bgr = image.ToImage<Bgr, byte>();
        return PreparePageCountForOcr(bgr);
    }

    public static Mat PreparePageCountForOcr(Image<Bgr, byte> bgr)
    {
        //bgr._EqualizeHist();
        //bgr._GammaCorrect(0.1d);
        using var resized = bgr.Resize(bgr.Width * 2, bgr.Height * 2, Inter.Lanczos4);
        var img = new Mat();
        CvInvoke.CvtColor(resized, img, ColorConversion.Bgr2Gray);
        //rgb(214, 180, 63)
        //rgb(93, 85, 67)
        //var mat2 = new Mat();
        //CvInvoke.BilateralFilter(img, mat2, 5, 50, 100);
        //img = mat2;
        CvInvoke.BitwiseNot(img, img);
        CvInvoke.Threshold(img, img, 160, 255, ThresholdType.Binary | ThresholdType.Otsu);
        //img = Erode(img);


        return img;
    }

    public static Mat PrepareAuctionCellForOcr(Mat mat, bool erode = false)
    {
        using var bgr = mat.ToImage<Bgr, byte>();
        //bgr._EqualizeHist();
        //bgr._GammaCorrect(0.5d);
        using var resized = bgr.Resize(bgr.Width * 3, bgr.Height * 3, Inter.Lanczos4);
        var colors = new List<Color>
        {
            Color.FromArgb(255, 155, 145, 125),
            Color.FromArgb(255, 255, 117, 117)
        };

        var img = MaskImage(resized, colors, 90);
        CvInvoke.CvtColor(img, img, ColorConversion.Bgr2Gray);

        CvInvoke.BitwiseNot(img, img);
        CvInvoke.Threshold(img, img, 100, 255, ThresholdType.Binary | ThresholdType.Otsu);
        if (erode) img = Erode(img);
        CvInvoke.GaussianBlur(img, img, new Size(3, 1), 0);

        return img;
    }

    public static Mat PrepareImageForOcr(Mat mat, int maskTolerance = 80)
    {
        using var bgr = mat.ToImage<Bgr, byte>();

        var colors = new List<Color>
        {
            Color.FromArgb(255, 155, 145, 125),
            Color.FromArgb(255, 255, 117, 117)
        };

        var img = MaskImage(bgr, colors, maskTolerance);


        CvInvoke.CvtColor(img, img, ColorConversion.Bgr2Gray);

        CvInvoke.BitwiseNot(img, img);
        CvInvoke.Threshold(img, img, 250, 255, ThresholdType.Binary | ThresholdType.Otsu);


        return img;
    }

    public static Mat PrepareItemNamesForOcr(Mat mat, bool isRetry)
    {
        using var bgr = mat.ToImage<Bgr, byte>();
        using var resized = bgr.Resize(bgr.Width * 3, bgr.Height * 3, isRetry ? Inter.Lanczos4 : Inter.Cubic);
        var colors = new List<Color>
        {
            Color.FromArgb(255, 155, 145, 125),
        };

        var img = MaskImage(resized, colors, isRetry ? 60 : 80);


        CvInvoke.CvtColor(img, img, ColorConversion.Bgr2Gray);
        CvInvoke.BitwiseNot(img, img);
        CvInvoke.Threshold(img, img, isRetry ? 120 : 80, 255, ThresholdType.Binary | ThresholdType.Otsu);
        CvInvoke.GaussianBlur(img, img, new Size(1, 1), 0);
        //img = Erode(img);
        return img;
    }



    private static Mat MaskImage(Image<Bgr, byte> bgr, List<Color> colors, int tolerance)
    {
        var combinedMask = new Mat(bgr.Size, DepthType.Cv8U, 1);
        combinedMask.SetTo(new MCvScalar(0));



        foreach (var color in colors)
        {
            var targetColor = new MCvScalar(color.B, color.G, color.R);

            var lowerBound = new MCvScalar(
                Math.Max(0, targetColor.V0 - (color.A != 255 ? color.A : tolerance)),
                Math.Max(0, targetColor.V1 - (color.A != 255 ? color.A : tolerance)),
                Math.Max(0, targetColor.V2 - (color.A != 255 ? color.A : tolerance))
            );

            var upperBound = new MCvScalar(
                Math.Min(255, targetColor.V0 + (color.A != 255 ? color.A : tolerance)),
                Math.Min(255, targetColor.V1 + (color.A != 255 ? color.A : tolerance)),
                Math.Min(255, targetColor.V2 + (color.A != 255 ? color.A : tolerance))
            );

            var mask = new Mat();
            CvInvoke.InRange(bgr, new ScalarArray(lowerBound), new ScalarArray(upperBound), mask);

            CvInvoke.BitwiseOr(combinedMask, mask, combinedMask);
        }

        var resultImage = new Mat();
        CvInvoke.BitwiseAnd(bgr, bgr, resultImage, combinedMask);

        return resultImage;
    }


    public static Rectangle MatchImage(Image<Bgr, byte> source, Image<Bgr, byte> template)
    {
        using var imageToShow = source.Copy();

        using var result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
        result.MinMax(out _, out var maxValues, out _, out var maxLocations);

        // You can try different values of the threshold. I guess somewhere between 0.75 and 0.95 would be good.
        if (!(maxValues[0] > 0.7)) return Rectangle.Empty;

        // This is a match. Do something with it, for example draw a rectangle around it.
        var match = new Rectangle(maxLocations[0], template.Size);
        imageToShow.Draw(match, new Bgr(Color.Red), 3);
        //ShowImage(imageToShow.ToBitmap());
        return match;
    }

}