using System.Text;
using PuppeteerSharp;
using Tenon.Puppeteer.Extensions.Models;
using Tenon.Serialization.Abstractions;
using Tenon.Serialization.Json;

namespace Tenon.Puppeteer.Extensions;

public static class PageExtension
{
    private static readonly ISerializer Serializer;

    static PageExtension()
    {
        Serializer = new SystemTextJsonSerializer();
    }

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

    public static async Task<string> EvaluateFunctionAsync<TReq>(this IPage page,
        PerformRequest<TReq> request)
    {
        CheckPage(page);
        var evaluateScript = new StringBuilder();
        evaluateScript.Append(request.FunctionName);
        evaluateScript.Append("('");
        evaluateScript.AppendFormat($"{Serializer.SerializeObject(request.FunctionParameter)}");
        evaluateScript.Append("')");
        var evaluateResult =
            await page.EvaluateExpressionAsync<string>(evaluateScript.ToString()).ConfigureAwait(false);
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