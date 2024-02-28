using Microsoft.Extensions.DependencyInjection;

namespace Tenon.Caching.RedisStackExchange;

internal sealed class RedisCacheOptions : ICachingOptionsExtension
{
    public void AddServices(IServiceCollection services)
    {
        throw new NotImplementedException();
    }
}