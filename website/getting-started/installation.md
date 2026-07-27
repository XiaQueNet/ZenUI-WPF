---
title: 安装
---

# 安装

ZenUI 提供两个互不依赖的 NuGet 包。只需要控件时安装 `ZenUI.Wpf`；只需要值转换器时安装 `ZenUI.Wpf.Converters`。

## PackageReference

在项目文件中添加：

```xml
<ItemGroup>
  <PackageReference Include="ZenUI.Wpf" Version="0.1.0-preview.5" />
</ItemGroup>
```

转换器包可以独立安装：

```xml
<ItemGroup>
  <PackageReference Include="ZenUI.Wpf.Converters" Version="0.1.0-preview.5" />
</ItemGroup>
```

> [!NOTE]
> 当前版本为预览版本。升级前请阅读仓库中的变更记录。

## 支持的目标框架

- .NET Framework 4.7.2
- .NET 8 for Windows

WPF 项目需要启用 `UseWPF`：

```xml
<PropertyGroup>
  <TargetFramework>net8.0-windows</TargetFramework>
  <UseWPF>true</UseWPF>
</PropertyGroup>
```

安装完成后继续阅读[第一个界面](quick-start.md)。
