using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Mapper.Abstractions;

namespace Tenon.Mapper.AutoMapper.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddAutoMapperSetup(this IServiceCollection services,
        params Type[] profileAssemblyMarkerTypes)
    {
        services.AddAutoMapper(profileAssemblyMarkerTypes);
        services.AddSingleton<IObjectMapper, AutoMapperObject>();
        return services;
    }

    public static IServiceCollection AddAutoMapperSetup(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddAutoMapper(assemblies);
        services.AddSingleton<IObjectMapper, AutoMapperObject>();
        return services;
    }
}