using PuppeteerSharp;
using Tenon.Puppeteer.Extensions.Models;

namespace Tenon.Puppeteer.Extensions;

public static class PageExtension
{
    public static string GetPageId(this Page page)
    {
        CheckPage(page);
        return page.MainFrame.Id;
    }

    public static async Task<bool> InjectScriptTagAsync(this IPage page, AddTagOptions options)
    {
        CheckPage(page);
        var jsIdentity = options.Id?.ToString();
        if (string.IsNullOrEmpty(jsIdentity))
            throw new ArgumentNullException(nameof(options.Id));
        if (string.IsNullOrEmpty(options.Content))
            throw new ArgumentNullException(nameof(options.Content));
        var htmlContent = await page.GetContentAsync().ConfigureAwait(false);
        if (htmlContent.IndexOf(jsIdentity, StringComparison.OrdinalIgnoreCase) == -1)
            await page.AddScriptTagAsync(options);
        htmlContent = await page.GetContentAsync().ConfigureAwait(false);
        return htmlContent.IndexOf(jsIdentity, StringComparison.OrdinalIgnoreCase) != -1;
    }

    private static void CheckPage(IPage page)
    {
        if (page == null)
            throw new NullReferenceException(nameof(page));
    }

    public static async Task<string> EvaluateExpressionAsync<TReq>(this IPage page,
        PerformRequest<TReq> request)
    {
        CheckPage(page);
        var evaluateScript = request.GenerateScript();
        var evaluateResult =
            await page.EvaluateExpressionAsync<string>(evaluateScript).ConfigureAwait(false);
        return evaluateResult;
    }

    public static async Task<dynamic> EvaluateFunctionAsync<TReq>(this IPage page,
        PerformRequest<TReq> request)
    {
        CheckPage(page);
        var evaluateScript = request.GenerateScript();
        var evaluateResult =
            await page.EvaluateFunctionAsync<dynamic>(evaluateScript).ConfigureAwait(false);
        return evaluateResult;
    }

    public static async Task<bool> IsReadyAsync(this IPage page)
    {
        CheckPage(page);
        var readyState = await page.EvaluateExpressionAsync<string>("document.readyState").ConfigureAwait(false);
        return readyState.Equals("complete", StringComparison.OrdinalIgnoreCase);
    }

    public static async Task<bool> IsActiveAsync(this IPage page)
    {
        CheckPage(page);
        var readyState = await page.EvaluateExpressionAsync<string>("document.visibilityState").ConfigureAwait(false);
        return readyState.Equals("visible", StringComparison.OrdinalIgnoreCase);
    }
}