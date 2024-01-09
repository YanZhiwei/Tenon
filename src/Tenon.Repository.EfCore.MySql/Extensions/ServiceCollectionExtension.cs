using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Repository.EfCore.MySql.Configurations;
using Tenon.Repository.EfCore.MySql.Transaction;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.Repository.EfCore.MySql.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddEfCoreMySql<TDbContext>(this IServiceCollection services,
        IConfigurationSection mySqlSection, Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
        where TDbContext : MySqlDbContext
    {
        var mySqlConfig = mySqlSection.Get<MySqlOptions>();
        if (mySqlConfig == null)
            throw new ArgumentNullException(nameof(mySqlConfig));
        services.Configure<MySqlOptions>(mySqlSection);
        services.AddDbContext<MySqlDbContext, TDbContext>(options =>
        {
            options.UseMySql(mySqlConfig.ConnectionString, ServerVersion.AutoDetect(mySqlConfig.ConnectionString),
                mySqlOptionsAction);
        });
        services.TryAddScoped<IUnitOfWork, MySqlUnitOfWork>();
        services.TryAddScoped(typeof(IEfRepository<EfEntity, long>), typeof(EfRepository<EfEntity>));
        return services;
    }
}