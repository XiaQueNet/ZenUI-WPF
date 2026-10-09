# 接入与版本

## 基线

本资料核对自 ZenUI-WPF 源码：`src/ZenUI.Wpf/ZenUI.Wpf.csproj` 的 `0.1.0-preview.12`、`src/ZenUI.Wpf.Converters/ZenUI.Wpf.Converters.csproj` 的 `0.1.0-preview.11`。不保证早期版本拥有这些 API，也不将本地源码版本描述为已发布版本。

两包互不依赖，按需安装。现有项目保留其包版本和集中版本管理约定。首次安装时确认包源可用版本并显式选择版本，不盲目复制预览版本号。安装命令形式为 `dotnet add <项目路径> package ZenUI.Wpf --version <已确认版本>`；转换器包同理，执行前替换参数。

源码提供 net462、net471、net472、net5.0-windows、net8.0-windows 资产。正式支持 .NET Framework 4.6.2 及以上和 .NET 8 及以上 WPF，.NET 5/6/7 使用兼容资产。保留消费者现有框架，不为接入组件擅自迁移框架。SDK 风格 WPF 项目需启用 `UseWPF`，现代 .NET 目标包含 `-windows`。

## 命名空间与默认控件

```xaml
<UserControl
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:zen="https://zenui.mnorg.cn/xaml/wpf">
    <StackPanel Margin="24">
        <zen:ZenTextBox Watermark="请输入名称" />
        <zen:ZenButton Margin="0,12,0,0" Content="保存" Variant="Primary" />
    </StackPanel>
</UserControl>
```

此 URI 是 XAML 命名空间标识，运行时不需要联网下载控件。C# 控件命名空间为 `ZenUI.Wpf.Controls`，主题管理为 `ZenUI.Wpf.Theming`。

自定义控件的默认样式通过程序集 `Themes/Generic.xaml` 自动加载。只放置这些控件时无需为每个控件手动指定 Style。

## 应用直接使用 Token 或具名样式

将下列字典合并进现有 `Application.Resources`，保留已有资源，不创建第二个 Application，也不覆盖整个 App.xaml：

```xaml
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="pack://application:,,,/ZenUI.Wpf;component/Themes/Generic.xaml" />
    </ResourceDictionary.MergedDictionaries>
</ResourceDictionary>
```

后续页面可使用 `Background="{DynamicResource ZenSurfaceBrush}"` 等资源。运行时主题和密度的调用见 [主题](theming.md)。

## 示例来源边界

组件库源码中的 `samples/ZenUI.Wpf.Gallery/Views` 展示单控件用法，`samples/ZenUI.Wpf.PosDemo` 展示业务组合。Gallery 的 `GalleryCardStyle`、`SectionTitleStyle`、`PageHeader` 等属于示例应用，并非组件库公开 API；借鉴结构时用目标项目自己的布局和样式替代。
