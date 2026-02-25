# 技术栈

> Step 3 产出 · 部件：main

| 类别 | 技术 | 版本 | 说明 |
|------|------|------|------|
| 语言 | C# | latest | LangVersion=latest，可空引用启用 |
| 运行时 | .NET | 9.0 | TargetFramework net9.0，全库统一 |
| 包管理 | NuGet / MSBuild | - | 通过 .csproj + Directory.Build.props 管理 |
| Web 框架 | ASP.NET Core | 9.0 | AspNetCore.*、认证/身份扩展 |
| 数据访问 | Entity Framework Core | 9.0 | Repository + EfCore 抽象，支持 MySql/Sqlite/Relational |
| 分布式缓存 | Redis | - | StackExchange.Redis、多种缓存/布隆/锁实现 |
| 服务发现/治理 | Consul | - | Infra.Consul、Refit/Grpc 客户端 |
| 消息/事件总线 | RabbitMQ + DotNetCore.CAP | CAP 8.3.x | 事件总线抽象 + Cap 实现 |
| 分布式 ID | Snowflake | - | Tenon.DistributedId.Snowflake |
| 对象映射 | AutoMapper / Mapster | 14 / 已引用 | Tenon.Mapper.* |
| 校验 | FluentValidation | 11.x | AspNetCore 扩展集成 |
| AOP/拦截 | Castle.Core | 5.1 | 异步拦截器，缓存拦截等 |
| HTTP 客户端 | Refit | 8.0 | Consul Refit 客户端 |
| 日志 | Serilog | - | Tenon.Infra.Serilog |
| 后台任务 | Hangfire | - | Tenon.Hangfire.Extensions |
| 构建/质量 | Microsoft.SourceLink | 1.1.1 | GitHub/GitLab 源码链接，包校验启用 |

**版本与规范**：version.props 管理主版本（0.0.1-alpha），common.props 统一作者、产品、多语言资源（zh-Hans/zh-Hant/en）、MIT 许可。
