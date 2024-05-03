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

    public Form1()
    {
        InitializeComponent();
        _chromeAccessibility = new ChromeAccessibility();
        _chromeAccessibility.Detect();
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
        var result = _page.GoToAsync("http://www.google.com").ConfigureAwait(false).GetAwaiter().GetResult();
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
                FunctionName = "pingTabPlugin",
                FunctionParameter = "hello"
            }).ConfigureAwait(false).GetAwaiter().GetResult();
            AddLog($"EvaluateFunction Result: {result}");
        }
        catch (Exception ex)
        {
            AddLog($"EvaluateFunction failed,error: {ex.Message}");
        }
    }
}