# Tenon.Caching.Interceptor.Castle

[![NuGet version](https://badge.fury.io/nu/Tenon.Caching.Interceptor.Castle.svg)](https://badge.fury.io/nu/Tenon.Caching.Interceptor.Castle)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

基于 Castle 动态代理的缓存拦截器，支持 Cache-Aside、延时双删与失败补偿，可与 Tenon.Caching.Abstractions 的任意缓存实现（如 Tenon.Caching.InMemory）配合使用。

## ✨ 功能特性

- 基于 Castle.Core.AsyncInterceptor 的同步/异步方法拦截
- Cache-Aside：先查缓存，未命中再执行方法并回写
- 延时双删：失效时先删缓存 → 执行方法 → 延迟后再删一次，降低脏读
- 失败补偿：删除失败时入队，供后续重试
- 可替换的缓存键生成器（`ICacheKeyGenerator`），默认按 前缀:类型名:方法名:参数 生成

## 📦 安装

```bash
dotnet add package Tenon.Caching.Interceptor.Castle
dotnet add package Tenon.Caching.InMemory
```

## 🚀 快速开始（与单元测试一致）

### 1. 注册依赖

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.Caching.InMemory.Extensions;
using Tenon.Caching.Interceptor.Castle;
using Tenon.Caching.Interceptor.Castle.Configurations;

var services = new ServiceCollection()
    .AddInMemoryCache()
    .AddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>()
    .Configure<CacheAsideInterceptorOptions>(o => o.DelayedDelete = TimeSpan.FromMilliseconds(1))
    .AddLogging(b => b.SetMinimumLevel(LogLevel.Warning));

using var provider = services.BuildServiceProvider();
```

### 2. 创建代理并调用

```csharp
using Castle.DynamicProxy;
using Microsoft.Extensions.Options;

var target = new MyService();
var interceptor = new CacheAsideAsyncInterceptor(
    provider.GetRequiredService<ICacheProvider>(),
    provider.GetRequiredService<ICacheKeyGenerator>(),
    provider.GetRequiredService<IOptions<CacheAsideInterceptorOptions>>().Value,
    provider.GetRequiredService<ILogger<CacheAsideAsyncInterceptor>>());

var generator = new ProxyGenerator();
var proxy = generator.CreateInterfaceProxyWithTarget<IMyService>(target, interceptor);

var first = await proxy.GetAsync(1);
var second = await proxy.GetAsync(1);  // 第二次可能命中缓存
// first == second
```

### 3. 目标服务与特性

```csharp
public interface IMyService
{
    Task<int> GetAsync(int id);
    Task<int> GetNoCacheAsync(int id);  // 无特性，直接执行
}

public class MyService : IMyService
{
    [CachingAbl(ExpirationInSec = 60)]
    public Task<int> GetAsync(int id) => Task.FromResult(id);

    public Task<int> GetNoCacheAsync(int id) => Task.FromResult(id);
}
```

## 📖 特性说明

### CachingAblAttribute（Cache-Aside）

| 属性 | 类型 | 默认 | 说明 |
|------|------|------|------|
| `ExpirationInSec` | int | 30 | 缓存过期秒数 |
| `CacheKeyPrefix` | string | "" | 键前缀 |
| `IsHighAvailability` | bool | true | 为 true 时异常不抛出仅记录 |
| `CacheKey` | string | "" | 可选固定键片段 |

```csharp
[CachingAbl(ExpirationInSec = 3600)]
public Task<Product?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
```

### CachingEvictAttribute（延时双删）

| 属性 | 类型 | 默认 | 说明 |
|------|------|------|------|
| `CacheKeys` | string[] | 空 | 要失效的键；空则按方法参数生成 |
| 基类属性 | 同上 | 同上 | CacheKeyPrefix、IsHighAvailability、CacheKey |

```csharp
[CachingEvict]
public Task UpdateAsync(int id, Product p) => _repo.UpdateAsync(id, p);

[CachingEvict(CacheKeys = new[] { "k1", "k2" })]
public Task RefreshAsync() => _repo.RefreshAsync();
```

### CachingParameterAttribute（参与键生成的参数）

若有任意参数带 `[CachingParameter]`，则仅用带标记的参数；否则用全部参数。

```csharp
[CachingAbl(ExpirationInSec = 600)]
public Task<Order?> GetOrderAsync(
    [CachingParameter] string orderId,
    string traceId)
```

## 🔧 默认缓存键生成器（与单元测试一致）

`DefaultCacheKeyGenerator` 行为：

- **无参方法**：`类型名:方法名:0`
- **带前缀**：`App:类型名:方法名:...`
- **带参数**：参数序列化后拼接到键
- **GetCacheKeys**：每个参数生成一条键（非数组参数时使用完整 args，多条键内容相同）

```csharp
// 示例：GetCacheKeyPrefix(method, "")  → "FakeTarget:GetNoArg:"
// 示例：GetCacheKeyPrefix(method, "P") → "P:FakeTarget:GetNoArg:"
```

## 🔧 自定义缓存键生成器

```csharp
public class CustomCacheKeyGenerator : ICacheKeyGenerator
{
    public string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix)
    {
        var key = $"{prefix}{methodInfo.DeclaringType?.Name}:{methodInfo.Name}";
        foreach (var arg in args) key += $":{arg}";
        return key;
    }

    public string[] GetCacheKeys(MethodInfo methodInfo, object[] args, string prefix)
        => new[] { GetCacheKey(methodInfo, args, prefix) };

    public string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix)
        => string.IsNullOrWhiteSpace(prefix)
            ? $"{methodInfo.DeclaringType?.Name}:{methodInfo.Name}:"
            : $"{prefix}:{methodInfo.DeclaringType?.Name}:{methodInfo.Name}:";
}

services.AddSingleton<ICacheKeyGenerator, CustomCacheKeyGenerator>();
```

## ⚙️ 配置项

| 类型 | 选项 | 默认 | 说明 |
|------|------|------|------|
| `CacheAsideInterceptorOptions` | `DelayedDelete` | 1 秒 | 延时双删中，第二次删除前的等待时间 |

```csharp
services.Configure<CacheAsideInterceptorOptions>(o =>
{
    o.DelayedDelete = TimeSpan.FromMilliseconds(500);
});
```

## 🔌 失败补偿队列

失效失败时入队到 `CachingEvictFailedQueue.Instance`：

```csharp
// Enqueue 后 TryDequeue 可获得键数组
CachingEvictFailedQueue.Instance.Enqueue(new[] { "key1", "key2" });
var ok = CachingEvictFailedQueue.Instance.TryDequeue(out var keys);  // true
// keys 即 ["key1", "key2"]

// 空队列时 TryDequeue 返回 false
```

## 🔨 项目依赖

- Castle.Core.AsyncInterceptor
- Tenon.Caching.Abstractions
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.Extensions.Configuration.Abstractions

使用前需提供 `ICacheProvider`（如 Tenon.Caching.InMemory）。

## 📝 使用注意

- 延时双删的延迟时间根据数据一致性与性能权衡设置。
- `IsHighAvailability = true` 时异常不抛出仅记录；设为 `false` 时抛出。
- 失效失败入队到 `CachingEvictFailedQueue.Instance`，可由后台任务消费重试。
- 避免用易变或大对象作为唯一参与键生成的参数。

## 🤝 参与贡献

欢迎参与项目贡献，请阅读仓库的贡献指南。

## 📄 开源协议

本项目采用 MIT 开源协议。
