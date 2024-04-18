namespace Tenon.Automation.Windows.Models;

public class WindowProperties
{
    public FormBorderStyle FormBorderStyle { get; set; } = FormBorderStyle.None;
    public bool ShowInTaskbar { get; set; } = false;
    public bool TopMost { get; set; } = true;
    public bool Visible { get; set; } = false;
    public int Left { get; set; } = 0;
    public int Top { get; set; } = 0;
    public int Width { get; set; } = 1;
    public int Height { get; set; } = 1;
    public Color BackColor { get; set; } = Color.Red;
}