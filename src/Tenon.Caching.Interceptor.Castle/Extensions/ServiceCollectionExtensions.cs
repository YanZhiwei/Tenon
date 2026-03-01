using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Interceptor.Castle.Configurations;
using Tenon.Caching.Interceptor.Castle.KeyGenerators;

namespace Tenon.Caching.Interceptor.Castle.Extensions;

/// <summary>
/// <see cref="IServiceCollection" /> 的缓存代理服务注册扩展。
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 一次性注册缓存拦截所需依赖，并扫描程序集中继承 <see cref="ICacheableService" /> 的接口，自动注册其实现为代理。
    /// 调用方需已注册 <see cref="ICacheProvider" />。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="assembly">要扫描的程序集。</param>
    /// <param name="configureOptions">可选；配置 <see cref="CacheAsideInterceptorOptions" />。</param>
    /// <param name="lifetime">服务生命周期，默认 Scoped。</param>
    /// <returns>当前 <see cref="IServiceCollection" />。</returns>
    public static IServiceCollection AddProxiesFromAssembly(
        this IServiceCollection services,
        Assembly assembly,
        Action<CacheAsideInterceptorOptions>? configureOptions = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        services.TryAddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>();
        services.Configure(configureOptions ?? (_ => { }));
        services.AddLogging();

        foreach (var (interfaceType, implType) in FindCacheableServicePairs(assembly))
            AddCachedProxyInternal(services, interfaceType, implType, lifetime);

        return services;
    }

    /// <summary>
    /// 一次性注册缓存拦截所需依赖及接口代理：<see cref="ICacheKeyGenerator" />、<see cref="CacheAsideInterceptorOptions" />、日志、<see cref="AddCachedProxy{TInterface,TImplementation}" />。
    /// 调用方需已注册 <see cref="ICacheProvider" />（如 AddInMemoryCache 或 AddCaching + UseInMemoryStorage/UseRedisStackExchange）。
    /// </summary>
    /// <typeparam name="TInterface">接口类型。</typeparam>
    /// <typeparam name="TImplementation">实现类型。</typeparam>
    /// <param name="services">服务集合。</param>
    /// <param name="configureOptions">可选；配置 <see cref="CacheAsideInterceptorOptions" />（如 DelayedDelete）。</param>
    /// <param name="lifetime">服务生命周期，默认 Scoped。</param>
    /// <returns>当前 <see cref="IServiceCollection" />，便于链式调用。</returns>
    public static IServiceCollection AddCachedProxyServices<TInterface, TImplementation>(
        this IServiceCollection services,
        Action<CacheAsideInterceptorOptions>? configureOptions = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>();
        services.Configure(configureOptions ?? (_ => { }));
        services.AddLogging();

        return services.AddCachedProxy<TInterface, TImplementation>(lifetime);
    }

    /// <summary>
    /// 注册带缓存拦截的接口代理。调用方需已注册 <see cref="ICacheProvider" />、<see cref="ICacheKeyGenerator" />、
    /// <see cref="OptionsConfigurationServiceCollectionExtensions.Configure{T}(IServiceCollection, System.Action{T})" /> 配置 <see cref="CacheAsideInterceptorOptions" />、<see cref="LoggingServiceCollectionExtensions.AddLogging" />。
    /// </summary>
    /// <typeparam name="TInterface">接口类型。</typeparam>
    /// <typeparam name="TImplementation">实现类型。</typeparam>
    /// <param name="services">服务集合。</param>
    /// <param name="lifetime">服务生命周期，默认 Scoped。</param>
    /// <returns>当前 <see cref="IServiceCollection" />，便于链式调用。</returns>
    public static IServiceCollection AddCachedProxy<TInterface, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<ProxyGenerator>();
        services.TryAddTransient(sp => new CacheAsideAsyncInterceptor(
            sp.GetRequiredService<ICacheProvider>(),
            sp.GetRequiredService<ICacheKeyGenerator>(),
            sp.GetRequiredService<IOptions<CacheAsideInterceptorOptions>>().Value,
            sp.GetRequiredService<ILogger<CacheAsideAsyncInterceptor>>()));

        services.TryAdd(new ServiceDescriptor(typeof(TImplementation), typeof(TImplementation), lifetime));
        services.Add(new ServiceDescriptor(
            typeof(TInterface),
            sp =>
            {
                var target = (TImplementation)sp.GetRequiredService(typeof(TImplementation));
                var interceptor = sp.GetRequiredService<CacheAsideAsyncInterceptor>();
                var generator = sp.GetRequiredService<ProxyGenerator>();
                return generator.CreateInterfaceProxyWithTarget<TInterface>(target, interceptor);
            },
            lifetime));

        return services;
    }

    private static IEnumerable<(Type InterfaceType, Type ImplType)> FindCacheableServicePairs(Assembly assembly)
    {
        var types = assembly.GetExportedTypes();
        var cacheableInterfaces = types
            .Where(t => t.IsInterface && t != typeof(ICacheableService) && t.GetInterfaces().Contains(typeof(ICacheableService)))
            .ToList();

        foreach (var iface in cacheableInterfaces)
        {
            var impls = types
                .Where(t => t.IsClass && !t.IsAbstract && iface.IsAssignableFrom(t))
                .ToList();
            if (impls.Count == 0)
                continue;

            var impl = impls.Count == 1
                ? impls[0]
                : impls.FirstOrDefault(c => c.Name == iface.Name.TrimStart('I')) ?? impls[0];
            yield return (iface, impl);
        }
    }

    private static void AddCachedProxyInternal(IServiceCollection services, Type interfaceType, Type implType, ServiceLifetime lifetime)
    {
        var method = typeof(ServiceCollectionExtensions)
            .GetMethod(nameof(AddCachedProxy), 2, [typeof(IServiceCollection), typeof(ServiceLifetime)])
            ?? throw new InvalidOperationException("AddCachedProxy generic method not found.");
        var generic = method.MakeGenericMethod(interfaceType, implType);
        generic.Invoke(null, [services, lifetime]);
    }
}
