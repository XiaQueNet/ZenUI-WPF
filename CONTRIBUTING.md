# 贡献指南

感谢你参与 ZenUI.Wpf。功能、修复、测试、示例和文档改进都可以通过 Pull Request 提交。

## 开始之前

- 开发环境为 Windows 10/11，并使用 `global.json` 指定的 .NET SDK。
- 可以使用 Visual Studio 2022，也可以直接使用 .NET CLI。
- 较大的功能、公共 API 或行为变更建议先通过 Issue 说明使用场景和设计方向。
- 每个分支和 Pull Request 应聚焦一个主题，不夹带无关的重构或格式化。

## 分支

默认开发分支为 `main`。从最新的 `main` 创建短期分支，建议按变更类型命名：

| 类型 | 分支名称 |
| --- | --- |
| 功能 | `feature/*` |
| 修复 | `fix/*` |
| 文档 | `docs/*` |
| 维护 | `chore/*` |

只有并行维护旧发布线时才创建 `release/<line>`，不为每个版本建立发布分支。

## 实施变更

### 控件与公共 API

ZenUI 只调整 WPF 控件的默认值和默认呈现，不覆盖或移除基类控件原有功能：

- 优先复用 WPF 已有的属性、事件、命令和行为契约。
- 新增或修改控件时，覆盖适用的默认、悬停、按下、键盘焦点、禁用、只读、验证错误及高对比度状态。
- 保留公开属性对应的模板、键盘操作、UI Automation 和可访问性契约。
- 为默认体验、可选能力和关键原生行为添加 STA 回归测试。
- 新增或修改的公共 API 必须提供有意义的 XML 文档注释。

完整要求参见[控件设计规范](docs/design/component-design.md)、[测试规范](docs/development/testing.md)和[C# 注释规范](docs/development/commenting.md)。

### 主题与视觉

- 颜色和动态尺寸使用语义化 Token，不在控件模板中硬编码可定制值。
- 验证 Light、Dark、HighContrast 以及适用的 Compact、Standard、Comfortable Density。
- 视觉变更应同步更新或补充 Gallery 示例，并检查不同 DPI 下的呈现。
- Pull Request 应附上能够说明变化的截图；涉及多个主题或状态时，提供对应对比。

主题资源的分层和兼容性要求参见[主题 Token 规范](docs/design/theme-tokens.md)。

### 文档与变更记录

- 文档使用中文，仓库内链接使用相对路径。
- 新增、移动或删除文档时，按需更新 `docs/README.md` 和根目录 `README.md`。
- 面向使用者的功能与 API 变更应更新 `CHANGELOG.md`；不兼容变更必须说明迁移影响，并按语义化版本处理。
- 提交前确认本地 Markdown 链接可以解析。

详细规则参见[文档编写规范](docs/development/documentation.md)。

## 提交前检查

根据变更范围按[测试规范](docs/development/testing.md#回归与验证)选择组件测试、单框架全量测试或全框架矩阵测试。准备提交 Pull Request 时，通常至少执行以下构建、`net472` 单框架全量测试和打包检查：

```powershell
dotnet restore ZenUI.Wpf.slnx
dotnet build ZenUI.Wpf.slnx -c Release --no-restore
dotnet test --project tests/ZenUI.Wpf.Tests/ZenUI.Wpf.Tests.csproj -c Release -f net472 --max-parallel-test-modules 1 --no-build
dotnet test --project tests/ZenUI.Wpf.Converters.Tests/ZenUI.Wpf.Converters.Tests.csproj -c Release -f net472 --max-parallel-test-modules 1 --no-build
$packageOutput = Join-Path ([System.IO.Path]::GetTempPath()) "ZenUI-WPF-packages"
dotnet pack src/ZenUI.Wpf/ZenUI.Wpf.csproj -c Release --no-build -o $packageOutput
dotnet pack src/ZenUI.Wpf.Converters/ZenUI.Wpf.Converters.csproj -c Release --no-build -o $packageOutput
git diff --check
```

如果变更只影响文档，可以省略编译、测试和打包，但仍需检查差异格式和本地链接。提交前同时确认：

- 没有编译器或 .NET 分析器警告。
- 新增行为具有对应测试，且测试同时兼容 `net472` 与 `net8.0-windows`。
- 没有提交构建产物、临时文件或与本次变更无关的文件。
- 注释、文档、示例和 `CHANGELOG.md` 已按变更范围同步更新。

## 提交信息

- Git 提交的标题和正文统一使用中文。
- 标题应简洁说明实际变更，例如：`美化各主题下的滚动条`。
- 一个提交尽量表达一个完整意图，避免将无关变更混在同一提交中。
- 正文用于说明必要的原因、兼容性影响或取舍，不逐项重复文件改动。

## Pull Request

Pull Request 的标题和说明使用中文，并包含：

- 变更目的和影响范围。
- 已执行的验证命令及结果。
- 关联的 Issue；没有关联 Issue 时说明背景。
- 公共 API、兼容性、安全或性能影响。
- 视觉变更的截图或前后对比。

提交后确认远程 CI 全部成功，并根据评审意见同步更新实现、测试和文档。

## 发布

公开版本由维护者发布。每个 NuGet 版本必须创建对应的 `v<version>` Tag 和 GitHub Release；发布前必须确认本地验证与远程 CI 全部成功。

正式发布包统一通过 `scripts/pack-release.ps1` 生成到 `artifacts/releases/<version>`，不要手工指定其他仓库内目录。完整流程参见[版本与发布规范](docs/maintainers/releasing.md)。
