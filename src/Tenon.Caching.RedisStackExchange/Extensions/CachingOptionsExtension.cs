using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tenon.Caching.RedisStackExchange.Extensions;

internal class CachingOptionsExtension(IConfigurationSection redisCacheSection, string? serviceKey = null)
    : ICachingOptionsExtension
{
    private readonly IConfigurationSection _redisCacheSection =
        redisCacheSection ?? throw new ArgumentNullException(nameof(redisCacheSection));

    public string? ServiceKey { get; } = serviceKey;

    public void AddServices(IServiceCollection services)
    {
        if (string.IsNullOrWhiteSpace(ServiceKey))
            services.AddRedisStackExchangeCache(_redisCacheSection);
        else
            services.AddKeyedRedisStackExchangeCache(ServiceKey, _redisCacheSection);
    }
}