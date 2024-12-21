using System.ComponentModel;
using TPS.Core;
using TPS.Core.Models;

namespace TPS.Desktop
{
    public partial class MainForm : ClickThroughForm
    {
        private CancellationTokenSource cancellationTokenSource;
        private SettingsForm settingsForm;
        private bool running = false;
        internal Rectangle[] rectangles = null;
        private readonly KeyboardHook hook = new();
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                // Set the form click-through
                cp.ExStyle |= 0x80000 /* WS_EX_LAYERED */ | 0x20 /* WS_EX_TRANSPARENT */;
                return cp;
            }
        }
        public MainForm()
        {

            hook.KeyPressed += Hook_KeyPressed;
            hook.RegisterHotKey(0, Keys.F3);
            hook.RegisterHotKey(0, Keys.F6);
            hook.RegisterHotKey(0, Keys.F8);
            hook.RegisterHotKey(Desktop.ModifierKeys.Control, Keys.C);
            ;
            cancellationTokenSource = new();
            MainWorker = new MainWorker(this, cancellationTokenSource.Token);

            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            hook.Dispose();
        }

        private async void Hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.F3:
                    if (running) return;
#if !DEBUG
                    await Program.UpdateMyApp();
#endif
                    SetRunning();
                    while (MainWorker.IsBusy) await Task.Delay(10);
                    if (!MainWorker.IsBusy) MainWorker.RunWorkerAsync();
                    break;
                case Keys.F6:
                    if (running) return;
                    if (settingsForm != null) return;
                    settingsForm = new SettingsForm();
                    settingsForm.TopMost = true;
                    WinApi.SetForegroundWindow(settingsForm.Handle);
                    var result = settingsForm.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        Logger.Log("Settings are updated.");
                    }
                    settingsForm.Dispose();
                    settingsForm = null;
                    break;
                case Keys.C:
                    if (e.Modifier != Desktop.ModifierKeys.Control) break;
                    if(!running) break;
                    running = false;
                    await cancellationTokenSource.CancelAsync();
                    Logger.Log("-------------");
                    Logger.Log("- Cancelled -");
                    Logger.Log("-------------");
                    ResetState();
                    break;
                case Keys.F8:
                    Application.Exit();
                    break;
            }
        }

        private void SetRunning()
        {
            running = true;
            ScanLabel.ForeColor = Color.DarkGray;
            ScanLabel.Invalidate();
            SettingsLabel.ForeColor = Color.DarkGray;
            SettingsLabel.Invalidate();
            CancelLabel.Visible = true;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                var settings = Settings.LoadSettings();
                Scanner.Initialize(settings);

            }
            catch (Exception exception)
            {
                var result = MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK);
                Application.Exit();
            }

            WindowHelper.SetTopMost(Handle);
            Top = GameWindow.WindowRect.Top;
            Left = GameWindow.WindowRect.Left;
            Width = GameWindow.WindowRect.Width;
            Height = GameWindow.WindowRect.Height;
            await GameWindow.BringToFront();


        }




        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            if (rectangles is not { Length: > 0 }) return;
            using var pen = new Pen(Color.Blue, 2);  // 3px wide black pen
            using var g = e.Graphics;
            foreach (var rect in rectangles)
            {
                var newRect = new Rectangle(rect.Location, rect.Size);
                newRect.Y -= Top - 6;
                newRect.X -= Left - 6;
                newRect.Width += 12;
                newRect.Height += 12;
                g.DrawRectangle(pen, rect);
            }
        }


        public void ResetState()
        {
            CancelLabel.Visible = false;
            return;
            MainWorker?.Dispose();
            MainWorker = null;
            cancellationTokenSource = new();
            MainWorker = new MainWorker(this, cancellationTokenSource.Token);
            ScanLabel.ForeColor = Color.WhiteSmoke;
            ScanLabel.Invalidate();
            SettingsLabel.ForeColor = Color.WhiteSmoke;
            SettingsLabel.Invalidate();
            running = false;
        }
    }
}
