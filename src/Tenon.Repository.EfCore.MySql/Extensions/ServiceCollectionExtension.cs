using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Repository.EfCore.MySql.Transaction;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.Repository.EfCore.MySql.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddEfCoreMySql<TDbContext>(this IServiceCollection services,
        Action<DbContextOptionsBuilder>? optionsBuilder = null)
        where TDbContext : MysqlDbContext
    {
        services.TryAddScoped<IUnitOfWork, MySqlUnitOfWork>();
        services.TryAddScoped(typeof(IEfRepository<EfEntity, long>), typeof(EfRepository<EfEntity>));
        if (optionsBuilder != null)
            services.AddDbContext<DbContext, TDbContext>(optionsBuilder);
        else
            services.AddDbContext<DbContext, TDbContext>(option =>
            {

            });
        return services;
    }
}