using System.Reflection;
using CleanArchitecture.Identity.Application.Dtos.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Abstractions;
using Tenon.DistributedId.Abstractions.Extensions;
using Tenon.DistributedId.Snowflake;
using Tenon.DistributedId.Snowflake.Configurations;
using Tenon.Extensions.System;
using Tenon.FluentValidation.AspNetCore.Extensions.Extensions;
using Tenon.Infra.Redis.StackExchangeProvider;
using Tenon.Mapper.AutoMapper.Extensions;

namespace CleanArchitecture.Identity.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static readonly Assembly MainAssembly = Assembly.GetExecutingAssembly();

    public static IServiceCollection AddApplication(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configurationManager == null)
            throw new ArgumentNullException(nameof(configurationManager));
        AddAbstractImpService(services);
        AddSnowflakeDistributedId(services, configurationManager);
        services.AddFluentValidationSetup(
            typeof(UserLoginDtoValidator).Assembly);
        services.AddAutoMapperSetup(typeof(AutoMapperProfile).Assembly);
        return services;
    }

    private static void AddSnowflakeDistributedId(IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        var distributedIdSection = configurationManager.GetSection("DistributedId");
        services.AddDistributedId(options =>
        {
            options.UseSnowflake(distributedIdSection);
            options.UseWorkerNode<StackExchangeProvider>(distributedIdSection.GetSection("WorkerNode"));
        });
        services.AddHostedService<SnowflakeWorkerNodeHostedService>();
    }

    private static void AddAbstractImpService(IServiceCollection services)
    {
        var abstractAppServiceTypes = MainAssembly.GetSubInterfaces(typeof(IAppService));
        foreach (var appServiceType in abstractAppServiceTypes)
        {
            var implType = MainAssembly.ExportedTypes.FirstOrDefault(type =>
                type.IsAssignableTo(appServiceType) && type.IsNotAbstractClass(true));
            if (implType is null) continue;
            services.AddScoped(appServiceType, implType);
        }
    }
}