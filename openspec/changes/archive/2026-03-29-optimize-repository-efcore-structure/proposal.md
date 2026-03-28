## Why

`Tenon.Repository.EfCore` 已具备清晰的领域划分（拦截器、审计实体、工作单元、扩展），但部分核心类型堆在程序集根目录，`EfRepository<TEntity>` 单文件体量过大，导航与职责边界不够直观。通过目录重组与适度拆分，可在不改变对外契约的前提下提升可维护性与后续演进效率。

## What Changes

- 按职责划分顶层文件夹（例如：`Repositories/`、`Context/`、`Query/`、`Infrastructure/` 或等效命名），将根目录散落的类型归位；根目录仅保留程序集入口或最少量的 `GlobalUsings`/说明性占位（若有）。
- 将 `EfRepository.cs` 按职责拆分为 **partial class**（查询、变更、分页等）或多文件同类，降低单文件认知负担；保持 `public` API 与行为一致。
- 审视可内聚的小类型（如 `DisposeAction`、`DatabaseColumnLength`、`PagedResult`）是否与 `Tenon.Repository` 或其它包重复；若仅本包使用则归入合适子命名空间与文件夹，避免无意义文件扩散。
- 统一 `namespace` 与文件夹的对应关系（二选一并文档化：**文件夹映射命名空间** 或 **文件夹仅作物理组织、命名空间保持扁平**），并一次性修正引用方（含 `Tenon.Repository.EfCore.MultiTenant` 等 `InternalsVisibleTo` 消费者）。
- **BREAKING**：若对外可见类型命名空间或类型位置（类型转发除外）发生变化，则对引用该程序集的解决方案视为破坏性变更，需在迁移说明中列出；若仅 `internal` 或文件移动且 `namespace` 不变，则通常 **非 BREAKING**。

## Capabilities

### New Capabilities

- `repository-efcore-module-layout`：描述 `Tenon.Repository.EfCore` 程序集内目录分层、命名空间策略及与 `Tenon.Repository` 抽象边界的约定，作为本变更的可验证“结构契约”（以行为不变为前提）。

### Modified Capabilities

- （无）本变更以结构与实现组织为主，不修改既有 `openspec/specs/` 中已发布的仓储行为需求；若后续评审发现需同步 `repository-contracts` 等规范，再在实现阶段追加 delta。

## Impact

- **代码**：`src/Tenon.Repository.EfCore/**` 下文件移动与可能的 partial 拆分；`using` 与项目引用调整。
- **依赖方**：引用 `Tenon.Repository.EfCore` 的宿主项目、扩展包（如 `Tenon.Repository.EfCore.MultiTenant`）；若公开 API/命名空间不变，影响限于本仓库内编译与测试。
- **构建/测试**：需全量编译相关测试项目，确认无行为回归。
