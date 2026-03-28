## Requirements

### Requirement: 审计接口文件夹隔离
`Tenon.Repository` 项目 SHALL 在根目录下包含 `Auditing/` 子文件夹，所有以 `*Auditable*` 为名的接口文件 SHALL 位于该子文件夹内，命名空间保持 `Tenon.Repository` 不变。

#### Scenario: Auditing 目录存在且包含正确文件
- **WHEN** 查看 `src/Tenon.Repository/Auditing/` 目录
- **THEN** 目录中包含且仅包含 `IDeletionAuditable.cs`、`ICreationModificationAuditable.cs`、`IAuditableEntity.cs` 三个文件

#### Scenario: 命名空间不因目录迁移而改变
- **WHEN** 打开任意已迁移的审计接口文件
- **THEN** 文件顶部 `namespace` 声明为 `Tenon.Repository`，不含子命名空间 `Auditing`

---

### Requirement: ICreationModificationAuditable 接口定义
系统 SHALL 提供 `ICreationModificationAuditable` 接口，用于跟踪实体的创建时间和最后更新时间，接口成员为 `CreatedAt` 和 `UpdatedAt`。

#### Scenario: 新接口名正确定义
- **WHEN** 查看 `Auditing/ICreationModificationAuditable.cs`
- **THEN** 接口声明为 `public interface ICreationModificationAuditable`，包含 `CreatedAt` 和 `UpdatedAt` 两个属性

#### Scenario: EfCore 实现类引用已更新
- **WHEN** 查看 `EfCreationModificationAuditEntity` 类声明
- **THEN** 实现声明为 `ICreationModificationAuditable`

#### Scenario: 拦截器类型检查已更新
- **WHEN** 查看 `TimestampAuditableFieldsInterceptor` 中的类型判断代码
- **THEN** `typeof(ICreationModificationAuditable)` 用于 ChangeTracker 条目枚举

---

### Requirement: IAuditableEntity 接口定义
系统 SHALL 提供 `IAuditableEntity<TKey>` 接口，聚合创建/修改时间、创建/更新者以及软删除的全量审计能力，继承自 `ICreationModificationAuditable` 和 `IDeletionAuditable<TKey>`。

#### Scenario: 新接口名正确定义且继承关系完整
- **WHEN** 查看 `Auditing/IAuditableEntity.cs`
- **THEN** 接口声明为 `public interface IAuditableEntity<TKey> : ICreationModificationAuditable, IDeletionAuditable<TKey>`

#### Scenario: EfFullAuditableEntity 引用已更新
- **WHEN** 查看 `EfFullAuditableEntity` 类声明
- **THEN** 实现声明为 `IAuditableEntity<long>`

#### Scenario: ITenantAuditable 继承已更新
- **WHEN** 查看 `ITenantAuditable<TUserKey, TTenantKey>` 接口声明
- **THEN** 继承声明包含 `IAuditableEntity<TUserKey>`

#### Scenario: EF 查询过滤器和拦截器类型检查已更新
- **WHEN** 查看 `AbstractEntityTypeConfiguration` 和 `FullAuditableFieldsInterceptor`
- **THEN** `typeof(IAuditableEntity<long>)` 用于类型检查

---

### Requirement: 派生类完整同步，无编译错误
所有直接或间接引用审计接口的代码 SHALL 在接口变更后通过 `dotnet build` 编译，不得存在任何 CS 错误。

#### Scenario: 全量构建成功
- **WHEN** 在仓库根目录执行 `dotnet build`
- **THEN** 构建输出显示 `Build succeeded`，错误数量为 0

#### Scenario: EfCreationModificationAuditEntity 类名与接口对称
- **WHEN** 查看 `Tenon.Repository.EfCore/Auditing/` 中的实现类
- **THEN** `EfCreationModificationAuditEntity` 实现 `ICreationModificationAuditable`，并位于 `Auditing/` 子文件夹内
