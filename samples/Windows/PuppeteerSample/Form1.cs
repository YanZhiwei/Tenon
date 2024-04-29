using PuppeteerSharp;
using Tenon.Infra.Windows.ChromiumAccessibility;
using Tenon.Infra.Windows.Form.Common;
using Tenon.Puppeteer.Extensions;

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
        _browser = Puppeteer.LaunchAsync(
            new LaunchOptions
            {
                Headless = false,
                ExecutablePath = _chromeAccessibility.InstallPath,
                DefaultViewport = null
            }).ConfigureAwait(false).GetAwaiter().GetResult();
        _page = _browser.NewPageAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        var result = _page.GoToAsync("http://www.google.com").ConfigureAwait(false).GetAwaiter().GetResult();
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
        var script = File.ReadAllText("tenon.js");
        var result = _page.InjectScriptTagAsync(new AddTagOptions
        {
            Id = "formartStr",
            Content = script
        }).ConfigureAwait(false).GetAwaiter().GetResult();
        var output = _page.EvaluateExpressionAsync<string>("formartStr('hello')").ConfigureAwait(false).GetAwaiter()
            .GetResult();
        AddLog($"InjectScript {result}");
    }
}