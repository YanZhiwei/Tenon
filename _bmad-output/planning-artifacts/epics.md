---
stepsCompleted: ["step-01-validate-prerequisites", "step-02-design-epics", "step-03-create-stories", "step-04-final-validation"]
inputDocuments:
  - "docs/architecture.md"
  - "docs/architecture-patterns.md"
  - "_bmad-output/project-context.md"
---

# Tenon - Epic 分解

## Overview

本文档基于现有项目文档与「将 samples 示例全部转换为单元测试」的范围说明，提炼需求并分解为可执行的 Epic 与 Story。未经过正式 PRD/Architecture 工作流，输入来自 docs/architecture.md、docs/architecture-patterns.md、_bmad-output/project-context.md。

## Requirements Inventory

### Functional Requirements

FR1: 为每个 sample 项目建立对应的单元测试覆盖，验证 sample 中演示的核心用法与配置可被测试验证。

FR2: 单元测试应覆盖 sample 中使用的 Tenon 库的扩展方法、选项配置与集成行为（在可单元化、不依赖真实外部服务的范围内）。

FR3: 测试项目命名与布局符合项目规范（位于 test/ 下，命名模式 {AssemblyName}Tests，与现有 test 项目一致）。

FR4: 覆盖范围内所有 sample：ConsulSample、ConsulRefitSample、FileUploadSample、FluentValidationSample、HangfireSample、MediatREventBusSample、MultiTenantSample、OpenApiSample、RabbitMq/RabbitMqDirectExchangeSample、SnowflakeSample、Windows/HookSample；若存在 Win32Sample 等其它 sample，一并纳入范围。

FR5: 当某 sample 对应的单元测试已建立且全部通过时，可删除该 sample 项目（以测试为唯一用法来源，避免重复维护）。

### NonFunctional Requirements

NFR1: 测试项目位于 test/ 目录，命名模式 {AssemblyName}Tests（如 Tenon.Caching.Redis → Tenon.Caching.RedisTests），与仓库现有测试项目一致。

NFR2: 测试项目仅引用对应的 src 项目与测试框架（xUnit/NUnit/MSTest 与仓库已有选择一致）；不引入生产 appsettings，使用测试专用配置或内存/模拟值。

NFR3: 从仓库根或 src 目录执行 dotnet test 可运行全部测试；不假设从测试项目目录启动；CI 从 ./src 执行 dotnet build。

NFR4: 测试代码遵循 project-context 中的 C# 与质量规范（Nullable、异步命名、复杂度 ≤10、无魔法数字等）。

### Additional Requirements

- 使用与仓库一致的测试框架（当前 test/ 下为 xUnit）；新测试项目保持同一框架。
- 目标框架 .NET 9.0；Nullable、ImplicitUsings 遵循 common.props。
- 不复制 sample 的 appsettings 到测试；对需配置的场景使用 InMemory、TestServer、Mock 或测试专用配置文件。
- 每个 sample 可对应一个或多个测试项目；若按所涉 src 包拆分为多个测试项目，命名与 test/ 下现有模式一致（如按库拆则 Tenon.XXXTests）。
- 架构模式：实现依赖抽象；测试主要针对扩展方法、选项绑定、DI 注册与可在进程内验证的集成行为；不要求 E2E 或真实外部服务。
- 在 scope 内的 samples 列表（用于 Epic/Story 拆分）：ConsulSample、ConsulRefitSample、FileUploadSample、FluentValidationSample、HangfireSample、MediatREventBusSample、MultiTenantSample、OpenApiSample、RabbitMqDirectExchangeSample、SnowflakeSample、Windows/HookSample；若有遗漏的 sample 目录可后续补充。
- 完成策略：某 sample 对应的 tests 已建立且单元测试全部通过后，可删除该 sample，以 test/ 为用法的唯一维护来源。

### FR Coverage Map

FR1: Epic 1 - 为每个 sample 建立对应单元测试并验证核心用法
FR2: Epic 1 - 测试覆盖扩展方法、选项配置与可单元化的集成行为
FR3: Epic 1 - 测试项目位于 test/、命名 {AssemblyName}Tests
FR4: Epic 1 - 覆盖范围内全部 sample（含列表与后续补充）
FR5: Epic 1 - 测试通过后可删除该 sample

## Epic List

### Epic 1: Samples 转单元测试与可选清理

为范围内每个 sample 建立对应单元测试，验证其核心用法与配置；测试全部通过后可删除该 sample，以 test/ 为用法唯一维护来源。

**FRs covered:** FR1, FR2, FR3, FR4, FR5

---

## Epic 1: Samples 转单元测试与可选清理

为范围内每个 sample 建立对应单元测试，验证其核心用法与配置；测试全部通过后可删除该 sample，以 test/ 为用法唯一维护来源。

### Story 1.1: ConsulSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 ConsulSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/ConsulSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖 Consul 服务发现核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/ConsulSample

### Story 1.2: ConsulRefitSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 ConsulRefitSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/ConsulRefitSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖 Consul + Refit 集成核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/ConsulRefitSample

### Story 1.3: FileUploadSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 FileUploadSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/FileUploadSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖文件上传与校验核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/FileUploadSample

### Story 1.4: FluentValidationSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 FluentValidationSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/FluentValidationSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖 FluentValidation 扩展与校验核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/FluentValidationSample

### Story 1.5: HangfireSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 HangfireSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/HangfireSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖 Hangfire 扩展与缓存等核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/HangfireSample

### Story 1.6: MediatREventBusSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 MediatREventBusSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/MediatREventBusSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖 MediatR 事件总线扩展核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/MediatREventBusSample

### Story 1.7: MultiTenantSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 MultiTenantSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/MultiTenantSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖多租户与仓储（Repository/EfCore）核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/MultiTenantSample

### Story 1.8: OpenApiSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 OpenApiSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/OpenApiSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖 OpenAPI 扩展核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/OpenApiSample

### Story 1.9: RabbitMqDirectExchangeSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 RabbitMqDirectExchangeSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/RabbitMq/RabbitMqDirectExchangeSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖 RabbitMQ Direct 交换机核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/RabbitMq/RabbitMqDirectExchangeSample

### Story 1.10: SnowflakeSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 SnowflakeSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/SnowflakeSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖分布式 ID（Snowflake）核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/SnowflakeSample

### Story 1.11: Windows HookSample 转单元测试与可选清理

As a 维护者/开发者，
I want 为 Windows/HookSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

**Acceptance Criteria:**

**Given** 仓库中存在 samples/Windows/HookSample 且 test/ 下尚无对应覆盖  
**When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖 Windows Form/Win32 Hook 核心用法的单元测试  
**Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
**And** 若团队选择应用 FR5，可在测试通过后删除 samples/Windows/HookSample
