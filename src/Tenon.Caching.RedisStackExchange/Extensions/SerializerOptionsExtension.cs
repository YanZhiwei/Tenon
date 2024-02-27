using Microsoft.Extensions.DependencyInjection;
using Tenon.Caching.Configurations;
using Tenon.Serialization.Json.Extensions;

namespace Tenon.Caching.RedisStackExchange.Extensions;

internal class SerializerOptionsExtension(bool useSystemTextJson, CachingOptions options)
    : ICachingOptionsExtension
{
    public void AddServices(IServiceCollection services)
    {
        if (options.KeyedServices)
        {
            if (useSystemTextJson)
                services.AddKeyedSystemTextJsonSerializer(options.KeyedServiceKey);
        }
        else
        {
            if (useSystemTextJson)
                services.AddSystemTextJsonSerializer();
        }
    }
}