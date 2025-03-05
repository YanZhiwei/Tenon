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

    /// <summary>
    ///     配置拦截器
    /// </summary>
    private static void ConfigureInterceptors(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
    {
        var auditableUser = serviceProvider.GetService<IUserResolver<long>>();
        options.AddInterceptors(auditableUser);
    }

    /// <summary>
    /// 配置多租户拦截器
    /// </summary>
    private static void ConfigureTenantInterceptors(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
    {
        var tenantResolver = serviceProvider.GetService<IEfTenantResolver>();
        options.AddTenantInterceptors(tenantResolver);
    }

    /// <summary>
    ///     添加拦截器
    /// </summary>
    private static void AddInterceptors(this DbContextOptionsBuilder options, IUserResolver<long> auditableUser = null)
    {
        // 添加时间戳审计拦截器（不依赖用户信息）
        options.AddInterceptors(new TimestampAuditableFieldsInterceptor());
        
        // 添加并发检查拦截器
        options.AddInterceptors(new ConcurrencyCheckInterceptor());
        
        // 添加依赖用户信息的拦截器
        if (auditableUser != null)
        {
            // 完整审计拦截器处理用户相关审计字段
            options.AddInterceptors(new FullAuditableFieldsInterceptor(auditableUser));
            
            // 删除审计拦截器处理软删除相关字段
            options.AddInterceptors(new DeletionAuditableFieldsInterceptor(auditableUser));
        }
    }

    /// <summary>
    /// 添加多租户拦截器
    /// </summary>
    private static void AddTenantInterceptors(this DbContextOptionsBuilder options, IEfTenantResolver tenantResolver = null)
    {
        // 添加时间戳审计拦截器（不依赖用户信息）
        options.AddInterceptors(new TimestampAuditableFieldsInterceptor());
        
        // 添加并发检查拦截器
        options.AddInterceptors(new ConcurrencyCheckInterceptor());
        
        // 添加依赖租户信息的拦截器
        if (tenantResolver != null)
        {
            // 完整审计拦截器处理用户相关审计字段
            options.AddInterceptors(new FullAuditableFieldsInterceptor(tenantResolver));
            
            // 删除审计拦截器处理软删除相关字段
            options.AddInterceptors(new DeletionAuditableFieldsInterceptor(tenantResolver));
            
            // 添加租户拦截器
            options.AddInterceptors(new TenantInterceptor(tenantResolver));
        }
    }

    /// <summary>
    ///     获取程序集中的所有实体类型
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <returns>实体类型集合</returns>
    private static Type[] GetEntityTypes(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(type =>
                type is { IsAbstract: false, IsInterface: false } &&
                Enumerable.Any(type.GetInterfaces(), i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>)))
            .ToArray();
    }

    /// <summary>
    /// 获取程序集中的所有多租户实体类型
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <returns>多租户实体类型集合</returns>
    private static Type[] GetTenantEntityTypes(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(type =>
                type is { IsAbstract: false, IsInterface: false } &&
                Enumerable.Any(type.GetInterfaces(), i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>)) &&
                Enumerable.Any(type.GetInterfaces(), i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITenant<>)))
            .ToArray();
    }

    /// <summary>
    ///     注册所有实体的仓储
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assembly">程序集</param>
    private static void RegisterRepositories(IServiceCollection services, Assembly assembly)
    {
        var entityTypes = GetEntityTypes(assembly);

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

    /// <summary>
    /// 注册所有多租户实体的仓储
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assembly">程序集</param>
    private static void RegisterTenantRepositories(IServiceCollection services, Assembly assembly)
    {
        var entityTypes = GetEntityTypes(assembly);
        var tenantEntityTypes = GetTenantEntityTypes(assembly);

        // 注册普通实体的仓储
        foreach (var entityType in entityTypes.Except(tenantEntityTypes))
        {
            var repositoryType = typeof(EfRepository<>).MakeGenericType(entityType);
            services.AddScoped(repositoryType);

            var repositoryInterfaceType = typeof(IRepository<,>).MakeGenericType(entityType, typeof(long));
            var efRepositoryInterfaceType = typeof(IEfRepository<,>).MakeGenericType(entityType, typeof(long));

            services.AddScoped(repositoryInterfaceType, sp => sp.GetRequiredService(repositoryType));
            services.AddScoped(efRepositoryInterfaceType, sp => sp.GetRequiredService(repositoryType));
        }

        // 注册多租户实体的仓储
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
}