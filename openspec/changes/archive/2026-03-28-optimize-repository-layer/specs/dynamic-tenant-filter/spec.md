## ADDED Requirements

### Requirement: 全局查询过滤器运行时动态解析租户 ID
`TenonDbContext` 的多租户全局查询过滤器 SHALL 在每次查询执行时从 `IEfTenantResolver` 实时读取 `TenantId`，而不是在 `OnModelCreating` 阶段将其快照为常量值。

#### Scenario: 同一 DbContext 实例处理不同租户请求
- **WHEN** 两个连续请求持有不同的 `IEfTenantResolver.TenantId`（如租户 A 和租户 B）
- **THEN** 每次查询的 SQL WHERE 条件中 `TenantId` 值应分别对应各自请求的租户 ID，不得混用

#### Scenario: DbContextPool 场景下租户隔离
- **WHEN** 应用使用 `AddDbContextPool` 且多个并发请求分属不同租户
- **THEN** 每个请求的查询 SHALL 仅返回其对应租户的数据，不得返回其他租户数据

### Requirement: 租户过滤与软删除过滤可组合共存
同时实现 `ITenant<long>` 和 `IDeletionAuditable<long>` 的实体，其全局查询过滤器 SHALL 同时包含租户隔离条件和软删除排除条件，且不得因 `HasQueryFilter` 的后注册覆盖前注册而丢失任一条件。

#### Scenario: 同时有软删除和租户的实体查询
- **WHEN** 查询一个同时实现 `ITenant<long>` 和 `IDeletionAuditable<long>` 的实体
- **THEN** 生成的 SQL SHALL 包含 `TenantId = @tenantId AND IsDeleted = 0` 两个条件
- **THEN** 已软删除的同租户记录不得出现在查询结果中
- **THEN** 未删除的其他租户记录不得出现在查询结果中

#### Scenario: 只有软删除无租户的实体查询
- **WHEN** 查询只实现 `IDeletionAuditable<long>` 但未实现 `ITenant<long>` 的实体
- **THEN** 生成的 SQL SHALL 包含 `IsDeleted = 0` 条件
- **THEN** 已软删除记录不得出现在查询结果中

### Requirement: 仓储层不重复追加租户过滤条件
`EfTenantRepository.GetDbSet` SHALL 不在 `IQueryable` 上手动追加 `Where(e => e.TenantId == currentTenantId)`，租户过滤 SHALL 完全由 DbContext 全局查询过滤器负责。

#### Scenario: 查询 SQL 中租户条件仅出现一次
- **WHEN** 通过 `EfTenantRepository` 执行任意查询
- **THEN** 生成的 SQL 中 `TenantId` 过滤条件 SHALL 只出现一次，不得重复

### Requirement: 写入操作由拦截器统一设置 TenantId
插入租户实体时，`TenantId` SHALL 由 `TenantInterceptor`（`SaveChangesInterceptor`）在 `SavingChanges` 阶段统一设置，`EfTenantRepository.InsertAsync` 不得再手动赋值。

#### Scenario: 插入实体时 TenantId 由拦截器赋值
- **WHEN** 调用 `EfTenantRepository.InsertAsync` 插入一个租户实体
- **THEN** 实体的 `TenantId` SHALL 被设置为当前 `IEfTenantResolver.TenantId` 的值
- **THEN** `EfTenantRepository.InsertAsync` 方法体中不应包含 `entity.TenantId =` 赋值语句
