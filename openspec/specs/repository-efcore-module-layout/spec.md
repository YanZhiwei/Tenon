### Requirement: 程序集目录与职责分区

`Tenon.Repository.EfCore` 源代码 SHALL 按约定分区组织：

- **`Repositories/`**：`EfRepository`（含 partial 实现）、`IEfRepository`。
- **`Infrastructure/`**：`EfCoreTypeScanner`、`DisposeAction`、`DatabaseColumnLength` 等基础设施与杂项工具类型。
- **既有子目录**：`Auditing/`、`Configurations/`、`Extensions/`、`Interceptors/`、`Transaction/` SHALL 仅承载对应职责。
- **项目根目录（允许）**：可放置团队约定的高频入口或基础类型（例如 `EfEntity`、`IConcurrency`、`PagedResult`、`TenonDbContext`、`AbstractEntityTypeConfiguration`），以避免过度嵌套；新增类型 SHOULD 优先归入上述子目录之一，除非明确属于「根目录基础类型」约定。

#### Scenario: 仓储实现与接口有明确归属

- **WHEN** 贡献者添加或修改 `EfRepository` / `IEfRepository` 相关实现
- **THEN** 对应源文件 SHALL 位于 `Repositories/`（或经评审批准的等效路径）

#### Scenario: 基础设施类型不混入仓储目录

- **WHEN** 贡献者添加类型扫描、释放包装或列长常量等横切工具
- **THEN** 该类型 SHALL 位于 `Infrastructure/`（或经评审批准的等效路径），而非 `Repositories/`

### Requirement: 对外可见 API 稳定性

在采用「物理目录可重组、命名空间保持 `Tenon.Repository.EfCore` 及现有子命名空间」策略的前提下，重构 SHALL NOT 未经约定地改变对外公开的托管 API 行为与签名（含 `public` 类型与 `public` 成员）；`EfRepository` 的拆分 MUST 使用 `partial class` 且保持方法实现语义一致。

#### Scenario: 消费者编译与行为不变

- **WHEN** 下游项目引用本程序集并仅升级包含该重构的版本
- **THEN** 在未修改其代码的前提下，解决方案 SHALL 能成功编译，且仓储相关运行时行为（含 `SaveChanges` 触发时机）SHALL 与变更前一致

#### Scenario: Partial 拆分不改变类型身份

- **WHEN** 将 `EfRepository<TEntity>` 拆分为多个源文件
- **THEN** 该类型在元数据中 SHALL 仍为单一 CLR 类型，且 `public`/`protected` 成员集合 SHALL 与拆分前一致

### Requirement: 仓储实现文件粒度

`EfRepository<TEntity>` 的实现 SHALL 拆分为多个 partial 文件，且每个文件 SHALL 主要承载单一职责（例如查询与变更分离），以降低单文件行数并便于审阅。

#### Scenario: 查询与变更分离可定位

- **WHEN** 审阅者需要修改仅影响查询或仅影响写入的逻辑
- **THEN** 其 SHALL 能在命名清晰的 partial 文件中定位主要改动区域，而无需在单一大文件中滚动查找
