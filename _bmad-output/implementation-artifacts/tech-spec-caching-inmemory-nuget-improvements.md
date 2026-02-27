---
title: 'Tenon.Caching.InMemory NuGet 包补充与优化'
slug: 'caching-inmemory-nuget-improvements'
created: '2025-02-27'
status: 'done'
stepsCompleted: [1, 2, 3, 4, 5]
tech_stack: ['.NET 9', 'Tenon.Caching.Abstractions', 'System.Runtime.Caching', 'Microsoft.Extensions.DependencyInjection', 'Microsoft.Extensions.Configuration.Abstractions']
files_to_modify: ['src/Tenon.Caching.InMemory/README.md', 'src/Tenon.Caching.InMemory/Extensions/ServiceCollectionExtension.cs', 'src/Tenon.Caching.InMemory/Configurations/InMemoryCachingOptions.cs', 'src/Tenon.Caching.InMemory/Extensions/CachingOptionsExtension.cs', 'src/Tenon.Caching.InMemory/Tenon.Caching.InMemory.csproj']
code_patterns: ['Options 模式 + ICachingOptionsExtension 注册', 'AddXxx() 扩展方法在 Extensions 目录', 'file-scoped namespace', 'ArgumentNullException.ThrowIfNull']
test_patterns: ['当前仓库无 Tenon.Caching.InMemoryTests 项目；CachingSample 为唯一运行时验证；若本规格含测试则需新建 test 项目或在后续迭代补充']
---

# Tech-Spec: Tenon.Caching.InMemory NuGet 包补充与优化

**Created:** 2025-02-27

## Overview

### Problem Statement

Tenon.Caching.InMemory 目标是对外提供 NuGet 包，方便外部项目像积木一样使用。当前实现已有 `MemoryCacheProvider`、无参 `AddInMemoryCache()` 以及通过 `CachingOptions.UseInMemoryStorage()` 的注册方式，但与 README 描述、可配置性及“开箱即用”的积木体验仍存在差距，需要系统性地补充与优化，使包达到可对外发布、易发现、易配置、文档与实现一致的标准。

### Solution

在现有实现基础上：（1）对齐 README 与真实 API（移除或修正错误示例如 `TryGet`）并统一两种注册方式的说明；（2）可选地增加基于 Options 的内存缓存配置（内存限制、轮询间隔等）并在 DI 扩展中生效；（3）确保 NuGet 包元数据与打包配置完整；（4）补充或调整单元测试以覆盖扩展注册与配置路径。最终使第三方通过“引用包 → 一行注册 → 可选配置”即可接入，无需翻阅源码。

### Scope

**In Scope:**
- 本包（Tenon.Caching.InMemory）内的 API 一致性、配置能力、README 与示例代码、NuGet 元数据及打包配置。
- 与 Tenon.Caching.Abstractions 的对接方式（ICacheProvider、CachingOptions、ICachingOptionsExtension）保持现有约定。
- 本包相关的单元测试与现有 CachingSample 的兼容性。

**Out of Scope:**
- 其他缓存实现包（如 Tenon.Caching.Redis、RedisStackExchange）的改动。
- 仓库根文档或跨包“缓存选型总览”文档（除非仅涉及本包引用与链接）。
- ICacheProvider 接口本身的变更（如新增 TryGet）。

## Context for Development

### Codebase Patterns

- **Options 与 DI**：使用 `IOptions`/配置节绑定；扩展方法在 `Extensions/` 下，`AddXxx()` 命名；实现通过 `ICachingOptionsExtension` 在 `AddCaching(setupAction)` 中注册。
- **项目规范**：遵循 `_bmad-output/project-context.md`（.NET 9、Nullable、异步命名、方法复杂度 ≤10、无方法体内注释、XML 文档等）。
- **包版本**：版本由 `src/version.props` 驱动，不在 csproj 中写死；NuGet 元数据来自 `nuget.props`（Release 打包）、`common.props`（Authors、EnablePackageValidation 等）；若需包级 Description/Title，可参考同层扩展包在 csproj 中补充。

### Files to Reference

| File | Purpose |
| ---- | ------- |
| `src/Tenon.Caching.InMemory/MemoryCacheProvider.cs` | 核心实现；构造函数 `(string? cacheName, long? cacheMemoryLimitMegabytes, int? physicalMemoryLimitPercentage, TimeSpan? pollingInterval)`，全 null 用 Default，否则具名实例；CachingOptionsExtension 当前仅无参构造 |
| `src/Tenon.Caching.InMemory/Extensions/ServiceCollectionExtension.cs` | 提供无参 `AddInMemoryCache()`，无带 Options 重载 |
| `src/Tenon.Caching.InMemory/Extensions/CachingOptionsExtension.cs` | 实现 `ICachingOptionsExtension.AddServices`；根据 `CachingOptions.KeyedServiceKey` 注册无参 `MemoryCacheProvider` 单例或键控单例 |
| `src/Tenon.Caching.InMemory/Configurations/InMemoryCachingOptions.cs` | 静态扩展类，仅含 `UseInMemoryStorage(this CachingOptions options, IConfigurationSection? inMemorySection = null)`，inMemorySection 未使用；**无** 具名 Options 类（CacheName/CacheMemoryLimitMegabytes 等），README 中的 options 属性当前不存在 |
| `src/Tenon.Caching.InMemory/README.md` | 第 34–43、82–93 行：`AddInMemoryCache(options => { ... })` 及 options 属性不存在；第 62、114 行：`TryGet` 不存在，应改为 Get + CacheValue |
| `src/Tenon.Caching.InMemory/Tenon.Caching.InMemory.csproj` | 无 Description/PackageTags；同层 OpenApi 扩展包在 PropertyGroup 中设 Description；需确认是否补充 |
| `src/Abstractions/Tenon.Caching.Abstractions/Extensions/ServiceCollectionExtension.cs` | `AddCaching(services, setupAction)`：构造 CachingOptions、执行 setupAction、遍历 options.Extensions 调用 AddServices(services) |
| `src/Abstractions/Tenon.Caching.Abstractions/ICacheProvider.cs` | 接口仅有 Get/CacheValue，无 TryGet |
| `samples/CachingSample/Program.cs` | 正确用法：AddInMemoryCache() + Get/CacheValue；可作 README 示例参考（Task 5 将移除 CachingSample） |

### Technical Decisions

- 以“积木式使用”为优先：第三方应能通过最少步骤（安装包 + 一行注册）使用默认行为，进阶时可通过配置或 Options 自定义内存限制等。
- README 与代码一致：示例只使用接口中已有的 Get/CacheValue；若保留“自定义配置”说明，需与即将实现或已实现的扩展签名一致。
- 配置扩展二选一：**选项 A** 仅增加 `AddInMemoryCache(Action<InMemoryCachingOptions>?)`，需新增具名 Options 类（与 MemoryCacheProvider 构造函数参数对应：CacheName、CacheMemoryLimitMegabytes、PhysicalMemoryLimitPercentage、PollingInterval），在扩展内用 options 构造 MemoryCacheProvider；**选项 B** 引入可绑定配置节的 Options 类，在 `UseInMemoryStorage(IConfigurationSection?)` 中绑定并传入 CachingOptionsExtension，扩展内根据 Options 构造 MemoryCacheProvider。Step 3 生成任务时确定选型；若本规格仅做 MVP（README + 元数据），配置能力可标为可选并在实现前定案。
- **调查结论**：当前无 `Tenon.Caching.InMemoryTests` 项目；CachingSample 为唯一引用本包的运行时验证。本规格新增单元测试并移除 CachingSample。

## Implementation Plan

### Tasks

- [x] **Task 1: README 修正**
  - File: `src/Tenon.Caching.InMemory/README.md`
  - Action: 将所有 `TryGet` 示例改为 `Get` + `CacheValue<T>.HasValue`/`Value`（第 62、114 行及“使用缓存服务”示例）；删除或改写“使用自定义配置”中 `AddInMemoryCache(options => { ... })` 为“当前支持无参 `AddInMemoryCache()`；需键控或统一入口时使用 `AddCaching(o => o.UseInMemoryStorage())`”；按约定顺序重组：快速开始（安装 + 一行注册 + 最小 Get/Set）→ 两种注册方式（无参 vs AddCaching + UseInMemoryStorage）→ 高级配置（若有）；配置选项表保留时加注“实现自定义配置后生效”或暂移入“规划中”。
  - Notes: 示例代码须可编译运行；CachingSample 移除后以本 README 与单元测试为示例来源。

- [x] **Task 2: 包元数据与打包**
  - File: `src/Tenon.Caching.InMemory/Tenon.Caching.InMemory.csproj`
  - Action: 在 `<PropertyGroup>` 内补充 `<Description>`（例如：基于 MemoryCache 的内存缓存实现，实现 Tenon.Caching.Abstractions 的 ICacheProvider）；可选 `<PackageTags>tenon;caching;memory;inmemory</PackageTags>`；不在此处写版本（由 version.props 驱动）。
  - Notes: 与同层 `Tenon.AspNetCore.OpenApi.Extensions.csproj` 的 Description 写法一致；确认 Release 下 `dotnet pack` 能产出 .nupkg 且含 README。

- [x] **Task 3（可选）: InMemory 配置能力（选项 A）**
  - Files: 新建 `src/Tenon.Caching.InMemory/Configurations/InMemoryCacheOptions.cs`；修改 `src/Tenon.Caching.InMemory/Extensions/ServiceCollectionExtension.cs`。
  - Action: 新增具名 Options 类 `InMemoryCacheOptions`（属性：CacheName、CacheMemoryLimitMegabytes、PhysicalMemoryLimitPercentage、PollingInterval），与 `MemoryCacheProvider` 构造函数参数对应；在 `ServiceCollectionExtension` 中增加重载 `AddInMemoryCache(this IServiceCollection services, Action<InMemoryCacheOptions>? configureOptions)`，内部构建 Options、用其值构造 `MemoryCacheProvider` 并 `TryAddSingleton<ICacheProvider, MemoryCacheProvider>`。若 `configureOptions == null` 则行为与无参 `AddInMemoryCache()` 一致（无参构造 Provider）。
  - Notes: 类名用 `InMemoryCacheOptions` 与静态扩展类 `InMemoryCachingOptions` 区分，避免混淆。本任务为可选；不做则 README 中不出现带 options 的代码示例。

- [x] **Task 4: 新增对应单元测试**
  - Files: 新建 `test/Tenon.Caching.InMemoryTests/Tenon.Caching.InMemoryTests.csproj` 及若干测试类。
  - Action: 测试项目引用 `src/Tenon.Caching.InMemory` 与 `Tenon.Caching.Abstractions`；添加用例：通过 `AddInMemoryCache()` 解析 `ICacheProvider` 并执行 Set/Get/Remove/Exists 验证；通过 `AddCaching(o => o.UseInMemoryStorage())` 解析 `ICacheProvider` 并执行 Set/Get 验证。若实现 Task 3，增加“带 configureOptions 的 AddInMemoryCache 能解析并写入/读取”的用例。将本包验证从 Sample 迁移到单元测试。
  - Notes: 测试框架与 solution 内他包一致（如 MSTest）；需把新测试项目加入 `src/Tenon.sln`（或当前 solution 所在路径）。

- [x] **Task 5: 移除对应 Sample 示例**
  - Files: 移除 `samples/CachingSample` 目录（或从 solution 中移除 CachingSample 项目引用）；若 solution 或 CI 中有对 CachingSample 的引用则一并更新。
  - Action: 删除 CachingSample 项目及其内容；从 solution 文件中移除对 CachingSample 的引用（若有）。本包的使用验证改由 Task 4 的单元测试承担。
  - Notes: 若仓库约定 samples 必须保留至少一个缓存示例，可改为“将 CachingSample 改为仅引用 Abstractions 的占位示例”并与产品负责人确认；否则按移除处理。

### Acceptance Criteria

- [x] **AC1**: Given 控制台应用已引用 Tenon.Caching.InMemory，When 调用 `services.AddInMemoryCache()` 并 BuildServiceProvider 解析 `ICacheProvider`，Then 成功解析且非 null，且对同一 key 执行 Set 后 Get 返回的 `CacheValue<T>` 的 HasValue 为 true、Value 与写入值一致。
- [x] **AC2**: Given 已引用 Tenon.Caching.Abstractions 与 Tenon.Caching.InMemory，When 调用 `services.AddCaching(o => o.UseInMemoryStorage())` 并解析 `ICacheProvider`，Then 成功解析且 Set/Get 行为与 MemoryCacheProvider 一致（可同 AC1 的断言方式）。
- [x] **AC3**: Given 当前 README 的“快速开始”与示例代码，When 用户按文档复制并在项目中运行，Then 示例仅使用 `Get` 与 `CacheValue<T>`，无 `TryGet`，且可编译并通过基本 Set/Get 验证。
- [x] **AC4**: Given Release 配置，When 对 `src/Tenon.Caching.InMemory` 执行 `dotnet pack`，Then 生成 .nupkg，包内包含 README.md，且包元数据含 Description（若已实现 Task 2）。
- [x] **AC5**: Given 已实现 Task 4，When 运行 `dotnet test` 针对 Tenon.Caching.InMemoryTests，Then 所有新增用例通过，且无依赖 CachingSample 的验证。
- [x] **AC6**: Given 已实现 Task 5，When 打开 solution 并构建，Then 不存在 CachingSample 项目或其对 InMemory 的引用已移除，且构建与打包不受影响。
- [x] **AC7**（若实现 Task 3）: Given 调用了 `AddInMemoryCache(options => { options.CacheMemoryLimitMegabytes = 512; })`，When 解析 `ICacheProvider` 并执行 Set/Get，Then 使用的为具名 MemoryCache 实例（非 Default），且注册与读写正常。

## Additional Context

### Dependencies

- **Tenon.Caching.Abstractions**：ICacheProvider、CachingOptions、ICachingOptionsExtension；本包仅引用 Abstractions，不引用其他实现包。
- **System.Runtime.Caching**：MemoryCache（.NET 9.0.0）。
- **Microsoft.Extensions.DependencyInjection**：扩展方法注册；若实现 Task 3 无需额外 Configuration 包，仅代码配置。
- **其他任务依赖**：Task 1 与 Task 2 无交叉依赖，可并行；Task 3 依赖 MemoryCacheProvider 现有构造函数；Task 4 依赖本包代码，可与 Task 1/2 并行开发；Task 5 在 Task 4 就绪后执行（移除 Sample 后以单元测试为验证手段）。

### Testing Strategy

- **单元/集成**：在 Tenon.Caching.InMemoryTests 中验证 DI 注册（AddInMemoryCache、AddCaching + UseInMemoryStorage）及 Set/Get/Remove/Exists 等与 MemoryCacheProvider 行为一致；若实现 Task 3 则验证带 Options 的注册。
- **手动**：按 README 快速开始复制到新控制台项目验证可编译运行；不再依赖 CachingSample（已移除）。
- **打包**：`dotnet pack src/Tenon.sln -c Release` 确认本包被包含且排除规则不误伤。

### Notes

- **README 更新**：Task 1 已包含完整 README 修正（示例 Get/CacheValue、两种注册方式、结构约定）；交付后文档即更新完毕。
- **Sample 移除**：CachingSample 移除后，本包使用方式以 README 与单元测试为唯一验证与示例来源。
- **后续考虑**：若需配置节绑定（选项 B），可在 UseInMemoryStorage(IConfigurationSection?) 中绑定 Options 并传入 CachingOptionsExtension，本规格不实现。

### README 结构约定（Party Mode 建议）

- **顺序**：快速开始（安装 + 一行 `AddInMemoryCache()` + 最小 Get/Set 示例）→ 两种注册方式（无参 vs `AddCaching(o => o.UseInMemoryStorage())`）→ 高级配置（若有）。
- **示例**：一律使用 `Get` + `CacheValue<T>.HasValue`/`Value`，不出现 `TryGet`，便于第三方按文档即可“拼上积木”。
