using Microsoft.Extensions.DependencyInjection;

namespace Tenon.MediatR.Extensions.EventBus.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatREventBus(this IServiceCollection services,
        Action<MediatRServiceConfiguration> configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));
        services.AddMediatR(configuration);
        services.AddScoped<IEventBus, MediatREventBus>();
        return services;
    }
}