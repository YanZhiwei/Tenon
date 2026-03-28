## Context

`Tenon.Repository` 是 Tenon 框架的核心契约层，当前包含 7 个文件，其中 3 个审计接口（`IDeletionAuditable`、`ITimestampAuditable`、`IFullAuditable`）与 `IEntity`、`IRepository`、`ICurrentUser` 平铺在同一根目录，随着接口数量增长，关注点混杂。

审计实现链如下：

```
ITimestampAuditable          IDeletionAuditable<TKey>
        ↓                             ↓
              IFullAuditable<TKey>（继承两者）
                      ↓
         EfFullAuditableEntity（EfCore 实现）
                      ↓
     EfTenantFullAuditableEntity（多租户扩展）
```

三个拦截器（`Timestamp-`、`FullAuditable-`、`DeletionAuditable-FieldsInterceptor`）在运行时自动填充审计字段，其 `typeof(IXxx)` 检查会受接口重命名影响。

## Goals / Non-Goals

**Goals:**
- 在 `Tenon.Repository` 下创建 `Auditing/` 子文件夹，将 3 个审计接口移入
- 接口重命名：`ITimestampAuditable` → `ICreationModificationAuditable`，`IFullAuditable<TKey>` → `IAuditableEntity<TKey>`
- 命名空间保持 `Tenon.Repository`，消除外部引用方的 `using` 语句变更
- 同步更新所有派生类、拦截器、EF 配置、多租户扩展中的引用

**Non-Goals:**
- 不拆分 `ICreationModificationAuditable` 为独立的 `ICreationAuditable` / `IModificationAuditable`（粒度已够，过度拆分带来继承链复杂性）
- 不修改 `IDeletionAuditable<TKey>` 的名称（命名已清晰，语义无歧义）
- 不变更业务实体（samples 中的 `Product`、`Category`、`Order`、`OrderItem`）的直接代码，它们通过继承链间接受益
- 不引入新的审计字段或行为

## Decisions

### 决策 1：命名空间不随目录结构变化

**选项 A（选定）**：保持 `namespace Tenon.Repository`，仅移动物理文件位置。
**选项 B**：改为 `namespace Tenon.Repository.Auditing`。

选 A 的理由：选项 B 会要求所有下游使用方（业务代码、EfCore 项目、MultiTenant 项目）添加 `using Tenon.Repository.Auditing`，破坏面扩大且收益为零；C# 不强制要求命名空间与目录结构一致，此为合理惯例。

---

### 决策 2：`ITimestampAuditable` → `ICreationModificationAuditable`

**选项 A（选定）**：重命名为 `ICreationModificationAuditable`，明确表达"创建时间 + 修改时间"语义。
**选项 B**：重命名为 `ITimeAuditable`（较短但仍模糊）。
**选项 C**：不重命名（维持现状）。

选 A 的理由：`Timestamp` 在数据库/消息语境中常表示单一时间戳字段，容易与 SQL `TIMESTAMP` 类型混淆；`CreationModification` 明确对应接口的两个属性 `CreatedAt` / `UpdatedAt`，与 `IDeletionAuditable` 的命名粒度对称。

---

### 决策 3：`IFullAuditable<TKey>` → `IAuditableEntity<TKey>`

**选项 A（选定）**：重命名为 `IAuditableEntity<TKey>`，与"实体"领域语境对齐。
**选项 B**：重命名为 `ICompleteAuditable<TKey>`（`Complete` 同样主观）。
**选项 C**：重命名为 `ICreationModificationDeletionAuditable<TKey>`（过长，可读性差）。

选 A 的理由：该接口是完整审计能力的顶层收口，实现它的类本身就是"具有完整审计能力的实体"，`IAuditableEntity` 语义自然；去除 `Full` 避免"Full 相对谁"的疑问。

---

### 决策 4：同步策略——全局文字替换

拦截器、EF 配置中使用了 `typeof(ITimestampAuditable)` / `typeof(IFullAuditable<long>)` 等反射式类型检查，必须一次性全局替换，不能分批。采用 IDE 重命名重构（或逐文件 StrReplace）确保无遗漏，替换后全量编译验证。

## Risks / Trade-offs

| 风险 | 缓解措施 |
|------|---------|
| 遗漏某处引用导致编译失败 | 替换完成后执行 `dotnet build` 全量验证，CI 必须绿色 |
| 外部 NuGet 消费者受破坏性变更影响 | 在 Release Notes 中标注 `BREAKING CHANGE`，发布时 Major 版本号递增 |
| 拦截器 `typeof` 检查替换遗漏 | 搜索全仓库 `ITimestampAuditable`、`IFullAuditable` 关键字，确认零残留 |

## Migration Plan

1. 在 `Tenon.Repository` 中创建 `Auditing/` 目录
2. 移动并重命名接口文件（保持 namespace 不变）
3. 按依赖顺序更新引用：
   - `Tenon.Repository.EfCore`（实现类 + 拦截器 + EF 配置）
   - `Tenon.Repository.EfCore.MultiTenant`（租户实体 + 配置）
   - `Tenon.Repository.EfCore.Sqlite`（扩展注册）
   - `Tenon.Repository.MultiTenant`（`ITenantAuditable` 继承声明）
4. `dotnet build` 全量编译，零错误
5. 运行现有测试套件，确认无回归

回滚：本次变更纯粹是重命名+移动，Git revert 单 commit 即可回滚。

## Open Questions

- `EfTimestampAuditEntity` 类名是否同步重命名为 `EfCreationModificationAuditEntity`？（实现类名称与接口对称性 vs 已有惯例保持）→ 建议同步，任务阶段确认。
