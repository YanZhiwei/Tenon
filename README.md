# Tenon

**像榫卯一样按需构建项目功能。**

面向 .NET 9 的模块化基础设施库：**抽象 + 多实现 + 扩展**，提供缓存、锁、事件总线、仓储、认证、服务发现等能力，按需引用、统一风格、便于替换。

---

## 为什么是 Tenon？

### 背景

做 .NET 后端或微服务时，常需要分布式缓存、锁、事件总线、仓储、多租户、JWT、服务发现等。若自己封装则重复造轮子；若拼凑各类第三方包，则风格不一、依赖杂乱，想换实现（如 Redis 换内存）往往要改大量业务代码。

Tenon 把这些**常见基础设施**做成一套**统一抽象 + 多种实现**：业务只依赖抽象与少量扩展包，按需选用实现，底层可换而不改业务代码。

### 解决的问题与收益

| 方面 | 说明 |
|------|------|
| **接口统一、实现可换** | 缓存、锁、布隆过滤器、事件总线等均有抽象接口，同一套代码可切换 Redis / 内存等，方便开发、测试与迁移。 |
| **少写样板代码** | 仓储、审计字段、乐观锁、多租户过滤、JWT + Scope 授权等提供基类与扩展，新项目直接复用。 |
| **与宿主深度集成** | 通过 `AddXxx()` + Options + 配置节与 ASP.NET Core / EF Core 一致，上手快。 |
| **按需引用** | 只引用需要的 NuGet 包，依赖清晰、不拖泥带水。 |
| **开箱即用** | 配好连接与 Options，几行 DI 即可启用缓存、锁、事件总线、多租户仓储等。 |
| **测试友好** | 抽象易 Mock；缓存/锁可换内存实现，单测无需真实 Redis。 |
| **文档与示例** | 各能力有对应 [samples](samples/) 与 [docs](docs/index.md)，便于接入与扩展。 |

---

## ✨ 能力一览

| 能力 | 说明 |
|------|------|
| 🌈 服务治理 | Consul（含 Grpc / Refit 客户端） |
| 📦 分布式缓存 | Redis（StackExchange）、内存、缓存拦截（Castle） |
| 😎 分布式锁 | Redis |
| 🚀 布隆过滤器 | Redis |
| 🌟 分布式事件总线 | DotNetCore.CAP |
| ⚙️ 分布式 ID | Snowflake |
| 🎨 消息队列 | RabbitMQ |
| 🔒 数据访问 | EF Core & Repository（MySQL / SQLite） |
| 🛡️ 认证与 OpenAPI | JWT Bearer、Scope 授权、OpenAPI 扩展 |
| ✅ 校验与后台任务 | FluentValidation、Hangfire |

---

## 快速开始

**环境**：.NET 9.0 SDK

```bash
git clone https://github.com/YanZhiwei/Tenon.git
cd Tenon/src
dotnet restore && dotnet build && dotnet test
```

运行示例：

```bash
cd samples/ConsulSample   # 或其他 samples 下的项目
dotnet run
```

详见 [开发指南](docs/development-guide.md)。

---

## 项目结构

```
Tenon/
├── src/                    # 主源码（Tenon.sln）
│   ├── Abstractions/       # 各能力抽象
│   ├── Extensions/         # ASP.NET Core、EF Core、FluentValidation 等扩展与 DI
│   ├── Infrastructures/    # Consul、Redis、RabbitMQ、Serilog、Polly、Castle 等
│   ├── Helpers/ & Models/  # 通用工具与共享模型
│   └── Tenon.*             # 各能力实现（Repository、Caching、BloomFilter、EventBus.Cap 等）
├── samples/                # 可运行示例
├── test/                   # 单元/集成测试
└── docs/                   # 项目文档
```

更多见 [源码树分析](docs/source-tree-analysis.md)、[架构模式](docs/architecture-patterns.md)。

---

## 示例项目

| 示例 | 说明 |
|------|------|
| [ConsulSample](samples/ConsulSample) | Consul 服务发现 |
| [ConsulRefitSample](samples/ConsulRefitSample) | Consul + Refit 客户端 |
| [SnowflakeSample](samples/SnowflakeSample) | 分布式 ID（Snowflake） |
| [OpenApiSample](samples/OpenApiSample) | OpenAPI / Swagger |
| [HangfireSample](samples/HangfireSample) | 后台任务（Hangfire） |
| [MultiTenantSample](samples/MultiTenantSample) | 多租户数据访问 |
| [FluentValidationSample](samples/FluentValidationSample) | FluentValidation |
| [FileUploadSample](samples/FileUploadSample) | 文件上传 |
| [RabbitMqDirectExchangeSample](samples/RabbitMq/RabbitMqDirectExchangeSample) | RabbitMQ 直连交换机 |
| [MediatREventBusSample](samples/MediatREventBusSample) | MediatR + 事件总线 |
| [Windows](samples/Windows/) | HookSample、Win32Sample |

---

## 使用示例

### 数据访问（EF Core + Repository）

实体继承 `EfBasicAuditEntity`，DbContext 继承 `MySqlDbContext`，在 `OnModelCreating` 中配置表与 `ApplyConfigurations`。

**注册：**

```csharp
services.AddEfCoreMySql<MySqlTestDbContext>(configuration.GetSection("MySql"));
```

**使用仓储：**

```csharp
public class Blog : EfBasicAuditEntity, IConcurrency
{
    public string Url { get; set; } = default!;
    public int Rating { get; set; }
    public byte[] RowVersion { get; set; } = default!;
}

// 注入 IRepository<Blog, long>
var list = await _blogRepository.GetListAsync(b => b.Rating > 0);
var entity = await _blogRepository.GetAsync(1);
await _blogRepository.InsertAsync(new Blog { Url = "https://example.com", Rating = 5 });
```

### 分布式缓存（Redis）

**注册：**

```csharp
services.AddSystemTextJsonSerializer()
    .AddRedisStackExchangeProvider(configuration.GetSection("Redis"));
// 多实例：AddKeyedRedisStackExchangeProvider(key, configuration.GetSection("Redis2"))
```

**使用（注入 `ICacheProvider`）：**

```csharp
await _cacheProvider.SetAsync("user:1", userDto, TimeSpan.FromMinutes(10));
var value = await _cacheProvider.GetAsync<UserDto>("user:1");
if (value.HasValue) { /* 使用 value.Value */ }
```

### 缓存拦截（AOP）

通过 Castle 动态代理对方法加缓存：先查缓存，未命中再执行方法并回写。

```csharp
// 注册拦截器与 ICacheProvider 后，在接口方法上标注
[CachingAbl(ExpirationInSec = 60)]
Task<UserDto> GetByIdAsync(long id);
```

### 分布式锁

**注册：**

```csharp
services.AddRedisStackExchangeDistributedLocker(configuration.GetSection("Redis"));
```

**使用（注入 `IDistributedLocker`）：**

```csharp
var key = "lock:order:123";
if (await _distributedLocker.LockTakeAsync(key, timeoutSeconds: 10))
{
    try
    {
        // 执行业务
    }
    finally
    {
        await _distributedLocker.LockReleaseAsync(key);
    }
}
```

### 布隆过滤器

**注册：**

```csharp
services.AddBloomFilter(opt =>
{
    opt.Name = "test";
    opt.Capacity = 1000;
    opt.ErrorRate = 0.01;
    opt.UseRedisStackExchange(configuration.GetSection("Redis"));
});
```

**使用（注入 `IBloomFilter`）：**

```csharp
await _bloomFilter.AddAsync("userId:1001");
var exists = await _bloomFilter.ExistsAsync("userId:1001");
```

### 分布式 ID（Snowflake）

**注册：**

```csharp
services.AddDistributedId(options =>
{
    options.UseSnowflake(configuration.GetSection("DistributedId"));
    options.UseWorkerNode<StackExchangeProvider>(configuration.GetSection("DistributedId:WorkerNode"));
});
```

**使用（注入 `IDGenerator`）：**

```csharp
var id = _idGenerator.GetNextId();
```

### 事件总线（CAP）

需配合 DotNetCore.CAP 的 `AddCap` 等配置；发布端注入 `IEventBusPublisher`，消息类型继承 `EventBusDescriptor`。

```csharp
await _eventBusPublisher.PublishAsync(new OrderCreatedDescriptor { Id = id, EventSource = "Order" });
```

### WebAPI（认证与基类 Controller）

继承 `AbstractController`，使用 `[AuthorizeScope]` 与 JWT Bearer；用户上下文在 `ConfigureJwtBearerAuthenticationOptions<IdentityAuthenticationHandler>` 的 `OnTokenValidated` 中填充。详见 [docs](docs/) 与示例。

```csharp
[Route("api/[controller]")]
[ApiController]
[AuthorizeScope([], AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : AbstractController
{
    [HttpGet("page")]
    public async Task<ActionResult<PagedResultDto<UserDto>>> GetPagedAsync([FromQuery] UserSearchPagedDto search)
        => await _userService.GetPagedAsync(search);
}
```

### 多租户

实体继承 `EfTenantEntity`，实现 `IEfTenantResolver`，DbContext 继承 `TenonDbContext` 并调用 `modelBuilder.ApplyTenantFilter()`。仓储提供 `GetListForCurrentTenantAsync()`、`ChangeTenant(tenantId)`、`DisableTenantFilter()`。完整示例见 [MultiTenantSample](samples/MultiTenantSample) 与 [MultiTenant.md](docs/MultiTenant.md)。

---

## 文档与版本

- **文档**：[docs/index.md](docs/index.md)（技术栈、架构、源码树、开发/部署指南）
- **版本**：由 `src/version.props` 统一管理；NuGet 包在 Release 下 `dotnet pack` 生成。

---

## 许可证

MIT
