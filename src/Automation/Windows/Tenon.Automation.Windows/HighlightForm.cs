using Tenon.Automation.Windows.Models;
using Tenon.Infra.Windows.Win32;
using Tenon.Infra.Windows.Win32.Models;
using SysForm = System.Windows.Forms.Form;

namespace Tenon.Automation.Windows;

public class HighlightForm : SysForm
{
    public HighlightForm(WindowProperties? properties = null)
    {
        properties = properties ?? new WindowProperties();
        FormBorderStyle = properties.FormBorderStyle;
        ShowInTaskbar = properties.ShowInTaskbar;
        TopMost = properties.TopMost;
        Visible = properties.Visible;
        Left = properties.Left;
        Width = properties.Width;
        Height = properties.Height;
        Top = properties.Top;
        BackColor = properties.BackColor;
        SetFormStyle();
    }

    private void SetFormStyle()
    {
        var style = Window.GetLong(Handle, WindowLongPtrIndex.ExStyle);
        if (style != IntPtr.Zero)
            Window.SetLong(Handle, WindowLongPtrIndex.ExStyle, (int)style | 0x00000080);
    }

    public void SetLocation(Rectangle rectangle)
    {
        if (rectangle.IsEmpty) return;
        BeginInvoke(() =>
        {
            Window.SetPos(Handle, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                SpecialWindowHandles.Topmost, SetWindowPosFlags.SwpNoActivate);
        });
    }

    public new void Show()
    {
        Window.Show(Handle, ShowWindowCommand.ShowNa);
    }
}