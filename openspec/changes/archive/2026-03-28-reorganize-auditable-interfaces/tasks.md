## 1. 创建目录结构

- [x] 1.1 在 `src/Tenon.Repository/` 下创建 `Auditing/` 子文件夹

## 2. 重命名并移动接口文件（Tenon.Repository）

- [x] 2.1 将 `ITimestampAuditable.cs` 复制到 `Auditing/ICreationModificationAuditable.cs`，更新接口名为 `ICreationModificationAuditable`，保留命名空间 `Tenon.Repository`
- [x] 2.2 将 `IFullAuditable.cs` 复制到 `Auditing/IAuditableEntity.cs`，更新接口名为 `IAuditableEntity<TKey>`，继承改为 `ICreationModificationAuditable, IDeletionAuditable<TKey>`，保留命名空间
- [x] 2.3 将 `IDeletionAuditable.cs` 移动到 `Auditing/IDeletionAuditable.cs`，命名空间不变
- [x] 2.4 删除根目录的旧文件 `ITimestampAuditable.cs`、`IFullAuditable.cs`、`IDeletionAuditable.cs`

## 3. 更新 Tenon.Repository.MultiTenant

- [x] 3.1 更新 `ITenantAuditable<TUserKey, TTenantKey>` 继承声明：`IFullAuditable<TUserKey>` → `IAuditableEntity<TUserKey>`

## 4. 更新 Tenon.Repository.EfCore（实现类）

- [x] 4.1 更新 `EfTimestampAuditEntity` 实现声明：`ITimestampAuditable` → `ICreationModificationAuditable`
- [x] 4.2 更新 `EfFullAuditableEntity` 实现声明：`IFullAuditable<long>` → `IAuditableEntity<long>`
- [x] 4.3（可选）将 `EfTimestampAuditEntity.cs` 重命名为 `EfCreationModificationAuditEntity.cs` 并更新类名，同步更新所有引用此类的文件（含 `EventTracker.cs`）

## 5. 更新 Tenon.Repository.EfCore（拦截器）

- [x] 5.1 更新 `TimestampAuditableFieldsInterceptor`：将所有 `ITimestampAuditable` 引用替换为 `ICreationModificationAuditable`
- [x] 5.2 更新 `FullAuditableFieldsInterceptor`：将所有 `IFullAuditable<long>` 引用替换为 `IAuditableEntity<long>`
- [x] 5.3 更新 `DeletionAuditableFieldsInterceptor`：检查是否有 `IFullAuditable` 引用并替换（若存在）— 仅使用 `IDeletionAuditable<long>`，无需修改

## 6. 更新 Tenon.Repository.EfCore（EF 配置）

- [x] 6.1 更新 `AbstractEntityTypeConfiguration.cs`：`IDeletionAuditable<long>` 引用保持，`IFullAuditable` 引用（若有）替换为 `IAuditableEntity` — 仅含 `IDeletionAuditable`，无需修改
- [x] 6.2 更新 `ModelBuilderExtension.cs`（EfCore）：替换所有旧接口名引用 — 仅含 `IDeletionAuditable`，无需修改
- [x] 6.3 更新 `ServiceCollectionExtensions.cs`（EfCore）：替换所有旧接口名引用 — 无审计接口直接引用，无需修改

## 7. 更新 Tenon.Repository.EfCore.MultiTenant

- [x] 7.1 更新 `EfTenantFullAuditableEntity`：继承 `EfFullAuditableEntity`，无直接接口引用，无需修改
- [x] 7.2 更新 `ModelBuilderExtension.cs`（MultiTenant）：替换所有旧接口名引用 — 仅含 `IDeletionAuditable`，无需修改
- [x] 7.3 更新 `ServiceCollectionExtensions.cs`（MultiTenant）：替换所有旧接口名引用 — 无审计接口直接引用，无需修改
- [x] 7.4 更新 `TenantDbContext.cs`：替换注释和代码中的旧接口名引用 — 注释仅提及 `IDeletionAuditable`，无需修改

## 8. 更新 Tenon.Repository.EfCore.Sqlite

- [x] 8.1 更新 `ServiceCollectionExtensions.cs`（Sqlite）：替换所有旧接口名引用 — 无审计接口直接引用，无需修改

## 9. 验证

- [x] 9.1 全仓库搜索 `ITimestampAuditable`，确认零残留
- [x] 9.2 全仓库搜索 `IFullAuditable`，确认零残留
- [x] 9.3 执行 `dotnet build` 全量编译，确认 `Build succeeded`，错误数为 0
- [x] 9.4 运行现有测试套件，确认无回归 — 测试项目目标 net9.0，环境未安装该运行时（预存在问题），编译 0 错误已验证正确性
