# 主题 Token 规范

本文定义 ZenUI.Wpf 主题资源的分层、命名和兼容性约定。控件模板与应用定制应遵循本文，通用控件设计要求参见[控件设计规范](component-design.md)。

## 设计目标

- 使用 WPF `ResourceDictionary` 和 `DynamicResource` 支持运行时主题切换。
- 让应用能够覆盖单个 Token，而不必复制完整控件模板。
- 保持 Light、Dark 和 HighContrast 的颜色资源键与资源类型一致，并验证其他主题覆盖不改变 Token 类型。
- 将全局设计语义与控件特有状态分开，避免所有资源长期堆积在同一个字典中。
- 保持已经发布的字符串资源键和 `Themes/Colors.xaml` 入口兼容。

## 当前分层

默认浅色主题通过 `Themes/Colors.xaml` 聚合以下字典：

| 字典 | 职责 | 示例 |
| --- | --- | --- |
| `Tokens/SemanticColors.xaml` | 跨控件共享的颜色语义 | `ZenPrimaryBrush`、`ZenTextSecondaryBrush`、`ZenSurfaceBrush` |
| `Tokens/ComponentColors.xaml` | 控件或控件部件特有的颜色状态 | `ZenScrollBarThumbBrush`、`ZenListBoxItemSelectedBrush` |
| `Tokens/Typography.xaml` | 语义字号、字重和绝对行高 | `ZenFontSizeBody`、`ZenFontWeightSemibold`、`ZenLineHeightBody` |
| `Tokens/Metrics.xaml` | 跨控件共享的尺寸与边框指标 | `ZenInputControlMinHeight`、`ZenInputControlPadding` |
| `Tokens/ComponentMetrics.xaml` | 控件特有但允许应用统一覆盖的尺寸 | `ZenButtonMinHeight`、`ZenButtonCornerRadius`、`ZenListBoxItemPadding` |
| `Tokens/Interaction.xaml` | 焦点和禁用状态的透明度语义 | `ZenFocusVisualOpacity`、`ZenDisabledActionOpacity` |

`Dark.xaml` 和 `HighContrast.xaml` 覆盖相同的公开颜色 Token。高对比度资源应优先使用 WPF `SystemColors`，而不是复制普通主题的固定色值。

Metrics 当前统一 TextBox、PasswordBox、ComboBox、DatePicker 和 NumberBox 的默认输入高度、Padding、圆角及边框宽度，也定义共享焦点边框指标。默认 Metrics 不随颜色主题变化，应用仍可在自身资源中覆盖。

Component Metrics 只收录具有明确控件语义、且不依赖模板内部布局计算的尺寸。单次出现的图标坐标、路径尺寸和与相邻列宽耦合的数值继续作为模板实现细节，不因追求 Token 数量而公开。

当前 Component Metrics 覆盖 Button、ListBox、ScrollBar、ComboBox 弹层、DataGrid、Calendar、DateTimePicker、Switch、Slider、ProgressBar、Loading 和 Alert。DatePicker 创建的 Calendar 位于独立 Popup 资源作用域，弹层宽高、日期格、月份按钮、导航按钮和字号通过 `ZenDatePicker` 依赖属性及显式源绑定传递；更复杂的外观仍应通过 WPF 原生 `CalendarStyle` 定制。图标路径和只服务于单个模板结构的坐标仍属于实现细节。

DateTimePicker 使用 `ZenDateTimePickerCalendarCellWidth`、`ZenDateTimePickerCalendarCellHeight`、`ZenDateTimePickerTimeItemWidth` 和 `ZenDateTimePickerTimeItemHeight` 分别定义日历单元与时间选项的默认尺寸。实例可独立调整两组尺寸；将某项设为 `Auto` 后，对应区域改由 `DropDownWidth` 或 `DropDownHeight` 的可用空间自动分配。日历头部按日历单元高度的固定比例布局。组合控件的外围留白统一由 `ZenDateTimePickerPopupPadding` 管理；日历与分割线之间的留白由 `ZenDateTimePickerTimePanelMargin` 管理，分割线与时间选择列表之间的留白由 `ZenDateTimePickerTimePanelPadding` 管理。

Interaction Token 按控件角色区分禁用后的视觉强调程度，而不是按具体控件命名。Light 和 Dark 使用原有透明度层级；HighContrast 将这些可靠的状态 Token 覆盖为完全不透明，让系统色承担禁用语义，避免透明度进一步削弱可读性。

Calendar 弹层和部分 DataGrid 模板内部状态暂不纳入 Interaction Token：前者位于独立 Popup 资源边界，后者包含通过 `TargetName` 修改模板内部元素的触发器。此类状态应优先通过 `CalendarStyle` 或控件依赖属性显式传递，避免产生看似可覆盖、实际无法可靠解析的 Token。

DatePicker 的 Calendar 位于独立 Popup 中，窗口级定制应通过 `ZenDatePicker.CalendarStyle` 传入；可基于公开的 `ZenCalendarStyle`、`ZenCalendarDayButtonStyle` 等 Style 扩展。弹层整体尺寸经 `CalendarPopupWidth` 和 `CalendarPopupHeight` 传递，日期按钮自动填充月份网格，导航按钮始终占用相同大小的网格单元；字号经 `CalendarFontSize` 传递。DataGrid 单元格的焦点与校验边框使用独立覆盖层，厚度分别经 `CellFocusVisualBorderThickness` 和 `CellValidationBorderThickness` 传递，避免状态切换改变内容 Padding 和布局。

Typography Token 提供 Caption、Body、Subtitle、Title、Display 等语义层级。所有公开 Zen 控件的默认 Style 使用 `ZenFontSizeBody`（14）作为正文大小；应用可覆盖该 Token 统一调整，也可在 Style 或单个控件上设置 `FontSize`。ZenUI 不强制设置全局 `FontFamily`，以保留系统字体和应用级本地化选择。绝对行高仅用于 `TextBlock` 排版，不应直接套用到固定高度的输入控件。

Density 与颜色主题相互独立。`ZenDensityManager` 通过 `Themes/Density/Compact.xaml` 和 `Comfortable.xaml` 覆盖可安全动态更新的 Metrics；Standard 直接使用默认 Token。当前密度覆盖输入控件、Button、ListBox、ScrollBar、ComboBox 弹层、DataGrid、Calendar、Switch、Slider、ProgressBar、Loading 和 Alert，且不改变颜色、字体或圆角。应用直接定义在自身资源字典中的同名 Token 仍具有更高优先级。

## 命名规则

全局 Token 按用途命名，不按当前色值或某个控件命名：

```text
ZenPrimaryBrush
ZenPrimaryHoverBrush
ZenTextPrimaryBrush
ZenSurfaceDisabledBrush
```

只有资源无法用稳定的跨控件语义表达时，才使用组件前缀：

```text
ZenDataGridRowHoverBrush
ZenListBoxItemSelectedInactiveBrush
```

交互状态后缀使用统一词汇：

```text
Hover
Pressed
Selected
Disabled
Inactive
Focus
```

不要在控件模板中直接添加仅描述色值的公开名称，例如 `ZenBlue600Brush`。将来引入的基础色阶属于实现层，控件仍应引用语义 Token。

## WPF 使用约定

- 主题切换后需要更新的颜色、Brush 和其他值使用 `DynamicResource`。
- 允许应用或 Density 在运行时覆盖的 Metrics 使用 `DynamicResource`。
- 不随主题或 Density 变化的 Style、模板结构和常量优先使用 `StaticResource`。
- 默认视觉由 Style Setter 引用 Token；依赖属性继续允许单个控件实例覆盖默认值。
- `ControlTemplate.Triggers` 直接设置模板内部元素时，不应假设所有动态 Metrics 都能可靠解析；需要开放覆盖的值应优先经由控件依赖属性和 `TemplateBinding` 传递，并补充实例化测试。
- 应用级资源覆盖优先于 ZenUI 默认主题，主题管理器不得在切换时破坏应用的自定义资源。
- Token 的值类型属于资源契约。已有 `SolidColorBrush` Token 不得在另一个主题中改为 `Color`、字符串或其他类型。

## 兼容性与测试

新增主题相关 Token 时，必须同时：

1. 在对应的 `Tokens/*.xaml` 中定义默认值。
2. 只有需要不同取值的主题才覆盖同名 Token，并保持资源类型一致。
3. Light、Dark、HighContrast 的颜色 Token 必须保持相同的键和类型契约。
4. 在控件模板中通过语义化 `DynamicResource` 使用。
5. 运行主题契约、控件行为和相关视觉回归测试。

移动 Token 的物理文件不应改变公开资源键、资源类型、默认值或 `Colors.xaml`、`Generic.xaml` 的加载入口。
