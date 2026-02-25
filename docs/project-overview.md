# 项目概览

> Document Project 工作流产出

## 项目名称与定位

- **名称**：Tenon  
- **标语**：像榫卯一样按需构建项目功能  
- **类型**：.NET 类库套件（library），以 NuGet 包形式提供基础设施与扩展能力

## 摘要

Tenon 提供缓存、分布式锁、布隆过滤器、分布式 ID、事件总线、消息队列、服务发现、仓储、认证与扩展等模块；通过抽象包 + 多实现 + ASP.NET Core/EF Core 扩展的方式，供业务应用按需引用。仓库为单体结构，单部件（main），无独立进程入口，入口仅存在于 samples 与 test。

## 技术栈摘要

| 类别     | 技术 |
|----------|------|
| 语言/运行时 | C# / .NET 9.0 |
| 数据访问   | Entity Framework Core 9，Repository 抽象 |
| 缓存/锁/布隆 | Redis（StackExchange）、内存缓存、Castle 拦截 |
| 服务发现   | Consul（含 Refit/Grpc 客户端） |
| 消息/事件  | RabbitMQ、DotNetCore.CAP、MediatR 扩展 |
| Web/认证  | ASP.NET Core、JWT、FluentValidation |
| 构建/发布  | MSBuild、NuGet、GitHub Actions |

## 架构与仓库结构

- **架构类型**：模块化类库套件（抽象 + 实现 + 扩展）
- **仓库结构**：单体，单部件；主源码在 `src/`，示例在 `samples/`，测试在 `test/`
- **详细**：[architecture.md](./architecture.md)、[source-tree-analysis.md](./source-tree-analysis.md)

## 文档索引

- 主入口与导航：[index.md](./index.md)
- 架构与技术：[architecture.md](./architecture.md)、[technology-stack.md](./technology-stack.md)、[architecture-patterns.md](./architecture-patterns.md)
- 开发与部署：[development-guide.md](./development-guide.md)、[deployment-guide.md](./deployment-guide.md)
- 现有文档与分类：[existing-documentation-inventory.md](./existing-documentation-inventory.md)、[project-classification.md](./project-classification.md)
