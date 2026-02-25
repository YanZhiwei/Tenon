---
title: '缓存模块实现现状与问题分析'
slug: 'cache-implementation-analysis'
created: '2025-02-25'
status: 'Completed'
stepsCompleted: [1, 2, 3, 4]
tech_stack: ['.NET 9', 'C#', 'Tenon.Caching.Abstractions', 'System.Runtime.Caching', 'IRedisProvider/StackExchange.Redis', 'Castle.Core', 'MSTest']
files_to_modify: ['src/Abstractions/Tenon.Caching.Abstractions/*', 'src/Tenon.Caching.InMemory/*', 'src/Tenon.Caching.Redis/*', 'src/Tenon.Caching.RedisStackExchange/*', 'src/Tenon.Caching.Interceptor.Castle/*', 'src/Extensions/Tenon.Hangfire.Extensions/Caching/*', 'test/Tenon.Caching.*Tests/*']
code_patterns: ['抽象优先 ICacheProvider', 'Options + AddXxx 扩展注册', 'sealed 实现类', 'CacheValue<T> 只读结构体', '两套 Redis 实为同一 RedisCacheProvider+不同 IRedisProvider 注入']
test_patterns: ['MSTest', '部分需 appsettings/Redis', 'InMemory 直接 new MemoryCacheProvider', 'RedisStackExchange 用 AddRedisStackExchangeCache']
---

# Tech-Spec: 缓存模块实现现状与问题分析

**Created:** 2025-02-25

## Overview

### Problem Statement

需要系统梳理 Tenon 缓存相关实现（抽象、多实现、拦截器与扩展），明确现状与存在的问题，为后续优化或重构提供依据。从 Tenon 作为「积木」的定位出发，需评估缓存模块是否让第三方在构建产品时能**开箱即用、无需关心底层缓存实现**，分析时需兼顾该集成体验。

### Solution

通过代码调查与结构化文档，产出一份技术规格级分析文档，包含实现结构、现状结论与问题清单。

### Scope

**In Scope:**
- 抽象层与各实现包（含 **Tenon.Caching.Redis** 与 **Tenon.Caching.RedisStackExchange** 两套 Redis 实现）
- Castle 拦截器（Tenon.Caching.Interceptor.Castle）
- 与缓存相关的扩展（如 Hangfire）
- **单元测试**：覆盖现状与缺口（各包/关键 API 是否有对应单元测试）
- **对外使用文档**：README、示例、API 文档等现状与缺口
- 现状描述
- 问题与风险清单

**Out of Scope:**
- 不包含具体代码重构或新功能实现
- 不包含可执行的改进任务/优先级
- 不包含性能压测或容量规划

## Context for Development

### Codebase Patterns

- 遵循 `_bmad-output/project-context.md`：.NET 9、Nullable、抽象优先、Options 模式、异步命名与 ConfigureAwait(false) 等。
- **抽象层**：`Tenon.Caching.Abstractions` 提供 `ICacheProvider`、`CacheValue<T>`、`CachingOptions`、`AddCaching(Action<CachingOptions>)`；实现通过 `ICachingOptionsExtension` 注册。
- **实现包**：InMemory（System.Runtime.Caching）、Redis（依赖 `Tenon.Infra.Redis` 的 `IRedisProvider`）、RedisStackExchange（无独立 Provider 类，复用 `Tenon.Caching.Redis.RedisCacheProvider` + `Tenon.Infra.Redis.StackExchangeProvider` 的 DI 扩展）。
- **两套 Redis**：Tenon.Caching.Redis = 通用 Redis 实现（任意 IRedisProvider）；Tenon.Caching.RedisStackExchange = 一站式扩展（AddRedisStackExchangeCache），内部仍注册 `RedisCacheProvider`。
- **拦截器**：Castle DynamicProxy，Cache-Aside + 延时双删，特性 `CachingAbl`/`CachingEvict`/`CachingParameter`。
- **Hangfire**：`IHangfireCacheProvider : ICacheProvider` 标识接口，无额外行为；Sample 中有 `HangfireMemoryCacheProvider` 实现。

### Files to Reference

| File / 目录 | Purpose |
| ----------- | ------- |
| src/Abstractions/Tenon.Caching.Abstractions/ | 抽象接口、CacheValue、CachingOptions、AddCaching 扩展 |
| src/Tenon.Caching.InMemory/ | MemoryCacheProvider、InMemoryCachingOptions；ServiceCollectionExtension 当前为空 |
| src/Tenon.Caching.Redis/RedisCacheProvider.cs | 唯一 Redis 实现体；依赖 IRedisProvider、ISerializer、CachingOptions |
| src/Tenon.Caching.RedisStackExchange/ | 仅扩展与配置，无独立 Provider；引用 Tenon.Caching.Redis + StackExchangeProvider |
| src/Tenon.Caching.Interceptor.Castle/ | CacheAsideAsyncInterceptor、DefaultCacheKeyGenerator、特性与配置 |
| src/Extensions/Tenon.Hangfire.Extensions/Caching/ | IHangfireCacheProvider、LoginAttemptTracker 等使用缓存处 |
| test/Tenon.Caching.InMemoryTests/ | MemoryCacheProvider 单元测试（MSTest） |
| test/Tenon.Caching.RedisTests/ | 引用 **Tenon.Caching** 项目（仓库中未见该 csproj），调用 AddRedisCache/AddKeyedRedisCache |
| test/Tenon.Caching.RedisStackExchangeTests/ | AddRedisStackExchangeCache 集成测试 |
| test/Tenon.Caching.Interceptor.CastleTests/ | DefaultCacheKeyGenerator 等；部分用例含 Assert.Fail() 未实现 |
| 各包 README.md | Abstractions、InMemory、Redis、RedisStackExchange、Interceptor 均有 README |

### Technical Decisions

- 产出形式：现状 + 问题清单；两套 Redis 实现均纳入分析。
- 分析视角包含**第三方集成体验**：API 一致性、包与扩展的可发现性、配置复杂度、与日志等其它基础设施积木的协同方式。
- **调查结论（供 Step 3 写入问题清单）**：① `RedisCacheProvider.KeysExpireAsync(IEnumerable<string>, TimeSpan)` 方法体为空，未实现。② Tenon.Caching.RedisTests 引用不存在的 `Tenon.Caching` 项目（AddRedisCache/AddKeyedRedisCache 来源不明），可能导致构建失败。③ Tenon.Caching.InMemory 的 ServiceCollectionExtension 为空，无 AddInMemoryCache 等统一入口。④ Interceptor 测试中 GetCacheKeysTest 含 Assert.Fail()，未完成。⑤ 无独立“缓存使用”Sample，仅 HangfireSample 内使用缓存。

## Implementation Plan

### Tasks

- [x] Task 1: 编写「实现结构总览」章节
  - File: 分析文档（建议 `_bmad-output/implementation-artifacts/cache-implementation-analysis.md` 或并入本 WIP 终稿）
  - Action: 基于 Context 的 Codebase Patterns 与 Files to Reference，写出抽象层 → 各实现包（InMemory / Redis / RedisStackExchange）/ 拦截器 / Hangfire 扩展的层次关系与职责；明确两套 Redis 实为同一 RedisCacheProvider + 不同 IRedisProvider 注入。
  - Notes: 可配简单结构图（如 Mermaid）或列表，便于第三方理解「积木」边界。

- [x] Task 2: 编写「各包现状」章节
  - File: 同上分析文档
  - Action: 按 Abstractions、InMemory、Redis、RedisStackExchange、Interceptor.Castle、Hangfire 扩展分别描述：主要类型、配置入口（Options/AddXxx）、对外 API 表面；标注 InMemory ServiceCollectionExtension 当前为空、RedisStackExchange 无独立 Provider 类。
  - Notes: 与 README 现有描述对齐，突出与「开箱即用」的差距（若有）。

- [x] Task 3: 编写「单元测试覆盖现状与缺口」章节
  - File: 同上分析文档
  - Action: 列出各测试项目（InMemoryTests、RedisTests、RedisStackExchangeTests、Interceptor.CastleTests）及其覆盖范围；标明缺口：RedisTests 依赖不存在的 Tenon.Caching 项目、Interceptor 中 GetCacheKeysTest 等未完成用例、缺少对 RedisCacheProvider 单测（仅通过 RedisStackExchange 集成测试覆盖）等。
  - Notes: 区分「需 Redis 的集成测试」与「可本地运行的单元测试」。

- [x] Task 4: 编写「对外使用文档现状与缺口」章节
  - File: 同上分析文档
  - Action: 汇总各包 README 内容与目标读者；指出缺口：无统一「缓存选型与快速开始」入口、无独立 Caching Sample、Hangfire 文档是否说明 IHangfireCacheProvider 使用方式等；从第三方集成体验角度列出「缺少的文档」。
  - Notes: 与 project-context 中「积木」定位一致。

- [x] Task 5: 编写「问题与风险清单」章节
  - File: 同上分析文档
  - Action: 将 Step 2 调查结论整理为可核对的问题清单：① RedisCacheProvider.KeysExpireAsync(keys, expiration) 未实现；② RedisTests 引用缺失的 Tenon.Caching 项目；③ InMemory 无 AddInMemoryCache 等统一 DI 入口；④ Interceptor 测试未完成用例；⑤ 无独立缓存 Sample。补充「第三方集成体验」维度问题（API 一致性、可发现性、配置复杂度等，若调查有则写入）。
  - Notes: 每项需可追溯至具体文件/行或包名，便于后续修复或排期。

### Acceptance Criteria

- [ ] AC 1: Given 已完成 Step 2 代码调查，when 阅读分析文档的「实现结构总览」，then 能明确缓存模块的抽象层、各实现包、两套 Redis 的关系及拦截器/扩展的职责边界。
- [ ] AC 2: Given 分析文档中的「各包现状」，when 按包查阅，then 能获知各包主要类型、配置方式及与 README 的对应关系。
- [ ] AC 3: Given 分析文档中的「单元测试覆盖现状与缺口」，when 按清单核对仓库，then 每项缺口可在对应测试项目或缺失引用中得到验证。
- [ ] AC 4: Given 分析文档中的「对外使用文档现状与缺口」，when 以第三方集成者视角阅读，then 能识别当前文档可支撑的与缺失的（快速开始、Sample、API 说明等）。
- [ ] AC 5: Given 分析文档中的「问题与风险清单」，when 逐项与代码/解决方案核对，then 每项问题可定位到具体文件或包，无模糊表述。
- [ ] AC 6: Given 文档全文，when 新读者（或新上下文中的 Agent）阅读，then 无需回看 Step 1/2 对话即可理解现状与问题，满足「自包含」的 Ready for Development 标准。

## Additional Context

### Dependencies

- 无外部服务或运行时依赖；依赖 Step 2 调查结果、`_bmad-output/project-context.md` 及当前仓库源码与测试结构。
- 若将分析文档单独输出为 md 文件，依赖 `_bmad-output/implementation-artifacts/` 目录存在。

### Testing Strategy

- 本规格产出为分析文档，不涉及代码测试实现。
- 建议通过「文档评审」与「与代码/解决方案逐项核对」验证问题清单与现状描述的准确性；可任选一项 AC 做抽样核对。

### Notes

- 当前已知可能缺失：**对应单元测试**、**对外使用文档**；分析时需重点核查并列入问题清单。
- **高风险/易遗漏**：RedisTests 对 Tenon.Caching 的引用若在 CI 中未编译该测试项目可能被掩盖；KeysExpireAsync 空实现若调用方少可能长期未暴露。
- **已知限制**：本规格不包含具体代码修复或新功能实现，问题清单仅描述现状与缺口，不排优先级或迭代计划。
- **后续可做（Out of Scope）**：基于本分析做改进任务拆分、补单元测试、补 Caching Sample、统一 AddInMemoryCache 等，需另开规格或迭代。

## Review Notes

- Adversarial review completed
- Findings: 10 total, 0 fixed, 10 skipped
- Resolution approach: skip
