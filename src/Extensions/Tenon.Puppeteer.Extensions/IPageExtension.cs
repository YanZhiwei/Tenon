using PuppeteerSharp;
using Tenon.Puppeteer.Extensions.Models;

namespace Tenon.Puppeteer.Extensions;

public static class PageExtension
{
    public static async Task<bool> InjectScriptTagAsync(this IPage page, AddTagOptions options)
    {
        var htmlContent = await page.GetContentAsync().ConfigureAwait(false);
        var jsIdentity = options.Id?.ToString();
        if (htmlContent.IndexOf(jsIdentity, StringComparison.OrdinalIgnoreCase) == -1)
            await page.AddScriptTagAsync(options);
        htmlContent = await page.GetContentAsync().ConfigureAwait(false);
        return htmlContent.IndexOf(jsIdentity, StringComparison.OrdinalIgnoreCase) != -1;
    }

    public static async Task<PerformResult<TRes>> EvaluateFunctionAsync<TReq, TRes>(this IPage page,
        PerformRequest<TReq> request)
    {
        return await Task.FromResult<TRes>(default);
    }

    public static async Task<PerformResult<string>> EvaluateFunctionAsync<TReq>(this IPage page,
        PerformRequest<TReq> request)
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        return "hello world";
    }
}