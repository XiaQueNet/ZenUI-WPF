# Contributing

## 开发环境

- Windows 10/11
- global.json 指定的 .NET SDK
- Visual Studio 2022（可选）

## 提交前检查

```powershell
dotnet restore ZenUI.Wpf.slnx
dotnet build ZenUI.Wpf.slnx -c Release --no-restore
dotnet test tests/ZenUI.Wpf.Tests/ZenUI.Wpf.Tests.csproj -c Release --no-build
dotnet pack src/ZenUI.Wpf/ZenUI.Wpf.csproj -c Release --no-build -o artifacts/packages
```

新增或修改控件时，需要覆盖默认、悬停、按下、键盘焦点、禁用、只读、验证错误和高对比度状态。继承 WPF 控件时，应保留其公开属性对应的模板契约，并为关键行为添加 STA 回归测试。

公共 API 的不兼容修改必须记录在 `CHANGELOG.md`，并按语义化版本调整主版本号。

## 提交信息

- Git 提交的标题和正文统一使用中文。
- 提交信息应简洁说明本次变更，例如：`美化各主题下的滚动条`。

## 分支与发布

- 默认开发分支为 `main`。
- 功能、修复、文档和维护工作使用短期分支，建议分别命名为 `feature/*`、`fix/*`、`docs/*` 和 `chore/*`。
- 不为每个版本创建分支；只有并行维护旧发布线时才创建 `release/<line>`。
- 每个公开 NuGet 版本必须创建对应的 `v<version>` Tag 和 GitHub Release。
- 发布前必须确认本地验证与远程 CI 全部成功。

完整流程参见 [版本与发布规范](docs/RELEASING.md)。
