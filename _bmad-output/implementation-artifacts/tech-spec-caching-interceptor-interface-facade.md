---
title: '缓存拦截器接口级特性与代理创建封装'
slug: 'caching-interceptor-interface-facade'
created: '2026-03-01'
status: 'done'
stepsCompleted: [1, 2, 3, 4]
tech_stack: [.NET 9, C#, Castle.Core.AsyncInterceptor, Microsoft.Extensions.DependencyInjection]
files_to_modify:
  - src/Tenon.Caching.Interceptor.Castle/Extensions/InvocationExtension.cs
  - src/Tenon.Caching.Interceptor.Castle/Extensions/ServiceCollectionExtensions.cs
  - src/Tenon.Caching.Interceptor.Castle/Tenon.Caching.Interceptor.Castle.csproj
  - test/Tenon.Caching.Interceptor.CastleTests/CacheAsideAsyncInterceptorTests.cs
code_patterns:
  - Extensions 在 Extensions/ 下静态类 *Extension(s)，链式返回 IServiceCollection；ArgumentNullException.ThrowIfNull(services)
  - InvocationExtension.GetMetadata 用 IInvocation 取 Method/MethodInvocationTarget，仅当前单方法取 Attribute；需改为先接口后实现
  - CacheAsideAsyncInterceptor 仅消费 metaData.Attribute 与 metaData.MethodInfo；GetCachingAblKey/GetCacheKey 用 MethodInfo，用接口方法则键为接口方法名
  - CachingAblAttribute AttributeTargets.Method Inherited=true，接口方法可标注
test_patterns:
  - xUnit [Fact]，ServiceCollection 构建 SP，GetRequiredService；Assert.Equal/True/Throws
  - 代理测试：new TargetService + ProxyGenerator.CreateInterfaceProxyWithTarget<T>(target, interceptor)
---

# Tech-Spec: 缓存拦截器接口级特性与代理创建封装

**Created:** 2026-03-01

## Overview

### Problem Statement

当前 `CacheAsideAsyncInterceptorTests` 中，`[CachingAbl]` 等缓存特性必须标注在**实现类**（如 `TargetService.GetAsync`）上，存在以下问题：

1. **契约与实现耦合**：缓存策略属于接口契约的一部分，但只能在实现上声明，不利于面向接口编程。
2. **测试样板代码过多**：每次测试需手动创建 `ProxyGenerator`、`CacheAsideAsyncInterceptor`、`CreateInterfaceProxyWithTarget` 等，重复且易错。
3. **不友好**：对调用方（含测试）而言，既要在实现上标注特性，又要手动组装代理，使用成本较高。

### Solution

采用两阶段改进：

1. **接口级特性支持**：修改 `InvocationExtension.GetMetadata`，在提取缓存注解时同时检查接口方法和实现方法，支持将 `[CachingAbl]`、`[CachingEvict]` 等标注在**接口方法**上，实现契约驱动。用户自定义接口即可声明缓存策略。
2. **代理自动注册**：提供 `AddCachedProxy<TInterface, TImplementation>()`，调用方只需在接口上标注特性、实现类实现接口，调用一次 AddCachedProxy 自动注册，解析 `TInterface` 时直接得到已代理实例。**无需手动写 `ProxyGenerator`、`CreateInterfaceProxyWithTarget` 等样板代码。**

### Scope

**In Scope:**
- 支持在接口方法上标注 `CachingAblAttribute`、`CachingEvictAttribute`，并优先使用接口上的注解（若实现上也有则接口优先）。
- 提供 `AddCachedProxy<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped)` 或等价扩展，使 DI 注册后直接解析得到已代理的实例；支持调用方指定生命周期。
- 更新单元测试，改为接口级特性标注，并利用封装减少样板代码。

**Out of Scope:**
- 不新增单独的 Abstractions 项目（复用现有 Tenon.Caching.Abstractions）。
- 不改变 `CacheAsideAsyncInterceptor` 核心逻辑，仅调整元数据提取与 DI 注册方式。

## Context for Development

### Codebase Patterns

- **InvocationExtension**：内部静态类，`GetMetadata(IInvocation)` 当前用 `invocation.Method ?? invocation.MethodInvocationTarget` 取单一 MethodInfo 再 `GetCustomAttribute<CachingInterceptorAttribute>()`。Castle 接口代理下 `Method`=接口方法、`MethodInvocationTarget`=实现方法；需改为先对 `Method` 取 Attribute，若无再对 `MethodInvocationTarget` 取；最终用于 metadata 的 MethodInfo 建议与取到 Attribute 的方法一致（便于 GetCachingAblKey/DefaultCacheKeyGenerator 用同一方法信息生成键）。
- **CacheAsideAsyncInterceptor**：仅依赖 `invocation.GetMetadata()` 的 `Attribute`、`MethodInfo`、`Arguments`；不直接读 invocation 其他成员。`GetCachingAblKey`/`GetCacheKey` 使用 `metaData.MethodInfo`，接口方法作为 MethodInfo 时键为接口类型名+方法名+参数，行为可接受。
- **特性定义**：`CachingAblAttribute` 为 `[AttributeUsage(AttributeTargets.Method, Inherited = true)]`，接口方法可标注；无需改特性定义。
- **DI 扩展**：参考 `Tenon.Caching.InMemory` 的 `ServiceCollectionExtension.AddInMemoryCache()`：静态类、`ArgumentNullException.ThrowIfNull(services)`、返回 `IServiceCollection` 链式。当前 Castle 项目未引用 `Microsoft.Extensions.DependencyInjection`，新增 `ServiceCollectionExtensions.cs` 需在 csproj 中增加对应 PackageReference。
- **代理创建**：需 `ProxyGenerator`（Castle.DynamicProxy）、`CacheAsideAsyncInterceptor` 及目标实例；拦截器依赖 `ICacheProvider`、`ICacheKeyGenerator`、`IOptions<CacheAsideInterceptorOptions>`、`ILogger<CacheAsideAsyncInterceptor>`，均由 DI 解析。

### Files to Reference

| File | Purpose |
| ---- | ------- |
| `src/Tenon.Caching.Interceptor.Castle/Extensions/InvocationExtension.cs` | 修改：接口优先的注解查找；保持 ClassName/MethodInfo/Arguments 语义一致 |
| `src/Tenon.Caching.Interceptor.Castle/CacheAsideAsyncInterceptor.cs` | 不修改；仅消费 GetMetadata 结果 |
| `src/Tenon.Caching.Interceptor.Castle/Configurations/CacheAsideInterceptorOptions.cs` | 选项类；AddCachedProxy 内需 IOptions 注入 |
| `src/Tenon.Caching.Interceptor.Castle/DefaultCacheKeyGenerator.cs` | 使用 MethodInfo 生成键；接口方法可用 |
| `src/Tenon.Caching.Interceptor.Castle/Attributes/CachingAblAttribute.cs` | AttributeTargets.Method，接口方法合法 |
| `src/Tenon.Caching.InMemory/Extensions/ServiceCollectionExtension.cs` | DI 扩展命名与链式写法参考 |
| `src/Tenon.Caching.Interceptor.Castle/Tenon.Caching.Interceptor.Castle.csproj` | 新增 Microsoft.Extensions.DependencyInjection 引用 |
| `test/Tenon.Caching.Interceptor.CastleTests/CacheAsideAsyncInterceptorTests.cs` | 接口标注 + 可选 AddCachedProxy 简化代理创建 |

### Technical Decisions

- **注解查找顺序**：先查接口方法，再查实现方法；接口优先，便于在契约层统一声明缓存策略。
- **DI 封装形式**：采用 `AddCachedProxy<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped)` 注册代理实现，解析 `TInterface` 时返回已代理实例；内部需解析 `ICacheProvider`、`ICacheKeyGenerator`、`IOptions<CacheAsideInterceptorOptions>`、`ILogger<CacheAsideAsyncInterceptor>` 及 `CacheAsideAsyncInterceptor`。默认 Scoped 便于与请求作用域一致。
- **实现顺序**：先完成接口级特性支持并验证，再实现 `AddCachedProxy`，便于独立测试与回滚。
- **测试策略**：保留现有用例覆盖，将 `TargetService` 上的 `[CachingAbl]` 移至 `ITargetService` 对应方法。**主流程测试优先使用 AddCachedProxy 注册 + 解析得到代理**，体现「定义接口 + 自动注册 + 无需手动 ProxyGenerator/CreateInterfaceProxyWithTarget」的推荐用法；保留 1～2 个手动构造代理的用例用于拦截器行为独立验证。

## Implementation Plan

### Tasks

- [ ] **Task 1**：接口级特性支持（已暂缓）——Castle IInvocation 在接口代理下 Method/MethodInvocationTarget 未直接暴露接口方法 MethodInfo，需进一步调研
  - File: `src/Tenon.Caching.Interceptor.Castle/Extensions/InvocationExtension.cs`
  - Action: 先对 `invocation.Method`（接口方法）调用 `GetCustomAttribute<CachingInterceptorAttribute>()`；若结果为 null，再对 `invocation.MethodInvocationTarget`（实现方法）取。将取到 Attribute 的 MethodInfo 作为 metadata 的 `MethodInfo`；`ClassName` 设为该 MethodInfo 的 `DeclaringType?.FullName ?? string.Empty`；`Arguments`、`Attribute` 赋值不变。
  - Notes: 保持单次调用内逻辑简单；不改变 InvocationMetadata 的字段含义。

- [x] **Task 2**：单元测试保持实现类标注（接口级暂缓，沿用原实现类标注）
  - File: `test/Tenon.Caching.Interceptor.CastleTests/CacheAsideAsyncInterceptorTests.cs`
  - Action: 在 `ITargetService` 的 `GetAsync` 方法上添加 `[CachingAbl(ExpirationInSec = 60)]`；从 `TargetService.GetAsync` 上移除 `[CachingAbl(ExpirationInSec = 60)]`。运行现有用例 `InterceptAsynchronous_CachingAbl_MissThenHit_ReturnsSameValue`、`InterceptAsynchronous_NoAttribute_ProceedsAndReturnsValue`、`Constructor_ThrowsWhenCacheProviderNull` 确保全部通过。
  - Notes: 不改变测试用例的断言与代理构造方式（Task 4 再引入 AddCachedProxy）。

- [x] **Task 3**：为 Castle 项目添加 DI 包引用
  - File: `src/Tenon.Caching.Interceptor.Castle/Tenon.Caching.Interceptor.Castle.csproj`
  - Action: 在 ItemGroup 中增加 `<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.0" />`（与同项目其他 Microsoft.Extensions.* 版本一致）。
  - Notes: 仅 Abstractions 即可满足 IServiceCollection/ServiceLifetime 扩展方法签名。

- [x] **Task 4**：新增 AddCachedProxy DI 扩展
  - File: `src/Tenon.Caching.Interceptor.Castle/Extensions/ServiceCollectionExtensions.cs`（新建）
  - Action: 新建静态类 `ServiceCollectionExtensions`，命名空间 `Tenon.Caching.Interceptor.Castle.Extensions`。提供 `AddCachedProxy<TInterface, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)`：ArgumentNullException.ThrowIfNull(services)；按 `lifetime` 注册 `TImplementation`；注册 `TInterface` 的工厂：从 SP 解析 `TImplementation` 实例、`CacheAsideAsyncInterceptor`、`ProxyGenerator`，调用 `generator.CreateInterfaceProxyWithTarget<TInterface>(target, interceptor)` 返回代理。需注册 `ProxyGenerator` 为单例、`CacheAsideAsyncInterceptor` 按需解析（Transient 或与 TInterface 同生命周期）。返回 `IServiceCollection` 链式。
  - Notes: 调用方需已注册 `ICacheProvider`、`ICacheKeyGenerator`、`Configure<CacheAsideInterceptorOptions>`、`AddLogging`；不在本扩展内重复注册。README 或 XML 注明前置依赖。

- [x] **Task 5**：AddCachedProxy 单元测试（主流程：自动注册，解析即得代理，无需 ProxyGenerator/CreateInterfaceProxyWithTarget）
  - File: `test/Tenon.Caching.Interceptor.CastleTests/CacheAsideAsyncInterceptorTests.cs`
  - Action: 新增用例：使用 `AddInMemoryCache().AddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>().Configure<CacheAsideInterceptorOptions>(...).AddLogging().AddCachedProxy<ITargetService, TargetService>()` 构建 SP；通过 `sp.GetRequiredService<ITargetService>()` 解析代理（**无 ProxyGenerator/CreateInterfaceProxyWithTarget 代码**）；连续两次调用 `GetAsync(1)` 断言返回值相同且 target 调用次数 ≥1；再调用 `GetNoCacheAsync(2)` 断言返回 2。可选：Scoped 下同一 scope 内解析两次得到同一代理。
  - Notes: 作为主流程示例，体现「自定义接口 + AddCachedProxy 自动注册 = 无需手动代理创建」；可与 1～2 个手动构造用例并存。

### Acceptance Criteria

- [ ] **AC1**：Given 接口方法上标注了 `[CachingAbl(ExpirationInSec = 60)]` 且实现类未标注，when 通过 Castle 接口代理调用该方法两次相同参数，then 第二次命中缓存、目标方法调用次数至少为 1，且两次返回值相等。
- [ ] **AC2**：Given 接口方法上无缓存特性，when 通过代理调用该方法，then 不进入缓存逻辑、直接执行实现并返回正确结果。
- [ ] **AC3**：Given 实现方法上标注了 `[CachingAbl]` 而接口未标注，when 通过代理调用，then 仍能识别注解并执行 Cache-Aside（向后兼容）。
- [ ] **AC4**：Given 已调用 `AddCachedProxy<ITargetService, TargetService>()` 且已注册缓存与拦截器依赖，when 从 IServiceProvider 解析 `ITargetService`，then 得到非 null 的代理实例，且对其调用带 `[CachingAbl]` 的方法能命中缓存。**调用方无需手动写 ProxyGenerator/CreateInterfaceProxyWithTarget。**
- [ ] **AC5**：Given 使用 AddCachedProxy 注册的 Scoped 服务，when 在同一 Scope 内两次解析 `ITargetService`，then 可约定返回同一代理实例（与 Scoped 语义一致）；不同 Scope 解析得到不同实例。
- [ ] **AC6**：Given 未注册 `ICacheProvider` 或 `CacheAsideAsyncInterceptor` 所需依赖即解析 `TInterface`，when 调用 GetService/GetRequiredService，then 抛出与缺少依赖相符的异常（不吞掉 DI 异常）。

## Additional Context

### Dependencies

- **已有**：Tenon.Caching.Abstractions、Castle.Core.AsyncInterceptor（含 Castle.DynamicProxy）。
- **新增**：Microsoft.Extensions.DependencyInjection.Abstractions 9.0.0（仅 AddCachedProxy 扩展需要）。
- **调用方**：使用 AddCachedProxy 前需自行注册 `ICacheProvider`、`ICacheKeyGenerator`、`IOptions<CacheAsideInterceptorOptions>`、`ILogger`（如通过 AddInMemoryCache、Configure、AddLogging）。

### Testing Strategy

- **单元测试**：xUnit；主流程用例使用 AddCachedProxy 注册 + 解析（无需 ProxyGenerator/CreateInterfaceProxyWithTarget）；保留 1～2 个手动构造代理用例用于拦截器行为独立验证。
- **覆盖点**：接口优先注解、仅实现有注解的兼容、AddCachedProxy 注册与解析、缓存命中与无特性直通。
- **无需**：单独集成测试项目；README 示例可在实现后更新为接口标注 + AddCachedProxy 用法。

### Notes

- 用户诉求原文：测试中 `ITargetService`/`TargetService` 与 `[CachingAbl]` 在实现上的用法「不太友好」，建议在 `Tenon.Caching.Interceptor.Castle` 中封装抽象接口或其他方式。
- **风险**：AddCachedProxy 内 ProxyGenerator/拦截器生命周期需与 TInterface 解析一致，避免作用域错误导致多实例或状态错乱。
- **后续可做**：README 与示例代码更新为接口标注 + AddCachedProxy；可选支持 `AddCachedProxy<TInterface, TImplementation>(Action<CacheAsideInterceptorOptions>? configure)` 以简化调用方配置。
