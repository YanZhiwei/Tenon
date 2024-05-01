using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using Tenon.Puppeteer.Extensions.Exceptions;
using Tenon.Puppeteer.Extensions.Models;

namespace Tenon.Puppeteer.Extensions;

public sealed class PuppeteerPool
{
    internal static readonly ConcurrentDictionary<string, PuppeteerMetadata> PuppeteerMetadataDict;

    static PuppeteerPool()
    {
        PuppeteerMetadataDict = new ConcurrentDictionary<string, PuppeteerMetadata>();
    }

    public static async Task<IBrowser> LaunchAsync(LaunchOptions options, ILoggerFactory loggerFactory = null)
    {
        var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, loggerFactory).ConfigureAwait(false);
        var keyId = browser.GetKeyId();
        if (!PuppeteerMetadataDict.TryAdd(keyId, new PuppeteerMetadata
        {
            BrowserType = browser.BrowserType,
            CreatedTime = DateTime.UtcNow,
            MainWindowHandle = browser.Process.MainWindowHandle,
            ProcessId = browser.Process.Id,
            WebSocketEndpoint = browser.WebSocketEndpoint
        }))
            throw new PuppeteerPoolException("Launch failed");
        browser.Closed += Browser_Closed;
        return browser;
    }

    private static void Browser_Closed(object? sender, EventArgs e)
    {
        if (sender is IBrowser browser)
        {
            var keyId = browser.GetKeyId();
            browser.Closed -= Browser_Closed;
            PuppeteerMetadataDict.TryRemove(keyId, out _);
        }
    }

    public static IReadOnlyCollection<PuppeteerMetadata> RunningPuppeteers => PuppeteerMetadataDict.Values.ToArray();
}