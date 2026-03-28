## Why

`Tenon.Repository` 仓储层存在多个设计缺陷：主键类型硬编码、多租户过滤器在模型构建时固定导致运行时租户隔离失效、`UpdateAsync` 存在不可达的逻辑分支、软删除与租户过滤器重复应用等问题，直接影响多租户系统的正确性和扩展性。

## What Changes

- **BREAKING** 修复 `UpdateAsync(entity, expressions[])` 中 `EntityState.Detached` 重复判断导致的不可达死代码，支持 detached 实体局部更新
- 修复 `TenonDbContext.ApplyTenantFilter` 在 `OnModelCreating` 阶段将 `tenantId` 快照为常量的严重缺陷，改为捕获 `IEfTenantResolver` 引用以实现运行时动态过滤
- 消除 `EfTenantRepository.GetDbSet` 手动租户过滤与 `DbContext` 全局查询过滤器的双重过滤冗余
- 消除 `TenantInterceptor` 与 `EfTenantRepository.InsertAsync` 的 TenantId 赋值双重设置
- 将 `DeletionAuditableFieldsInterceptor` 中级联软删除递归逻辑添加访问限制，防止循环引用导致的栈溢出
- 修复 `CountAsync` 返回类型从 `int` 改为 `long`，避免大数据量下整数溢出（**BREAKING** 影响接口签名）
- 为 `IRepository.GetAllAsync()` 添加 `CancellationToken` 参数保持 API 一致性（**BREAKING**）
- 将 `DisposeAction` 内部类迁移至独立文件
- 修复文件名 `IAuditTimestamps.cs` 与接口名 `ITimestampAuditable` 不一致问题

## Capabilities

### New Capabilities

- `dynamic-tenant-filter`: 运行时动态租户过滤——DbContext 全局查询过滤器通过捕获 resolver 引用而非快照值来实现每请求正确的租户隔离
- `partial-update-detached`: 支持对 detached 实体进行局部列更新，无需实体处于 tracked 状态

### Modified Capabilities

- `repository-contracts`: `IRepository` 接口签名变更（`CountAsync` 返回 `long`，`GetAllAsync` 增加 CancellationToken）

## Impact

- **受影响项目**：`Tenon.Repository`、`Tenon.Repository.EfCore`
- **下游影响**：所有实现/使用 `IRepository<TEntity,TKey>` 的项目需适配 `CountAsync` 返回类型变更和 `GetAllAsync` 签名变更
- **运行时行为变更**：修复后多租户查询将按每请求 TenantId 正确隔离，现有依赖"全局固定租户过滤"的代码逻辑需验证
- **无新外部依赖**
