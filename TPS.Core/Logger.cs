using System.Text;

namespace TPS.Core;

public class Logger
{
    public delegate void LogEntryEventHandler(string message);
    private static readonly long CurrentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    private static readonly StringBuilder fileLogBuilder = new();
    private static readonly StringBuilder consoleLogBuilder = new();

    public static LogEntryEventHandler OnLogEntry;
    public static void Log(string message, bool autoFlush = false)
    {
        fileLogBuilder.AppendLine(message);
        consoleLogBuilder.AppendLine(message);
        if (autoFlush) Flush();
        OnLogEntry?.Invoke(message);
        //Console.WriteLine(message);
    }

    public static void Debug(string message)
    {
        //#if DEBUG
        //        Console.WriteLine(message);

        //#endif
        fileLogBuilder.AppendLine(message);
    }

    private static readonly object locker = new();
    public static void Flush()
    {

        var fileLog = fileLogBuilder.ToString();
        fileLogBuilder.Clear();
        var consoleLog = consoleLogBuilder.ToString();
        consoleLogBuilder.Clear();
        if (!string.IsNullOrEmpty(fileLog))
        {
            try
            {
                File.AppendAllText($"logs\\log-{CurrentTimestamp}.txt", fileLog);

            }
            catch (Exception e)
            {
                // Ignore
            }

        }

        if (!string.IsNullOrEmpty(consoleLog))
        {
            Console.Write(consoleLog);
        }

    }
}