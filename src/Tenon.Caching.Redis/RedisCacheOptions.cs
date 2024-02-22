using Microsoft.Extensions.DependencyInjection;

namespace Tenon.Caching.Redis;

internal sealed class RedisCacheOptions : ICachingOptionsExtension
{
    public void AddServices(IServiceCollection services)
    {
        throw new NotImplementedException();
    }
}