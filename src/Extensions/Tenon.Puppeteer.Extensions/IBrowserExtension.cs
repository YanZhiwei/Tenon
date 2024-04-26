using PuppeteerSharp;

namespace Tenon.Puppeteer.Extensions;

public static class BrowserExtension
{
    public static async Task<IPage> AttachTo(this IBrowser browser, Point point)
    {
        return await Task.FromResult<IPage>(null);
    }
}