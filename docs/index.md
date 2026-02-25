# Tenon 项目文档索引

> 主入口，供 AI 与人工快速定位文档

## 项目概览

- **类型**：单体，单部件（main），library
- **主语言**：C#
- **架构**：模块化类库套件（抽象 + 实现 + 扩展）

## 快速参考

- **技术栈**：[technology-stack.md](./technology-stack.md)
- **架构模式**：[architecture-patterns.md](./architecture-patterns.md)
- **源码结构**：[source-tree-analysis.md](./source-tree-analysis.md)、[critical-folders-summary.md](./critical-folders-summary.md)

## 本工作流生成的文档

- [项目概览](./project-overview.md)
- [架构文档](./architecture.md)
- [技术栈](./technology-stack.md)
- [架构模式](./architecture-patterns.md)
- [源码树分析](./source-tree-analysis.md)
- [关键文件夹摘要](./critical-folders-summary.md)
- [开发指南](./development-guide.md)
- [部署指南](./deployment-guide.md)
- [综合分析（main）](./comprehensive-analysis-main.md)
- [现有文档清单](./existing-documentation-inventory.md)
- [用户提供的上下文](./user-provided-context.md)
- [项目分类报告](./project-classification.md)

## 已有项目文档

- [MultiTenant](./MultiTenant.md)

## 入门

1. 安装 .NET 9.0 SDK，克隆仓库，在 `src` 下执行 `dotnet restore`、`dotnet build`。
2. 运行示例：进入 `samples/` 下任意示例目录，执行 `dotnet run`。
3. 测试：在 `src` 或 `test` 下执行 `dotnet test`。
4. 详细步骤见 [development-guide.md](./development-guide.md)。

## 棕地 PRD / AI 辅助开发

- 规划新功能或写棕地 PRD 时，可将本索引（或 [project-overview.md](./project-overview.md)、[architecture.md](./architecture.md)）作为上下文输入。
- 库无 API/数据模型/UI 清单文档；集成与用法以各包 README 与 samples 为准。
