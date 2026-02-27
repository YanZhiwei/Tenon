# Tenon.Caching.InMemory

[![NuGet version](https://badge.fury.io/nu/Tenon.Caching.InMemory.svg)](https://badge.fury.io/nu/Tenon.Caching.InMemory)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

基于 Microsoft.Extensions.Caching.Memory (IMemoryCache) 的高性能内存缓存实现，为 .NET 应用程序提供简单且灵活的缓存操作接口。

## ✨ 功能特性

- 🚀 基于 IMemoryCache（Microsoft.Extensions.Caching.Memory）的高性能实现
- 💉 集成依赖注入框架
- 🎯 统一的 ICacheProvider 接口
- 🔄 自动过期缓存清理
- 🛡️ 保证线程安全

## 📦 安装方式

通过 NuGet 包管理器安装：

```bash
dotnet add package Tenon.Caching.InMemory
```

## 🚀 快速开始

### 1. 注册服务

在 `Program.cs` 中配置服务，使用默认内存缓存（`MemoryCache.Default`）：

```csharp
services.AddInMemoryCache();
```

### 2. 使用缓存服务

```csharp
public class MyService
{
    private readonly ICacheProvider _cache;

    public MyService(ICacheProvider cache)
    {
        _cache = cache;
    }

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

适用于单实例、无需键控的场景：

```csharp
services.AddInMemoryCache();
```

### 方式二：通过 AddCaching + UseInMemoryStorage（键控或统一入口）

需要与 Tenon.Caching.Abstractions 的 `AddCaching` 配合、或使用键控服务时：

```csharp
services.AddCaching(options =>
{
    options.UseInMemoryStorage();
    // 可选：options.KeyedServiceKey = "InMemory";
});
```

> **说明**：通过本方式注册时，当前仅使用默认内存缓存（无内存限制、轮询间隔等配置）。如需自定义缓存名称、内存限制或轮询间隔，请使用下方「配置说明」中的带配置注册：`AddInMemoryCache(options => { … })`。

## ⚙️ 配置说明

- **无参注册**：`AddInMemoryCache()` 使用 `MemoryCache.Default`，无需配置。
- **带配置注册**：可通过 `AddInMemoryCache(options => { ... })` 使用 `InMemoryCacheOptions` 自定义行为：

| 选项 | 类型 | 说明 |
|------|------|------|
| `CacheName` | string? | 缓存实例名称；与其余选项均为 null 时使用默认缓存。 |
| `CacheMemoryLimitMegabytes` | long? | 缓存最大内存限制（MB）。 |
| `PhysicalMemoryLimitPercentage` | int? | 占物理内存的百分比上限（0–100）。 |
| `PollingInterval` | TimeSpan? | 过期项轮询清理间隔；未设置时默认 2 分钟。 |

示例：

```csharp
services.AddInMemoryCache(options =>
{
    options.CacheName = "MyAppCache";
    options.CacheMemoryLimitMegabytes = 100;
    options.PollingInterval = TimeSpan.FromMinutes(5);
});
```

## 🔨 项目依赖

- Microsoft.Extensions.Caching.Memory
- Tenon.Caching.Abstractions
- Microsoft.Extensions.DependencyInjection.Abstractions

## 📁 项目结构

```
Tenon.Caching.InMemory/
├── Configurations/
│   └── InMemoryCacheOptions.cs              # 内存缓存选项类（用于 AddInMemoryCache(options => …)）
├── Extensions/
│   ├── CachingOptionsExtension.cs           # ICachingOptionsExtension 实现（DI 注册）
│   ├── CachingOptionsInMemoryExtensions.cs # CachingOptions 扩展（UseInMemoryStorage）
│   └── ServiceCollectionExtension.cs       # 服务注册扩展（AddInMemoryCache）
├── MemoryCacheProvider.cs                   # 内存缓存实现
└── Tenon.Caching.InMemory.csproj
```

## 📝 使用注意事项

- 根据应用程序需求为缓存项设置合适的过期时间。
- 采用统一的缓存键命名规范，便于排查与清理。
- 本包为进程内缓存，多实例或分布式场景请选用 Redis 等实现。
- 方式二（`AddCaching` + `UseInMemoryStorage`）注册时不可配置内存限制等，见上方「方式二」说明。

## 🤝 参与贡献

欢迎参与项目贡献！请阅读仓库的贡献指南了解如何参与开发。

## 📄 开源协议

本项目采用 MIT 开源协议。
