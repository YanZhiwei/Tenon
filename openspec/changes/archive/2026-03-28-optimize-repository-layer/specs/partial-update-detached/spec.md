## ADDED Requirements

### Requirement: UpdateAsync 支持对 Detached 实体进行局部列更新
`EfRepository.UpdateAsync(TEntity entity, Expression<Func<TEntity, object>>[] updatingExpressions, CancellationToken)` SHALL 支持当实体处于 `EntityState.Detached` 状态时，将其状态切换为 `Unchanged` 并仅将指定属性标记为 `Modified`，从而实现局部列更新而不产生全量 UPDATE。

#### Scenario: Detached 实体局部列更新成功
- **WHEN** 传入一个未被 DbContext 追踪（`Detached`）的实体和若干属性表达式
- **THEN** 方法 SHALL 将该实体 `entry.State` 设置为 `Unchanged`
- **THEN** 只有 `updatingExpressions` 中指定的属性的 `IsModified` SHALL 为 `true`
- **THEN** 执行 `SaveChangesAsync` 后生成的 SQL UPDATE 语句中 SHALL 只包含指定的列

#### Scenario: Added/Deleted 状态实体调用局部更新抛出异常
- **WHEN** 传入一个 `EntityState.Added` 或 `EntityState.Deleted` 状态的实体
- **THEN** 方法 SHALL 抛出 `InvalidOperationException`，不执行任何数据库操作

#### Scenario: Modified/Unchanged 状态实体调用局部更新按指定列过滤
- **WHEN** 传入一个已被追踪且处于 `Modified` 或 `Unchanged` 状态的实体和属性表达式
- **THEN** 方法 SHALL 仅保留 `updatingExpressions` 指定属性的 Modified 标记，其他属性的 `IsModified` SHALL 设为 `false`
- **THEN** SQL UPDATE 语句中 SHALL 只包含指定的列

### Requirement: UpdateAsync 逻辑分支无死代码
`EfRepository.UpdateAsync(entity, expressions[])` 的实现 SHALL 不包含不可达的代码分支，所有 `EntityState` 分支判断逻辑 SHALL 相互排斥且覆盖完整。

#### Scenario: 代码静态分析无不可达分支警告
- **WHEN** 对 `UpdateAsync(entity, expressions[])` 方法进行静态分析
- **THEN** 不应存在任何不可达的 `if` 分支
