using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Repository.EfCore;
using Tenon.Repository.EfCore.Configurations;
using Tenon.Repository.EfCore.Interceptors;
using Tenon.Repository.EfCore.MultiTenant;
using Tenon.Repository.EfCore.MultiTenant.Interceptors;
using Tenon.Repository.EfCore.Transaction;

namespace Tenon.Repository.EfCore.Extensions;

/// <summary>
/// 多租户 EF Core 服务注册扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加多租户 EF Core 配置
    /// </summary>
    /// <typeparam name="TDbContext">数据库上下文类型</typeparam>
    /// <typeparam name="TUnitOfWork">工作单元类型</typeparam>
    public static IServiceCollection AddTenantEfCore<TDbContext, TUnitOfWork>(
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
            ConfigureTenantInterceptors(serviceProvider, options);
            optionsAction(options);
        });

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());

        RegisterTenantRepositories(services, typeof(TDbContext).Assembly);

        services.AddScoped<IUnitOfWork, TUnitOfWork>();

        return services;
    }

    /// <summary>
    /// 添加多租户 EF Core 配置（使用默认 UnitOfWork）
    /// </summary>
    public static IServiceCollection AddTenantEfCore<TDbContext>(
        this IServiceCollection services,
        IConfigurationSection dbSection,
        Action<DbContextOptionsBuilder> optionsAction)
        where TDbContext : DbContext
    {
        return AddTenantEfCore<TDbContext, UnitOfWork>(services, dbSection, optionsAction);
    }

    private static void ConfigureTenantInterceptors(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
    {
        var tenantResolver = serviceProvider.GetService<IEfTenantResolver>();
        AddTenantInterceptors(options, tenantResolver);
    }

    private static void AddTenantInterceptors(this DbContextOptionsBuilder options, IEfTenantResolver? tenantResolver = null)
    {
        options.AddInterceptors(new TimestampAuditableFieldsInterceptor());
        options.AddInterceptors(new ConcurrencyCheckInterceptor());

        if (tenantResolver != null)
        {
            options.AddInterceptors(new FullAuditableFieldsInterceptor(tenantResolver));
            options.AddInterceptors(new DeletionAuditableFieldsInterceptor(tenantResolver));
            options.AddInterceptors(new TenantInterceptor(tenantResolver));
        }
    }

    private static void RegisterTenantRepositories(IServiceCollection services, Assembly assembly)
    {
        var entityTypes = EfCoreTypeScanner.GetEntityTypes(assembly);
        var tenantEntityTypes = GetTenantEntityTypes(assembly);

        foreach (var entityType in entityTypes.Except(tenantEntityTypes))
        {
            var repositoryType = typeof(EfRepository<>).MakeGenericType(entityType);
            services.AddScoped(repositoryType);

            var repositoryInterfaceType = typeof(IRepository<,>).MakeGenericType(entityType, typeof(long));
            var efRepositoryInterfaceType = typeof(IEfRepository<,>).MakeGenericType(entityType, typeof(long));

            services.AddScoped(repositoryInterfaceType, sp => sp.GetRequiredService(repositoryType));
            services.AddScoped(efRepositoryInterfaceType, sp => sp.GetRequiredService(repositoryType));
        }

        foreach (var entityType in tenantEntityTypes)
        {
            var repositoryType = typeof(EfTenantRepository<>).MakeGenericType(entityType);
            services.AddScoped(repositoryType);

            var repositoryInterfaceType = typeof(IRepository<,>).MakeGenericType(entityType, typeof(long));
            var efRepositoryInterfaceType = typeof(IEfRepository<,>).MakeGenericType(entityType, typeof(long));
            var tenantRepositoryInterfaceType = typeof(ITenantRepository<,,>).MakeGenericType(entityType, typeof(long), typeof(long));

            services.AddScoped(repositoryInterfaceType, sp => sp.GetRequiredService(repositoryType));
            services.AddScoped(efRepositoryInterfaceType, sp => sp.GetRequiredService(repositoryType));
            services.AddScoped(tenantRepositoryInterfaceType, sp => sp.GetRequiredService(repositoryType));
        }
    }

    private static Type[] GetTenantEntityTypes(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(type =>
                type is { IsAbstract: false, IsInterface: false } &&
                type.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>)) &&
                type.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITenant<>)))
            .ToArray();
    }
}
