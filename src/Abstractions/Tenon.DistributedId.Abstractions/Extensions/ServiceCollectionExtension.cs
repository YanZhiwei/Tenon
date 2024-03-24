using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.DistributedId.Abstractions.Configurations;

namespace Tenon.DistributedId.Abstractions.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDistributedId(this IServiceCollection services,
        Action<DistributedIdOptions> setupAction)
    {
        if (setupAction == null)
            throw new ArgumentNullException(nameof(setupAction));
        var options = new DistributedIdOptions();
        setupAction(options);


        foreach (var serviceExtension in options.Extensions)
            serviceExtension.AddServices(services);

        //if (!string.IsNullOrWhiteSpace(options.KeyedServiceKey))
        //    services.TryAddKeyedSingleton(options.KeyedServiceKey, options);
        //else
        services.TryAddSingleton(options);
        return services;
    }
}