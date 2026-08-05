# 主题、Density 定制与迁移指南

本文面向在应用中使用 ZenUI.Wpf 的开发者，说明如何切换主题与界面密度、覆盖公开 Token，以及如何从复制模板或硬编码尺寸迁移到稳定的定制入口。Token 的设计和兼容性规则参见[主题 Token 规范](../design/theme-tokens.md)。

## 引入默认资源

ZenUI 控件会通过程序集主题机制获得默认样式。应用需要直接引用 Token 或具名 Style 时，在应用资源中合并 `Generic.xaml`：

```xaml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary
                Source="pack://application:,,,/ZenUI.Wpf;component/Themes/Generic.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

应用自身定义的资源应放在 `MergedDictionaries` 之后。这样同名本地资源的优先级高于 ZenUI 默认主题和 Density 覆盖。

## 运行时切换主题

使用 `ZenThemeManager` 切换 Light、Dark 和 HighContrast：

```csharp
using ZenUI.Wpf.Theming;

ZenThemeManager.ApplyTheme(
    Application.Current.Resources,
    ZenTheme.Dark);
```

默认情况下，资源字典会持续跟随 Windows 高对比度状态。系统开启高对比度后，ZenUI 自动改用 `HighContrast`；系统关闭后，恢复调用时请求的主题。重应用发生在首次调用 `ApplyTheme` 时所在的 UI Dispatcher。

主题预览器或截图工具需要强制显示指定主题时，可以关闭系统跟随：

```csharp
ZenThemeManager.ApplyTheme(
    previewWindow.Resources,
    ZenTheme.Dark,
    respectSystemHighContrast: false);
```

对同一资源字典使用 `respectSystemHighContrast: false` 会停止此前建立的系统高对比度跟随。

## 运行时切换 Density

颜色主题与界面密度相互独立：

```csharp
using ZenUI.Wpf.Theming;

ZenDensityManager.ApplyDensity(
    Application.Current.Resources,
    ZenDensity.Compact);
```

可用密度如下：

| Density | 用途 |
| --- | --- |
| `Compact` | 信息密集、以鼠标和键盘操作为主的桌面界面 |
| `Standard` | 默认规格，保持 ZenUI 的标准视觉 |
| `Comfortable` | 需要更大间距和命中区域的界面 |

Density 会更新输入控件、Button、ListBox、ScrollBar、ComboBox Popup、ContextMenu、DataGrid、Calendar、Switch、Slider、ProgressBar 和 Alert 的相关尺寸，不改变颜色、字体或圆角语义。

## 覆盖公开 Token

应用可以只覆盖需要改变的 Token，不必复制整个控件模板：

```xaml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary
                Source="pack://application:,,,/ZenUI.Wpf;component/Themes/Generic.xaml" />
        </ResourceDictionary.MergedDictionaries>

        <SolidColorBrush x:Key="ZenPrimaryBrush" Color="#6750A4" />
        <SolidColorBrush x:Key="ZenPrimaryHoverBrush" Color="#5B4594" />

        <sys:Double
            xmlns:sys="clr-namespace:System;assembly=mscorlib"
            x:Key="ZenInputControlMinHeight">38</sys:Double>
        <Thickness x:Key="ZenDataGridCellPadding">16,0</Thickness>
    </ResourceDictionary>
</Application.Resources>
```

应用本地同名 Token 的优先级高于主题和 Density 字典，因此切换主题或密度后仍保留应用定制。覆盖时必须保持资源类型，例如 Brush Token 继续使用 `SolidColorBrush`，尺寸 Token 继续使用 `Double`、`Thickness` 或 `CornerRadius`。

常用定制入口：

| 目标 | Token 或属性 |
| --- | --- |
| 主色与交互色 | `ZenPrimaryBrush`、`ZenPrimaryHoverBrush`、`ZenPrimaryPressedBrush` |
| 输入控件高度与内边距 | `ZenInputControlMinHeight`、`ZenInputControlPadding` |
| 按钮高度与内边距 | `ZenButtonMinHeight`、`ZenButtonPadding` |
| DataGrid 表头、行和单元格 | `ZenDataGridColumnHeaderHeight`、`ZenDataGridRowMinHeight`、`ZenDataGridCellPadding` |
| Calendar 弹层与导航 | `ZenCalendarPopupWidth`、`ZenCalendarPopupHeight`、`ZenCalendarNavigationButtonSize` |
| DateTimePicker 选择单元 | `ZenDateTimePickerSelectionCellWidth`、`ZenDateTimePickerSelectionCellHeight` |
| Switch 与 Slider | `ZenSwitchWidth`、`ZenSwitchHeight`、`ZenSliderThumbSize`、`ZenSliderTrackThickness` |
| Alert 内边距 | `ZenAlertPadding` |
| Expander 布局 | `ZenExpanderHeaderPadding`、`ZenExpanderContentPadding`、`ZenExpanderCornerRadius`、`ZenExpanderGlyphSize` |
| ContextMenu 表面与菜单项 | `ZenContextMenuPadding`、`ZenContextMenuItemMinHeight`、`ZenContextMenuItemPadding` |

完整 Key 和当前解析值可在 Gallery 的“设计 Token”页面查看。

## 定制 Calendar Popup

DatePicker 的 Calendar 位于独立 Popup 中。颜色与 Density 尺寸由 `ZenDatePicker` 显式传入 Popup；更复杂的外观应通过 `CalendarStyle` 传入，避免依赖窗口视觉树查找：

```xaml
<Style
    x:Key="AppCalendarDayButtonStyle"
    BasedOn="{StaticResource ZenCalendarDayButtonStyle}"
    TargetType="{x:Type CalendarDayButton}">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<Style
    x:Key="AppCalendarStyle"
    BasedOn="{StaticResource ZenCalendarStyle}"
    TargetType="{x:Type Calendar}">
    <Setter
        Property="CalendarDayButtonStyle"
        Value="{StaticResource AppCalendarDayButtonStyle}" />
</Style>

<zen:ZenDatePicker CalendarStyle="{StaticResource AppCalendarStyle}" />
```

单个 DatePicker 也可以通过 `CalendarPopupWidth`、`CalendarPopupHeight`、`CalendarFontSize`、`CalendarButtonPadding` 和 `CalendarNavigationButtonSize` 覆盖默认结果。日期按钮不固定宽高，而是自动填充月份网格。

`ZenDateTimePicker` 默认由 `SelectionCellWidth` 和 `SelectionCellHeight` 驱动弹层的自然尺寸；星期、日期和时分秒选择项共享同一单元尺寸，日历头部高度为单元高度的 1.25 倍。将单元尺寸设为 `Auto` 并设置 `DropDownWidth` 或 `DropDownHeight`，可以改为由弹层整体尺寸驱动对应方向的自动均分。四个属性均为显式值时，控件尊重全部设置，不自动修正内容溢出或剩余空间。

## 从硬编码样式迁移

建议按以下顺序迁移，避免一次性复制或替换全部模板：

1. 记录应用当前覆盖的颜色、尺寸、Style 和完整 `ControlTemplate`。
2. 将固定颜色映射到语义 Brush Token，例如主操作使用 `ZenPrimaryBrush`，正文使用 `ZenTextPrimaryBrush`。
3. 将跨页面重复的高度、Padding 和轨道尺寸映射到 Metrics 或 Component Metrics。
4. 单个控件的例外继续使用依赖属性或派生 Style，不要为一次性数值新增全局 Token。
5. Calendar Popup 使用 `CalendarStyle` 和 `ZenDatePicker` 的尺寸属性；DataGrid 状态边框使用公开依赖属性。
6. 删除已由 Token 或 Style 覆盖替代的复制模板，每次只迁移一类控件。
7. 在 Light、Dark、HighContrast × Compact、Standard、Comfortable 下回归焦点、禁用、校验错误和 Popup。

常见迁移映射：

| 旧做法 | 推荐做法 |
| --- | --- |
| 在多个模板中写死 `#3D63D2` | 引用或覆盖 `ZenPrimaryBrush` |
| 为紧凑页面复制全部控件 Style | 调用 `ZenDensityManager.ApplyDensity(..., Compact)` |
| 复制 DataGrid 模板修改行高 | 覆盖 `ZenDataGridRowMinHeight` |
| 在 Popup 内通过 `FindAncestor` 查找 Window | 经控件属性或 `CalendarStyle` 显式传递 |
| 用透明度削弱高对比度禁用态 | 保留 HighContrast 的系统色和完整不透明度 |

## 兼容性注意事项

- 已公开 Token 的字符串 Key、资源类型和 Standard 默认值属于兼容性契约。
- `Dark.xaml` 与 `HighContrast.xaml` 必须保持相同的公开颜色 Token 集合。
- Compact 与 Comfortable 必须保持相同的 Density Token 集合和类型。
- 模板内部名称只有 `PART_` 契约和文档明确说明的入口适合依赖。
- Popup 在混合 DPI 多显示器上的最终位置由 WPF 与系统工作区共同决定，发布前应按[测试规范](../development/testing.md#popup-与多显示器检查)完成人工检查。
