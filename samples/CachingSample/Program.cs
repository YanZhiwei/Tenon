using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.Caching.Abstractions;
using Tenon.Caching.InMemory.Extensions;

namespace CachingSample;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .Build();

        var services = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            })
            .AddInMemoryCache();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<ICacheProvider>();

        const string key = "sample:hello";
        const string value = "Tenon.Caching.InMemory";

        cache.Set(key, value, TimeSpan.FromMinutes(1));
        var get = cache.Get<string>(key);
        Console.WriteLine(get.HasValue ? $"Get: {get.Value}" : "Get: (miss)");

        await cache.SetAsync(key + ":async", value, TimeSpan.FromMinutes(1)).ConfigureAwait(false);
        var getAsync = await cache.GetAsync<string>(key + ":async").ConfigureAwait(false);
        Console.WriteLine(getAsync.HasValue ? $"GetAsync: {getAsync.Value}" : "GetAsync: (miss)");

        await cache.KeysExpireAsync(new[] { key }, TimeSpan.FromSeconds(2)).ConfigureAwait(false);
        Console.WriteLine("KeysExpireAsync(keys, 2s) invoked.");
        Console.WriteLine("Done. Press Enter to exit.");
        Console.ReadLine();
    }
}
