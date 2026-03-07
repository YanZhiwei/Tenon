# Tenon

**English** | [中文](README.md)

**Build project features on demand, like mortise and tenon joints.**

A modular infrastructure library for .NET 9: **Abstractions + multiple implementations + extensions**. It provides caching, locking, event bus, repository, authentication, service discovery, and more—reference what you need, with a consistent style and easy replacement.

---

## Why Tenon?

### Background

When building .NET backends or microservices, you often need distributed caching, locking, event bus, repository, multi-tenancy, JWT, service discovery, and more. Building these from scratch duplicates effort; mixing third-party packages leads to inconsistent style, tangled dependencies, and painful migrations when switching implementations (e.g., Redis to in-memory).

Tenon delivers these **common infrastructure pieces** as a **unified abstraction with multiple implementations**: your business code depends only on abstractions and a few extension packages. Choose the implementations you need, and swap them without changing business logic.

### Problems Solved and Benefits

| Aspect | Description |
|--------|-------------|
| **Unified interfaces, swappable implementations** | Caching, locking, bloom filter, event bus, etc. all have abstract interfaces. Use Redis, in-memory, or others with the same code, for development, testing, and migration. |
| **Less boilerplate** | Base classes and extensions for repository, audit fields, optimistic locking, multi-tenant filtering, JWT + Scope authorization—reuse in new projects. |
| **Deep host integration** | `AddXxx()` + Options + configuration sections align with ASP.NET Core and EF Core for quick onboarding. |
| **Reference only what you need** | Reference only the NuGet packages you use; keep dependencies clear and minimal. |
| **Ready to use** | Configure connection and Options, add a few lines of DI, and enable caching, locking, event bus, multi-tenant repository, etc. |
| **Test-friendly** | Easy to mock abstractions; use in-memory implementations for cache/lock so unit tests don't need Redis. |
| **Docs and samples** | Each capability has corresponding [samples](samples/) and [docs](docs/index.md) for integration and extension. |

---

## ✨ Capabilities Overview

| Capability | Description |
|------------|-------------|
| 🌈 Service governance | Consul (including Grpc / Refit clients) |
| 📦 Distributed cache | Redis (StackExchange), in-memory, cache interceptor (Castle) |
| 😎 Distributed lock | Redis |
| 🚀 Bloom filter | Redis |
| 🌟 Distributed event bus | DotNetCore.CAP |
| ⚙️ Distributed ID | Snowflake |
| 🎨 Message queue | RabbitMQ |
| 🔒 Data access | EF Core & Repository (MySQL / SQLite) |
| 🛡️ Authentication & OpenAPI | JWT Bearer, Scope authorization, OpenAPI extensions |
| ✅ Validation & background jobs | FluentValidation, Hangfire |

---

## Quick Start

**Requirements:** .NET 9.0 SDK

```bash
git clone https://github.com/YanZhiwei/Tenon.git
cd Tenon/src
dotnet restore && dotnet build && dotnet test
```

Run samples:

```bash
cd samples/ConsulSample   # or any project under samples/
dotnet run
```

See [Development Guide](docs/development-guide.md) for details.

---

## Project Structure

```
Tenon/
├── src/                    # Main source (Tenon.sln)
│   ├── Abstractions/       # Abstractions for each capability
│   ├── Extensions/         # ASP.NET Core, EF Core, FluentValidation extensions and DI
│   ├── Infrastructures/    # Consul, Redis, RabbitMQ, Serilog, Polly, Castle, etc.
│   ├── Helpers/ & Models/  # Shared utilities and models
│   └── Tenon.*             # Implementations (Repository, Caching, BloomFilter, EventBus.Cap, etc.)
├── samples/                # Runnable samples
├── test/                   # Unit and integration tests
└── docs/                   # Project documentation
```

See also [Source Tree Analysis](docs/source-tree-analysis.md) and [Architecture Patterns](docs/architecture-patterns.md).

---

## Sample Projects

| Sample | Description |
|--------|-------------|
| [ConsulSample](samples/ConsulSample) | Consul service discovery |
| [ConsulRefitSample](samples/ConsulRefitSample) | Consul + Refit client |
| [SnowflakeSample](samples/SnowflakeSample) | Distributed ID (Snowflake) |
| [OpenApiSample](samples/OpenApiSample) | OpenAPI / Swagger |
| [HangfireSample](samples/HangfireSample) | Background jobs (Hangfire) |
| [MultiTenantSample](samples/MultiTenantSample) | Multi-tenant data access |
| [FluentValidationSample](samples/FluentValidationSample) | FluentValidation |
| [FileUploadSample](samples/FileUploadSample) | File upload |
| [RabbitMqDirectExchangeSample](samples/RabbitMq/RabbitMqDirectExchangeSample) | RabbitMQ direct exchange |
| [MediatREventBusSample](samples/MediatREventBusSample) | MediatR + event bus |
| [Windows](samples/Windows/) | HookSample, Win32Sample |

---

## Usage Examples

### Data Access (EF Core + Repository)

Entities inherit `EfBasicAuditEntity`; DbContext inherits `MySqlDbContext`. In `OnModelCreating`, configure tables and `ApplyConfigurations`.

**Registration:**

```csharp
services.AddEfCoreMySql<MySqlTestDbContext>(configuration.GetSection("MySql"));
```

**Using the repository:**

```csharp
public class Blog : EfBasicAuditEntity, IConcurrency
{
    public string Url { get; set; } = default!;
    public int Rating { get; set; }
    public byte[] RowVersion { get; set; } = default!;
}

// Inject IRepository<Blog, long>
var list = await _blogRepository.GetListAsync(b => b.Rating > 0);
var entity = await _blogRepository.GetAsync(1);
await _blogRepository.InsertAsync(new Blog { Url = "https://example.com", Rating = 5 });
```

### Distributed Cache (Redis)

**Registration:**

```csharp
services.AddSystemTextJsonSerializer()
    .AddRedisStackExchangeProvider(configuration.GetSection("Redis"));
// Multiple instances: AddKeyedRedisStackExchangeProvider(key, configuration.GetSection("Redis2"))
```

**Usage (inject `ICacheProvider`):**

```csharp
await _cacheProvider.SetAsync("user:1", userDto, TimeSpan.FromMinutes(10));
var value = await _cacheProvider.GetAsync<UserDto>("user:1");
if (value.HasValue) { /* use value.Value */ }
```

### Cache Interceptor (AOP)

Apply caching to methods via Castle dynamic proxy: check cache first, execute method on miss, then write back.

```csharp
// After registering interceptor and ICacheProvider, annotate interface methods
[CachingAbl(ExpirationInSec = 60)]
Task<UserDto> GetByIdAsync(long id);
```

### Distributed Lock

**Registration:**

```csharp
services.AddRedisStackExchangeDistributedLocker(configuration.GetSection("Redis"));
```

**Usage (inject `IDistributedLocker`):**

```csharp
var key = "lock:order:123";
if (await _distributedLocker.LockTakeAsync(key, timeoutSeconds: 10))
{
    try
    {
        // Do work
    }
    finally
    {
        await _distributedLocker.LockReleaseAsync(key);
    }
}
```

### Bloom Filter

**Registration:**

```csharp
services.AddBloomFilter(opt =>
{
    opt.Name = "test";
    opt.Capacity = 1000;
    opt.ErrorRate = 0.01;
    opt.UseRedisStackExchange(configuration.GetSection("Redis"));
});
```

**Usage (inject `IBloomFilter`):**

```csharp
await _bloomFilter.AddAsync("userId:1001");
var exists = await _bloomFilter.ExistsAsync("userId:1001");
```

### Distributed ID (Snowflake)

**Registration:**

```csharp
services.AddDistributedId(options =>
{
    options.UseSnowflake(configuration.GetSection("DistributedId"));
    options.UseWorkerNode<StackExchangeProvider>(configuration.GetSection("DistributedId:WorkerNode"));
});
```

**Usage (inject `IDGenerator`):**

```csharp
var id = _idGenerator.GetNextId();
```

### Event Bus (CAP)

Requires DotNetCore.CAP configuration (`AddCap`, etc.). Inject `IEventBusPublisher` for publishing; message types inherit `EventBusDescriptor`.

```csharp
await _eventBusPublisher.PublishAsync(new OrderCreatedDescriptor { Id = id, EventSource = "Order" });
```

### WebAPI (Authentication and base Controller)

Inherit `AbstractController`, use `[AuthorizeScope]` with JWT Bearer. User context is populated in `ConfigureJwtBearerAuthenticationOptions<IdentityAuthenticationHandler>` `OnTokenValidated`. See [docs](docs/) and samples for details.

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

### Multi-Tenancy

Entities inherit `EfTenantEntity`; implement `IEfTenantResolver`; DbContext inherits `TenonDbContext` and calls `modelBuilder.ApplyTenantFilter()`. Repository provides `GetListForCurrentTenantAsync()`, `ChangeTenant(tenantId)`, `DisableTenantFilter()`. See [MultiTenantSample](samples/MultiTenantSample) and [MultiTenant.md](docs/MultiTenant.md) for full examples.

---

## Documentation and Versioning

- **Documentation:** [docs/index.md](docs/index.md) (tech stack, architecture, source tree, development and deployment guides)
- **Versioning:** Managed by `src/version.props`; NuGet packages are produced with `dotnet pack` in Release mode.

---

## License

MIT
