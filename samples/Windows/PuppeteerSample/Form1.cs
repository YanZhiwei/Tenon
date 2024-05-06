using PuppeteerSample.Request;
using PuppeteerSharp;
using Tenon.Infra.Windows.ChromiumAccessibility;
using Tenon.Infra.Windows.Form.Common;
using Tenon.Puppeteer.Extensions;
using Tenon.Puppeteer.Extensions.Models;
using Tenon.Windows.Puppeteer.Extensions;
using Point = System.Drawing.Point;

namespace PuppeteerSample;

public partial class Form1 : Form
{
    private readonly ChromeAccessibility _chromeAccessibility;

    private IBrowser _browser;
    private IPage _page;
    private readonly Dictionary<string, Point> _elecFromPoints = new();
    public Form1()
    {
        InitializeComponent();
        _chromeAccessibility = new ChromeAccessibility();
        _chromeAccessibility.Detect();

        _elecFromPoints.Add("http://www.baidu.com", new Point()
        {
            Y = 335,
            X = 1200
        });
        _elecFromPoints.Add("https://seleniumbase.io/w3schools/iframes", new Point()
        {
            Y = 191,
            X = 1425
        });
        _elecfromPoint = _elecFromPoints["http://www.baidu.com"];
    }

    private void button1_Click(object sender, EventArgs e)
    {
        _browser = PuppeteerPool.LaunchAsync(
            new LaunchOptions
            {
                Headless = false,
                ExecutablePath = _chromeAccessibility.InstallPath,
                DefaultViewport = null
            }).ConfigureAwait(false).GetAwaiter().GetResult();
        _page = _browser.NewPageAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        var result = _page.GoToAsync("https://www.baidu.com").ConfigureAwait(false).GetAwaiter().GetResult();
        AddLog(
            $"GoToAsync Result: {result},title:{_page.GetTitleAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
    }

    private void AddLog(string message)
    {
        listBox1.UIBeginThread(ls => ls.AddItemSelected($"{DateTime.Now:HH:mm:ss} {message}"));
    }

    private void button2_Click(object sender, EventArgs e)
    {
        _browser.CloseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private void button3_Click(object sender, EventArgs e)
    {
        var script = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Script", "tenon.js"));
        var result = _page.InjectScriptTagAsync(new AddTagOptions
        {
            Id = "formartStr",
            Content = script
        }).ConfigureAwait(false).GetAwaiter().GetResult();
        AddLog($"InjectScript Result:{result}");
    }

    private void button4_Click(object sender, EventArgs e)
    {
        //var output = _page.EvaluateExpressionAsync<string>("formartStr('hello')").ConfigureAwait(false).GetAwaiter()
        //    .GetResult();
        var result = _page.EvaluateExpressionAsync(new PerformRequest<string>
        {
            FunctionName = "formartStr",
            FunctionParameter = "hello"
        }).ConfigureAwait(false).GetAwaiter().GetResult();
        AddLog($"EvaluateFunction Result: {result}");
    }

    private void button5_Click(object sender, EventArgs e)
    {
        var result = _page.IsReadyAsync().GetAwaiter().GetResult();
        AddLog($"IsReady Result: {result}");
    }

    private void button6_Click(object sender, EventArgs e)
    {
        var result = _page.IsActiveAsync().GetAwaiter().GetResult();
        AddLog($"IsActive Result: {result}");
    }

    private void button7_Click(object sender, EventArgs e)
    {
        _page.BringToFrontAsync().GetAwaiter().GetResult();
    }

    private void button8_Click(object sender, EventArgs e)
    {
        var pages = _browser.GetPagesByTitleAsync("google").GetAwaiter().GetResult();
        if (pages?.Any() ?? false)
        {
            foreach (var page in pages)
            {
                AddLog($"search page title:{page.GetTitleAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
        }
    }

    private void button9_Click(object sender, EventArgs e)
    {
        var pages = _browser.GetPagesByUrlAsync("https://www.google.com.hk/search?q*").GetAwaiter().GetResult();
        if (pages?.Any() ?? false)
        {
            foreach (var page in pages)
            {
                AddLog($"search page url:{page.Url}");
            }
        }
    }

    private void button10_Click(object sender, EventArgs e)
    {
        Point point = new Point((int)numericUpDown1.Value, (int)numericUpDown2.Value);
        IBrowser connectBrowser = WindowsPuppeteer.AttachToAsync(point).ConfigureAwait(false).GetAwaiter().GetResult();
        if (connectBrowser != null)
        {
            AddLog($"from point:{point} connectBrowser succeeded.");
            var page = _browser.NewPageAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            page.GoToAsync("https://twitter.com/home").ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else
        {
            AddLog($"from point:{point} connectBrowser failed.");
        }

    }

    private void button11_Click(object sender, EventArgs e)
    {
        var script = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Script", "tenonplugin.js"));
        var result = _page.InjectScriptTagAsync(new AddTagOptions
        {
            Id = "tenon_call_plugin",
            Content = script
        }).ConfigureAwait(false).GetAwaiter().GetResult();
        AddLog($"Inject plugin script Result:{result}");
    }

    private void button12_Click(object sender, EventArgs e)
    {
        try
        {
            var result = _page.EvaluateFunctionAsync(new PluginPerformRequest<string>
            {
                FunctionName = "pingTab",
                FunctionParameter = "hello"
            }).ConfigureAwait(false).GetAwaiter().GetResult();
            AddLog($"pingTab Result: {result}");
        }
        catch (Exception ex)
        {
            AddLog($"pingTab failed,error: {ex.Message}");
        }
    }

    private void button13_Click(object sender, EventArgs e)
    {
        try
        {
            var result = _page.EvaluateFunctionAsync(new PluginPerformRequest<Point>
            {
                FunctionName = "elementFromPoint",
                FunctionParameter = _elecfromPoint
            }).ConfigureAwait(false).GetAwaiter().GetResult();
            this.textBox1.UIBeginThread(txt => txt.Text = result["result"].ToString());
            AddLog($"elementFromPoint Result: {result}");
        }
        catch (Exception ex)
        {
            AddLog($"elementFromPoint failed,error: {ex.Message}");
        }
    }

    private void button14_Click(object sender, EventArgs e)
    {
        try
        {
            var result = _page.EvaluateFunctionAsync(new PluginPerformRequest<string>
            {
                FunctionName = "getElementRect",
                FunctionParameter = this.textBox1.Text.Trim()
            }).ConfigureAwait(false).GetAwaiter().GetResult();
            AddLog($"getElementRect Result: {result}");
        }
        catch (Exception ex)
        {
            AddLog($"getElementRect failed,error: {ex.Message}");
        }
    }

    private Point _elecfromPoint;
    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox comboBox = sender as ComboBox;
        var key = comboBox.SelectedItem?.ToString();
        if (key != null && _page != null)
        {
            _page.GoToAsync(key).ConfigureAwait(false).GetAwaiter().GetResult();
            _elecfromPoint = _elecFromPoints[key];
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        comboBox1.SelectedIndex = 0;
    }
}