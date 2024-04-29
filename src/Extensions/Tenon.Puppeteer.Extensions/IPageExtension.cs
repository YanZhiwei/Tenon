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

    public static async Task<bool> InjectScriptTagAsync(this IPage page, AddTagOptions options)
    {
        var htmlContent = await page.GetContentAsync().ConfigureAwait(false);
        var jsIdentity = options.Id?.ToString();
        if (htmlContent.IndexOf(jsIdentity, StringComparison.OrdinalIgnoreCase) == -1)
            await page.AddScriptTagAsync(options);
        htmlContent = await page.GetContentAsync().ConfigureAwait(false);
        return htmlContent.IndexOf(jsIdentity, StringComparison.OrdinalIgnoreCase) != -1;
    }

    public static async Task<string> EvaluateFunctionAsync<TReq>(this IPage page,
        PerformRequest<TReq> request)
    {
        var evaluateScript = new StringBuilder();
        evaluateScript.Append(request.FunctionName);
        evaluateScript.Append("('");
        evaluateScript.AppendFormat($"{Serializer.SerializeObject(request.FunctionParameter)}");
        evaluateScript.Append("')");
        var evaluateResult =
            await page.EvaluateExpressionAsync<string>(evaluateScript.ToString()).ConfigureAwait(false);
        return evaluateResult;
    }
}