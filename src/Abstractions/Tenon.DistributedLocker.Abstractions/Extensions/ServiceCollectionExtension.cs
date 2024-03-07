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