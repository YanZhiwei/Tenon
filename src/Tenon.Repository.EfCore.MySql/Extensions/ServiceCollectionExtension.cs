using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Repository.EfCore.Interceptors;
using Tenon.Repository.EfCore.MySql.Configurations;
using Tenon.Repository.EfCore.MySql.Transaction;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.Repository.EfCore.MySql.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddEfCoreMySql<TDbContext>(this IServiceCollection services,
        IConfigurationSection mySqlSection, Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null,
        IInterceptor[]? interceptors = null)
        where TDbContext : MySqlDbContext
    {
        var mySqlConfig = mySqlSection.Get<MySqlOptions>();
        if (mySqlConfig == null)
            throw new ArgumentNullException(nameof(mySqlConfig));
        services.Configure<MySqlOptions>(mySqlSection);
        services.AddDbContext<MySqlDbContext, TDbContext>((serviceProvider, options) =>
        {
            if (interceptors?.Any() ?? false)
                options.AddInterceptors(interceptors);
            var fullAuditableInterceptor = serviceProvider.GetService<FullAuditableFieldsInterceptor>();
            if (fullAuditableInterceptor != null)
                options.AddInterceptors(fullAuditableInterceptor);
            options.UseMySql(mySqlConfig.ConnectionString, ServerVersion.AutoDetect(mySqlConfig.ConnectionString),
                mySqlOptionsAction);
        });
        services.AddScoped<IUnitOfWork, MySqlUnitOfWork>();
        services.AddScoped(typeof(IEfRepository<EfEntity, long>), typeof(EfRepository<EfEntity>));
        return services;
    }
}