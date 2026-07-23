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
