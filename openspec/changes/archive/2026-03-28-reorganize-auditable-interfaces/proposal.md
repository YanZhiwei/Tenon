## Why

`Tenon.Repository` 项目根目录中混放了审计接口（`IDeletionAuditable`、`ITimestampAuditable`、`IFullAuditable`）与非审计契约（`IEntity`、`IRepository`、`ICurrentUser`），随着审计关注点增长，可发现性下降且职责边界模糊。此外，部分接口命名粒度不一致，`ITimestampAuditable` 语义覆盖了创建与修改两个时间戳但名称未能体现，`IFullAuditable` 中"Full"缺乏领域语义；统一整理能为后续扩展（如 IP 审计、操作类型审计）奠定清晰基础。

## What Changes

- 在 `Tenon.Repository` 中新建 `Auditing/` 子文件夹，将所有审计接口文件移入
- 接口重命名优化：
  - `ITimestampAuditable` → `ICreationModificationAuditable`（明确表达创建+修改时间语义）
  - `IFullAuditable<TKey>` → `IAuditableEntity<TKey>`（与"实体"语境对齐，去除歧义的"Full"）
  - `IDeletionAuditable<TKey>` 命名保持，但随上层接口命名联动更新继承声明
- **BREAKING**: 接口重命名对所有实现类及引用方均为破坏性变更，需同步更新派生类：
  - `EfTimestampAuditEntity`（实现 `ITimestampAuditable`）
  - `EfFullAuditableEntity`（实现 `IFullAuditable<long>`）
  - `EfTenantFullAuditableEntity`（继承 `EfFullAuditableEntity`）
  - 三个拦截器：`TimestampAuditableFieldsInterceptor`、`FullAuditableFieldsInterceptor`、`DeletionAuditableFieldsInterceptor`
  - EF Core 配置：`AbstractEntityTypeConfiguration`、`ModelBuilderExtension`（两处）
  - `ITenantAuditable<TUserKey, TTenantKey>`（继承 `IFullAuditable`）
  - 所有 `ServiceCollectionExtensions` 中的注册引用

## Capabilities

### New Capabilities

- `auditable-interface-organization`: 审计接口的文件夹结构规范与命名约定，定义 `Auditing/` 目录布局、接口重命名映射表以及派生类同步规则

### Modified Capabilities

## Impact

- **Tenon.Repository**：新增 `Auditing/` 目录，移动并重命名 3 个接口文件，命名空间保持 `Tenon.Repository` 不变（避免引用方 using 语句变更）
- **Tenon.Repository.EfCore**：更新 `EfFullAuditableEntity`、`EfTimestampAuditEntity`、3 个拦截器、`AbstractEntityTypeConfiguration`、`ModelBuilderExtension`、`ServiceCollectionExtensions`
- **Tenon.Repository.EfCore.MultiTenant**：更新 `EfTenantFullAuditableEntity`、`ITenantAuditable`、`ModelBuilderExtension`、`ServiceCollectionExtensions`、`TenantDbContext`
- **Tenon.Repository.EfCore.Sqlite**：更新 `ServiceCollectionExtensions`
- **samples/MultiTenantSample**：业务实体（`Product`、`Category`、`Order`、`OrderItem`）通过继承链间接受影响，无需直接修改
- **Tenon.MessageTracker.EfCore**：`EventTracker` 通过 `EfTimestampAuditEntity` 间接受影响
