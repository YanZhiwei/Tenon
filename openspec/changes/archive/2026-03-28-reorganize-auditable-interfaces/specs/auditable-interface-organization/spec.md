## ADDED Requirements

### Requirement: 审计接口文件夹隔离
`Tenon.Repository` 项目 SHALL 在根目录下包含 `Auditing/` 子文件夹，所有以 `*Auditable*` 为名的接口文件 SHALL 位于该子文件夹内，命名空间保持 `Tenon.Repository` 不变。

#### Scenario: Auditing 目录存在且包含正确文件
- **WHEN** 查看 `src/Tenon.Repository/Auditing/` 目录
- **THEN** 目录中包含且仅包含 `IDeletionAuditable.cs`、`ICreationModificationAuditable.cs`、`IAuditableEntity.cs` 三个文件

#### Scenario: 命名空间不因目录迁移而改变
- **WHEN** 打开任意已迁移的审计接口文件
- **THEN** 文件顶部 `namespace` 声明为 `Tenon.Repository`，不含子命名空间 `Auditing`

---

### Requirement: ITimestampAuditable 重命名为 ICreationModificationAuditable
系统 SHALL 将 `ITimestampAuditable` 接口重命名为 `ICreationModificationAuditable`，接口成员（`CreatedAt`、`UpdatedAt`）保持不变。

#### Scenario: 旧接口名不再存在
- **WHEN** 在整个解决方案中搜索 `ITimestampAuditable`
- **THEN** 搜索结果为零匹配（不含注释和字符串字面量）

#### Scenario: 新接口名正确定义
- **WHEN** 查看 `Auditing/ICreationModificationAuditable.cs`
- **THEN** 接口声明为 `public interface ICreationModificationAuditable`，包含 `CreatedAt` 和 `UpdatedAt` 两个属性

#### Scenario: EfCore 实现类引用已更新
- **WHEN** 查看 `EfTimestampAuditEntity` 类声明
- **THEN** 实现声明为 `ICreationModificationAuditable` 而非 `ITimestampAuditable`

#### Scenario: 拦截器类型检查已更新
- **WHEN** 查看 `TimestampAuditableFieldsInterceptor` 中的类型判断代码
- **THEN** `typeof(ICreationModificationAuditable)` 替代原来的 `typeof(ITimestampAuditable)`

---

### Requirement: IFullAuditable 重命名为 IAuditableEntity
系统 SHALL 将 `IFullAuditable<TKey>` 接口重命名为 `IAuditableEntity<TKey>`，继承关系（`ICreationModificationAuditable` 和 `IDeletionAuditable<TKey>`）保持不变，成员（`CreatedBy`、`UpdatedBy`）保持不变。

#### Scenario: 旧接口名不再存在
- **WHEN** 在整个解决方案中搜索 `IFullAuditable`
- **THEN** 搜索结果为零匹配（不含注释和字符串字面量）

#### Scenario: 新接口名正确定义且继承关系完整
- **WHEN** 查看 `Auditing/IAuditableEntity.cs`
- **THEN** 接口声明为 `public interface IAuditableEntity<TKey> : ICreationModificationAuditable, IDeletionAuditable<TKey>`

#### Scenario: EfFullAuditableEntity 引用已更新
- **WHEN** 查看 `EfFullAuditableEntity` 类声明
- **THEN** 实现声明为 `IAuditableEntity<long>` 而非 `IFullAuditable<long>`

#### Scenario: ITenantAuditable 继承已更新
- **WHEN** 查看 `ITenantAuditable<TUserKey, TTenantKey>` 接口声明
- **THEN** 继承声明包含 `IAuditableEntity<TUserKey>` 而非 `IFullAuditable<TUserKey>`

#### Scenario: EF 查询过滤器和拦截器类型检查已更新
- **WHEN** 查看 `AbstractEntityTypeConfiguration` 和 `FullAuditableFieldsInterceptor`
- **THEN** 所有 `typeof(IFullAuditable<long>)` 替换为 `typeof(IAuditableEntity<long>)`

---

### Requirement: 派生类完整同步，无编译错误
所有直接或间接引用被重命名接口的代码 SHALL 在重命名完成后通过 `dotnet build` 编译，不得存在任何 CS 错误。

#### Scenario: 全量构建成功
- **WHEN** 在仓库根目录执行 `dotnet build`
- **THEN** 构建输出显示 `Build succeeded`，错误数量为 0

#### Scenario: EfTimestampAuditEntity 类名同步更新（可选）
- **WHEN** 查看 `Tenon.Repository.EfCore` 中的实现类
- **THEN** 若决定同步重命名，`EfTimestampAuditEntity` 应重命名为 `EfCreationModificationAuditEntity`，并在所有引用处同步更新
