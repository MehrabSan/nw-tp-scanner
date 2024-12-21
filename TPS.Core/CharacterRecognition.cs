using Emgu.CV;
using Emgu.CV.OCR;
using TesseractOCR;
using TesseractOCR.Enums;
using PageSegMode = TesseractOCR.Enums.PageSegMode;


namespace TPS.Core;

public static class CharacterRecognition
{
    private static readonly string DataPath = Path.Combine(AppContext.BaseDirectory, "data");
    private static readonly Engine Engine = new(DataPath, Language.English);
    private static readonly Engine NumbersEngine = new(DataPath, Language.English);


    public static string Text(Mat mat)
    {
        using var emguPix = new Pix(mat);
        using var pix = TesseractOCR.Pix.Image.Create(emguPix.Ptr);
        using var page = Engine.Process(pix, PageSegMode.SingleBlock);
        var text = page.Text;
        if (text == null) text = string.Empty;
        return text.Replace("\n", " ").Trim();
    }

    public static string ItemName(Mat mat)
    {
        using var emguPix = new Pix(mat);
        using var pix = TesseractOCR.Pix.Image.Create(emguPix.Ptr);
        using var page = Engine.Process(pix, PageSegMode.SingleBlock);
        var text = page.Text;
        if (text == null) text = string.Empty;
        return text.Replace("\n", " ").Trim();
    }

    public static int Number(Mat mat)
    {
        using var emguPix = new Pix(mat);
        using var pix = TesseractOCR.Pix.Image.Create(emguPix.Ptr);
        using var page = NumbersEngine.Process(pix, PageSegMode.SingleBlock);
        //var confidence = page.MeanConfidence;
        var text = page.Text;
        //Logger.Log($"{text} Confidence: {confidence}");
        var cleaned= new string(text.Where(char.IsDigit).ToArray());
        if (int.TryParse(cleaned, out var parsed)) return parsed;
        return -1;
    }

    public static void Dispose()
    {
        Engine?.Dispose();
        NumbersEngine?.Dispose();

    }
}