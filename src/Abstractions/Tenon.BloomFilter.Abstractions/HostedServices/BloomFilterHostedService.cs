using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tenon.BloomFilter.Abstractions.HostedServices;

public sealed class BloomFilterHostedService(
    IEnumerable<IBloomFilter> bloomFilters,
    ILogger<BloomFilterHostedService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var bloomFilter in bloomFilters)
        {
            var result = await bloomFilter.InitAsync();
            logger.LogInformation($"BloomFilter:{bloomFilter.Options.Name} Init result: {result}");
        }
    }
}