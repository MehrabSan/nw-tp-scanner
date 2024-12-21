using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TPS.Core;
using Velopack;

namespace TPS.Desktop;

internal static class Program
{
    [STAThread]
    private static async Task Main()
    {
        VelopackApp.Build().Run();
#if !DEBUG
        try
        {
            await UpdateMyApp();

        }
        catch (Exception e)
        {
            var result = MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
#endif

#if DEBUG
        Scanner.Version = "DEBUG";
#else
        var version = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
        Scanner.Version = $"{version.FileMajorPart}.{version.FileMinorPart}.{version.FileBuildPart}";
#endif
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());

        Application.ApplicationExit += Application_ApplicationExit;
    }

    private static void Application_ApplicationExit(object sender, EventArgs e)
    {
        CharacterRecognition.Dispose();
    }

    internal static async Task UpdateMyApp()
    {
        var mgr = new UpdateManager("https://nwmpapp.gaming.tools/nwmp-app");

        // check for new version
        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
            return; // no update available

        var result = MessageBox.Show("A new version found. Updating", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
        // download new version
        await mgr.DownloadUpdatesAsync(newVersion);

        // install new version and restart app
        mgr.ApplyUpdatesAndRestart(newVersion);
    }

}