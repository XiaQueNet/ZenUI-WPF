# 版本与发布规范

本文档规定 `ZenUI.Wpf` 与 `ZenUI.Wpf.Converters` 的版本号、分支、Tag、NuGet 和 GitHub Release 操作。后续发布应以本文档为准。

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

版本配置分别位于：

- `src/ZenUI.Wpf/ZenUI.Wpf.csproj`
- `src/ZenUI.Wpf.Converters/ZenUI.Wpf.Converters.csproj`

两个包独立决定是否发布；只有包含实际代码、API、资源、文档或包元数据变更的包才提升版本并上传 NuGet：

- `VersionPrefix`：下一目标版本的 `MAJOR.MINOR.PATCH`。
- `VersionSuffix`：预发布后缀；稳定版本应删除或清空。
- `AssemblyVersion`：同一兼容发布线保持稳定。
- `FileVersion`：四段式文件版本，随发布的数字版本更新；预发布后缀不写入该字段。

当前 `0.1.x` 发布线分别使用：

```xml
<!-- ZenUI.Wpf -->
<VersionPrefix>0.1.0</VersionPrefix>
<VersionSuffix>preview.11</VersionSuffix>
<AssemblyVersion>0.1.0.0</AssemblyVersion>
<FileVersion>0.1.0.0</FileVersion>

<!-- ZenUI.Wpf.Converters -->
<VersionPrefix>0.1.0</VersionPrefix>
<VersionSuffix>preview.11</VersionSuffix>
<AssemblyVersion>0.1.0.0</AssemblyVersion>
<FileVersion>0.1.0.0</FileVersion>
```

进入 `1.x` 后，整个兼容发布线原则上保持 `AssemblyVersion=1.0.0.0`；只有进入新的破坏性主版本时才改为 `2.0.0.0`。

CI 在 `.github/workflows/ci.yml` 中仍为两个包生成相同版本的 `0.1.0-ci.<run_number>` 构件，用于持续验证，不代表两个正式包必须同步发布。准备公开版本时，只修改本次实际发布的项目版本，并同步更新 `CHANGELOG.md` 与对应安装文档；未变更的包保持上一公开版本。

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
4. 更新 Gallery 属性引入版本：在 `samples/ZenUI.Wpf.Gallery/Controls/PropertyTable.xaml.cs` 中，将本次发布涉及的 `UnreleasedPropertyVersion` 条目统一替换为实际版本号字符串（例如 `"0.1.0-preview.9"`）。保留 `UnreleasedPropertyVersion = "未发布"` 常量不变，也不得修改不属于本次发布的未发布项。
5. 搜索该文件中剩余的 `UnreleasedPropertyVersion` 条目，逐项确认它们确实不属于本次发布，避免新增或重命名的公共属性漏记引入版本。
6. 确认 README、安装命令、包元数据和发布说明正确。

### 2. 本地验证

```powershell
dotnet restore ZenUI.Wpf.slnx
dotnet build ZenUI.Wpf.slnx -c Release --no-restore
dotnet test ZenUI.Wpf.slnx -c Release --max-parallel-test-modules 1 --no-build --no-restore
foreach ($framework in @('net5.0-windows', 'net6.0-windows', 'net7.0-windows')) {
    dotnet run --project tests/ZenUI.Wpf.ModernCompatibilityTests/ZenUI.Wpf.ModernCompatibilityTests.csproj -c Release -f $framework --no-build --no-restore
}
dotnet list ZenUI.Wpf.slnx package --vulnerable --include-transitive
```

要求：

- 构建 0 警告、0 错误。
- .NET Framework 4.6.2～4.8.1 与 .NET 8～10 完整测试矩阵全部通过。
- .NET 5、6、7 兼容性契约测试全部通过。
- 公共属性、依赖属性、附加属性和路由事件的命名与类型配对审计通过。
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

推送 `v*` Tag 会触发 `.github/workflows/release.yml`。发布工作流会先复用 `.github/workflows/ci.yml` 完成构建、完整测试矩阵和兼容性测试，随后进入 GitHub Environment `生产环境`；若该环境配置了 Required reviewers，需审批后才会继续。

### 5. 从最终提交打包

CD 会根据 Tag 版本选择 `PackageVersion` 完全一致的项目，并使用唯一发布脚本从该 Tag 的临时 Git Worktree 重新还原、构建、测试、检查依赖并打包。不要复用提交前生成的旧包，否则 Source Link 可能记录错误提交。

例如，推送 `v0.1.0-preview.8` 时，只有项目版本为 `0.1.0-preview.8` 的包会进入发布目录。若没有项目版本与 Tag 匹配，发布任务立即失败；两个项目版本都匹配时会同时发布。

需要在本地复现打包时使用：

```powershell
.\scripts\pack-release.ps1 -Version <version> -Package <package-id>
```

同时发布两个确有变更的包时使用 `-Package ZenUI.Wpf,ZenUI.Wpf.Converters`。正式产物固定生成到 `artifacts/releases/<version>`。脚本只验证并打包显式选择的项目，同时检查 Tag 提交、包 ID、包版本、License、README、Changelog、Logo、目标框架、XML 文档、符号包和 Repository Commit。随后使用隔离的临时 WPF 消费者项目，在 .NET Framework 4.6.2～4.8.1 与 .NET 5～10 上从本地发布目录安装包、编译 C# 和 XAML，并确认 NuGet 为每个目标框架选择了正确资产，最后生成 `SHA256SUMS.txt`。预检包不得写入该目录；需要重新生成尚未公开的同一版本时，显式传入 `-Force`。

发布前检查 `.nupkg`：

- 所选包的包 ID 和版本正确，发布目录中不包含未选择的包。
- MIT License、README、Logo、目标框架和 XML 文档存在。
- `.snupkg` 已生成。
- Repository URL 指向 `XiaQueNet/ZenUI-WPF`。
- Repository Commit 与 Tag 指向一致。
- 发布目录中没有可能误传的旧版本包。

### 6. 发布 NuGet

CD 使用 NuGet Trusted Publishing，不保存长期 API Key：

1. 发布 Job 仅在 `v*` Tag 上运行，并仅为该 Job 授予 `id-token: write`。
2. Job 必须声明 GitHub Environment `生产环境`，与 NuGet.org Trusted Publishing 策略完全一致。
3. `NuGet/login@v1` 在上传前通过 OIDC 换取短期 API Key。
4. `dotnet nuget push` 使用短期 Key 上传已验证的 `.nupkg`；对应 `.snupkg` 随包发布。

GitHub Environment `生产环境` 中必须配置 Secret `NUGET_USER`，值为创建 Trusted Publishing 策略的 NuGet.org 用户名（Profile name，不是邮箱）。不要在仓库中保存用户名之外的 NuGet 凭据。

发布任务允许跳过已存在的同版本包，以便在部分包已上传、但 GitHub Release 创建失败时安全重试。NuGet.org 上的已有版本仍不可覆盖。不要单独手工修改 `.nupkg`；若上传后发现内容问题，应修复源码并发布新版本。

### 7. 创建 GitHub Release

NuGet 上传成功后，CD 自动创建或更新 GitHub Release：

- Tag：`v<version>`。
- 标题：`ZenUI WPF <version>`。
- 带预发布后缀的版本自动标记为 pre-release。
- 发布说明从 GitHub 自动生成；合并 PR 的标题和说明应清晰、优先使用中文。
- 附件只包含本次所选包对应的 `.nupkg`、`.snupkg` 和 `SHA256SUMS.txt`。

安装命令：

```powershell
dotnet add package ZenUI.Wpf --version <ZenUI.Wpf version>
dotnet add package ZenUI.Wpf.Converters --version <Converters version>
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
| [`0.1.0-preview.4`](https://www.nuget.org/packages/ZenUI.Wpf/0.1.0-preview.4) | [`v0.1.0-preview.4`](https://github.com/XiaQueNet/ZenUI-WPF/releases/tag/v0.1.0-preview.4) | `ae8aea2` | Preview |
| [`0.1.0-preview.3`](https://www.nuget.org/packages/ZenUI.Wpf/0.1.0-preview.3) | [`v0.1.0-preview.3`](https://github.com/XiaQueNet/ZenUI-WPF/releases/tag/v0.1.0-preview.3) | `9dce7ed` | Preview |
| [`0.1.0-preview.2`](https://www.nuget.org/packages/ZenUI.Wpf/0.1.0-preview.2) | [`v0.1.0-preview.2`](https://github.com/XiaQueNet/ZenUI-WPF/releases/tag/v0.1.0-preview.2) | `5c3663f` | Preview |
| [`0.1.0-preview.1`](https://www.nuget.org/packages/ZenUI.Wpf/0.1.0-preview.1) | [`v0.1.0-preview.1`](https://github.com/XiaQueNet/ZenUI-WPF/releases/tag/v0.1.0-preview.1) | `f2cfe71` | Preview |

当前版本不创建单独 Release 分支；后续开发继续在 `main`，每次公开发布创建新 Tag。

## 发布自动化

`.github/workflows/release.yml` 已实现 Tag 驱动的 CD，`.github/workflows/ci.yml` 仅负责可复用的持续集成验证：

1. 推送 `v*` Tag 后运行完整 CI。
2. 自动验证 Tag 格式，并选择版本与 Tag 一致的项目。
3. 从 Tag 提交重新构建、测试、扫描漏洞、打包并校验产物。
4. 通过 GitHub Environment `生产环境` 和 NuGet Trusted Publishing 发布。
5. 自动创建 GitHub Release 并上传包、符号包和校验和。

版本准备、Changelog 整理、Tag 创建和 `生产环境` 审批仍由维护者负责。首次启用前必须在 GitHub 中创建并保护 `生产环境`，配置 `NUGET_USER`，并确认 NuGet.org 策略中的工作流文件为 `release.yml`、环境名称为 `生产环境`，且仓库信息与实际值完全一致。
