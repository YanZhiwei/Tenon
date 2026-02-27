# Tenon.Caching.InMemory

[![NuGet version](https://badge.fury.io/nu/Tenon.Caching.InMemory.svg)](https://badge.fury.io/nu/Tenon.Caching.InMemory)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

基于 Microsoft.Extensions.Caching.Memory (IMemoryCache) 的高性能内存缓存实现，为 .NET 应用程序提供简单且灵活的缓存操作接口。

## ✨ 功能特性

- 基于 IMemoryCache（Microsoft.Extensions.Caching.Memory）的高性能实现
- 集成依赖注入框架
- 统一的 ICacheProvider 接口
- 自动过期缓存清理
- 保证线程安全

## 📦 安装

```bash
dotnet add package Tenon.Caching.InMemory
```

## 🚀 快速开始

### 1. 注册服务

```csharp
using Microsoft.Extensions.DependencyInjection;
using Tenon.Caching.InMemory.Extensions;

// 无参注册，使用默认 IMemoryCache
var services = new ServiceCollection();
services.AddInMemoryCache();

using var provider = services.BuildServiceProvider();
var cache = provider.GetRequiredService<ICacheProvider>();
```

### 2. 基本读写（与单元测试一致）

```csharp
const string key = "test:key1";
const string value = "value1";

// 写入，并设置过期时间
cache.Set(key, value, TimeSpan.FromMinutes(1));

// 读取，通过 CacheValue<T>.HasValue / Value 判断与取值
var result = cache.Get<string>(key);
if (result.HasValue)
    Console.WriteLine(result.Value);
```

### 3. 移除与存在检查

```csharp
// 写入后检查存在
cache.Set("mykey", "x", TimeSpan.FromMinutes(1));
bool exists = cache.Exists("mykey");  // true

// 移除；键不存在时也返回 true（表示“已确保不存在”）
bool removed = cache.Remove("mykey");
exists = cache.Exists("mykey");  // false
```

### 4. 批量移除

```csharp
cache.Set("a", 1, TimeSpan.FromMinutes(1));
cache.Set("b", 2, TimeSpan.FromMinutes(1));
cache.Set("c", 3, TimeSpan.FromMinutes(1));

long removedCount = cache.RemoveAll(new[] { "a", "b", "c" });  // 3
```

### 5. 在业务类中注入使用

```csharp
public class MyService
{
    private readonly ICacheProvider _cache;

    public MyService(ICacheProvider cache) => _cache = cache;

    public async Task<string> GetOrSetAsync(string key)
    {
        var value = _cache.Get<string>(key);
        if (value.HasValue)
            return value.Value!;

        var data = await FetchFromSourceAsync(key);
        _cache.Set(key, data, TimeSpan.FromMinutes(30));
        return data;
    }
}
```

所有读写均通过 `Get<T>` / `Set` 与 `CacheValue<T>.HasValue` / `Value` 完成，接口无 `TryGet`。

## 📖 两种注册方式

### 方式一：无参注册（默认缓存）

```csharp
services.AddInMemoryCache();
```

适用于单实例、无需键控的场景。

### 方式二：AddCaching + UseInMemoryStorage（键控或统一入口）

```csharp
using Tenon.Caching.Abstractions.Extensions;

services.AddCaching(options =>
{
    options.UseInMemoryStorage();
    // 可选：options.KeyedServiceKey = "InMemory";
});
```

> 通过本方式注册时，当前仅使用默认内存缓存（无内存限制、轮询间隔等配置）。如需自定义缓存名称、内存限制或轮询间隔，请使用下方「配置说明」中的带配置注册。

## ⚙️ 配置说明

- **无参注册**：`AddInMemoryCache()` 内部调用 `AddMemoryCache()` 并注册单例 `ICacheProvider`，使用默认选项。
- **带配置注册**：通过 `AddInMemoryCache(options => { ... })` 使用 `InMemoryCacheOptions`：

| 选项 | 类型 | 说明 |
|------|------|------|
| `CacheName` | string? | 缓存实例名称；与其余选项均为 null 时使用默认缓存。 |
| `CacheMemoryLimitMegabytes` | long? | 缓存最大内存限制（MB）。 |
| `PhysicalMemoryLimitPercentage` | int? | 占物理内存的百分比上限（0–100）；在 Extensions.Caching.Memory 下不生效。 |
| `PollingInterval` | TimeSpan? | 过期项轮询清理间隔；未设置时默认 2 分钟。 |

示例（与单元测试一致）：

```csharp
services.AddInMemoryCache(options =>
{
    options.CacheName = "TestNamedCache";
    options.CacheMemoryLimitMegabytes = 10;
    options.PollingInterval = TimeSpan.FromMinutes(1);
});
```

带配置时传入 `null` 与无参行为一致：`AddInMemoryCache((Action<InMemoryCacheOptions>?)null)`。

## 🔨 项目依赖

- Microsoft.Extensions.Caching.Memory
- Tenon.Caching.Abstractions
- Microsoft.Extensions.DependencyInjection.Abstractions

## 📁 项目结构

```
Tenon.Caching.InMemory/
├── Configurations/
│   └── InMemoryCacheOptions.cs
├── Extensions/
│   ├── CachingOptionsExtension.cs
│   ├── CachingOptionsInMemoryExtensions.cs
│   └── ServiceCollectionExtension.cs
├── MemoryCacheProvider.cs
└── Tenon.Caching.InMemory.csproj
```

## 📝 使用注意事项

- 为缓存项设置合适的过期时间。
- 采用统一的缓存键命名规范，便于排查与清理。
- 本包为进程内缓存，多实例或分布式场景请选用 Redis 等实现。
- 方式二（`AddCaching` + `UseInMemoryStorage`）注册时不可配置内存限制等，见「方式二」说明。

## 🤝 参与贡献

欢迎参与项目贡献，请阅读仓库的贡献指南。

## 📄 开源协议

本项目采用 MIT 开源协议。
