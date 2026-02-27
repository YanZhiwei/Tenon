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
```

需同时提供 `ICacheProvider` 实现（例如 `Tenon.Caching.InMemory`）：

```bash
dotnet add package Tenon.Caching.InMemory
```

## 🚀 快速开始

### 1. 注册依赖

需注册：缓存实现、键生成器、拦截器选项、以及本库的拦截器。示例（配合内存缓存）：

```csharp
// 缓存实现
services.AddInMemoryCache();

// 拦截器选项（延时双删的第二次删除前等待时间）
services.Configure<CacheAsideInterceptorOptions>(options =>
{
    options.DelayedDelete = TimeSpan.FromSeconds(1);
});

// 缓存键生成器（可选，不注册则需自行提供 ICacheKeyGenerator）
services.AddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>();

// 拦截器（需 ICacheProvider、ICacheKeyGenerator、CacheAsideInterceptorOptions、ILogger）
services.AddSingleton<CacheAsideAsyncInterceptor>();
```

### 2. 使用 Castle 创建代理

拦截器实现 Castle 的 `IAsyncInterceptor`，需通过 Castle 的 `ProxyGenerator` 对接口/类创建代理并注入拦截器。示例：

```csharp
// 伪代码示例：在解析服务时用代理包装实现类
var generator = new ProxyGenerator();
var interceptor = serviceProvider.GetRequiredService<CacheAsideAsyncInterceptor>();
var target = serviceProvider.GetRequiredService<ProductService>();
var proxy = generator.CreateInterfaceProxyWithTarget<IProductService>(target, interceptor);
```

实际项目中可将上述逻辑封装为扩展方法或工厂，按需注册为 `IProductService` 的实现。

### 3. 在方法上使用特性

```csharp
public interface IProductService
{
    Task<Product?> GetByIdAsync(int id);
    Task UpdateAsync(Product product);
}

public class ProductService : IProductService
{
    [CachingAbl(ExpirationInSec = 3600)]  // 先查缓存，未命中执行方法并写入缓存 1 小时
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    [CachingEvict]  // 执行前后延时双删；可配置 CacheKeys 指定要删的键
    public async Task UpdateAsync(Product product)
    {
        await _repository.UpdateAsync(product);
    }
}
```

## 📖 特性说明

### CachingAblAttribute（Cache-Aside）

标记“先查缓存，未命中再执行并回写”的方法：

| 属性 | 类型 | 说明 |
|------|------|------|
| `ExpirationInSec` | int | 缓存过期秒数，默认 30 |
| `CacheKeyPrefix` | string | 继承自基类，键前缀 |
| `IsHighAvailability` | bool | 继承自基类，为 true 时异常不抛出仅记录 |
| `CacheKey` | string | 继承自基类，可选固定键片段 |

```csharp
[CachingAbl(ExpirationInSec = 1800, CacheKeyPrefix = "Product")]
public async Task<Product?> GetByCodeAsync(string code) => await _repo.GetByCodeAsync(code);
```

### CachingEvictAttribute（失效 / 延时双删）

标记“执行前后删除缓存”的方法，支持延时双删与失败入队：

| 属性 | 类型 | 说明 |
|------|------|------|
| `CacheKeys` | string[] | 要失效的键或键模板，可与方法参数组合；空则按方法参数生成 |
| `CacheKeyPrefix` / `IsHighAvailability` / `CacheKey` | 同基类 | 同上 |

```csharp
[CachingEvict]
public async Task UpdateAsync(int id, Product product) => await _repo.UpdateAsync(id, product);

[CachingEvict(CacheKeys = new[] { "Product:List" })]  // 同时删指定 key
public async Task RefreshAsync() => await _repo.RefreshAsync();
```

### CachingParameterAttribute（参与键生成的参数）

标记哪些参数参与缓存键生成；**未标记任何参数时，默认使用全部参数**。

```csharp
[CachingAbl(ExpirationInSec = 600)]
public async Task<Order?> GetOrderAsync(
    [CachingParameter] string orderId,   // 参与生成 key
    string traceId)                        // 未标记，若其他参数有标记则可不参与（由生成器决定）
```

默认的 `DefaultCacheKeyGenerator` 行为：若有任意参数带 `[CachingParameter]`，则仅用带标记的参数；否则用全部参数。

## 🔧 自定义缓存键生成器

实现 `ICacheKeyGenerator` 的三个方法即可：

```csharp
public class CustomCacheKeyGenerator : ICacheKeyGenerator
{
    public string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix)
    {
        // 自定义单键生成逻辑
        var key = $"{prefix}{methodInfo.DeclaringType?.Name}:{methodInfo.Name}";
        foreach (var arg in args)
            key += $":{arg}";
        return key;
    }

    public string[] GetCacheKeys(MethodInfo methodInfo, object[] args, string prefix)
    {
        // 批量失效时返回多键
        return new[] { GetCacheKey(methodInfo, args, prefix) };
    }

    public string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix)
    {
        return string.IsNullOrWhiteSpace(prefix)
            ? $"{methodInfo.DeclaringType?.Name}:{methodInfo.Name}:"
            : $"{prefix}:{methodInfo.DeclaringType?.Name}:{methodInfo.Name}:";
    }
}

// 注册
services.AddSingleton<ICacheKeyGenerator, CustomCacheKeyGenerator>();
```

## ⚙️ 配置项

| 类型 | 选项 | 说明 |
|------|------|------|
| `CacheAsideInterceptorOptions` | `DelayedDelete` | 延时双删中，第二次删除前的等待时间，默认 1 秒 |

## 🔨 项目依赖

- Castle.Core.AsyncInterceptor
- Tenon.Caching.Abstractions
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.Extensions.Configuration.Abstractions

使用前需提供 `ICacheProvider` 的实现（如 Tenon.Caching.InMemory）。

## 📝 使用注意

- 延时双删的延迟时间根据数据一致性与性能需求权衡设置。
- `IsHighAvailability = true` 时，缓存读写/删除异常不会抛出，仅记录日志；设为 `false` 时抛出。
- 失效失败会入队到 `CachingEvictFailedQueue.Instance`，可由后台任务消费并重试删除。
- 缓存键生成依赖方法参数：避免用易变或大对象作为唯一参与键生成的参数。

## 🤝 参与贡献

欢迎参与项目贡献，请阅读仓库的贡献指南。

## 📄 开源协议

本项目采用 MIT 开源协议。
