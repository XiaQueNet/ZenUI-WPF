# 控件设计规范

本文是 ZenUI.Wpf 控件设计与评审的统一入口。ZenUI 的目标是优化 WPF 控件的默认体验，而不是缩减 WPF 已有能力。新增控件、修改控件模板或调整公开行为时，均应遵循本文。

测试实现细则参见[测试规范](../development/testing.md)，源码注释与公共 API 文档要求参见[C# 注释规范](../development/commenting.md)。

## 设计原则

- 保留基类控件原有的属性、事件、命令、选择、编辑、键盘操作、UI Automation 与可访问性契约。
- 主要通过默认值、主题资源和控件模板调整外观与交互反馈。
- 视觉上需要弱化的原生状态，应调整默认呈现，而不是删除或屏蔽对应行为。
- 非默认视觉应通过依赖属性、语义化资源 Token、具名样式或模板入口保留为可选能力，并能够独立开启和定制。
- 默认体验与可选能力都必须有自动化测试，避免外观调整意外破坏原生功能。

只有控件本身具有明确且已记录的不同语义时，才可以改变原生行为契约。此类变更必须说明兼容性影响、补充回归测试，并按项目的版本与发布规则处理。

例如，`ZenDataGrid` 默认只显示行 Hover 反馈，但选择行为仍然存在。使用者可以分别开启行选中高亮和当前单元格焦点框：

```xaml
<zen:ZenDataGrid
    IsRowSelectionHighlightEnabled="True"
    IsCellFocusVisualEnabled="True" />
```

## 公共 API

- 优先复用 WPF 基类已有的属性、事件和命令，不为相同语义创建另一套 API。
- 新增可配置行为时，使用符合 WPF 习惯的依赖属性、路由事件或命令。
- 布尔属性的名称应清楚表达 `true` 所开启的能力；数值属性应明确单位、有效范围和边界行为。
- 新增或修改的公共类型、公共成员、依赖属性、路由事件、命令和枚举应提供有意义的 XML 文档注释。
- 公共文档描述稳定契约，不承诺可替换的内部实现细节。
- 公共 API 的不兼容修改必须记录在 `CHANGELOG.md`，并按语义化版本调整主版本号。

### 命名原则

公共 API 应描述使用者能够理解和配置的控件语义，模板、触发器和代码应负责把这些语义映射到具体的 WPF 实现机制。不能仅因为模板使用 `IsMouseOver` 或名为 `MouseOver` 的 VisualState，就把触发机制直接暴露为公共属性名称。

选择名称时按以下顺序判断：

1. 相同语义是否已有 WPF 基类 API；有则直接复用。
2. WPF 控件生态是否已经形成稳定且没有歧义的常用名称；有则优先沿用。
3. 候选名称是否准确描述使用者可观察的状态、行为或内容，而不是当前模板的实现方式。
4. 名称是否会与 WPF 中已有的强语义产生错误预期。
5. 在语义完整的前提下，删除由所属控件、属性或命名空间已经提供的重复上下文。

WinUI、Web 或其他 UI 框架的名称只能作为参考。除非其语义与 ZenUI 属性完全一致，并且不会破坏 WPF 使用者的既有认知，否则不为了跨框架一致或表面上的“现代化”而改名。例如，输入控件继续使用 WPF 控件库中常见的 `Watermark`，数值步长可以使用清晰的 `Step`，不机械替换为其他框架的 `PlaceholderText` 或 `SmallChange`。

### 状态属性

状态属性采用“状态 + 被设置的视觉属性”结构。默认状态直接复用控件已有属性，不增加 `Normal` 前缀：

| 状态 | 背景属性 | 说明 |
| --- | --- | --- |
| 默认 | `Background` | 复用基类属性 |
| 悬停 | `HoverBackground` | 描述用户体验状态，不暴露 `IsMouseOver` 触发机制 |
| 按下 | `PressedBackground` | 用于按钮或其他可按压控件 |
| 禁用 | `DisabledBackground` | 对应不可操作状态 |
| 焦点 | `FocusedBackground` | 对应键盘或逻辑焦点反馈 |
| 选中 | `SelectedBackground` | 用于列表项、行、单元格等选择控件 |
| 勾选或开启 | `CheckedBackground` | 用于 CheckBox、RadioButton、ToggleButton 或 Switch |

前景和边框使用相同结构，例如 `HoverForeground`、`PressedBorderBrush` 和 `SelectedForeground`。控件只公开确有定制需求的状态属性，不为追求表格完整而增加未使用的 API。

`Hover` 是公共体验语义；`IsMouseOver`、`MouseOver` VisualState 和具体指针事件是模板实现。模板可以使用 WPF 原生机制实现 Hover，但公共 API 不应因此改名为 `MouseOverBackground`。组合状态通常由模板和 Token 处理，除非存在明确且稳定的独立定制需求，不增加 `SelectedHoverBackground` 一类组合属性。

### 名称精确度

- 避免单独使用 `Mode`、`Type`、`Option`、`State` 等不能说明作用对象的宽泛名称。名称应指出被控制的对象和维度，例如使用 `SpinButtonLayout`，而不是 `ButtonMode`。
- 不滥用 WPF 中已有强默认语义的名称。例如，`Orientation` 通常表示整个控件或内容的排列轴；若属性只控制 NumberBox 增减按钮的布局，应使用 `SpinButtonLayout`，不能让使用者误以为它会旋转或重排整个控件。
- 布尔属性使用 `Is`、`Has`、`Can` 等前缀，并让名称准确说明 `true` 的效果。若属性控制的是密码显示按钮，应使用 `IsPasswordRevealButtonEnabled`，而不是可能被理解为控制整个明文显示能力的 `IsPasswordRevealEnabled`。
- 枚举属性名称应表达枚举值共同描述的维度。Info、Success、Warning、Error 表达严重级别时，使用 `Severity` 和 `AlertSeverity`，不使用含义宽泛的 `Variant`。
- 只有在名称可能冲突或离开所属类型后语义不完整时，才给类型增加控件名前缀。`SpinButtonLayout` 已经能够独立表达含义，不扩展为 `NumberBoxSpinButtonLayout`。
- 属性名与枚举类型同名是允许的，例如 `SpinButtonLayout SpinButtonLayout`。不应仅为避免同名而引入 `Mode`、`Kind` 等无额外语义的后缀。
- 表示尺寸时优先使用 WPF 已有后缀和类型，例如 `Width`、`Height`、`Size`、`Thickness`、`Padding` 和 `CornerRadius`；名称应说明尺寸属于哪个部件。

### WPF 成员配对

依赖属性、附加属性和路由事件必须遵循 WPF 的标准配对命名：

```csharp
public AlertSeverity Severity
{
    get { return (AlertSeverity)GetValue(SeverityProperty); }
    set { SetValue(SeverityProperty, value); }
}

public static readonly DependencyProperty SeverityProperty;
```

依赖属性标识符使用 `<属性名>Property`，注册名、CLR 包装器和标识符必须一致。附加属性使用 `Get<PropertyName>`、`Set<PropertyName>` 和 `<PropertyName>Property`；路由事件使用 `<EventName>` 和 `<EventName>Event`。

标记为 `EditorBrowsableState.Never` 的成员仍然是公共 API，仍会被 XAML、反射和已编译代码访问。不能把该标记当成内部可见性边界；模板基础设施成员一旦公开，也必须按兼容性规则管理。

## 模板契约

- 模板修改不得破坏 WPF 规定或控件公开声明的 `PART_*`、VisualState、绑定和布局契约。
- 必需的模板部件使用 `PART_` 命名，并通过 `TemplatePartAttribute` 声明。
- 模板内部元素优先使用清晰结构和语义化资源 Key 表达用途，不依赖大段注释解释结构。
- 模板应继续响应基类公开属性；不能因为默认模板暂时未使用某项属性而使其失效。
- 非契约的视觉树结构属于实现细节，不应成为使用者定制控件的必要入口。

## 交互状态

新增或修改控件时，应根据控件能力覆盖下列状态。某个状态不适用时，应在评审或测试中能够说明原因。

| 状态 | 设计要求 |
| --- | --- |
| 默认 | 信息层级清楚，文本、边框和背景使用主题资源 |
| Hover | 提供可感知但不过度突出的指针反馈 |
| Pressed | 与 Hover 有明确差异，并保持内容和布局稳定 |
| 键盘焦点 | 焦点可见，不以鼠标 Hover 代替键盘焦点 |
| 选中或勾选 | 视觉状态与控件的实际选择值一致 |
| Disabled | 明确不可操作，同时保持必要的内容可读性 |
| 只读 | 与 Disabled 区分；允许的选择、滚动或复制行为应继续工作 |
| 验证错误 | 支持适用控件的 WPF `Validation.HasError` 状态 |
| 高对比度 | 使用系统颜色，保持边界、内容和焦点可辨识 |

状态组合也必须可用，例如“键盘焦点 + 验证错误”或“选中 + Disabled”。状态视觉不得改变控件原有的选择、编辑和命令语义。

## 主题与资源

- 控件颜色通过语义化 `DynamicResource` 获取，不在模板中散布只适用于单一主题的固定颜色。
- 主题 Token 的分层、命名和兼容性要求遵循[主题 Token 规范](theme-tokens.md)。
- 默认支持 Light、Dark 和 HighContrast；高对比度主题使用 Windows 系统颜色并尊重系统设置。
- Token 按用途命名，不按某一处当前色值命名。应用应能够覆盖单个 Token。
- 具名样式和模板入口应可独立使用，不要求使用者复制整个默认模板才能完成常见定制。
- 主题切换后，现有控件实例应能获得新的动态资源。
- 布局和状态反馈应在项目约定的 100%、150% 和 200% DPI 下保持清晰、稳定。

## 输入、自动化与可访问性

- 保留并测试基类控件的键盘操作、鼠标操作、命令路由和焦点行为。
- 不用颜色作为状态的唯一表达方式；焦点、错误和选择等关键信息应保持可辨识。
- UI Automation Peer 应暴露正确的控件类型、名称、状态和适用 Pattern。
- 模板内的装饰元素不应干扰焦点顺序、命中测试或自动化树语义。
- 密码等敏感数据不得为了绑定便利复制为长期存在的明文。`ZenPasswordBox` 应继续通过不携带明文的事件和按需读取的 `SecurePassword` 提供能力。

## 测试与视觉回归

- 测试应同时验证默认呈现、可选能力和继承自 WPF 的原生行为。
- 创建或操作 WPF 控件、窗口、模板、Dispatcher 或 UI Automation Peer 的测试使用 `[STATestClass]`。
- 模板测试可以定位公开的 `PART_*` 部件，不应依赖易变的非契约视觉树层级。
- 视觉快照覆盖 Light、Dark、HighContrast 以及 100%、150%、200% DPI。
- 视觉快照不能替代行为断言；调整默认视觉时仍需验证原生选择、编辑、输入和自动化能力。
- 测试必须同时兼容 `net472` 和 `net8.0-windows`。

具体的测试组织、断言边界和 WPF 测试方法遵循[测试规范](../development/testing.md)。

## 控件评审清单

提交新增或修改的控件前，确认：

- [ ] 没有删除或屏蔽基类已有的公开能力。
- [ ] 新增 API 复用了 WPF 语义，并具有完整的 XML 文档注释。
- [ ] 公共属性描述控件语义，没有泄漏触发器、VisualState 或模板部件等实现细节。
- [ ] 状态属性使用一致的 `Hover`、`Pressed`、`Disabled`、`Focused`、`Selected` 或 `Checked` 前缀。
- [ ] 没有为了跨框架一致而替换 WPF 使用者已经熟悉且语义准确的名称。
- [ ] `Mode`、`Orientation`、`Variant` 等宽泛或具有强既定语义的名称经过了作用域和歧义检查。
- [ ] CLR 属性、依赖属性标识符、附加属性访问器和路由事件标识符正确配对。
- [ ] `PART_*`、VisualState、绑定和布局契约保持有效。
- [ ] 默认、Hover、Pressed、键盘焦点、Disabled、只读、验证错误和高对比度状态已按适用范围覆盖。
- [ ] 状态组合不会造成内容不可读、焦点不可见或布局跳动。
- [ ] 颜色使用语义化 `DynamicResource`，并验证 Light、Dark 和 HighContrast。
- [ ] 键盘、鼠标、UI Automation 和可访问性契约得到保留。
- [ ] 默认呈现、可选能力和关键原生行为都有 STA 回归测试。
- [ ] Gallery 普通示例保留组件默认字号；专项定制遵循[文档编写规范](../development/documentation.md#gallery-控件示例)并明确限制作用域。
- [ ] 视觉变化已在 100%、150% 和 200% DPI 下审查。
- [ ] 不兼容变更已记录兼容性影响，并更新 `CHANGELOG.md` 和版本号。
