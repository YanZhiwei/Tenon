# 部署指南

> Step 6 产出 · 基于 CI/CD 与仓库现状

## 适用对象

本仓库为 **.NET 类库集合**，无独立运行态部署；“部署”指 **CI 构建** 与 **NuGet 包发布**。

## CI/CD 流水线

### 1. 构建（Build）

- **工作流**：`.github/workflows/dotnet.yml`
- **触发**：推送到或 PR 进入 `dev` 分支
- **步骤**：Ubuntu，.NET 9.0.x，在 `./src` 下 `dotnet restore`、`dotnet build --no-restore`
- **用途**：验证提交可成功编译

### 2. 发布到 NuGet（Publish）

- **工作流**：`.github/workflows/nuget_publish.yml`
- **触发**：在 `dev` 分支上手动执行（workflow_dispatch）
- **步骤**：
  1. Windows 环境，.NET 9.0.x
  2. 构建：`dotnet build --configuration Release` 针对 `src/Tenon.sln`
  3. 打包：`dotnet pack` 输出到临时目录
  4. 上传构建产物为 artifact
  5. 下游 job 下载 artifact，对每个 `.nupkg` 执行 `dotnet nuget push` 到 NuGet.org
- **排除**：路径或文件名包含 `Sample`、`CleanArchitecture`、`Tests` 的 nupkg 不推送
- **机密**：需在仓库配置 **NUGET_API_KEY**（NuGet.org API Key）

## 本地“发布”测试

```bash
cd src
dotnet pack Tenon.sln -c Release -o ../nupkgs
# 产出在 ../nupkgs，可按需推送到本地或私有 NuGet 源
```

## 基础设施

- 未使用 Docker/K8s/Helm；构建与发布均在 GitHub Actions 内完成。
- 使用 NuGet.org 作为发布目标；若改用私有源，需修改 workflow 中的 `--source` 与认证方式。
