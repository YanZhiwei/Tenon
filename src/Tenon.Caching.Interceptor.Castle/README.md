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

## 🚀 快速开始

### 方式一：程序集扫描（推荐，多接口批量注册）

接口继承 `ICacheableService`（定义于 Tenon.Caching.Abstractions），通过 `AddProxiesFromAssembly` 自动扫描并注册：

```csharp
using Tenon.Caching.Abstractions;
using Tenon.Caching.InMemory.Extensions;
using Tenon.Caching.Interceptor.Castle.Extensions;

public interface IUserService : ICacheableService
{
    Task<User?> GetByIdAsync(int id);
}
public class UserService : IUserService { /* ... */ }

var services = new ServiceCollection()
    .AddInMemoryCache()
    .AddProxiesFromAssembly(typeof(IUserService).Assembly, o => o.DelayedDelete = TimeSpan.FromMilliseconds(1));

using var sp = services.BuildServiceProvider();
using var scope = sp.CreateScope();
var proxy = scope.ServiceProvider.GetRequiredService<IUserService>();
```

**约定**：扫描程序集中继承 `ICacheableService` 的接口，查找实现类（若有多实现，优先 `IXxxService` → `XxxService`）。

### 方式二：单接口注册（AddCachedProxyServices）

适用于少数接口、或不想让接口继承 `ICacheableService` 的场景：

```csharp
using Tenon.Caching.InMemory.Extensions;
using Tenon.Caching.Interceptor.Castle.Extensions;

public interface IOrderService
{
    Task<Order?> GetByIdAsync(int id);
}
public class OrderService : IOrderService { /* ... */ }

var services = new ServiceCollection()
    .AddInMemoryCache()
    .AddCachedProxyServices<IOrderService, OrderService>(o => o.DelayedDelete = TimeSpan.FromMilliseconds(1));

using var sp = services.BuildServiceProvider();
using var scope = sp.CreateScope();
var proxy = scope.ServiceProvider.GetRequiredService<IOrderService>();
```

### 目标服务与特性标注

在实现类的方法上使用 `[CachingAbl]`、`[CachingEvict]` 等特性；无特性的方法直接透传：

```csharp
public class UserService : IUserService
{
    [CachingAbl(ExpirationInSec = 60)]
    public Task<User?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task<int> GetCountAsync() => _repo.GetCountAsync();  // 无特性，不缓存
}
```

---

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
public Task<Order?> GetOrderAsync([CachingParameter] string orderId, string traceId)
```

---

## ⚙️ 配置项

| 类型 | 选项 | 默认 | 说明 |
|------|------|------|------|
| `CacheAsideInterceptorOptions` | `DelayedDelete` | 1 秒 | 延时双删中，第二次删除前的等待时间 |

```csharp
services.Configure<CacheAsideInterceptorOptions>(o =>
    o.DelayedDelete = TimeSpan.FromMilliseconds(500));
```

---

## 🔧 缓存键生成器

### 默认行为

- **无参方法**：`类型名:方法名:0`
- **带前缀**：`App:类型名:方法名:...`
- **带参数**：参数序列化后拼接到键
- **GetCacheKeys**：每个参数生成一条键（非数组参数时使用完整 args）

### 自定义生成器

```csharp
using Tenon.Caching.Interceptor.Castle.KeyGenerators;

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

---

## 🔌 失败补偿队列

失效失败时入队到 `CachingEvictFailedQueue.Instance`，可由后台任务消费重试：

```csharp
CachingEvictFailedQueue.Instance.Enqueue(new[] { "key1", "key2" });
var ok = CachingEvictFailedQueue.Instance.TryDequeue(out var keys);  // true，keys = ["key1", "key2"]
```

---

## 🔨 依赖与注意

**依赖**：Castle.Core.AsyncInterceptor、Tenon.Caching.Abstractions、Microsoft.Extensions.*  
**前置**：使用前需注册 `ICacheProvider`（如 Tenon.Caching.InMemory 或 Tenon.Caching.RedisStackExchange）。

**使用注意**：
- 延时双删的延迟时间根据数据一致性与性能权衡设置。
- `IsHighAvailability = true` 时异常不抛出仅记录；设为 `false` 时抛出。
- 避免用易变或大对象作为唯一参与键生成的参数。

---

## 📌 高级用法：手动创建代理

若需脱离 DI 手动创建代理（如测试、脚本场景），可自行注册依赖后使用 `ProxyGenerator`：

```csharp
using Tenon.Caching.Interceptor.Castle.KeyGenerators;

var services = new ServiceCollection()
    .AddInMemoryCache()
    .AddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>()
    .Configure<CacheAsideInterceptorOptions>(o => { })
    .AddLogging();
using var sp = services.BuildServiceProvider();

var target = new MyService();
var interceptor = new CacheAsideAsyncInterceptor(
    sp.GetRequiredService<ICacheProvider>(),
    sp.GetRequiredService<ICacheKeyGenerator>(),
    sp.GetRequiredService<IOptions<CacheAsideInterceptorOptions>>().Value,
    sp.GetRequiredService<ILogger<CacheAsideAsyncInterceptor>>());
var generator = new ProxyGenerator();
var proxy = generator.CreateInterfaceProxyWithTarget<IMyService>(target, interceptor);
```

---

## 🤝 参与贡献

欢迎参与项目贡献，请阅读仓库的贡献指南。

## 📄 开源协议

本项目采用 MIT 开源协议。
