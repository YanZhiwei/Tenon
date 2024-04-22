using Tenon.Automation.Windows;
using Tenon.Infra.Windows.ChromiumAccessibility;
using Tenon.Infra.Windows.Form.Common;
using Tenon.Infra.Windows.Win32;

namespace AutomationSample;

public partial class Form1 : Form
{
    private readonly ChromeAccessibility _chromeAccessibility;
    private readonly WindowsHighlightRectangle _highlightRectangle;

    public Form1()
    {
        InitializeComponent();
        _highlightRectangle = new WindowsHighlightRectangle();
        _chromeAccessibility = new ChromeAccessibility();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        _highlightRectangle.SetLocation(new Rectangle
        {
            Height = (int)numericUpDown4.Value,
            Width = (int)numericUpDown3.Value,
            Y = (int)numericUpDown2.Value,
            X = (int)numericUpDown1.Value
        });
    }

    private void button2_Click(object sender, EventArgs e)
    {
        _highlightRectangle.Hide();
    }

    private void button3_Click(object sender, EventArgs e)
    {
        _highlightRectangle.Close();
    }

    private void AddLog(string message)
    {
        listBox1.UIBeginThread(ls => ls.AddItemSelected($"{DateTime.Now:HH:mm:ss} {message}"));
    }

    private void button4_Click(object sender, EventArgs e)
    {
        var mainWindowHandle = _chromeAccessibility.GetMainWindowHandle();
        AddLog($"chrome mainWindowHandle:{mainWindowHandle}");
        var rectangle = Window.GetRectangle(mainWindowHandle);
        if (rectangle.HasValue)
        {
            AddLog($"chrome rectangle:{rectangle.Value}");
            _highlightRectangle.SetLocation(rectangle.Value);
        }
    }

    private void button5_Click(object sender, EventArgs e)
    {
        var tabWindowHandle = _chromeAccessibility.GetRenderWidgetHostHandle();
        AddLog($"chrome renderWidgetHost:{tabWindowHandle}");
        var rectangle = Window.GetRectangle(tabWindowHandle);
        if (rectangle.HasValue)
        {
            AddLog($"chrome renderWidgetHost rectangle:{rectangle.Value}");
            _highlightRectangle.SetLocation(rectangle.Value);
        }
    }

    private void button6_Click(object sender, EventArgs e)
    {
        var mainWindowHandle = _chromeAccessibility.GetMainWindowHandle();
        AddLog($"chrome mainWindowHandle:{mainWindowHandle}");
        var rectangle = Window.GetExtendedFrameBounds(mainWindowHandle);
        if (rectangle.HasValue)
        {
            AddLog($"chrome actualRect rectangle:{rectangle.Value}");
            _highlightRectangle.SetLocation(rectangle.Value);
        }
    }
}