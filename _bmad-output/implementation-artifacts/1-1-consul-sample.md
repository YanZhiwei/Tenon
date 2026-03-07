# Story 1.1: ConsulSample 转单元测试与可选清理

Status: ready-for-dev

<!-- 可选：在 dev-story 前运行 validate-create-story 做质量检查。 -->

## Story

As a 维护者/开发者，
I want 为 ConsulSample 建立对应单元测试并验证其核心用法与配置，
So that 该能力由 test/ 覆盖，测试通过后可删除该 sample，避免重复维护。

## Acceptance Criteria

1. **Given** 仓库中存在 samples/ConsulSample 且 test/ 下尚无对应覆盖  
   **When** 在 test/ 下建立符合 NFR1–NFR4 的测试项目并编写覆盖 Consul 服务发现核心用法的单元测试  
   **Then** 所有新增单元测试通过（dotnet test），且测试项目命名与布局符合项目规范  
   **And** 若团队选择应用 FR5，可在测试通过后删除 samples/ConsulSample

## Tasks / Subtasks

- [ ] Task 1 (AC: #1) — 在 test/ 下新建测试项目
  - [ ] 新建 `test/Tenon.Infra.ConsulTests`，命名符合 {AssemblyName}Tests
  - [ ] 引用 `Tenon.Infra.Consul`、`Tenon.Abstractions`（若需）、xUnit，目标 net9.0
  - [ ] 不引用 production appsettings，使用测试用配置或内存/模拟
- [ ] Task 2 (AC: #1) — 覆盖 Consul 扩展与配置
  - [ ] 测试 `ServiceCollectionExtension.AddConsul`：options 绑定、null 校验（ArgumentNullException）
  - [ ] 测试 `ConsulOptions` 从 IConfigurationSection 正确绑定
  - [ ] 测试 `HostExtension.UseConsulRegistrationCenter` 的 null 校验（host、getServiceAddressHandle）
  - [ ] 在可单元化范围内覆盖 RegistrationProvider / 服务注册相关逻辑（可用 mock IWebApiServiceDescriptor、IHost）
- [ ] Task 3 (AC: #1) — 验证与收尾
  - [ ] 从仓库根或 src 执行 `dotnet test` 确保新测试通过
  - [ ] 确认测试代码符合 project-context（Nullable、异步命名、复杂度等）
  - [ ] （可选）若团队采纳 FR5：测试全部通过后删除 samples/ConsulSample

## Dev Notes

- **架构约束**：实现依赖抽象；测试仅引用 Tenon.Infra.Consul 及所需抽象（Tenon.Abstractions）。不引入 samples 的 appsettings；使用 in-memory config 或 Mock。
- **ConsulSample 核心用法**：`AddConsul(services, configuration.GetSection("Consul"))`、`UseConsulRegistrationCenter(host, getServiceAddressHandle)`；依赖 `IWebApiServiceDescriptor`（来自 Tenon.Abstractions）、`WebServiceDescriptor.CreateInstance(assembly)`。
- **待测源码**：`src/Infrastructures/Tenon.Infra.Consul/Extensions/ServiceCollectionExtension.cs`（AddConsul、AddConsulDiscovery）、`HostExtension.cs`（UseConsulRegistrationCenter）、`Configurations/ConsulOptions.cs`。Sample 未使用 AddConsulDiscovery，但同库能力可一并覆盖。
- **测试框架**：与仓库一致使用 xUnit（见 test/Tenon.Caching.InMemoryTests）。CI 从 `./src` 执行 `dotnet build`；`dotnet test` 从仓库根或 src 或指定测试项目路径运行。
- **依赖外部服务**：当前仅包含不依赖 Consul 服务的单元测试（options 绑定、null 校验）。需要真实 Consul 的集成测试暂不编写，待准备好 Consul 环境后再补充。

### Project Structure Notes

- 测试项目路径：`test/Tenon.Infra.ConsulTests/`，与现有 `Tenon.Caching.InMemoryTests`、`Tenon.Caching.Interceptor.CastleTests` 并列。
- 命名：{AssemblyName}Tests → Tenon.Infra.ConsulTests；与 project-context 中 “Tenon.Caching.Redis → Tenon.Caching.RedisTests” 一致。
- 不复制 samples/ConsulSample 的 appsettings；Options 测试使用 ConfigurationBuilder + in-memory 字典或 test-specific json。

### References

- [Source: _bmad-output/project-context.md] — Testing Rules（test/ 布局、命名、xUnit、不引入生产 appsettings）、Code Quality、Async/Exceptions。
- [Source: _bmad-output/planning-artifacts/epics.md] — Epic 1、Story 1.1、FR1–FR5、NFR1–NFR4。
- [Source: samples/ConsulSample/Program.cs] — AddConsul、UseConsulRegistrationCenter、IWebApiServiceDescriptor 用法。
- [Source: src/Infrastructures/Tenon.Infra.Consul/] — ServiceCollectionExtension、HostExtension、ConsulOptions。

## Dev Agent Record

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List

### File List
