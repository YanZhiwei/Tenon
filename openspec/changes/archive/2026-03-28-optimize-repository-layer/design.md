## Context

`Tenon.Repository` 是整个框架的数据访问抽象层，目前包含：
- `Tenon.Repository`：核心接口定义（`IRepository`、`IEntity`、各审计接口）
- `Tenon.Repository.EfCore`：EF Core 实现（`EfRepository`、`EfTenantRepository`、拦截器、DbContext）

经过代码审查发现以下几类问题，按严重程度排序：

**严重（影响运行时正确性）**
1. `TenonDbContext.ApplyTenantFilter` 在 `OnModelCreating` 时将 `_tenantResolver.TenantId` 作为常量嵌入全局查询过滤器的 lambda 中。EF Core 在应用池（`AddDbContextPool`）场景下 `OnModelCreating` 只执行一次，导致后续所有请求共享同一个租户过滤值，多租户隔离完全失效。
2. `UpdateAsync(entity, expressions[])` 中 `Detached` 状态的判断逻辑存在死代码——第一个 `if (Detached) throw` 之后还有 `if (Detached) { entry.State = Unchanged; ... }`，后者永远不可达，导致 detached 实体的局部更新功能缺失。

**中等（API 设计缺陷）**
3. 软删除过滤器 (`ApplySoftDeleteQueryFilter`) 和租户过滤器 (`ApplyTenantFilter`) 分两次调用 `HasQueryFilter`，但 EF Core 的 `HasQueryFilter` 是替换语义，后者会覆盖前者，导致同时实现软删除和租户的实体只有租户过滤生效（`ApplyTenantFilter` 中虽有手动合并，但执行顺序导致 `ApplySoftDeleteQueryFilter` 的注册被覆盖）。
4. `EfTenantRepository.GetDbSet` 手动追加 `Where(e => e.TenantId == _currentTenantId)`，与 `DbContext` 层的全局查询过滤器产生双重过滤（SQL 中生成冗余条件）。
5. `TenantInterceptor.ApplyTenantId` 与 `EfTenantRepository.InsertAsync` 均在保存前设置 `TenantId`，存在双重赋值。
6. `CountAsync` 返回 `int`，大数据量下溢出风险。
7. `IRepository.GetAllAsync()` 缺少 `CancellationToken`，API 不一致。

**低（代码质量）**
8. `DisposeAction` 内部类混入 `EfTenantRepository.cs`。
9. 文件名 `IAuditTimestamps.cs` 与接口名 `ITimestampAuditable` 不一致。
10. `DeletionAuditableFieldsInterceptor` 级联软删除无深度保护。

## Goals / Non-Goals

**Goals:**
- 修复多租户全局查询过滤器运行时动态化，确保每请求按正确的 TenantId 隔离
- 修复 `UpdateAsync` 局部更新的死代码，使 detached 实体局部更新功能可用
- 统一软删除与租户过滤器的注册，消除 `HasQueryFilter` 覆盖问题
- 消除双重租户过滤和双重 TenantId 赋值冗余
- 修复 `CountAsync` 类型溢出和 `GetAllAsync` API 不一致问题
- 代码整洁：迁移 `DisposeAction`，修复文件名不一致

**Non-Goals:**
- 不引入新的 ORM 支持（仅修复现有 EF Core 实现）
- 不重构泛型主键（`EfRepository` 硬编码 `long` 主键）到泛型版本（范围过大，留作后续）
- 不添加分页到 `IRepository` 基础接口
- 不修改 `Tenon.Repository.EfCore.MySql` 和 `Tenon.Repository.EfCore.Sqlite` 的具体实现（只涉及通用层）

## Decisions

### D1：动态租户过滤器——捕获 Resolver 引用而非值

**决策**：`ApplyTenantFilter` 接受 `IEfTenantResolver` 而非 `long tenantId`，在 lambda 中闭包捕获 resolver 引用，每次查询执行时实时调用 `resolver.TenantId`。

```csharp
// 旧（错误）：tenantId 在 OnModelCreating 时快照
modelBuilder.ApplyTenantFilter(_tenantResolver.TenantId);

// 新（正确）：resolver 被 lambda 闭包捕获，每次查询时取值
modelBuilder.ApplyTenantFilter(_tenantResolver);
// 内部：Expression.Property(Expression.Constant(resolver), nameof(IEfTenantResolver.TenantId))
// 或更简单地通过直接使用 EF.Property 或 HasQueryFilter(e => e.TenantId == resolver.TenantId)
```

**备选方案**：使用 EF Core 的全局过滤器 + `HasQueryFilter` 传入 lambda（直接捕获 resolver）。

**结论**：直接在 `ModelBuilderExtension.ApplyTenantFilter` 中将参数改为 `IEfTenantResolver`，lambda 内部引用 `resolver.TenantId`（属性访问，运行时求值）。

### D2：合并软删除 + 租户全局过滤器，解决 HasQueryFilter 覆盖

**决策**：废弃 `ApplySoftDeleteQueryFilter` 单独方法，将软删除逻辑并入 `ApplyTenantFilter` 的组合过滤逻辑中，并在 `OnModelCreating` 中统一调用一次。对于只有软删除但无租户的实体，保留单独的软删除过滤。

具体策略：
1. 在 `OnModelCreating` 中先调用 `ApplySoftDeleteQueryFilter`（覆盖所有软删除实体）
2. 再调用 `ApplyTenantFilter`（对于实现 `ITenant` 的实体，重新组合租户+软删除，覆盖步骤1的软删除注册）
3. 确保 `ApplyTenantFilter` 对同时实现两个接口的实体正确组合，避免仅注册租户过滤

当前代码中 `ApplyTenantFilter` 已有组合逻辑，只需确保 `TenonDbContext.OnModelCreating` 调用顺序是：先软删除，再租户（租户方法内部已做组合）即可。

### D3：移除 EfTenantRepository.GetDbSet 中的手动租户过滤

**决策**：依赖 DbContext 全局查询过滤器（D1 修复后）来处理租户过滤，`EfTenantRepository.GetDbSet` 不再手动追加 `Where(e => e.TenantId == _currentTenantId)`。

`ChangeTenant`/`DisableTenantFilter` 语义调整：由于仓储层不再控制过滤条件，这两个方法需要改为操作 `_tenantResolver` 的 TenantId 或者通过 `DbContext.IgnoreQueryFilters()` 实现。

**注意**：`IEfTenantResolver` 需要是可写的（或提供切换方法），或者 `ChangeTenant` 改为临时修改 resolver 状态。这需要 `IEfTenantResolver` 支持 `SetTenantId` 方法。

### D4：TenantInterceptor 与 InsertAsync 二选一

**决策**：保留 `TenantInterceptor` 作为保底（SaveChanges 级别），移除 `EfTenantRepository.InsertAsync` 中的手动 `entity.TenantId = _currentTenantId` 赋值，统一由拦截器负责。

**备选**：只保留仓储层赋值，移除拦截器。
**结论**：选择拦截器方案，更符合横切关注点分离原则。

### D5：修复 UpdateAsync 死代码

修复 `UpdateAsync(entity, expressions[])` 中的逻辑分支：
```csharp
// 修复前（错误）：
if (entry.State == EntityState.Detached)
    throw new InvalidOperationException(...);  // 第一个检查，Detached 时抛出

// ... 后面的 if (entry.State == EntityState.Detached) 永远不可达

// 修复后：
if (entry.State == EntityState.Added || entry.State == EntityState.Deleted)
    throw new InvalidOperationException(...);

if (entry.State == EntityState.Detached)
{
    entry.State = EntityState.Unchanged;
    foreach (var expression in updatingExpressions)
        entry.Property(expression).IsModified = true;
}
else if (entry.State == EntityState.Modified)
{
    var propNames = updatingExpressions.Select(x => x.GetMemberName()).ToArray();
    foreach (var propEntry in entry.Properties)
        if (!propNames.Contains(propEntry.Metadata.Name))
            propEntry.IsModified = false;
}

return await DbContext.SaveChangesAsync(token);
```

### D6：CountAsync 返回 long

`IRepository.CountAsync` 和 `IRepository.AnyAsync` 相关计数方法的返回类型：
- `CountAsync` 改为返回 `long`（接口 + EfRepository 实现）
- EF Core 的 `LongCountAsync` 替换 `CountAsync`

### D7：GetAllAsync 增加 CancellationToken

接口 `IRepository.GetAllAsync()` 改为 `GetAllAsync(CancellationToken token = default)`，实现中传递给 `ToListAsync`。

## Risks / Trade-offs

| 风险 | 缓解措施 |
|------|---------|
| `CountAsync` 返回 `long` 是 Breaking Change，下游代码需改 `int` 为 `long` 或隐式转换可能编译错误 | 搜索全项目所有 `CountAsync` 调用点，列入 tasks 清单 |
| `GetAllAsync` 新增参数虽有默认值，不影响现有调用，但接口实现类需同步更新 | 编译报错驱动修复 |
| D3（移除手动过滤）依赖 D1（动态 resolver）先完成，顺序错误会导致无租户过滤 | 按任务依赖顺序实施，先修 DbContext 过滤器 |
| `ChangeTenant` 改为操作 resolver 状态，在并发场景下 scoped resolver 需保证线程安全 | 确认 `IEfTenantResolver` 是 Scoped 注册（每请求一个实例），并发安全 |
| `DeletionAuditableFieldsInterceptor` 级联删除无深度限制，循环引用会栈溢出 | 添加已访问 HashSet 防止重复处理 |

## Migration Plan

1. 修改 `Tenon.Repository`：接口签名变更（`CountAsync` + `GetAllAsync`）
2. 修改 `Tenon.Repository.EfCore`：
   a. 修复 `ModelBuilderExtension.ApplyTenantFilter` 接受 resolver
   b. 修复 `TenonDbContext` 传递 resolver 引用
   c. 修复 `EfRepository.UpdateAsync` 死代码
   d. 移除 `EfTenantRepository.GetDbSet` 手动过滤（依赖步骤 a/b 完成后）
   e. 移除 `EfTenantRepository.InsertAsync` 中的 `entity.TenantId =` 赋值
   f. 修复 `DeletionAuditableFieldsInterceptor` 级联深度
   g. 迁移 `DisposeAction` 到独立文件
   h. 重命名 `IAuditTimestamps.cs` → `ITimestampAuditable.cs`
3. 修复所有下游编译错误（`CountAsync`/`GetAllAsync` 调用点）

**回滚**：所有改动均在同一 PR，回滚直接 revert。无数据迁移，不需要数据库变更。
