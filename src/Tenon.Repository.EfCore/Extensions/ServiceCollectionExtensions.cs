using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Repository.EfCore.Configurations;
using Tenon.Repository.EfCore.Interceptors;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.Repository.EfCore.Extensions;

/// <summary>
///     EF Core 扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     添加 EF Core 配置
    /// </summary>
    /// <typeparam name="TDbContext">数据库上下文类型</typeparam>
    /// <typeparam name="TUnitOfWork">工作单元类型</typeparam>
    public static IServiceCollection AddEfCore<TDbContext, TUnitOfWork>(
        this IServiceCollection services,
        IConfigurationSection dbSection,
        Action<DbContextOptionsBuilder> optionsAction)
        where TDbContext : DbContext
        where TUnitOfWork : class, IUnitOfWork
    {
        var dbConfig = dbSection.Get<DbOptions>();
        if (dbConfig == null)
            throw new ArgumentNullException(nameof(dbConfig));

        services.Configure<DbOptions>(dbSection);

        services.AddDbContext<TDbContext>((serviceProvider, options) =>
        {
            ConfigureInterceptors(serviceProvider, options);
            optionsAction(options);
        });

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());

        RegisterRepositories(services, typeof(TDbContext).Assembly);

        services.AddScoped<IUnitOfWork, TUnitOfWork>();

        return services;
    }

    /// <summary>
    ///     添加 EF Core 配置（使用默认 UnitOfWork）
    /// </summary>
    public static IServiceCollection AddEfCore<TDbContext>(
        this IServiceCollection services,
        IConfigurationSection dbSection,
        Action<DbContextOptionsBuilder> optionsAction,
        IInterceptor[]? interceptors = null)
        where TDbContext : DbContext
    {
        return AddEfCore<TDbContext, UnitOfWork>(services, dbSection, optionsAction);
    }

    private static void ConfigureInterceptors(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
    {
        var auditableUser = serviceProvider.GetService<ICurrentUser<long>>();
        AddInterceptors(options, auditableUser);
    }

    private static void AddInterceptors(this DbContextOptionsBuilder options, ICurrentUser<long>? auditableUser = null)
    {
        options.AddInterceptors(new TimestampAuditableFieldsInterceptor());
        options.AddInterceptors(new ConcurrencyCheckInterceptor());

        if (auditableUser != null)
        {
            options.AddInterceptors(new FullAuditableFieldsInterceptor(auditableUser));
            options.AddInterceptors(new DeletionAuditableFieldsInterceptor(auditableUser));
        }
    }

    private static void RegisterRepositories(IServiceCollection services, Assembly assembly)
    {
        var entityTypes = EfCoreTypeScanner.GetEntityTypes(assembly);

        foreach (var entityType in entityTypes)
        {
            var repositoryType = typeof(EfRepository<>).MakeGenericType(entityType);
            services.AddScoped(repositoryType);

            var repositoryInterfaceType = typeof(IRepository<,>).MakeGenericType(entityType, typeof(long));
            var efRepositoryInterfaceType = typeof(IEfRepository<,>).MakeGenericType(entityType, typeof(long));

            services.AddScoped(repositoryInterfaceType, sp => sp.GetRequiredService(repositoryType));
            services.AddScoped(efRepositoryInterfaceType, sp => sp.GetRequiredService(repositoryType));
        }
    }
}
