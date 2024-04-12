using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.AspNetCore.Identity.EfCore.Sqlite.Extensions.Extensions;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Identity.Repository.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRepository(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        services.AddIdentityEfCoreSqlite<UserIdentityDbContext, User, Role, long>(
            configurationManager.GetSection("Sqlite"));
        services.AddSingleton<AbstractDbContextConfiguration, IdentityDbContextConfiguration>();
        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        });
        var userIdentityBuilder = new IdentityBuilder(typeof(User), typeof(Role), services);
        userIdentityBuilder.AddEntityFrameworkStores<UserIdentityDbContext>()
            .AddDefaultTokenProviders()
            .AddRoleManager<RoleManager<Role>>()
            .AddUserManager<UserManager<User>>();
        return services;
    }
}