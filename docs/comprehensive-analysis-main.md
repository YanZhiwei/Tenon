# 综合分析（main）

> Step 4 产出 · 项目类型：library，仅做模式扫描

## 配置管理 (config_patterns)

- **位置**：根目录无 appsettings；各 **samples/** 与 **test/** 下存在 `appsettings.json` / `appsettings.Development.json`
- **方式**：Microsoft.Extensions.Configuration 系，通过 DI 注入 IConfiguration / IOptions
- **库内**：Extensions 中部分带 appsettings.json（如 Tenon.Serilog.AspNetCore.Extensions）用于默认配置

## 入口与引导 (entry_point_patterns)

- **库本身**：无进程入口，仅被引用
- **可执行入口**：**samples/** 下各示例的 `Program.cs`（ConsulSample、SnowflakeSample、OpenApiSample、HangfireSample、MultiTenantSample、FileUploadSample、FluentValidationSample、ConsulRefitSample、RabbitMqDirectExchangeSample、MediatREventBusSample；Windows 下 HookSample、Win32Sample）

## 共享与复用 (shared_code_patterns)

- **src/Abstractions/**、**src/Extensions/Tenon.Extensions/**、**src/Helpers/**、**src/Models/** 为各实现包与示例共享
- **common.props / version.props / nuget.props** 在 src 下统一版本与包元数据

## 异步与事件 (async_event_patterns)

- **DotNetCore.CAP**（Tenon.EventBus.Cap）、**MediatR** 扩展（Tenon.MediatR.Extensions.EventBus）、**RabbitMQ**（Tenon.Infra.RabbitMq）用于事件/消息

## CI/CD (ci_cd_patterns)

| 工作流 | 触发 | 说明 |
|--------|------|------|
| `.github/workflows/dotnet.yml` | push/PR → dev | 使用 .NET 9.0.x，在 `./src` 下 restore + build |
| `.github/workflows/nuget_publish.yml` | workflow_dispatch 于 dev | Windows 构建 Release、pack 到临时目录、上传 artifact；下游 job 下载并 `dotnet nuget push` 到 NuGet.org（排除含 Sample/Tests 的 nupkg），需 NUGET_API_KEY |

## 本地化 (localization_patterns)

- **common.props** 中 `SatelliteResourceLanguages`: zh-Hans、zh-Hant、en

---

**结论**：library 无 API/数据模型/UI 扫描需求；配置与入口以 samples/test 为主，CI 为构建 + NuGet 发布。
