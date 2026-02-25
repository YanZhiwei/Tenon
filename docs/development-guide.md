# 开发指南

> Step 6 产出 · 部件：main

## 前置条件

- **.NET SDK**：9.0.x（与 CI 及各 csproj 的 TargetFramework 一致）
- **IDE**：Visual Studio 2022 或 VS Code + C# 扩展（可选）
- **Git**：仓库克隆与 SourceLink 需 Git

## 环境与配置

- 根目录无全局 appsettings；**samples/** 与 **test/** 下使用 `appsettings.json` / `appsettings.Development.json`。
- 本地开发如需 Redis/Consul/RabbitMQ 等，请在对应 sample 的 appsettings 中配置连接信息。

## 克隆与还原

```bash
git clone https://github.com/YanZhiwei/Tenon.git
cd Tenon
cd src
dotnet restore
```

## 构建

```bash
# 在仓库根目录
cd src
dotnet build
# 或指定解决方案
dotnet build Tenon.sln
```

## 测试

```bash
cd src
dotnet test
# 或指定测试项目
dotnet test ../test/Tenon.Repository.EfCore.Tests/Tenon.Repository.EfCore.Tests.csproj
```

## 运行示例

```bash
cd samples/ConsulSample   # 或其他 sample
dotnet run
```

## 包与版本

- 版本由 **src/version.props** 统一管理（当前 0.0.1-alpha）。
- 公共属性与 NuGet 元数据：**src/common.props**、**src/nuget.props**。
- 发布时在 Release 配置下会生成 NuGet 包（参见部署说明）。
