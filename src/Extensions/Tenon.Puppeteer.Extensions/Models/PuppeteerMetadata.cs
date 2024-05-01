using PuppeteerSharp;

namespace Tenon.Puppeteer.Extensions.Models;
[Serializable]
public sealed class PuppeteerMetadata
{
    public SupportedBrowser BrowserType { get; set; }
    public string WebSocketEndpoint { get; set; }
    public IntPtr MainWindowHandle { get; set; }
    public int ProcessId { get; set; }
    public DateTime CreatedTime { get; set; }
}