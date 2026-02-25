# 项目分类报告

> 由 Document Project 工作流 Step 1 生成 · 深度扫描模式

---

## 1. 项目结构 (Project Structure)

- **仓库类型**：单体（Monolith）  
  单一代码库，无独立 client/server 或多应用分区。

- **根目录**：`d:\Repos\Github\YanZhiwei\Tenon`

- **顶层目录**：
  - **src/** — 主源码：类库与基础设施（Abstractions、Extensions、Infrastructures、Helpers、Models、Tenon.* 等）
  - **samples/** — 示例项目（Consul、RabbitMq、Snowflake、MultiTenant 等）
  - **test/** — 单元/集成测试项目
  - **docs/** — 项目知识/文档输出目录

- **主解决方案**：`src/Tenon.sln`（包含大量 .csproj 引用）

- **技术标记**：
  - 大量 `*.csproj`（.NET 项目）
  - `TargetFramework`: net9.0
  - 无 `package.json`、无前端 SPA 根目录，无独立 API 宿主根目录

---

## 2. 项目部件元数据 (Project Parts Metadata)

| 字段 | 值 |
|------|-----|
| **part_id** | main |
| **project_type_id** | library |
| **display_name** | Tenon（主库） |
| **root_path** | `d:\Repos\Github\YanZhiwei\Tenon` |
| **说明** | 多 NuGet 风格类库的集合，按需组合使用（如榫卯）。包含抽象、扩展、基础设施、仓储、缓存、分布式锁、事件总线等。 |

**文档要求（来自 documentation-requirements · library）**  
- requires_api_scan: false  
- requires_data_models: false  
- requires_state_management: false  
- requires_ui_components: false  
- requires_deployment_config: false  
- critical_directories: src/; lib/; dist/; pkg/; build/; target/  
- key_file_patterns: *.csproj; pom.xml 等  

---

**分类结论**：单体、单部件、类型 **library**；主技术栈 **.NET 9.0 / C#**。
