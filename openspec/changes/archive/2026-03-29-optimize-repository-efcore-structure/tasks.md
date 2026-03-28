## 1. 基线与约定确认

- [x] 1.1 在解决方案内检索 `Tenon.Repository.EfCore` 的引用与友元程序集（含 `Tenon.Repository.EfCore.MultiTenant`），确认无依赖物理路径或反射限定路径
- [x] 1.2 确认采用 design 中的决策 A（保持现有 `namespace`，仅移动文件）或记录采用 B 的全量替换计划

## 2. 目录重组

- [x] 2.1 使用 `git mv` 将 `EfRepository`、`IEfRepository`、`PagedResult` 迁入 `Repositories/`
- [x] 2.2 将 `TenonDbContext`、`AbstractEntityTypeConfiguration` 迁入 `Context/`
- [x] 2.3 将 `EfEntity`、`IConcurrency` 迁入 `Entities/`（或设计选定的等效名称）
- [x] 2.4 将 `EfCoreTypeScanner`、`DisposeAction`、`DatabaseColumnLength` 迁入 `Infrastructure/`
- [x] 2.5 全解决方案构建，修正 `using` 与项目内引用（若命名空间未变，通常仅需 IDE/分析器提示清理）

## 3. 拆分 EfRepository

- [x] 3.1 将 `EfRepository<TEntity>` 改为 `partial`，并拆出查询相关方法至 `EfRepository.Query.cs`（或等效命名）
- [x] 3.2 拆出插入/更新/删除相关方法至 `EfRepository.Commands.cs`（或等效命名）
- [x] 3.3 将 `GetDbSet` 等共享辅助方法固定在单一 partial 中，避免重复定义

## 4. 验证

- [x] 4.1 运行与 `Tenon.Repository.EfCore` 相关的单元测试与集成测试（若有）
- [x] 4.2 对比变更前后公开 API（如使用 `dotnet pack` 或 API 兼容性工具）确认无意外签名变化

## 5. 收尾

- [x] 5.1 更新本变更的 `tasks.md` 勾选状态，并在 PR/说明中记录是否采用命名空间策略 B 与 `PagedResult`/`DisposeAction` 等开放问题的结论

---

**实施记录（5.1）**

- **命名空间策略**：采用决策 **A**，公开类型仍位于 `Tenon.Repository.EfCore` 等既有命名空间，仅调整物理路径。
- **1.1 结论**：未发现对源文件路径或 `Assembly` 按路径加载 `EfCore` 类型的依赖；`EfCoreTypeScanner` 仍为 `internal`，友元 `Tenon.Repository.EfCore.MultiTenant` 通过类型名引用，已随解决方案编译验证。
- **4.1**：仓库中无单独引用 `Tenon.Repository.EfCore` 的测试项目；已通过 `dotnet build` 验证核心包及 `MultiTenant` / `Sqlite` / `MySql` / `MessageTracker.EfCore` 依赖链。
- **4.2**：已执行 `dotnet pack -c Debug`（`Tenon.Repository.EfCore`），打包成功；未引入新的公开 API。
- **开放问题**：`PagedResult`、`DisposeAction` 是否迁出或合并维持 design 中的“后续评审”，本变更未改动其归属与语义。
