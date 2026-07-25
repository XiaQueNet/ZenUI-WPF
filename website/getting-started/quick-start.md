---
title: 第一个界面
---

# 第一个界面

在窗口或用户控件上声明 ZenUI 的稳定 XAML 命名空间：

```xaml
<Window
    x:Class="Example.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:zen="https://zenui.mnorg.cn/xaml/wpf">
    <StackPanel Margin="24">
        <zen:ZenTextBox
            Margin="0,0,0,12"
            Watermark="请输入名称" />
        <zen:ZenButton
            Content="保存"
            Variant="Primary" />
    </StackPanel>
</Window>
```

控件默认样式由 `Themes/Generic.xaml` 自动加载，不需要在应用资源中额外注册。

## 使用转换器

转换器使用独立命名空间，并且转换器类型本身是 XAML 标记扩展，无需预先创建资源：

```xaml
<Window
    xmlns:zc="https://zenui.mnorg.cn/xaml/wpf/converters">
    <ProgressBar
        Visibility="{Binding IsLoading,
            Converter={zc:BoolToVisibilityConverter}}" />
</Window>
```

下一步可以了解[主题切换与 Token 覆盖](theming.md)。
