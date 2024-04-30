using System.Text.RegularExpressions;
using PuppeteerSharp;

namespace Tenon.Puppeteer.Extensions;

public static class BrowserExtension
{
    public static async Task<IPage> AttachToAsync(this IBrowser browser, Point point)
    {
        return await Task.FromResult<IPage>(null);
    }

    public static async Task<IPage?> GetActivePageAsync(this IBrowser browser)
    {
        CheckBrowser(browser);
        var pages = await browser.PagesAsync().ConfigureAwait(false);
        if (!pages.Any()) return null;
        foreach (var page in pages)
            if (await page.IsActiveAsync())
                return page;

        return null;
    }

    private static void CheckBrowser(IBrowser browser)
    {
        if (browser == null)
            throw new NullReferenceException(nameof(browser));
    }

    public static async Task<IPage[]?> GetPagesByTitleAsync(this IBrowser browser, string title)
    {
        CheckBrowser(browser);
        var pages = await browser.PagesAsync().ConfigureAwait(false);
        if (!pages.Any()) return null;
        var titleRegexPattern = "^" + Regex.Escape(title).Replace("\\*", ".*") + "$";
        var searchPages = new List<IPage>();
        foreach (var page in pages)
        {
            var pageTitle = await page.GetTitleAsync().ConfigureAwait(false);
            if (Regex.IsMatch(pageTitle, titleRegexPattern, RegexOptions.IgnoreCase)) searchPages.Add(page);
        }

        return searchPages.ToArray();
    }

    public static async Task<IPage?> GetFirstOrDefaultPageByTitleAsync(this IBrowser browser, string title)
    {
        CheckBrowser(browser);
        var pages = await browser.PagesAsync().ConfigureAwait(false);
        if (!pages.Any()) return null;
        var titleRegexPattern = "^" + Regex.Escape(title).Replace("\\*", ".*") + "$";
        foreach (var page in pages)
        {
            var pageTitle = await page.GetTitleAsync().ConfigureAwait(false);
            if (Regex.IsMatch(pageTitle, titleRegexPattern, RegexOptions.IgnoreCase))
                return page;
        }

        return null;
    }

    public static async Task<IPage[]?> GetPagesByUrlAsync(this IBrowser browser, string url)
    {
        CheckBrowser(browser);
        var pages = await browser.PagesAsync().ConfigureAwait(false);
        if (!pages.Any()) return null;
        var titleRegexPattern = "^" + Regex.Escape(url).Replace("\\*", ".*") + "$";
        var searchPages = new List<IPage>();
        foreach (var page in pages)
        {
            var pageUrl = page.Url;
            if (Regex.IsMatch(pageUrl, titleRegexPattern, RegexOptions.IgnoreCase)) searchPages.Add(page);
        }

        return searchPages.ToArray();
    }

    public static async Task<IPage?> GetFirstOrDefaultPageByUrlAsync(this IBrowser browser, string url)
    {
        CheckBrowser(browser);
        var pages = await browser.PagesAsync().ConfigureAwait(false);
        if (!pages.Any()) return null;
        var urlRegexPattern = "^" + Regex.Escape(url).Replace("\\*", ".*") + "$";
        foreach (var page in pages)
        {
            var pageUrl = page.Url;
            if (Regex.IsMatch(pageUrl, urlRegexPattern, RegexOptions.IgnoreCase))
                return page;
        }

        return null;
    }
}