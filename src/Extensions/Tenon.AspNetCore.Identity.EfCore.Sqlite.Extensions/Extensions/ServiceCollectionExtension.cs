using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Repository.EfCore;
using Tenon.Repository.EfCore.Interceptors;
using Tenon.Repository.EfCore.Sqlite.Configurations;
using Tenon.Repository.EfCore.Sqlite.Transaction;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.AspNetCore.Identity.EfCore.Sqlite.Extensions.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddIdentityEfCoreSqlite<TDbContext, TUser, TRole, TKey>(
        this IServiceCollection services,
        IConfigurationSection sqliteSection, Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
        where TDbContext : IdentityDbContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        var sqliteConfig = sqliteSection.Get<SqliteOptions>();
        if (sqliteConfig == null)
            throw new ArgumentNullException(nameof(sqliteConfig));
        services.Configure<SqliteOptions>(sqliteSection);
        services.AddDbContext<DbContext, TDbContext>(options =>
        {
            options.UseSqlite(sqliteConfig.ConnectionString, sqliteOptionsAction);
        });
        foreach (var type in typeof(TDbContext).Assembly.DefinedTypes
                     .Where(t => t is { IsAbstract: false, IsGenericTypeDefinition: false }
                                 && typeof(AbstractEntityTypeConfiguration).IsAssignableFrom(t)))
            services.AddSingleton(typeof(AbstractEntityTypeConfiguration), type);
        services.AddScoped<IUnitOfWork, SqliteUnitOfWork>();
        services.AddScoped(typeof(IEfRepository<EfEntity, long>), typeof(EfRepository<EfEntity>));
        return services;
    }
}