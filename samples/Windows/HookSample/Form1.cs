using Tenon.Infra.Windows.Form.Common;
using Tenon.Infra.Windows.Win32;
using Tenon.Infra.Windows.Win32.Hooks;

namespace HookSample;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        KeyboardHook.KeyUp += KeyboardHook_KeyUp;
        KeyboardHook.KeyDown += KeyboardHook_KeyDown;
        KeyboardHook.Install();
    }

    private void KeyboardHook_KeyUp(object? sender, KeyEventArgs e)
    {
        lsOutput.UIThread(ls => ls.AddItemSelected($"[{DateTime.Now.ToShortDateString()}] KeyUp:{e.KeyCode}"));
    }

    private void KeyboardHook_KeyDown(object? sender, KeyEventArgs e)
    {
        lsOutput.UIThread(ls => ls.AddItemSelected($"[{DateTime.Now.ToShortDateString()}] KeyDown:{e.KeyCode}"));
    }

    private void button2_Click(object sender, EventArgs e)
    {
        KeyboardHook.KeyUp -= KeyboardHook_KeyUp;
        KeyboardHook.KeyDown -= KeyboardHook_KeyDown;
        KeyboardHook.Uninstall();
    }

    private void button3_Click(object sender, EventArgs e)
    {
        MouseHook.DoubleClick += MouseHook_Click;
        MouseHook.LeftButtonDown += MouseHook_Click;
        MouseHook.LeftButtonUp += MouseHook_Click;
        MouseHook.MiddleButtonDown += MouseHook_Click;
        MouseHook.MiddleButtonUp += MouseHook_Click;
        MouseHook.MouseMove += MouseHook_Click;
        MouseHook.MouseWheel += MouseHook_Click;
        MouseHook.MouseWheel += MouseHook_Click;
        MouseHook.RightButtonDown += MouseHook_Click;
        MouseHook.RightButtonUp += MouseHook_Click;
        MouseHook.Install();
    }

    private void MouseHook_Click(object? sender, MouseEventArgs e)
    {
        var windowHandle = Window.Get(e.Location);
        AddLog($"Windows hWnd:{windowHandle}");
        if (windowHandle != IntPtr.Zero) AddLog($"Windows className:{Window.GetClassName(windowHandle)}");

        //AddLog(
        //    $"[{DateTime.Now.ToShortDateString()}] MouseHook x:{e.X},x:{e.Y},Button:{e.Button.ToString()},Clicks:{e.Clicks}");
    }

    private void AddLog(string message)
    {
        lsOutput.UIBeginThread(ls => ls.AddItemSelected($"{DateTime.Now:HH:mm:ss} {message}"));
    }

    private void button4_Click(object sender, EventArgs e)
    {
        MouseHook.DoubleClick -= MouseHook_Click;
        MouseHook.LeftButtonDown -= MouseHook_Click;
        MouseHook.LeftButtonUp -= MouseHook_Click;
        MouseHook.MiddleButtonDown -= MouseHook_Click;
        MouseHook.MiddleButtonUp -= MouseHook_Click;
        MouseHook.MouseMove -= MouseHook_Click;
        MouseHook.MouseWheel -= MouseHook_Click;
        MouseHook.MouseWheel -= MouseHook_Click;
        MouseHook.RightButtonDown -= MouseHook_Click;
        MouseHook.RightButtonUp -= MouseHook_Click;
        MouseHook.Uninstall();
    }
}