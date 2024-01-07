using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenon.Repository.EfCore.MySql.Transaction;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.Repository.EfCore.MySql.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddEfCoreMySql(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsBuilder)
    {
        services.TryAddScoped<IUnitOfWork, MySqlUnitOfWork>();
        services.TryAddScoped(typeof(IEfRepository<,>), typeof(EfRepository<>));
        services.AddDbContext<DbContext, MysqlDbContext>(optionsBuilder);

        return services;
    }
}