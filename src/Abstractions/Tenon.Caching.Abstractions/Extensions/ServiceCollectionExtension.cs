using System;
using Microsoft.Extensions.DependencyInjection;
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
        services.Configure(setupAction);
        foreach (var serviceExtension in options.Extensions)
            serviceExtension.AddServices(services);
        return services;
    }
}