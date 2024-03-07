using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.DistributedLocker.Abstractions.Configurations;

namespace Tenon.DistributedLocker.Abstractions.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDistributedLocker(this IServiceCollection services,
        Action<DistributedLockerOptions> setupAction)
    {
        if (setupAction == null)
            throw new ArgumentNullException(nameof(setupAction));
        var options = new DistributedLockerOptions();
        setupAction(options);

        foreach (var serviceExtension in options.Extensions)
            serviceExtension.AddServices(services);

        if (!string.IsNullOrWhiteSpace(options.KeyedServiceKey))
            services.TryAddKeyedSingleton(options.KeyedServiceKey, options);
        else
            services.TryAddSingleton(options);
        return services;
    }
}