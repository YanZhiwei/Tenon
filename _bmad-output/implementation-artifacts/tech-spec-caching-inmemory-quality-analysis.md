---
title: 'Tenon.Caching.InMemory 高质量开源与 NuGet 规范符合性分析'
slug: 'caching-inmemory-quality-analysis'
created: '2025-02-27'
status: 'done'
stepsCompleted: [1, 2, 3, 4]
tech_stack: ['.NET 9', 'Tenon.Caching.Abstractions', 'System.Runtime.Caching 9.0.0', 'Microsoft.Extensions.DependencyInjection 9.0.0', 'Microsoft.Extensions.Configuration.Abstractions 9.0.0', 'xUnit']
files_to_modify: ['src/Tenon.Caching.InMemory/README.md', '_bmad-output/implementation-artifacts/tech-spec-wip.md']
code_patterns: ['Options 在 Configurations/（InMemoryCacheOptions）', '扩展在 Extensions/（ServiceCollectionExtension、CachingOptionsExtension、CachingOptionsInMemoryExtensions）', 'file-scoped namespace', 'sealed 实现类', 'ArgumentNullException.ThrowIfNull', 'TryAddSingleton/TryAddKeyedSingleton']
test_patterns: ['test/Tenon.Caching.InMemoryTests，xUnit，单文件 ServiceCollectionExtensionTests，覆盖无参/带 Options/AddCaching+UseInMemoryStorage/Remove/Exists/RemoveAll']
---

# Tech-Spec: Tenon.Caching.InMemory 高质量开源与 NuGet 规范符合性分析

**Created:** 2025-02-27

## Overview

### Problem Statement

Tenon.Caching.InMemory 目标作为高质量开源项目及 NuGet 包对外提供。需在**不改变当前对外接口**的前提下，从代码架构、目录结构、类定义与 README 编写等维度，系统评估其是否符合项目规范（project-context）与高质量开源/NuGet 的常见最佳实践，并给出可操作的改进建议（若有）。

### Solution

基于 `_bmad-output/project-context.md` 与仓库既有惯例，对 Tenon.Caching.InMemory 进行结构化符合性分析：梳理目录与文件组织、类与命名设计、选项与扩展模式、XML 文档、README 准确性与结构、NuGet 元数据等；形成结论与可选改进任务，使后续实施有据可依。

### Scope

**In Scope:**
- 代码架构与接口一致性（保持 ICacheProvider 等现有接口不变）。
- 目录结构、文件命名与类型组织。
- 类定义（密封性、选项类、扩展类、XML 文档、复杂度与规范）。
- README 的准确性、示例代码、配置说明、项目结构描述。
- 与 project-context 的符合性（Nullable、异步、Options、DI、可见性等）。

**Out of Scope:**
- 修改 Tenon.Caching.Abstractions 中的 ICacheProvider、CachingOptions 等接口与类型。
- 其他缓存实现包（如 Tenon.Caching.Redis）的改动。
- 版本号、CI/CD 或打包流程的变更（除非与本包元数据直接相关）。

## Context for Development

### Codebase Patterns

- **项目规范**：遵循 `_bmad-output/project-context.md`（.NET 9、Nullable、异步命名、方法复杂度 ≤10、无方法体内注释、XML 文档、Options 与 DI 扩展命名等）。
- **目录与命名**：Options 类在 `Configurations/InMemoryCacheOptions.cs`；扩展方法在 `Extensions/`（`ServiceCollectionExtension`、`CachingOptionsExtension`、`CachingOptionsInMemoryExtensions`）。`CachingOptionsInMemoryExtensions` 提供 `UseInMemoryStorage(this CachingOptions)`，命名与选项类区分清晰。
- **DI 与选项**：`ServiceCollectionExtension` 提供无参与 `Action<InMemoryCacheOptions>` 重载，内部用 options 构造 `MemoryCacheProvider` 并注册单例；`CachingOptionsExtension` 仅根据 `KeyedServiceKey` 注册无参 `MemoryCacheProvider()`，键控路径下无法传入内存/轮询等配置（UseInMemoryStorage 的 IConfigurationSection 当前未使用）。
- **核心实现**：`MemoryCacheProvider` 为 sealed、实现 IDisposable；构造函数 4 个可选参数，符合“最多 3–4 参数”；方法均为短逻辑、无方法内注释、有完整 XML 文档；`RemoveAll`/`KeysExpireAsync(keys, expiration)` 对 IEnumerable 做 `.ToList()` 避免多次枚举，符合 LINQ 规范。
- **相关规格**：`tech-spec-caching-inmemory-nuget-improvements.md` 已完成；本规格侧重**符合性分析**与**质量评估**，不重复已实施任务。

### Files to Reference

| File | Purpose / 调查结论 |
| ---- | ------------------ |
| `src/Tenon.Caching.InMemory/MemoryCacheProvider.cs` | 核心实现；sealed、IDisposable、完整 XML 文档；构造函数与 Set/Get/Remove/Exists/RemoveAll/KeysExpireAsync 实现；无阻塞 async、无方法内注释，复杂度低。 |
| `src/Tenon.Caching.InMemory/Configurations/InMemoryCacheOptions.cs` | 选项类，四属性与 Provider 构造参数对应；PascalCase、可 set；README 配置表与此一致。 |
| `src/Tenon.Caching.InMemory/Extensions/CachingOptionsInMemoryExtensions.cs` | 静态类，扩展 CachingOptions.UseInMemoryStorage(options, IConfigurationSection?)；inMemorySection 未使用；已迁至 Extensions 并与选项类命名区分。 |
| `src/Tenon.Caching.InMemory/Extensions/CachingOptionsExtension.cs` | ICachingOptionsExtension；AddServices 中键控时固定无参 `new MemoryCacheProvider()`，无配置传入（已知限制）。 |
| `src/Tenon.Caching.InMemory/Extensions/ServiceCollectionExtension.cs` | AddInMemoryCache 无参与带 configureOptions 重载；带 options 时手动 new InMemoryCacheOptions 并构造 Provider，符合“Options 在 AddXxx 中注册并文档化”。 |
| `src/Tenon.Caching.InMemory/README.md` | 快速开始、两种注册方式、配置表与示例正确；项目结构已列 Configurations/InMemoryCacheOptions 与 Extensions 下三文件及角色说明。 |
| `src/Tenon.Caching.InMemory/Tenon.Caching.InMemory.csproj` | net9.0、Nullable、ImplicitUsings、Description、PackageTags；依赖 Abstractions + Configuration.Abstractions + DI + System.Runtime.Caching 9.0.0。 |
| `test/Tenon.Caching.InMemoryTests/ServiceCollectionExtensionTests.cs` | xUnit；6 个用例：无参/带 Options/null Options、AddCaching+UseInMemoryStorage、Remove/Exists、RemoveAll；无对 MemoryCacheProvider 的直接单元测试（仅通过 DI）。 |
| `_bmad-output/project-context.md` | 已加载；Options 在 Configurations/、扩展在 Extensions/、sealed、readonly、异步命名、异常与 Dispose 等规则已对照。 |

### Technical Decisions

- **分析基准**：以 project-context 为主，辅以常见开源/NuGet 实践（README 结构、示例可运行、包描述与标签）。
- **接口不变**：所有建议不涉及 ICacheProvider、CachingOptions 等 Abstractions 的 API 变更。
- **结论形式**：每条分析维度在 Step 3 给出“符合 / 部分符合 / 不符合”及证据；改进任务仅包含已确认差距（如 README 项目结构遗漏 InMemoryCacheOptions.cs）；键控路径下无法传配置列为“已知限制/可选增强”，不强制在本规格中实现。

## Implementation Plan

### 符合性结论矩阵

| 维度 | 结论 | 证据 |
|------|------|------|
| 目录结构 | 符合 | Configurations/ 仅选项类 InMemoryCacheOptions；Extensions/ 含 ServiceCollectionExtension、CachingOptionsExtension、CachingOptionsInMemoryExtensions；根目录仅 MemoryCacheProvider。与 project-context 一致。 |
| 类命名与拆分 | 符合 | 选项类 InMemoryCacheOptions；扩展方法容器 CachingOptionsInMemoryExtensions（*Extensions 后缀），已迁至 Extensions/，与选项类区分清晰。 |
| 类定义 | 符合 | MemoryCacheProvider sealed、IDisposable、XML 文档完整、无方法内注释、IEnumerable 单次枚举；CachingOptionsExtension 键控时无参 Provider 为已知限制。 |
| README | 符合 | 项目结构已列 Configurations/InMemoryCacheOptions.cs 与 Extensions 下三文件及角色说明；示例使用 Get/CacheValue；配置表与 InMemoryCacheOptions 属性一致。 |
| project-context | 符合 | .NET 9、Nullable、异步命名、Options 在 Configurations、扩展在 Extensions、sealed、TryAddSingleton。 |

### Tasks

- [x] **Task 0（已实施）: 扩展类迁移与重命名**
  - File: 已新增 `src/Tenon.Caching.InMemory/Extensions/CachingOptionsInMemoryExtensions.cs`，已删除 `Configurations/InMemoryCachingOptions.cs`；已更新 `ServiceCollectionExtension.cs` 的 cref、README 项目结构。
  - Action: 将原 InMemoryCachingOptions 迁至 Extensions 并重命名为 CachingOptionsInMemoryExtensions，与选项类命名区分。
  - Notes: 本次 quick-spec 会话前已完成。

- [x] **Task 1: 确认 README 项目结构与代码一致**
  - File: `src/Tenon.Caching.InMemory/README.md`
  - Action: 核对「项目结构」小节：应包含 Configurations/InMemoryCacheOptions.cs、Extensions/ 下 CachingOptionsExtension.cs、CachingOptionsInMemoryExtensions.cs、ServiceCollectionExtension.cs，且每项有简短角色说明（选项类 vs 扩展）。若已一致则仅做验收，无需修改。
  - Notes: 已验收，与当前代码一致。

- [x] **Task 2: 在文档中注明键控路径的已知限制**
  - File: `src/Tenon.Caching.InMemory/README.md` 或本规格的 Notes
  - Action: 在「两种注册方式」或「使用注意事项」中增加一句：通过 `AddCaching(o => o.UseInMemoryStorage())` 注册时，当前仅使用默认内存缓存（无内存限制/轮询间隔等配置）；需自定义配置请使用 `AddInMemoryCache(options => { … })`。
  - Notes: 已在「方式二」下增加引用块说明，并在「使用注意事项」中补充一条。

### Acceptance Criteria

- [x] **AC1**: Given 本规格的符合性结论矩阵，When 逐项对照当前代码与 README，Then 目录结构、类命名与拆分、类定义、README、project-context 五维度的结论与证据与仓库现状一致。
- [x] **AC2**: Given README 的「项目结构」小节，When 读者查看该小节，Then 列出 Configurations/InMemoryCacheOptions.cs 与 Extensions/ 下三文件，且每项有「选项类」或「扩展（UseInMemoryStorage / AddInMemoryCache / DI 注册）」等角色说明，无遗漏或错误文件名。
- [x] **AC3**: Given 用户按 README「快速开始」与「两种注册方式」操作，When 复制示例代码并运行，Then 仅使用 Get/CacheValue（无 TryGet），且可编译并通过基本 Set/Get 行为验证。
- [x] **AC4**: Given 键控注册方式（AddCaching + UseInMemoryStorage），When 用户查阅 README 或本规格，Then 能得知当前不支持传入 InMemoryCacheOptions（仅默认缓存），或已在文档中注明该限制。

## Additional Context

### 分析维度（Step 2 展开）

- **目录结构**：Configurations vs Extensions vs 根目录类型是否与 project-context 及同层包一致；README 中“项目结构”是否与当前文件一致。
- **类命名与拆分**：
  - **命名**：`InMemoryCacheOptions`（选项类）与 `CachingOptionsInMemoryExtensions`（静态扩展类，提供 UseInMemoryStorage）已区分；后者已迁至 Extensions/ 并采用 *Extensions 后缀，符合 project-context。
  - **拆分**：一类型一文件、Options 在 Configurations/、扩展在 Extensions/，已落实。
- **类定义**：MemoryCacheProvider（sealed、IDisposable、构造函数参数）；CachingOptionsExtension 键控时是否缺少配置传递；XML 文档完整性；方法复杂度与长度。
- **README**：与 API 一致（Get/CacheValue、无 TryGet）；两种注册方式描述；配置表与 InMemoryCacheOptions 属性一致；示例可编译；项目结构、依赖、注意事项、贡献与协议。

### Dependencies

- **包依赖**：Tenon.Caching.Abstractions、System.Runtime.Caching 9.0.0、Microsoft.Extensions.DependencyInjection、Microsoft.Extensions.Configuration.Abstractions（UseInMemoryStorage 的 IConfigurationSection 参数）。
- **规格依赖**：本分析不修改 Abstractions 接口；与 tech-spec-caching-inmemory-nuget-improvements 已实施内容（README、InMemoryCacheOptions、单元测试）一致。

### Testing Strategy

- **单元测试**：沿用 Tenon.Caching.InMemoryTests（AddInMemoryCache 无参/带 Options、AddCaching+UseInMemoryStorage、Remove/Exists/RemoveAll）；本规格无新增用例要求。
- **验收方式**：AC1–AC4 通过人工核对 README 与代码、可选按 README 示例新建控制台项目验证可编译运行。
- **后续**：若为键控路径增加 InMemoryCacheOptions 支持，需在 InMemoryTests 中增加对应集成用例。

### Notes

- **已知限制**：通过 `AddCaching(o => o.UseInMemoryStorage())` 注册时，当前仅使用无参 `MemoryCacheProvider()`，无法传入内存限制/轮询间隔等；需自定义配置请使用 `AddInMemoryCache(options => { … })`。建议在 README 中注明（Task 2）。
- **Out of Scope 补充**：不包含为 UseInMemoryStorage 键控路径增加 InMemoryCacheOptions 支持（属后续增强）。

## Review Notes

- Adversarial review completed
- Findings: 4 total, 2 fixed (F1 表述修正、F4 长句拆为两句), 2 skipped (F2 重复说明保留、F3 版式未改)
- Resolution approach: auto-fix
