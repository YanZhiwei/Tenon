using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tenon.Serialization.Json.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSystemTextJsonSerializer(this IServiceCollection services)
    {
        services.TryAddSingleton<ISerializer, SystemTextJsonSerializer>();
        return services;
    }

    public static IServiceCollection AddKeyedSystemTextJsonSerializer(this IServiceCollection services,
        string serviceKey)
    {
        if (string.IsNullOrWhiteSpace(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        services.TryAddKeyedSingleton<ISerializer, SystemTextJsonSerializer>(serviceKey);
        return services;
    }
}