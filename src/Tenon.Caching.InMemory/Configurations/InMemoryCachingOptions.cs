using Microsoft.Extensions.Configuration;
using Tenon.Caching.Abstractions.Configurations;
using Tenon.Caching.InMemory.Extensions;

namespace Tenon.Caching.InMemory.Configurations;

public static class InMemoryCachingOptions
{
    public static CachingOptions UseInMemoryStorage(this CachingOptions options,
        IConfigurationSection redisCacheSection)
    {
        options.RegisterExtension(new CachingOptionsExtension());
        return options;
    }
}