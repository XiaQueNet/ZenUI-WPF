# 主题、密度和 Token

先按 [接入](getting-started.md) 合并 Generic.xaml，使应用自己的页面也能查到 Token。控件基础字典只需合并一次。

## 运行时切换

在拥有资源字典的 UI 线程上调用；以下代码可用于现有事件处理器或应用启动流程：

```csharp
ZenUI.Wpf.Theming.ZenThemeManager.ApplyTheme(
    System.Windows.Application.Current.Resources,
    ZenUI.Wpf.Theming.ZenTheme.Dark);

ZenUI.Wpf.Theming.ZenDensityManager.ApplyDensity(
    System.Windows.Application.Current.Resources,
    ZenUI.Wpf.Theming.ZenDensity.Compact);
```

主题为 Light、Dark、HighContrast，密度为 Compact、Standard、Comfortable。两者独立管理。Standard 会移除密度覆盖，不需要引用不存在的 Density/Standard.xaml；Light 恢复基础配色，不要猜测 Themes/Light.xaml。

`ApplyTheme(resources, theme, respectSystemHighContrast = true)` 默认持续跟随 Windows 高对比度设置。保留该默认行为，除非需求明确要求关闭。手动品牌色覆盖仍可能压过高对比度颜色，因此要单独检查覆盖作用域。

## 常用资源键与类型

| 用途 | 资源键 | 类型 |
| --- | --- | --- |
| 主操作及交互态 | ZenPrimaryBrush、ZenPrimaryHoverBrush、ZenPrimaryPressedBrush | Brush |
| 主操作上的文字 | ZenOnAccentBrush | Brush |
| 页面/卡片背景 | ZenSurfaceBrush、ZenSurfaceMutedBrush | Brush |
| 主要/次要文字 | ZenTextPrimaryBrush、ZenTextSecondaryBrush | Brush |
| 提示文字 | ZenTextPlaceholderBrush | Brush |
| 边框/分割线 | ZenBorderBrush、ZenDividerBrush | Brush |
| 焦点 | ZenFocusBrush | Brush |
| 状态 | ZenSuccessBrush、ZenWarningBrush、ZenErrorBrush、ZenInfoBrush | Brush |
| 输入框内边距 | ZenInputControlPadding | Thickness |
| 输入框圆角 | ZenInputControlCornerRadius | CornerRadius |
| 输入框最小高度 | ZenInputControlMinHeight | Double |

颜色不能作为字符串覆盖 Brush，圆角不能用 Double 替代 CornerRadius。优先查真实 Token，避免猜测 PrimaryColor、ZenAccentColor 等资源名。

页面使用示例：

```xaml
<Border Padding="20" Background="{DynamicResource ZenSurfaceMutedBrush}">
    <TextBlock Text="订单详情" Foreground="{DynamicResource ZenTextPrimaryBrush}" />
</Border>
```

局部品牌色覆盖示例，放入目标视图的 Resources 中；注意同时设计悬停、按下和文字对比度：

```xaml
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <SolidColorBrush x:Key="ZenPrimaryBrush" Color="#2457C5" />
    <SolidColorBrush x:Key="ZenPrimaryHoverBrush" Color="#1D48A6" />
    <SolidColorBrush x:Key="ZenPrimaryPressedBrush" Color="#163982" />
    <CornerRadius x:Key="ZenInputControlCornerRadius">8</CornerRadius>
</ResourceDictionary>
```

主题切换只更新管理器维护的字典。更近作用域的值、直接写在资源字典中的覆盖，以及控件本地属性值可能继续优先生效；这不表示切换失败。若品牌色需要随主题/高对比度调整，将品牌覆盖作为可替换的独立字典管理，不永久固定一组颜色。

仅当改变结构时再写 ControlTemplate。普通按钮外观优先使用 Variant、Appearance；普通圆角使用 CornerRadius。不要为整页统一颜色复制全部控件模板。

源码定位：`src/ZenUI.Wpf/Theming`、`src/ZenUI.Wpf/Themes/Tokens`、`Themes/Dark.xaml`、`Themes/HighContrast.xaml` 和 `Themes/Density`。
