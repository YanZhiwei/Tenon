using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Caching.Abstractions.Configurations;

namespace Tenon.Caching.Abstractions.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCaching(this IServiceCollection services, Action<CachingOptions> setupAction)
    {
        if (setupAction == null)
            throw new ArgumentNullException(nameof(setupAction));
        var options = new CachingOptions();
        setupAction(options);
        if (options.KeyedServices && string.IsNullOrWhiteSpace(options.KeyedServiceKey))
            throw new ArgumentNullException(nameof(options.KeyedServiceKey));

        foreach (var serviceExtension in options.Extensions)
            serviceExtension.AddServices(services);

        if (options.KeyedServices)
            services.TryAddKeyedSingleton(options.KeyedServiceKey, options);
        else
            services.TryAddSingleton(options);
        return services;
    }
}