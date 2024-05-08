using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tenon.AspNetCore.Authentication.Google.Extensions.Extensions;

public static class ServiceCollectionExtension
{
    public static AuthenticationBuilder AddGoogleScope(this AuthenticationBuilder builder,
        IConfigurationSection googleSection, Action<GoogleOptions>? googleOptionsAction = null)
    {
        if (googleSection == null)
            throw new ArgumentNullException(nameof(googleSection));
        var googleConfig = googleSection.Get<Configurations.GoogleOptions>();
        if (googleConfig == null)
            throw new ArgumentNullException(nameof(Configurations.GoogleOptions));
        if (string.IsNullOrEmpty(googleConfig.ClientId))
            throw new ArgumentNullException(nameof(googleConfig.ClientId));
        if (string.IsNullOrEmpty(googleConfig.ClientSecret))
            throw new ArgumentNullException(nameof(googleConfig.ClientSecret));
        googleConfig.Events = new OAuthEvents();
        builder.AddGoogle(opts =>
        {
            opts.Events = googleConfig.Events;
            if (!string.IsNullOrEmpty(googleConfig.Prompt))
                opts.Events.OnRedirectToAuthorizationEndpoint = context =>
                {
                    context.RedirectUri += $"&prompt={googleConfig.Prompt}";
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            googleOptionsAction?.Invoke(opts);
            opts.ClientId = googleConfig.ClientId;
            opts.ClientSecret = googleConfig.ClientSecret;
            if (googleConfig.Scope?.Count >= 1)
                foreach (var scope in googleConfig.Scope)
                    opts.Scope.Add(scope);
            if (!string.IsNullOrEmpty(googleConfig.AccessType))
                opts.AccessType = googleConfig.AccessType;
        });
        return builder;
    }
}