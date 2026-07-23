# 版本与发布规范

本文档规定 ZenUI.Wpf 的版本号、分支、Tag、NuGet 和 GitHub Release 操作。后续发布应以本文档为准。

## 核心原则

1. NuGet 包版本遵循 [Semantic Versioning 2.0.0](https://semver.org/lang/zh-CN/)。
2. 每个公开发布到 NuGet.org 的版本必须有且只有一个同版本 Git Tag。
3. Tag 用于永久标记发布代码；分支用于继续开发。不得因为发布一个版本就机械地创建一个新分支。
4. NuGet.org 上已经发布的包版本不可覆盖。发现问题时发布新版本，不得移动原 Tag 或重写原发布提交。
5. 正式包必须从已经提交且工作区干净的代码生成，包内 Source Link 提交号必须与 Tag 指向一致。
6. 只有本地验证和远程 CI 全部通过后才能发布。

## 版本号

包版本格式：

```text
MAJOR.MINOR.PATCH[-PRERELEASE]
```

| 类型 | 示例 | 规则 |
| --- | --- | --- |
| CI 构建 | `0.1.0-ci.123` | 仅用于持续集成构件，不作为正式 Release |
| 预览版 | `0.1.0-preview.2` | 功能和公共 API 仍可能调整 |
| 候选版 | `0.1.0-rc.1` | 功能基本冻结，仅处理发布阻断问题 |
| 稳定版 | `0.1.0` | 当前开发阶段可供常规使用的版本 |
| 修复版 | `0.1.1` | 仅包含向后兼容的缺陷修复 |
| 功能版 | `0.2.0` | 增加功能；在 `0.x` 阶段仍允许谨慎调整公共 API |
| 主版本 | `1.0.0`、`2.0.0` | `1.0.0` 起承诺公共 API 稳定；之后的破坏性修改必须提升 MAJOR |

预发布后缀统一使用小写和点号序号：

```text
preview.1
preview.2
rc.1
```

不要混用 `preview1`、`Preview-1`、`beta01` 等格式。

## 项目中的版本来源

版本配置位于 `src/ZenUI.Wpf/ZenUI.Wpf.csproj`：

- `VersionPrefix`：下一目标版本的 `MAJOR.MINOR.PATCH`。
- `VersionSuffix`：预发布后缀；稳定版本应删除或清空。
- `AssemblyVersion`：同一兼容发布线保持稳定。
- `FileVersion`：四段式文件版本，随发布的数字版本更新；预发布后缀不写入该字段。

当前 `0.1.x` 发布线使用：

```xml
<VersionPrefix>0.1.0</VersionPrefix>
<VersionSuffix>preview.1</VersionSuffix>
<AssemblyVersion>0.1.0.0</AssemblyVersion>
<FileVersion>0.1.0.0</FileVersion>
```

进入 `1.x` 后，整个兼容发布线原则上保持 `AssemblyVersion=1.0.0.0`；只有进入新的破坏性主版本时才改为 `2.0.0.0`。

CI 在 `.github/workflows/ci.yml` 中生成 `0.1.0-ci.<run_number>` 包。调整目标发布线时，必须同步修改项目版本、CI 包版本和 `CHANGELOG.md`。

## 分支策略

### 长期分支

- `main`：唯一默认开发分支，始终代表下一个版本。
- `release/<line>`：仅在需要并行维护旧发布线时创建，例如 `release/0.1`、`release/1.x`。

### 短期分支

- `feature/<name>`：新功能。
- `fix/<name>`：缺陷修复。
- `docs/<name>`：文档。
- `chore/<name>`：构建、CI 和仓库维护。

短期分支合并后应删除。

### 何时创建 Release 分支

以下情况才创建：

- `main` 已进入 `0.2.0` 开发，但仍需发布 `0.1.1`。
- `main` 已进入 `2.0.0` 开发，但仍需维护 `1.x`。
- 稳定版发布前需要冻结一条发布线，只接受阻断修复。

以下情况不创建：

- 发布 `preview.1`、`preview.2` 或普通修复版时，`main` 仍可直接承载开发。
- 仅为了保存某个版本的源代码。Tag 已承担这一职责。
- 为每个 Patch 或每个 NuGet 包创建同名分支。

需要维护旧版本时：

```powershell
git switch -c release/0.1 v0.1.0
git cherry-pick <fix-commit>
```

修复验证完成后，在该分支创建 `v0.1.1` Tag。修复也应根据适用性合并或移植回 `main`。

## Tag 规范

所有公开包使用以下 Tag 格式：

```text
v<NuGet PackageVersion>
```

示例：

```text
v0.1.0-preview.1
v0.1.0
v0.1.1
v1.0.0
```

发布后禁止：

- 移动或强制覆盖 Tag。
- 删除 Tag 后用同名 Tag 指向其他提交。
- 用同一个 NuGet 版本重新打包不同内容。

手动发布时优先创建带说明的 Tag：

```powershell
git tag -a v0.1.0-preview.2 -m "ZenUI.Wpf 0.1.0-preview.2"
git push origin v0.1.0-preview.2
```

由 GitHub Release 或后续发布自动化创建的轻量 Tag 也可以接受，但必须准确指向最终发布提交。

## 标准发布流程

### 1. 准备版本

1. 确认目标版本符合语义化版本规则。
2. 修改 `VersionPrefix`、`VersionSuffix`、`AssemblyVersion` 和 `FileVersion`。
3. 将本次改动从 `CHANGELOG.md` 的 `Unreleased` 整理到带日期的版本章节。
4. 确认 README、安装命令、包元数据和发布说明正确。

### 2. 本地验证

```powershell
dotnet restore ZenUI.Wpf.slnx
dotnet build ZenUI.Wpf.slnx -c Release --no-restore
dotnet test tests/ZenUI.Wpf.Tests/ZenUI.Wpf.Tests.csproj -c Release --no-build --no-restore
dotnet list ZenUI.Wpf.slnx package --vulnerable --include-transitive
```

要求：

- 构建 0 警告、0 错误。
- 两个目标框架测试全部通过。
- 没有已知易受攻击的直接或传递依赖。
- `git diff --check` 通过。

### 3. 提交并等待 CI

```powershell
git status
git add <reviewed-files>
git commit -m "准备 ZenUI.Wpf <version> 发布"
git push origin main
```

必须先审查完整 diff。推送后等待 GitHub Actions 的 Restore、Build、Test、视觉快照、Pack 和构件上传全部成功。

### 4. 创建 Tag

确认工作区干净、本地 `HEAD` 与 `origin/main` 一致，并且该提交的 CI 成功后创建 Tag：

```powershell
git tag -a v<version> -m "ZenUI.Wpf <version>"
git push origin v<version>
```

### 5. 从最终提交打包

Tag 创建后，在该提交上重新构建和打包。不要复用提交前生成的旧包，否则 Source Link 可能记录错误提交。

```powershell
dotnet build src/ZenUI.Wpf/ZenUI.Wpf.csproj -c Release --no-restore
dotnet pack src/ZenUI.Wpf/ZenUI.Wpf.csproj -c Release --no-build -o artifacts/packages
```

发布前检查 `.nupkg`：

- 包 ID 和版本正确。
- MIT License、README、Logo、目标框架和 XML 文档存在。
- `.snupkg` 已生成。
- Repository URL 指向 `XiaQueNet/ZenUI-WPF`。
- Repository Commit 与 Tag 指向一致。
- 发布目录中没有可能误传的旧版本包。

### 6. 发布 NuGet

API Key 必须：

- 仅授予 `ZenUI.Wpf` 所需的 Push 权限。
- 设置合理的有效期。
- 不写入源码、脚本、日志、Issue 或聊天。
- 泄露后立即撤销或刷新。

示例：

```powershell
$secureKey = Read-Host "NuGet API Key" -AsSecureString
$env:NUGET_API_KEY = [Net.NetworkCredential]::new("", $secureKey).Password

dotnet nuget push "artifacts/packages/ZenUI.Wpf.<version>.nupkg" `
  --api-key $env:NUGET_API_KEY `
  --source "https://api.nuget.org/v3/index.json"

Remove-Item Env:\NUGET_API_KEY
```

不要单独手工修改 `.nupkg`。若上传后发现问题，应修复源码并发布新版本。

### 7. 创建 GitHub Release

- Tag：`v<version>`。
- 标题：`ZenUI.Wpf <version>`。
- 预发布版本必须勾选 **Set as a pre-release**。
- 发布说明使用中文，包含主要变更、安装命令、兼容性提示和破坏性修改。
- 上传对应的 `.nupkg` 与 `.snupkg`。

安装命令：

```powershell
dotnet add package ZenUI.Wpf --version <version>
```

### 8. 发布后验证

- GitHub Release 可匿名访问。
- Tag 指向正确提交。
- 两个附件可以下载且大小正确。
- NuGet 页面完成索引，版本、Logo、README、License、依赖和项目链接正确。
- 使用干净的临时 WPF 项目安装并构建该版本。
- `main` 工作区保持干净。

NuGet 索引通常需要几分钟。在索引完成前页面可能暂时返回 404。

## 热修复

如果 `main` 与已发布版本仍兼容，直接在 `main` 修复并发布 Patch。

如果 `main` 已包含下一发布线的破坏性修改：

1. 从需要修复的稳定 Tag 创建 `release/<line>`。
2. 仅 cherry-pick 必要修复。
3. 完整运行构建、测试、漏洞和打包检查。
4. 发布新的 Patch 版本和 Tag。
5. 将修复同步回 `main`。

绝不通过覆盖旧 NuGet 包或移动旧 Tag 的方式修复。

## 当前发布记录

| 版本 | Tag | 提交 | 类型 |
| --- | --- | --- | --- |
| [`0.1.0-preview.1`](https://www.nuget.org/packages/ZenUI.Wpf/0.1.0-preview.1) | [`v0.1.0-preview.1`](https://github.com/XiaQueNet/ZenUI-WPF/releases/tag/v0.1.0-preview.1) | `f2cfe71` | Preview |

当前版本不创建单独 Release 分支；后续开发继续在 `main`，每次公开发布创建新 Tag。

## 后续自动化目标

当前发布仍包含人工操作。后续应增加 Tag 驱动的发布工作流：

1. 推送 `v*` Tag。
2. 自动验证 Tag 与项目版本一致。
3. 自动构建、测试、漏洞扫描和打包。
4. 使用 NuGet Trusted Publishing 或最小权限密钥发布。
5. 自动创建 GitHub Release 并上传附件。

自动化完成前，严格执行本文档的人工检查清单。
