using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Tenon.Infra.Consul.Configurations;
using Tenon.Infra.Consul.LoadBalancer;
using Tenon.Infra.Consul.RefitClient.Configurations;
using Tenon.Serialization.Json;

namespace Tenon.Infra.Consul.RefitClient.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConsulRestClient<TRestClient>(this IServiceCollection services,
        IConfigurationSection consulRestClientSection, ILoadBalancer? loadBalancer = null)
        where TRestClient : class
    {
        if (consulRestClientSection == null)
            throw new ArgumentNullException(nameof(consulRestClientSection));
        var consulRestClientOptions = consulRestClientSection.Get<ConsulRestClientOptions>();
        if (consulRestClientOptions == null)
            throw new ArgumentNullException(nameof(consulRestClientOptions));
        loadBalancer ??= new RandomLoadBalancer();
        var contentSerializer =
            new SystemTextJsonContentSerializer(SystemTextJsonSerializer.DefaultJsonSerializerOption);
        var refitSettings = new RefitSettings(contentSerializer);
        var httpClientBuilder = services.AddRefitClient<TRestClient>(refitSettings);
        httpClientBuilder.ConfigureHttpClient(httpClient =>
                httpClient.BaseAddress = new Uri(consulRestClientOptions.ConsulUrl))
            .AddHttpMessageHandler<ConsulDiscoverDelegatingHandler>();
        return services.Configure<ConsulRestClientOptions>(consulRestClientSection)
            .Configure<ConsulDiscoveryOptions>(consulRestClientSection)
            .Configure<ConsulRestClientOptions>(consulRestClientSection)
            .AddSingleton(loadBalancer)
            .AddScoped<ConsulDiscoverDelegatingHandler>();
    }
}