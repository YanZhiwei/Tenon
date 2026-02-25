# 架构模式

> Step 3 产出 · 部件：main

- **项目类型**：library（多包类库集合）
- **架构风格**：**模块化类库套件**（按需组合，类似“榫卯”）

**结构要点**：

1. **分层**  
   - **Abstractions**：接口与抽象（Caching、DistributedId、Mapper、Serialization、EventBus、BloomFilter、MessageTracker 等）  
   - **实现包**：以 Tenon.* 或 Infrastructures 命名（如 Tenon.Caching.Redis、Tenon.Repository.EfCore）  
   - **Extensions**：对 ASP.NET Core、EF Core、FluentValidation、Hangfire 等的扩展与 DI 注册  
   - **Helpers/Models**：通用工具与领域模型

2. **依赖方向**  
   - 实现依赖抽象；Extensions 依赖实现与 ASP.NET Core/EF Core 等宿主栈  
   - 无 UI、无独立进程入口，作为 NuGet 被宿主应用引用

3. **宿主集成**  
   - 可选 ASP.NET Core（认证、OpenAPI、身份、配置）  
   - 可选 EF Core（MySql/Sqlite/Relational）  
   - 配置通过 Microsoft.Extensions.Configuration.* 与 Options 注入

4. **质量与发布**  
   - EnablePackageValidation、SourceLink、符号包；多语言资源；README 随包发布。

**总结**：面向 .NET 9 的模块化基础设施库，以抽象 + 多实现 + 扩展的方式提供缓存、锁、事件总线、仓储、身份、Consul、RabbitMQ 等能力。
