## Context

`Tenon.Repository.EfCore` 当前将 `EfRepository<TEntity>`、`TenonDbContext`、工具类型（如 `EfCoreTypeScanner`、`DisposeAction`）等放在程序集根目录，而已有 `Auditing/`、`Configurations/`、`Extensions/`、`Interceptors/`、`Transaction/` 等子目录。根目录与文件夹并存导致“新代码放哪”缺少单一约定；`EfRepository.cs` 接近 400 行，读改成本高。程序集通过 `InternalsVisibleTo` 暴露给 `Tenon.Repository.EfCore.MultiTenant`，移动 `internal` 类型时需同步验证。

约束：优先保持对外可见 API 稳定；与 `Tenon.Repository` 中 `IRepository` 等契约对齐；EF Core 9、.NET 9 目标框架不变。

## Goals / Non-Goals

**Goals:**

- 建立清晰、可重复的目录分层，使仓储实现、DbContext、基础设施与领域扩展各司其职。
- 在不改变行为的前提下拆分 `EfRepository`（`partial`），降低单文件复杂度。
- 明确命名空间策略（见下文决策），减少后续贡献者的犹豫成本。

**Non-Goals:**

- 重写仓储语义、替换 `IRepository`/`IEfRepository` 方法签名或变更拦截器行为。
- 将 `PagedResult` 等类型迁出本程序集（除非评审发现与 `Tenon.Repository` 重复且合并收益明确）。
- 引入新的第三方依赖 solely for 文件夹/文档。

## Decisions

### 1. 推荐目录映射（可在实现时微调命名，但需一次性统一）

| 区域 | 建议路径 | 迁入的典型类型 |
|------|----------|----------------|
| 仓储与分页 API | `Repositories/` | `EfRepository`（拆分为多个 partial）、`IEfRepository`、`PagedResult` |
| 数据上下文与映射基类 | `Context/` | `TenonDbContext`、`AbstractEntityTypeConfiguration` |
| 实体与并发标记 | `Entities/`（或 `Model/`） | `EfEntity`、`IConcurrency` |
| 扫描与杂项基础设施 | `Infrastructure/` | `EfCoreTypeScanner`、`DisposeAction`、`DatabaseColumnLength` |
| 已有目录 | 保持 | `Auditing/`、`Configurations/`、`Extensions/`、`Interceptors/`、`Transaction/` |

**理由**：按“读模型 / 写路径 / 宿主上下文 / 横切基础设施”分区，与常见 EF 分层习惯一致；`Extensions` 继续承载 `ServiceCollectionExtensions`、`ModelBuilderExtension`。

### 2. 命名空间策略（二选一，推荐 A）

- **A（推荐，默认非 BREAKING）**：文件夹仅作物理组织，**公开类型仍使用 `Tenon.Repository.EfCore`（及现有子命名空间如 `Tenon.Repository.EfCore.Extensions`）**，新文件不强制新增子命名空间。
- **B（可选，BREAKING 风险）**：为 `Repositories`、`Context` 等引入 `Tenon.Repository.EfCore.Repositories` 等子命名空间，需全解决方案替换 `using`、更新公开文档与版本说明。

本变更实施阶段默认采用 **A**，除非产品明确要求命名空间与文件夹一一对应。

### 3. 拆分 `EfRepository` 的方式

- 使用 **`partial class EfRepository<TEntity>`**，按职责拆文件，例如：
  - `EfRepository.Query.cs`：`Where`、`Get*`、`GetAll`、`GetPaged*`、`AnyAsync`、`CountAsync`
  - `EfRepository.Commands.cs`：`Insert*`、`Update*`、`Remove*`
- 保持 `GetDbSet` 等 `protected` 辅助方法在一处定义（可放在主文件或单独的 `EfRepository.Core.cs`），避免循环依赖。

**备选**：按“同步读/写”拆分过细会增加文件跳转；若团队偏好 2 个文件即可合并 Query+Commands 为“读写”两大块。

### 4. `internal` 与友元程序集

- `EfCoreTypeScanner` 等 `internal` 类型移动后，**命名空间可不变**，确保 `MultiTenant` 等友元仍能通过 `InternalsVisibleTo` 编译（若友元使用完全限定名则风险更低）。

## Risks / Trade-offs

- **[Risk] 大量文件移动导致合并冲突** → 在短窗口内完成移动与拆分，避免与功能分支长期并行；使用 `git mv` 保留历史。
- **[Risk] 误以为“重构”可改行为** → 拆分前后运行现有测试与关键集成场景；对比 `SaveChanges` 调用次数等敏感点（原则上不变）。
- **[Trade-off] 不引入子命名空间** → 目录更清晰但 `namespace` 仍较扁平；接受度由团队统一，后续可单独发起命名空间迁移变更。

## Migration Plan

1. 在分支上按设计完成目录与 partial 拆分；全解决方案构建。
2. 若有引用方使用反射按程序集路径加载类型（少见），需检索 `Assembly.GetType` 等并验证不受影响。
3. 发布说明：若仅内部文件移动且公开 API 不变，记为**内部可维护性改进**；若采用决策 B，则 semver 主版本或次版本按团队规范处理。

## Open Questions

- `PagedResult` 是否更适合与抽象层 `Tenon.Repository` 共处（若多实现复用）——需在代码库内检索引用后决定，可作为本变更子任务或后续变更。
- `DisposeAction` 是否可被 `IDisposable` 组合或 BCL 模式替代而不改变语义——可选清理项，非本变更必做。
