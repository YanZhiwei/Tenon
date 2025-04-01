using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Tenon.Abstractions.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     注册应用服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assembly">程序集</param>
    public static void RegisterAppServices(this IServiceCollection services, Assembly assembly)
    {
        var serviceTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        t.GetInterfaces().Any(i => i != typeof(IAppService) &&
                                                   typeof(IAppService).IsAssignableFrom(i)));

        foreach (var implementationType in serviceTypes)
        {
            var serviceInterfaces = implementationType.GetInterfaces()
                .Where(i => i != typeof(IAppService) &&
                            typeof(IAppService).IsAssignableFrom(i));

            foreach (var serviceInterface in serviceInterfaces)
                services.AddScoped(serviceInterface, implementationType);
        }
    }
}