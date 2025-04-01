using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tenon.FluentValidation.AspNetCore.Extensions.Options;

namespace Tenon.FluentValidation.AspNetCore.Extensions;

/// <summary>
///     FluentValidation 服务扩展
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    ///     为 ASP.NET Core WebAPI 添加 FluentValidation 验证器
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assemblies">包含验证器的程序集</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddFluentValidation(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        return services.AddFluentValidation(options => { }, assemblies);
    }

    /// <summary>
    ///     为 ASP.NET Core WebAPI 添加 FluentValidation 验证器
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置节</param>
    /// <param name="assemblies">包含验证器的程序集</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddFluentValidation(this IServiceCollection services,
        IConfigurationSection configuration,
        params Assembly[] assemblies)
    {
        services.AddOptions<FluentValidationOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return AddFluentValidationCore(services, assemblies);
    }

    /// <summary>
    ///     为 ASP.NET Core WebAPI 添加 FluentValidation 验证器
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <param name="assemblies">包含验证器的程序集</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddFluentValidation(this IServiceCollection services,
        Action<FluentValidationOptions> configureOptions,
        params Assembly[] assemblies)
    {
        services.AddOptions<FluentValidationOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return AddFluentValidationCore(services, assemblies);
    }

    private static IServiceCollection AddFluentValidationCore(IServiceCollection services, Assembly[] assemblies)
    {
        var options = services.BuildServiceProvider()
            .GetRequiredService<IOptions<FluentValidationOptions>>()
            .Value;

        // 注册验证器
        services.AddValidatorsFromAssemblies(assemblies, options.ValidatorLifetime);

        // 配置 API 行为选项
        if (options.DisableDefaultModelValidation)
            services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });

        return services;
    }
}