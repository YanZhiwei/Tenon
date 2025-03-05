# Tenon 多租户功能设计与实现

## 1. 概述

Tenon 框架的多租户功能提供了一种机制，使得单个应用程序实例可以为多个租户提供服务，同时确保每个租户的数据彼此隔离。这种隔离是通过在数据访问层自动应用租户过滤器实现的，无需在应用程序代码中显式添加租户筛选条件。

## 2. 核心接口

### 2.1 ITenant<TKey>

定义了多租户实体的基本接口，包含一个租户标识属性：

```csharp
public interface ITenant<TKey> where TKey : struct
{
    TKey TenantId { get; set; }
}
```

### 2.2 ITenantRepository<TEntity, TKey, TTenantKey>

扩展了标准仓储接口，提供了多租户特定的数据访问方法：

```csharp
public interface ITenantRepository<TEntity, TKey, TTenantKey> : IRepository<TEntity, TKey>, ITenantFilterable<TTenantKey>
    where TEntity : class, IEntity<TKey>, ITenant<TTenantKey>
    where TKey : struct
    where TTenantKey : struct
{
    Task<List<TEntity>> GetListForCurrentTenantAsync(
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default);
    
    Task<TEntity> GetForCurrentTenantAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    // 其他方法...
}
```

### 2.3 ITenantResolver<TUserKey, TTenantKey>

扩展了用户解析器接口，增加了租户标识属性：

```csharp
public interface ITenantResolver<TUserKey, TTenantKey> : IUserResolver<TUserKey>
    where TUserKey : struct
    where TTenantKey : struct
{
    TTenantKey? TenantId { get; }
}
```

### 2.4 ITenantFilterable<TTenantKey>

提供了租户过滤控制功能：

```csharp
public interface ITenantFilterable<TTenantKey> where TTenantKey : struct
{
    IDisposable ChangeTenant(TTenantKey tenantId);
    IDisposable DisableTenantFilter();
}
```

### 2.5 ITenantAuditable<TUserKey, TTenantKey>

结合了审计和租户接口，支持多租户审计功能：

```csharp
public interface ITenantAuditable<TUserKey, TTenantKey> : IFullAuditable<TUserKey>, ITenant<TTenantKey>
    where TUserKey : struct
    where TTenantKey : struct
{
}
```

## 3. 实现类

### 3.1 EfTenantEntity

多租户实体基类，实现了 `ITenant<long>` 接口：

```csharp
public abstract class EfTenantEntity : EfEntity, ITenant<long>
{
    public long TenantId { get; set; }
}
```

### 3.2 EfTenantFullAuditableEntity

多租户完整审计实体基类，继承自 `EfFullAuditableEntity`，实现了 `ITenant<long>` 接口：

```csharp
public abstract class EfTenantFullAuditableEntity : EfFullAuditableEntity, ITenant<long>
{
    public long TenantId { get; set; }
}
```

### 3.3 EfTenantRepository<TEntity>

多租户仓储实现，继承自 `EfRepository<TEntity>`，实现了 `ITenantRepository` 和 `ITenantFilterable` 接口：

```csharp
public class EfTenantRepository<TEntity> : EfRepository<TEntity>, 
    ITenantRepository<TEntity, long, long>,
    ITenantFilterable<long>
    where TEntity : class, IEntity<long>, ITenant<long>
{
    private readonly IEfTenantResolver _tenantResolver;
    private readonly DbContext _dbContext;

    public EfTenantRepository(DbContext dbContext, IEfTenantResolver tenantResolver = null) 
        : base(dbContext)
    {
        _dbContext = dbContext;
        _tenantResolver = tenantResolver;
    }

    // 实现方法...
}
```

### 3.4 TenantInterceptor

EF Core 拦截器，自动设置新增实体的租户 ID：

```csharp
public class TenantInterceptor : SaveChangesInterceptor
{
    private readonly IEfTenantResolver _tenantResolver;

    public TenantInterceptor(IEfTenantResolver tenantResolver)
    {
        _tenantResolver = tenantResolver;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        SetTenantId(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    // 其他方法...
}
```

## 4. 扩展方法

### 4.1 ModelBuilderExtension.ApplyTenantFilter()

为实现 `ITenant<long>` 的实体添加全局租户过滤：

```csharp
public static ModelBuilder ApplyTenantFilter(this ModelBuilder modelBuilder)
{
    var entityTypes = modelBuilder.Model.GetEntityTypes()
        .Where(e => typeof(ITenant<long>).IsAssignableFrom(e.ClrType) && !e.ClrType.IsAbstract);

    foreach (var entityType in entityTypes)
    {
        var parameter = Expression.Parameter(entityType.ClrType, "e");
        var tenantIdProperty = Expression.Property(parameter, nameof(ITenant<long>.TenantId));
        var tenantIdFilter = Expression.Lambda(Expression.Equal(tenantIdProperty, Expression.Constant(0L)), parameter);

        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(tenantIdFilter);
    }

    return modelBuilder;
}
```

### 4.2 ServiceCollectionExtensions.AddTenantEfCore()

添加多租户 EF Core 服务注册：

```csharp
public static IServiceCollection AddTenantEfCore<TDbContext, TUnitOfWork>(
    this IServiceCollection services,
    IConfigurationSection dbSection,
    Action<DbContextOptionsBuilder> optionsAction)
    where TDbContext : DbContext
    where TUnitOfWork : class, IUnitOfWork
{
    // 实现...
}
```

## 5. 使用示例

### 5.1 定义多租户实体

```csharp
public class TenantBlog : EfTenantEntity
{
    public string Url { get; set; }
    public int Rating { get; set; }
    public virtual ICollection<TenantPost> Posts { get; set; } = default!;
}
```

### 5.2 定义租户解析器

```csharp
public class CurrentTenantResolver : IEfTenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurrentTenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public long? TenantId => GetTenantIdFromHeader();
    
    public long? UserId => GetUserIdFromHeader();
    
    // 实现方法...
}
```

### 5.3 配置 DbContext

```csharp
public sealed class TenantDbContext : TenonDbContext
{
    public TenantDbContext(DbContextOptions options, IEfTenantResolver tenantResolver)
        : base(options, tenantResolver)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 应用租户过滤器
        modelBuilder.ApplyTenantFilter();
    }
}
```

### 5.4 依赖注入配置

```csharp
services.AddHttpContextAccessor();
services.AddScoped<IEfTenantResolver, CurrentTenantResolver>();
services.AddTenantEfCore<TenantDbContext>(configuration.GetSection("MySql"), 
    options => options.UseMySql(
        configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(configuration.GetConnectionString("MySql")),
        mySqlOptions => mySqlOptions.MigrationsAssembly(typeof(TenantDbContext).Assembly.GetName().Name)));
```

### 5.5 使用多租户仓储

```csharp
// 获取当前租户的博客列表
public async Task<List<TenantBlog>> GetBlogsForCurrentTenantAsync()
{
    return await _tenantBlogRepository.GetListForCurrentTenantAsync();
}

// 切换租户上下文
public async Task<List<TenantBlog>> GetBlogsForSpecificTenantAsync(long tenantId)
{
    using (_tenantBlogRepository.ChangeTenant(tenantId))
    {
        return await _tenantBlogRepository.GetListAsync();
    }
}

// 禁用租户过滤，获取所有租户的博客
public async Task<List<TenantBlog>> GetBlogsForAllTenantsAsync()
{
    using (_tenantBlogRepository.DisableTenantFilter())
    {
        return await _tenantBlogRepository.GetListAsync();
    }
}
```

## 6. 最佳实践

1. **始终使用租户解析器**：避免手动设置租户 ID，而是依赖租户解析器自动处理。

2. **使用 ChangeTenant 和 DisableTenantFilter**：在需要临时切换租户上下文或禁用租户过滤的场景中，使用这些方法并配合 using 语句确保上下文正确恢复。

3. **结合审计功能**：使用 `EfTenantFullAuditableEntity` 同时获得多租户和完整审计功能。

4. **注意性能**：全局查询过滤器可能会影响性能，特别是在复杂查询中。在必要时使用 `DisableTenantFilter` 优化性能。

5. **安全考虑**：确保租户解析器的实现是安全的，防止租户 ID 被篡改。

## 7. 注意事项

1. 多租户过滤器会与软删除过滤器组合使用，确保两者兼容。

2. 在使用 Include 导航属性时，EF Core 会自动应用租户过滤器到关联实体。

3. 在批量操作中，可能需要临时禁用租户过滤器以提高性能。

4. 确保在单元测试中正确模拟租户解析器。
