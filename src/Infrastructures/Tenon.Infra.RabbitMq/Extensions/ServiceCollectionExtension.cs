using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Infra.RabbitMq.Configurations;

namespace Tenon.Infra.RabbitMq.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services,
        IConfigurationSection rabbitMqSection)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var rabbitMqConfig = rabbitMqSection.Get<RabbitMqOptions>();
        if (rabbitMqConfig == null)
            throw new NullReferenceException(nameof(rabbitMqConfig));
        services.Configure<RabbitMqOptions>(rabbitMqSection);
        return services.AddSingleton<RabbitMqConnection>();
    }
}