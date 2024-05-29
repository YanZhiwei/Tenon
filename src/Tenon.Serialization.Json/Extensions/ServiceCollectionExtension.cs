using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Serialization.Abstractions;

namespace Tenon.Serialization.Json.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSystemTextJsonSerializer(this IServiceCollection services,
        JsonSerializerOptions? options = null)
    {
        if (options == null)
            services.TryAddSingleton<ISerializer, SystemTextJsonSerializer>();
        else
            services.TryAddSingleton<ISerializer>(new SystemTextJsonSerializer(options));
        return services;
    }


    public static IServiceCollection AddKeyedSystemTextJsonSerializer(this IServiceCollection services,
        string? serviceKey, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(serviceKey))
            throw new ArgumentNullException(nameof(serviceKey));
        if (options == null)
            services.TryAddKeyedSingleton<ISerializer, SystemTextJsonSerializer>(serviceKey);
        else
            services.TryAddKeyedSingleton<ISerializer>(serviceKey, new SystemTextJsonSerializer(options));
        return services;
    }
}