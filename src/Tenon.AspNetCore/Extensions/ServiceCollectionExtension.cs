using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.AspNetCore.Authentication.Bearer;
using Tenon.AspNetCore.Authorization;
using Tenon.AspNetCore.Configuration;

namespace Tenon.AspNetCore.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureJwtBearerAuthenticationOptions<TBearerAuthenticationHandler>(
        this IServiceCollection services, IConfigurationSection jwtSection,
        Action<BearerSchemeOptions> configureOptions, string? displayName = null)
        where TBearerAuthenticationHandler : AuthenticationHandler<BearerSchemeOptions>
    {
        if (jwtSection == null)
            throw new ArgumentNullException(nameof(jwtSection));
        var jwtOptions = jwtSection.Get<JwtOptions>();
        if (jwtOptions == null)
            throw new ArgumentNullException(nameof(jwtSection));

        services.AddAuthentication(BearerDefaults.AuthenticationScheme)
            .AddScheme<BearerSchemeOptions, TBearerAuthenticationHandler>(BearerDefaults.AuthenticationScheme,
                displayName,
                configureOptions);
        return services.Configure<JwtOptions>(jwtSection);
    }

    public static IServiceCollection AddAuthorization<TAuthorizationHandler, TAuthorizationRequirement>(
        this IServiceCollection services) where TAuthorizationRequirement : IAuthorizationRequirement, new()
        where TAuthorizationHandler : class, IAuthorizationHandler
    {
        services
            .AddScoped<IAuthorizationHandler, TAuthorizationHandler>();
        return services
            .AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizePolicy.Default,
                    policy => { policy.Requirements.Add(new TAuthorizationRequirement()); });
            });
    }


    public static IServiceCollection AddCors(this IServiceCollection services, string policyName,
        IConfigurationSection corsHostSection)
    {
        if (string.IsNullOrWhiteSpace(policyName))
            throw new ArgumentNullException(nameof(policyName));
        if (corsHostSection == null)
            throw new ArgumentNullException(nameof(corsHostSection));
        var corsHosts = corsHostSection.Get<string>();
        if (string.IsNullOrWhiteSpace(corsHosts))
            throw new ArgumentNullException(nameof(corsHosts));
        Action<CorsPolicyBuilder> corsPolicyAction =
            corsPolicy => corsPolicy.AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        if (corsHosts == "*")
            corsPolicyAction += corsPolicy => corsPolicy.SetIsOriginAllowed(_ => true);
        else
            corsPolicyAction += corsPolicy => corsPolicy.WithOrigins(corsHosts.Split(','));

        return services.AddCors(options => options.AddPolicy(policyName, corsPolicyAction));
    }
}