using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Mapper.Abstractions;

namespace Tenon.Mapper.Mapster.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddMapsterSetup(this IServiceCollection services, params Assembly[] assemblies)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(assemblies);
        var mapperConfig = new MapsterMapper.Mapper(typeAdapterConfig);
        services.AddSingleton<IMapper>(mapperConfig);
        services.AddSingleton<IObjectMapper, MapsterObject>();
        return services;
    }
}