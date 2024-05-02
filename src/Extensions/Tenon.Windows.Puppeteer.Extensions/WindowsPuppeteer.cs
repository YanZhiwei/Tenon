using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using Tenon.Infra.Windows.Win32;
using Tenon.Puppeteer.Extensions;
using Point = System.Drawing.Point;

namespace Tenon.Windows.Puppeteer.Extensions;

public static class WindowsPuppeteer
{
    public static async Task<IBrowser?> AttachToAsync(Point point, ILoggerFactory? loggerFactory = null)
    {
        if (point.IsEmpty) return null;
        var mainWindowHandle = Window.GetTop(point);
        if (mainWindowHandle == IntPtr.Zero) return null;
        var puppeteer = PuppeteerPool.RunningPuppeteers.FirstOrDefault(p => p.MainWindowHandle == mainWindowHandle);
        if (puppeteer == null) return null;
        return await PuppeteerSharp.Puppeteer.ConnectAsync(new ConnectOptions
        {
            BrowserWSEndpoint = puppeteer.WebSocketEndpoint
        }, loggerFactory).ConfigureAwait(false);
    }
}