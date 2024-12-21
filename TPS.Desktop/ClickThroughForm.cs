namespace TPS.Desktop;

public class ClickThroughForm : Form
{
    private const int WS_EX_TRANSPARENT = 0x00000020;
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= WS_EX_TRANSPARENT;
            return cp;
        }
    }
}