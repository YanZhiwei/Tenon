using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tenon.FluentValidation.AspNetCore.Extensions.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddFluentValidationSetup(this IServiceCollection services,
        Action<FluentValidationAutoValidationConfiguration>? configurationSetUpAction = null,
        params Assembly[] assemblies)
    {
        services.AddFluentValidationAutoValidation(configurationSetUpAction);
        services.AddValidatorsFromAssemblies(assemblies);
        return services;
    }
}