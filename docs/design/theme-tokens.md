# 主题 Token 规范

本文定义 ZenUI.Wpf 主题资源的分层、命名和兼容性约定。控件模板与应用定制应遵循本文，通用控件设计要求参见[控件设计规范](component-design.md)。

## 设计目标

- 使用 WPF `ResourceDictionary` 和 `DynamicResource` 支持运行时主题切换。
- 让应用能够覆盖单个 Token，而不必复制完整控件模板。
- 保持 Light、Dark 和 HighContrast 的资源键与资源类型一致。
- 将全局设计语义与控件特有状态分开，避免所有资源长期堆积在同一个字典中。
- 保持已经发布的字符串资源键和 `Themes/Colors.xaml` 入口兼容。

## 当前分层

默认浅色主题通过 `Themes/Colors.xaml` 聚合以下字典：

| 字典 | 职责 | 示例 |
| --- | --- | --- |
| `Tokens/SemanticColors.xaml` | 跨控件共享的颜色语义 | `ZenPrimaryBrush`、`ZenTextSecondaryBrush`、`ZenSurfaceBrush` |
| `Tokens/ComponentColors.xaml` | 控件或控件部件特有的颜色状态 | `ZenScrollBarThumbBrush`、`ZenListBoxItemSelectedBrush` |
| `Tokens/Metrics.xaml` | 跨控件共享的尺寸与边框指标 | `ZenInputControlMinHeight`、`ZenInputControlPadding` |
| `Tokens/ComponentMetrics.xaml` | 控件特有但允许应用统一覆盖的尺寸 | `ZenButtonCornerRadius`、`ZenListBoxItemPadding` |

`Dark.xaml` 和 `HighContrast.xaml` 覆盖相同的公开颜色 Token。高对比度资源应优先使用 WPF `SystemColors`，而不是复制普通主题的固定色值。

Metrics 当前统一 TextBox、PasswordBox、ComboBox、DatePicker 和 NumberBox 的默认输入高度、Padding、圆角及边框宽度，也定义共享焦点边框指标。默认 Metrics 不随颜色主题变化，应用仍可在自身资源中覆盖。

Component Metrics 只收录具有明确控件语义、且不依赖模板内部布局计算的尺寸。单次出现的图标坐标、路径尺寸和与相邻列宽耦合的数值继续作为模板实现细节，不因追求 Token 数量而公开。

后续可在不改变现有 Token 的前提下增加：

- `Typography.xaml`：字号、字重和行高。
- Density 字典：Compact、Standard、Comfortable 等桌面密度。

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

1. 在默认 Light 主题中定义资源。
2. 在 Dark 主题中提供相同键和相同类型。
3. 在 HighContrast 主题中提供相同键和相同类型。
4. 在控件模板中通过语义化 `DynamicResource` 使用。
5. 运行主题契约测试和相关视觉回归测试。

移动 Token 的物理文件不应改变公开资源键、资源类型、默认值或 `Colors.xaml`、`Generic.xaml` 的加载入口。
