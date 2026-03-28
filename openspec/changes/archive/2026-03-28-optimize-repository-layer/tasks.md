## 1. Tenon.Repository 接口契约变更（Breaking Changes）

- [x] 1.1 将 `IRepository<TEntity,TKey>.CountAsync` 返回类型从 `Task<int>` 改为 `Task<long>`
- [x] 1.2 为 `IRepository<TEntity,TKey>.GetAllAsync` 添加 `CancellationToken token = default` 参数
- [x] 1.3 将 `IAuditTimestamps.cs` 文件重命名为 `ITimestampAuditable.cs`（接口名已正确，修复文件名不一致）

## 2. EfCore - 修复动态租户全局过滤器（严重 Bug）

- [x] 2.1 修改 `ModelBuilderExtension.ApplyTenantFilter` 签名，将参数从 `long tenantId` 改为 `IEfTenantResolver tenantResolver`
- [x] 2.2 在 `ApplyTenantFilter` 内部的 lambda 表达式中改为访问 `tenantResolver.TenantId`（运行时求值），而非使用常量
- [x] 2.3 修改 `TenonDbContext` 构造函数，存储 `IEfTenantResolver` 引用（已存在），在 `OnModelCreating` 中传递 resolver 而非 `_tenantResolver.TenantId`
- [x] 2.4 验证修复：确认同一 DbContext 实例中不同租户请求生成的 SQL 包含各自正确的 TenantId 值

## 3. EfCore - 消除软删除与租户过滤器的 HasQueryFilter 覆盖冲突

- [x] 3.1 审查 `TenonDbContext.OnModelCreating` 中 `ApplySoftDeleteQueryFilter` 和 `ApplyTenantFilter` 的调用顺序
- [x] 3.2 确认 `ApplyTenantFilter` 对同时实现 `ITenant<long>` 和 `IDeletionAuditable<long>` 的实体正确组合两个条件（当前代码已有此逻辑，验证是否覆盖了 `ApplySoftDeleteQueryFilter` 的注册）
- [x] 3.3 如存在覆盖问题，将 `ApplySoftDeleteQueryFilter` 的逻辑完全内联到 `ApplyTenantFilter` 中，或调整调用顺序确保组合过滤器最终生效

## 4. EfCore - 移除双重租户过滤冗余

- [x] 4.1 删除 `EfTenantRepository.GetDbSet` 中的 `query.Where(e => e.TenantId == _currentTenantId)` 手动过滤逻辑（依赖步骤 2 完成后）
- [x] 4.2 重新评估 `ChangeTenant` / `DisableTenantFilter` 的实现方式——由于全局过滤器已动态读取 resolver，这两个方法需要操作 resolver 的 TenantId 状态或通过 `IgnoreQueryFilters` 实现
- [x] 4.3 删除 `EfTenantRepository.InsertAsync` 中的 `entity.TenantId = _currentTenantId` 赋值（由 `TenantInterceptor` 统一负责）
- [x] 4.4 删除 `EfTenantRepository.InsertAsync(IEnumerable)` 中对每个实体的 `entity.TenantId = _currentTenantId` 赋值循环

## 5. EfCore - 修复 UpdateAsync 死代码（局部更新功能）

- [x] 5.1 重构 `EfRepository.UpdateAsync(entity, expressions[], token)` 方法，修复 `EntityState.Detached` 判断逻辑
- [x] 5.2 确保 `Added`/`Deleted` 状态抛出 `InvalidOperationException`
- [x] 5.3 确保 `Detached` 状态正确执行 `entry.State = EntityState.Unchanged` 然后按表达式标记 `IsModified`
- [x] 5.4 确保 `Modified` 状态正确按表达式过滤，关闭无关属性的 `IsModified`

## 6. EfCore - 实现接口契约变更（适配步骤 1 的 Breaking Changes）

- [x] 6.1 将 `EfRepository.CountAsync` 实现改用 `LongCountAsync`，返回 `long`
- [x] 6.2 为 `EfRepository.GetAllAsync` 添加 `CancellationToken token = default` 参数，传递给 `ToListAsync`

## 7. EfCore - 修复 DeletionAuditableFieldsInterceptor 级联深度

- [x] 7.1 在 `DeletionAuditableFieldsInterceptor.HandleSoftDelete` 中添加 `HashSet<object> visited` 防止循环引用
- [x] 7.2 在递归处理集合和单个导航属性时，先检查 `visited` 集合，已处理过的实体跳过

## 8. 代码整洁

- [x] 8.1 将 `EfTenantRepository.cs` 末尾的 `DisposeAction` 内部类提取到独立文件 `DisposeAction.cs`
- [x] 8.2 搜索全项目中所有调用 `CountAsync` 的位置，将接收变量类型从 `int` 改为 `long`（或 `var` 自动推断）
- [x] 8.3 搜索全项目中所有调用 `GetAllAsync()` 的位置，确认无编译错误（新参数有默认值，通常无需修改）

## 9. 验证

- [x] 9.1 确认整个解决方案编译无错误
- [x] 9.2 运行现有单元测试/集成测试确保无回归（项目暂无测试，跳过）
- [ ] 9.3 手工验证多租户查询隔离：切换租户后查询结果不包含其他租户数据
- [ ] 9.4 手工验证 detached 实体局部更新：UPDATE SQL 仅包含指定列
