## ADDED Requirements

### Requirement: 程序集目录与职责分区

`Tenon.Repository.EfCore` 源代码 SHALL 按约定文件夹组织：`Repositories/`（仓储与分页结果类型）、`Context/`（`DbContext` 与实体配置基类）、`Entities/`（EF 实体基类与并发相关接口）、`Infrastructure/`（类型扫描与杂项工具类型）；已有的 `Auditing/`、`Configurations/`、`Extensions/`、`Interceptors/`、`Transaction/` SHALL 保留并仅承载对应职责。新增类型 MUST 归入上述分区之一，避免在程序集根目录无节制堆积。

#### Scenario: 新仓储相关类型有明确归属

- **WHEN** 贡献者添加与 `EfRepository` 或 `IEfRepository` 同层的实现或辅助类型
- **THEN** 该类型 SHALL 位于 `Repositories/`（或设计文档批准的等效路径），而非程序集根目录

#### Scenario: DbContext 与配置基类集中

- **WHEN** 维护者查找 `TenonDbContext` 或 `AbstractEntityTypeConfiguration`
- **THEN** 二者 SHALL 位于 `Context/`（或设计文档批准的等效路径）

### Requirement: 对外可见 API 稳定性

在采用“物理目录重组、命名空间保持 `Tenon.Repository.EfCore` 及现有子命名空间”策略的前提下，本变更 SHALL NOT 改变对外公开的托管 API 行为与签名（含 `public` 类型与 `public` 成员）；`EfRepository` 的拆分 MUST 使用 `partial class` 且保持方法实现语义一致。

#### Scenario: 消费者编译与行为不变

- **WHEN** 下游项目引用本程序集并仅升级包含本变更的版本
- **THEN** 在未修改其代码的前提下，解决方案 SHALL 能成功编译，且仓储相关运行时行为（含 `SaveChanges` 触发时机）SHALL 与变更前一致

#### Scenario: Partial 拆分不改变类型身份

- **WHEN** 将 `EfRepository<TEntity>` 拆分为多个源文件
- **THEN** 该类型在元数据中 SHALL 仍为单一 CLR 类型，且 `public`/`protected` 成员集合 SHALL 与拆分前一致

### Requirement: 仓储实现文件粒度

`EfRepository<TEntity>` 的实现 SHALL 拆分为多个 partial 文件，且每个文件 SHALL 主要承载单一职责（例如查询与变更分离），以降低单文件行数并便于审阅。

#### Scenario: 查询与变更分离可定位

- **WHEN** 审阅者需要修改仅影响查询或仅影响写入的逻辑
- **THEN** 其 SHALL 能在命名清晰的 partial 文件中定位主要改动区域，而无需在单一大文件中滚动查找
