---
title: 主题
---

# 主题

ZenUI 默认使用浅色主题，同时提供深色和高对比度主题。

## 在运行时切换主题

```csharp
using System.Windows;
using ZenUI.Wpf.Theming;

ZenThemeManager.ApplyTheme(
    Application.Current.Resources,
    ZenTheme.Dark);
```

`ApplyTheme` 默认尊重 Windows 高对比度设置。在主题预览工具等需要强制指定主题的场景，可以将第三个参数设置为 `false`：

```csharp
ZenThemeManager.ApplyTheme(
    Application.Current.Resources,
    ZenTheme.Dark,
    respectSystemHighContrast: false);
```

## 显式合并默认主题

应用需要直接使用 ZenUI 颜色资源或具名样式时，可以在 `App.xaml` 中显式合并默认主题：

```xaml
<ResourceDictionary
    Source="pack://application:,,,/ZenUI.Wpf;component/Themes/Generic.xaml" />
```

所有控件颜色都通过语义化 `DynamicResource` 获取，因此应用可以在主题字典之后覆盖单个 Token。

应用覆盖主题资源时，应保持资源的语义用途，并在升级 ZenUI 后检查相关视觉状态。
