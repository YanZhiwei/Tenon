### Requirement: CountAsync 返回 long 类型
`IRepository<TEntity, TKey>.CountAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken token)` 的返回类型 SHALL 为 `Task<long>`，以支持超过 `int.MaxValue`（约 21 亿）的数据量统计而不发生整数溢出。

#### Scenario: 正常数据量计数
- **WHEN** 调用 `CountAsync` 且满足条件的记录数在 `int` 范围内（如 1000 条）
- **THEN** 方法 SHALL 返回正确的 `long` 类型计数值

#### Scenario: 超大数据量计数不溢出
- **WHEN** 满足条件的记录数超过 `int.MaxValue`
- **THEN** 方法 SHALL 返回正确的 `long` 类型计数值，不发生溢出或截断

### Requirement: GetAllAsync 接受 CancellationToken 参数
`IRepository<TEntity, TKey>.GetAllAsync` 的签名 SHALL 为 `GetAllAsync(CancellationToken token = default)`，以与接口中其他异步方法保持一致，支持调用方传入取消令牌。

#### Scenario: 传入 CancellationToken 可取消查询
- **WHEN** 调用 `GetAllAsync(cancellationToken)` 且在查询执行前取消了令牌
- **THEN** 方法 SHALL 抛出 `OperationCanceledException`，不返回数据

#### Scenario: 不传参数时使用默认值（向后兼容）
- **WHEN** 调用 `GetAllAsync()` 不传任何参数
- **THEN** 方法 SHALL 正常执行并返回所有实体，行为与修改前一致
