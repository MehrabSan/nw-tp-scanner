using System.ComponentModel;
using System.Text;
using Emgu.CV;
using TPS.Core;
using TPS.Core.Utils;

namespace TPS.Desktop;

internal class ReportState(ReportType type, object data)
{
    internal ReportType Type { get; set; } = type;
    internal object Data { get; set; } = data;
}

internal enum ReportType
{
    Log,
    Rect
}

internal class MainWorker : BackgroundWorker
{
    private readonly MainForm _mainForm;
    private readonly CancellationToken _cancellationToken;

    public MainWorker(MainForm mainForm, CancellationToken cancellationToken)
    {
        _mainForm = mainForm;
        _cancellationToken = cancellationToken;
        WorkerReportsProgress = true;
        WorkerSupportsCancellation = true;
        DoWork += MainWorker_DoWork;
        ProgressChanged += MainWorker_ProgressChanged;
        RunWorkerCompleted += MainWorker_RunWorkerCompleted;
        Logger.OnLogEntry += OnLogEntry;
        GameWindow.OnDrawRect += OnDrawRect;
    }

    private void OnDrawRect(Rectangle[] rectangles)
    {
        Report(ReportType.Rect, rectangles);
    }

    private void OnLogEntry(string message)
    {
        Report(ReportType.Log, message);
    }

    private void Report(ReportType type, object data)
    {
        ReportProgress(0, new ReportState(type, data));
    }


    private void MainWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        AsyncHelper.RunSync(() => Task.Delay(30, _cancellationToken));
        try
        {
            AsyncHelper.RunSync(() => Scanner.StartScan(_cancellationToken));
            Logger.Log("------------");
            Logger.Log("- All Done -");
            Logger.Log("------------");
        }
        catch (Exception exception)
        {
            Logger.Log(exception.ToString());

        }

    }




    private void MainWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {

        if (e.UserState is not ReportState reportState) return;
        switch (reportState.Type)
        {
            case ReportType.Log:
                if (reportState.Data is not string logData) return;
                try
                {
                    _mainForm.LogBox.AppendText(logData + Environment.NewLine);
                }
                catch (Exception exception)
                {
                    //
                }
                break;
            case ReportType.Rect:
                _mainForm.rectangles = reportState.Data as Rectangle[];
                _mainForm.DrawPanel.Invalidate();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MainWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        _mainForm.ResetState();
    }

    protected override void Dispose(bool disposing)
    {
        Logger.OnLogEntry -= OnLogEntry;
        GameWindow.OnDrawRect -= OnDrawRect;
        base.Dispose(disposing);
    }
}

