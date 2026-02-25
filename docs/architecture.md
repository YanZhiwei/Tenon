# 架构文档

> Step 8 产出 · 单部件 main（library）

## 概述

Tenon 是以 .NET 9 为主的**模块化类库套件**，采用“榫卯”式按需组合：通过抽象包定义能力，通过实现包与扩展包提供具体实现和宿主集成，供业务应用引用。

## 技术栈摘要

- **语言与运行时**：C# / .NET 9.0
- **核心能力**：缓存（内存/Redis）、分布式锁、布隆过滤器、分布式 ID（Snowflake）、事件总线（Cap）、消息（RabbitMQ）、服务发现（Consul）、仓储（EF Core）、认证/授权（ASP.NET Core）、对象映射（AutoMapper/Mapster）、校验（FluentValidation）等
- **构建与包**：MSBuild、NuGet、version.props/common.props/nuget.props 统一版本与元数据

详见 [technology-stack.md](./technology-stack.md)。

## 架构风格

- **类型**：library（多包库）
- **模式**：抽象 + 多实现 + 扩展；依赖方向为 实现 → 抽象，扩展 → 实现/宿主
- **入口**：库无进程入口；可执行入口仅在 **samples/** 下各示例的 Program.cs

详见 [architecture-patterns.md](./architecture-patterns.md)。

## 源码结构

- **关键目录**：`src/`（含 Abstractions、Extensions、Infrastructures、Helpers、Models 及各 Tenon.* 包）、`samples/`、`test/`
- **解决方案**：`src/Tenon.sln` 为主入口
- **单部件**：无多应用/多服务分区，无部件间集成文档需求

详见 [source-tree-analysis.md](./source-tree-analysis.md)。

## 开发与部署

- **开发**：.NET 9.0 SDK，在 `src` 下 restore/build，在 samples 或 test 下运行/测试
- **部署**：仅 CI 构建与 NuGet 发布；无运行时部署

详见 [development-guide.md](./development-guide.md)、[deployment-guide.md](./deployment-guide.md)。

## 测试策略

- 测试项目位于 **test/**，使用 xUnit 等 .NET 测试框架
- CI 在 `src` 下执行 build，测试可通过 `dotnet test` 在本地或 CI 中运行
