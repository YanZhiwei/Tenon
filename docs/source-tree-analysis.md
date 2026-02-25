# 源码树分析

> Step 5 产出 · 部件：main

```
Tenon/                          # 仓库根
├── .github/
│   └── workflows/               # CI/CD：Build (dotnet.yml)、Publish (nuget_publish.yml)
├── docs/                        # 项目知识/文档输出（含本工作流产出）
├── samples/                     # 示例应用（入口 Program.cs）
│   ├── ConsulSample/
│   ├── ConsulRefitSample/
│   ├── SnowflakeSample/
│   ├── OpenApiSample/
│   ├── HangfireSample/
│   ├── MultiTenantSample/
│   ├── FileUploadSample/
│   ├── FluentValidationSample/
│   ├── RabbitMq/RabbitMqDirectExchangeSample/
│   ├── MediatREventBusSample/
│   └── Windows/ (HookSample, Win32Sample)
├── src/                         # 主源码（关键目录）
│   ├── Tenon.sln                # 主解决方案入口
│   ├── common.props             # 公共 MSBuild 属性
│   ├── version.props            # 版本号
│   ├── nuget.props              # NuGet 包元数据与 SourceLink
│   ├── Abstractions/            # 抽象层：接口与可选默认实现
│   │   ├── Tenon.Abstractions/
│   │   ├── Tenon.Caching.Abstractions/
│   │   ├── Tenon.DistributedId.Abstractions/
│   │   ├── Tenon.DistributedLocker.Abstractions/
│   │   ├── Tenon.EventBus.Abstractions/
│   │   ├── Tenon.Serialization.Abstractions/
│   │   ├── Tenon.BloomFilter.Abstractions/
│   │   ├── Tenon.Mapper.Abstractions/
│   │   └── Tenon.MessageTracker.Abstractions/
│   ├── Extensions/              # 对 ASP.NET Core、EF、FluentValidation 等的扩展与 DI
│   │   ├── Tenon.Extensions/
│   │   ├── Tenon.EntityFrameworkCore.Extensions/
│   │   ├── Tenon.AspNetCore.OpenApi.Extensions/
│   │   ├── Tenon.AspNetCore.Identity.Extensions/
│   │   ├── Tenon.FluentValidation.Extensions/
│   │   ├── Tenon.FluentValidation.AspNetCore.Extensions/
│   │   ├── Tenon.StackExchange.Redis.Extensions/
│   │   ├── Tenon.Hangfire.Extensions/
│   │   ├── Tenon.MediatR.Extensions.EventBus/
│   │   └── Windows/Tenon.Windows.Extensions/
│   ├── Infrastructures/         # 基础设施实现
│   │   ├── Tenon.Infra.Consul/
│   │   ├── Tenon.Infra.Consul.GrpcClient/
│   │   ├── Tenon.Infra.Consul.RefitClient/
│   │   ├── Tenon.Infra.Redis/
│   │   ├── Tenon.Infra.Redis.StackExchangeProvider/
│   │   ├── Tenon.Infra.RabbitMq/
│   │   ├── Tenon.Infra.Serilog/
│   │   ├── Tenon.Infra.Polly/
│   │   ├── Tenon.Infra.Castle/
│   │   ├── Tenon.Infra.Swagger/
│   │   └── Windows/ (Tenon.Infra.Windows.Form, Tenon.Infra.Windows.Win32)
│   ├── Helpers/
│   │   └── Tenon.Helper/        # 通用工具（如 RegexDefaults、Checker、Convert）
│   ├── Models/
│   │   └── Tenon.Models/        # 共享领域模型与接口
│   ├── Tenon.AspNetCore/        # ASP.NET Core 认证/授权/本地化等
│   ├── Tenon.AspNetCore.Abstractions/
│   ├── Tenon.Repository/        # 仓储抽象
│   ├── Tenon.Repository.EfCore/
│   ├── Tenon.Repository.EfCore.MySql/
│   ├── Tenon.Repository.EfCore.Sqlite/
│   ├── Tenon.Caching.InMemory/
│   ├── Tenon.Caching.Redis/
│   ├── Tenon.Caching.RedisStackExchange/
│   ├── Tenon.Caching.Interceptor.Castle/
│   ├── Tenon.DistributedLocker.Redis/
│   ├── Tenon.DistributedLocker.RedisStackExchange/
│   ├── Tenon.BloomFilter.Redis/
│   ├── Tenon.BloomFilter.RedisStackExchange/
│   ├── Tenon.DistributedId.Snowflake/
│   ├── Tenon.EventBus.Cap/
│   ├── Tenon.MessageTracker.EfCore/
│   ├── Tenon.Serialization.Json/
│   ├── Tenon.Mapper.AutoMapper/
│   ├── Tenon.Mapper.Mapster/
│   └── (其余 Tenon.* 包)
└── test/                        # 单元/集成测试项目
    └── Tenon.*Tests/
```

## 关键目录说明

| 目录 | 用途 |
|------|------|
| **src/Abstractions/** | 各能力抽象，被实现包与扩展引用 |
| **src/Extensions/** | 宿主集成（ASP.NET Core、EF Core、FluentValidation 等） |
| **src/Infrastructures/** | Consul、Redis、RabbitMQ、Serilog、Polly、Castle、Windows 等实现 |
| **src/Helpers/** | 通用工具库 |
| **src/Models/** | 跨包共享模型 |
| **src/Tenon.sln** | 主解决方案入口，含 src 内项目及对 samples、test 的引用 |
| **samples/** | 可执行示例，入口为各 Program.cs |
| **test/** | 测试项目，入口为测试框架 |

## 入口与集成

- **库入口**：无；通过 NuGet 引用各 Tenon.* 包。
- **可执行入口**：仅 **samples/** 下各示例的 `Program.cs`。
- **单部件**：无多部件间集成路径；samples 引用 src 内项目。
