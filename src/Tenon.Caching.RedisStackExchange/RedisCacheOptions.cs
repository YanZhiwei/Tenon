using Microsoft.Extensions.DependencyInjection;
using Tenon.Caching.Abstractions;

namespace Tenon.Caching.RedisStackExchange;

internal sealed class RedisCacheOptions : ICachingOptionsExtension
{
    public void AddServices(IServiceCollection services)
    {
        throw new NotImplementedException();
    }
}