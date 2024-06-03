using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Repository.EfCore.Interceptors;
using Tenon.Repository.EfCore.Sqlite.Configurations;
using Tenon.Repository.EfCore.Sqlite.Transaction;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.Repository.EfCore.Sqlite.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddEfCoreSqlite<TDbContext>(this IServiceCollection services,
        IConfigurationSection sqliteSection, Action<SqliteDbContextOptionsBuilder>? sqliteOptionsAction = null)
        where TDbContext : DbContext
    {
        var sqliteConfig = sqliteSection.Get<SqliteOptions>();
        if (sqliteConfig == null)
            throw new ArgumentNullException(nameof(sqliteConfig));
        services.Configure<SqliteOptions>(sqliteSection);
        services.AddDbContext<DbContext, TDbContext>(options =>
        {
            options.AddInterceptors(new SavingInterceptor());
            options.UseSqlite(sqliteConfig.ConnectionString, sqliteOptionsAction);
        });
        services.AddScoped<IUnitOfWork, SqliteUnitOfWork>();
        services.AddScoped(typeof(IEfRepository<EfEntity, long>), typeof(EfRepository<EfEntity>));
        return services;
    }
}