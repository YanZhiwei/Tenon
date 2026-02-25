# 缓存模块实现现状与问题分析

**文档类型：** 技术分析（现状 + 问题清单）  
**基于规格：** `tech-spec-cache-implementation-analysis.md`  
**日期：** 2025-02-25

---

## 1. 实现结构总览

### 1.1 层次关系

Tenon 缓存模块采用「抽象 + 多实现 + 扩展」的积木式结构，第三方通过引用抽象与所选实现包即可接入，无需关心底层存储细节。

```
                    ┌─────────────────────────────────────┐
                    │   Tenon.Caching.Abstractions        │
                    │   ICacheProvider, CacheValue<T>,    │
                    │   CachingOptions, AddCaching()      │
                    └─────────────────┬───────────────────┘
                                      │
        ┌─────────────────────────────┼─────────────────────────────┐
        │                             │                             │
        ▼                             ▼                             ▼
┌───────────────┐           ┌─────────────────┐           ┌─────────────────────────┐
│ InMemory      │           │ Redis 实现      │           │ 扩展 / 使用方            │
│ MemoryCache   │           │ (同一实现体)     │           │                         │
│ Provider      │           │ RedisCacheProvider│           │ Interceptor.Castle      │
│               │           │                 │           │ Hangfire.Extensions     │
└───────────────┘           └────────┬────────┘           └─────────────────────────┘
                                      │
                    ┌─────────────────┴─────────────────┐
                    │                                   │
                    ▼                                   ▼
        ┌───────────────────────┐           ┌───────────────────────┐
        │ Tenon.Caching.Redis   │           │ Tenon.Caching.        │
        │ (通用：任意 IRedis     │           │ RedisStackExchange    │
        │  Provider)            │           │ (一站式：StackExchange │
        │ 无自带 DI 扩展*       │           │  + RedisCacheProvider)│
        └───────────────────────┘           └───────────────────────┘
```

\* 当前仓库中 `Tenon.Caching.Redis` 仅含 `RedisCacheProvider` 与 README，未提供 `AddRedisCache`/`AddKeyedRedisCache`；测试项目引用的 `Tenon.Caching` 项目在仓库中不存在。

### 1.2 职责边界

| 层级 | 包/组件 | 职责 |
|------|---------|------|
| 抽象 | Tenon.Caching.Abstractions | 定义 `ICacheProvider`、`CacheValue<T>`、`CachingOptions`；提供 `AddCaching(Action<CachingOptions>)`，实现通过 `ICachingOptionsExtension` 注册。 |
| 实现 | Tenon.Caching.InMemory | 基于 `System.Runtime.Caching.MemoryCache` 的 `MemoryCacheProvider`；Options：InMemoryCachingOptions。 |
| 实现 | Tenon.Caching.Redis | 提供 **唯一** Redis 实现体 `RedisCacheProvider`，依赖 `IRedisProvider`（Tenon.Infra.Redis）、`ISerializer`、`CachingOptions`。 |
| 实现 | Tenon.Caching.RedisStackExchange | **无独立 Provider 类**；通过扩展方法注册 `RedisCacheProvider` + `Tenon.Infra.Redis.StackExchangeProvider`，提供 `AddRedisStackExchangeCache` / `AddKeyedRedisStackExchangeCache`。 |
| AOP | Tenon.Caching.Interceptor.Castle | Castle DynamicProxy 实现 Cache-Aside + 延时双删；特性：`CachingAbl`、`CachingEvict`、`CachingParameter`；依赖 `ICacheProvider`、`ICacheKeyGenerator`。 |
| 扩展 | Tenon.Hangfire.Extensions | 定义 `IHangfireCacheProvider : ICacheProvider` 标识接口；Sample 中有 `HangfireMemoryCacheProvider` 实现。 |

### 1.3 两套 Redis 的关系

- **Tenon.Caching.Redis**：通用 Redis 缓存实现包，仅包含 `RedisCacheProvider`，依赖抽象 `IRedisProvider`。需由调用方自行注册 `IRedisProvider`（例如通过其他基础设施包）。
- **Tenon.Caching.RedisStackExchange**：面向 StackExchange.Redis 的「一站式」包，引用 `Tenon.Caching.Redis` 与 `Tenon.Infra.Redis.StackExchangeProvider`，在 DI 中注册 `RedisCacheProvider` + StackExchange 的 `IRedisProvider` 实现。

因此两套 Redis 的**实现体是同一个类**（`RedisCacheProvider`），区别仅在于 **IRedisProvider 的注入方式**（通用 vs 固定 StackExchange）。

---

## 2. 各包现状

### 2.1 Tenon.Caching.Abstractions

- **主要类型**：`ICacheProvider`、`CacheValue<T>`、`CachingOptions`、`ICachingOptionsExtension`；扩展方法 `AddCaching(Action<CachingOptions>)`（位于 `Extensions/ServiceCollectionExtension.cs`）。
- **配置入口**：通过 `AddCaching(setupAction)` 传入 `CachingOptions`（如 KeyedServiceKey、MaxRandomSecond、Prefix）；各实现通过 `options.RegisterExtension(extension)` 注册自身。
- **对外 API**：接口方法包括 Set/Get/Remove/Exists、RemoveAll、KeysExpireAsync（两重载）；`CacheValue<T>` 为只读结构体，提供 HasValue、IsNull、Value 及 Null/NoValue 静态实例。
- **与 README**：README 描述了接口、CacheValue、各实现包链接及基础用法，与代码一致。

### 2.2 Tenon.Caching.InMemory

- **主要类型**：`MemoryCacheProvider`（实现 `ICacheProvider`、`IDisposable`），`InMemoryCachingOptions`；扩展类 `ServiceCollectionExtension` 存在但**当前为空**（无 `AddInMemoryCache` 等公开入口）。
- **配置入口**：无统一 DI 扩展；测试中直接 `AddSingleton<ICacheProvider, MemoryCacheProvider>()` 或通过构造函数传入缓存名、内存限制等。
- **对外 API**：与 `ICacheProvider` 一致；内部使用 `System.Runtime.Caching.MemoryCache`，支持默认实例或自定义配置实例。
- **与 README**：README 描述功能与用法；与「开箱即用」的差距：**缺少类似 AddRedisStackExchangeCache 的 AddInMemoryCache 扩展**，第三方需自行构造并注册 Provider。

### 2.3 Tenon.Caching.Redis

- **主要类型**：`RedisCacheProvider`（sealed，实现 `ICacheProvider`），依赖 `IRedisProvider`、`ISerializer`、`CachingOptions`；键名通过 `CachingOptions.Prefix` 重写，过期支持 MaxRandomSecond 抖动。
- **配置入口**：本包内无 DI 扩展；由 Tenon.Caching.RedisStackExchange 或其它提供 IRedisProvider 的包负责注册 `RedisCacheProvider`。
- **对外 API**：与接口一致；**已知问题**：`KeysExpireAsync(IEnumerable<string> cacheKeys, TimeSpan expiration)` 方法体为空，未实现（见问题清单）。
- **与 README**：README 描述为「Redis 缓存抽象层实现」、支持多种 Redis 客户端，与当前「仅实现体 + 无自带扩展」一致。

### 2.4 Tenon.Caching.RedisStackExchange

- **主要类型**：无独立 Provider 类；`Extensions/ServiceCollectionExtension.cs` 提供 `AddRedisStackExchangeCache`、`AddKeyedRedisStackExchangeCache`；配置类 `RedisStackExchangeCachingOptions`、扩展类 `CachingOptionsExtension`、`SerializerOptionsExtension`。
- **配置入口**：`AddRedisStackExchangeCache(IConfigurationSection redisSection, Action<CachingOptions>? setupAction)`；内部注册 StackExchange 的 Redis 实现、序列化、`RedisCacheProvider`。
- **对外 API**：通过注册的 `ICacheProvider` 暴露，与抽象一致。
- **与 README**：README 描述基于 StackExchange.Redis、安装与快速入门，与代码一致；第三方选型时可直接使用本包作为「一站式」Redis 缓存。

### 2.5 Tenon.Caching.Interceptor.Castle

- **主要类型**：`CacheAsideAsyncInterceptor`、`DefaultCacheKeyGenerator`（实现 `ICacheKeyGenerator`）、特性 `CachingAblAttribute`、`CachingEvictAttribute`、`CachingParameterAttribute`；配置 `CacheAsideInterceptorOptions`；扩展 `CachingInterceptorExtension`。
- **配置入口**：通过 `AddCachingInterceptor(Action<CacheAsideInterceptorOptions>)` 等注册拦截器与选项；需配合 Castle 动态代理使用。
- **对外 API**：以特性标注方法实现 Cache-Aside 与 Evict；缓存键由 `ICacheKeyGenerator` 生成，可替换默认实现。
- **与 README**：README 描述 Cache-Aside、延时双删、特性用法与自定义键生成器，与代码一致。

### 2.6 Hangfire 扩展（缓存相关）

- **主要类型**：`IHangfireCacheProvider : ICacheProvider`（标识接口）；Sample 中 `HangfireMemoryCacheProvider` 实现该接口并用于登录尝试追踪等场景。
- **配置入口**：依赖 Hangfire 与缓存扩展的注册方式；需单独注册实现 `IHangfireCacheProvider` 的类。
- **对外 API**：与 `ICacheProvider` 相同；标识接口便于 Hangfire 相关功能单独注入缓存实现。

---

## 3. 单元测试覆盖现状与缺口

### 3.1 现有测试项目与范围

| 测试项目 | 框架 | 覆盖范围 | 备注 |
|----------|------|----------|------|
| Tenon.Caching.InMemoryTests | MSTest | MemoryCacheProvider：Set/Get、Remove、Exists、RemoveAll、KeysExpireAsync 等 | 直接注册 `MemoryCacheProvider`，不依赖 Redis；可本地运行。 |
| Tenon.Caching.RedisTests | MSTest | 测试中调用 `AddRedisCache`、`AddKeyedRedisCache` | **依赖不存在的 Tenon.Caching 项目**，引用命名空间 `Tenon.Caching.Extensions`、`Tenon.Caching.Redis.Extensions`；当前仓库无该 csproj，构建可能失败或未包含在解决方案中。 |
| Tenon.Caching.RedisStackExchangeTests | MSTest | AddRedisStackExchangeCache / AddKeyedRedisStackExchangeCache 注册后的 ICacheProvider：Set/Get、Remove、Exists、过期等 | 需 appsettings.json 中 Redis 连接；集成测试，需 Redis 可用。 |
| Tenon.Caching.Interceptor.CastleTests | MSTest | DefaultCacheKeyGenerator（GetCacheKeyTest）、ITestService 相关 | 部分用例含 **Assert.Fail()** 未实现（如 GetCacheKeysTest）；需 appsettings 时可能依赖 Redis。 |

### 3.2 缺口汇总

- **RedisTests 项目**：引用 `Tenon.Caching` 与 `Tenon.Caching.Redis.Extensions`，仓库中无对应项目，AddRedisCache/AddKeyedRedisCache 来源不明；若解决方案包含该测试项目，构建会报错。
- **RedisCacheProvider 单测**：无仅针对 `Tenon.Caching.Redis.RedisCacheProvider` 的单元测试（不通过 Redis）；仅通过 RedisStackExchangeTests 的集成测试间接覆盖，难以在无 Redis 环境下验证逻辑（如键前缀、序列化、KeysExpireAsync 重载等）。
- **Interceptor 测试**：GetCacheKeysTest 等用例存在 `Assert.Fail()`，未完成实现，无法作为稳定验收依据。
- **InMemory**：测试覆盖较好，但缺少通过「统一 DI 扩展」注册的测试（因当前无 AddInMemoryCache）。
- **区分**：需 Redis 的集成测试（RedisStackExchangeTests、部分 RedisTests）与可本地运行的单元测试（InMemoryTests、部分 Interceptor）混在不同项目中，CI 需区分或提供 Redis 环境。

---

## 4. 对外使用文档现状与缺口

### 4.1 现状

- **各包 README**：Abstractions、InMemory、Redis、RedisStackExchange、Interceptor 均有 README，包含安装方式、核心 API 说明、示例代码（如 Get/Set、特性用法、自定义键生成器）。
- **目标读者**：以开发者为主，能理解 NuGet 引用与代码片段；Abstractions README 中列出了各实现包链接，便于选型。
- **Hangfire**：Hangfire 扩展有 README；是否明确说明 `IHangfireCacheProvider` 的注册与使用方式需在具体文档中核对。

### 4.2 缺口（第三方集成体验）

- **无统一「缓存选型与快速开始」入口**：缺少一份总览文档（如 docs 或根 README）说明「何时用 InMemory / Redis / RedisStackExchange」、最小依赖与一行式注册示例，第三方需逐个打开各包 README。
- **无独立 Caching Sample**：samples 下无专门演示「仅缓存」的示例项目；仅 HangfireSample 内使用缓存（如 HangfireMemoryCacheProvider），未覆盖 RedisStackExchange 或纯 InMemory 的快速体验。
- **API 文档**：无 XML 文档站或集中 API 列表；仅靠 README 片段，复杂重载（如 KeysExpireAsync 两重载）行为需看源码。
- **配置与 Options**：各包 Options（CachingOptions、InMemoryCachingOptions、Redis 配置节点）的配置节名称、环境变量等未在统一位置说明，第三方需翻阅代码或各 README。
- **与「积木」定位的差距**：从「像积木一样方便第三方构建产品、不关心底层」的角度，缺少「我只要缓存，三步接入」的引导（包选择 → 安装 → 注册 + 配置），文档分散在各包。

---

## 5. 问题与风险清单

以下问题均可追溯至具体文件或包，便于修复与排期。

| # | 问题描述 | 位置/依据 | 类别 |
|---|----------|-----------|------|
| 1 | `RedisCacheProvider.KeysExpireAsync(IEnumerable<string> cacheKeys, TimeSpan expiration)` 方法体为空，未实现。 | `src/Tenon.Caching.Redis/RedisCacheProvider.cs` 对应重载方法体仅有空行。 | 实现缺陷 |
| 2 | Tenon.Caching.RedisTests 引用不存在的 `Tenon.Caching` 项目；使用的 `AddRedisCache`、`AddKeyedRedisCache` 来源不明，可能导致构建失败。 | `test/Tenon.Caching.RedisTests/Tenon.Caching.RedisTests.csproj` 引用 `Tenon.Caching`；`RedisStackExchangeCacheProviderTests.cs` 使用上述扩展。 | 工程/依赖 |
| 3 | Tenon.Caching.InMemory 的 ServiceCollectionExtension 为空，无 `AddInMemoryCache` 等统一 DI 入口，第三方需手动构造并注册 MemoryCacheProvider。 | `src/Tenon.Caching.InMemory/Extensions/ServiceCollectionExtension.cs` 类体为空。 | 易用性 |
| 4 | Interceptor 测试中 GetCacheKeysTest 含 `Assert.Fail()`，未完成实现。 | `test/Tenon.Caching.Interceptor.CastleTests/DefaultCacheKeyBuilderTests.cs` 中 GetCacheKeysTest。 | 测试 |
| 5 | 无独立「缓存使用」Sample，仅 HangfireSample 内使用缓存，第三方无法快速体验纯缓存接入。 | `samples/` 下无 CachingSample；HangfireSample 内含缓存相关代码。 | 文档/示例 |
| 6 | 无统一缓存选型与快速开始文档，第三方需逐个查看各包 README。 | docs/ 与各包 README 中无总览式「缓存选型 + 三步接入」文档。 | 文档 |
| 7 | RedisCacheProvider 无独立单元测试（仅通过 RedisStackExchange 集成测试覆盖），无 Redis 环境下难以验证键前缀、序列化、KeysExpireAsync 等行为。 | 无 `Tenon.Caching.RedisTests` 中仅针对 RedisCacheProvider 的 mock 单测。 | 测试 |

### 5.2 第三方集成体验维度（补充）

- **API 一致性**：ICacheProvider 各实现 API 一致；但 KeysExpireAsync 两重载在 Redis 实现中一处未实现，导致行为不一致。
- **可发现性**：RedisStackExchange 与 Interceptor 的扩展方法易发现；InMemory 无扩展，需知悉 MemoryCacheProvider 类名。
- **配置复杂度**：RedisStackExchange 需配置连接串；InMemory 若未来提供 AddInMemoryCache，可收敛配置方式，当前较分散。

### 5.3 风险与备注

- **RedisTests 对 Tenon.Caching 的引用**：若 CI 未编译该测试项目，问题可能被掩盖；建议在解决方案中确认该测试项目状态。
- **KeysExpireAsync 空实现**：若调用方少，可能长期未暴露；建议静态扫描或补充调用该重载的测试。
- 本清单仅描述现状与缺口，不排优先级或迭代计划；具体修复与改进需另开任务或规格。

---

**文档结束。** 若需基于本分析做改进任务拆分、补单测、补 Sample 或统一 AddInMemoryCache，建议另开技术规格或迭代计划。
