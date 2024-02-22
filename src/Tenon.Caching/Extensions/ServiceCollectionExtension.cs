using System;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Caching.Configurations;

namespace Tenon.Caching.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCaching(this IServiceCollection services, Action<CachingOptions> setupAction)
    {
        if (setupAction == null)
            throw new ArgumentNullException(nameof(setupAction));

        var options = new CachingOptions();
        setupAction(options);
        foreach (var serviceExtension in options.Extensions)
            serviceExtension.AddServices(services);
        return services;
    }
}