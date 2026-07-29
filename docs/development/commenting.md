# C# 注释规范

本文约定 ZenUI.Wpf 源码中的注释写法。注释用于补充代码无法清楚表达的契约、原因和限制，不用于逐行翻译代码。

## 基本原则

- 优先通过准确命名和清晰结构表达意图。
- 注释说明“为什么”“有什么约束”，不重复“代码做了什么”。
- 修改实现时同步更新附近注释；无法确认仍然正确的注释应删除或核实。
- 项目注释使用中文，框架类型、协议名、API 名称等保留原文。
- 自动生成文件保持生成器输出，不手工整理其中的注释。

不推荐：

```csharp
// 将值设置为最大值。
value = Maximum;
```

推荐：

```csharp
// 依赖属性的值也可能由绑定或动画写入，因此统一在 CoerceValueCallback 中限制范围。
value = Math.Min(value, Maximum);
```

## XML 文档注释

`ZenUI.Wpf` 是对外发布的控件库。新增或修改公共 API 时，应为类型和成员提供有意义的 XML 文档注释，包括：

- `public` 类型及其公开成员；
- 控件的依赖属性、路由事件和命令；
- 枚举及其成员；
- 不明显的参数约束、返回值、异常和线程要求。

仅继承基类或接口契约、且没有额外语义的成员，可使用 `<inheritdoc />`。普通构造函数、私有成员和显而易见的重写无需为覆盖率而补空泛注释。

### 标准句式

公共 API 注释采用与 Microsoft .NET API 文档一致的职责句式，并以中文句号结尾：

- 类型：使用“表示……”；仅包含静态成员的管理类可使用“提供……”。
- 构造函数：使用“初始化 `<see cref="类型"/>` 类的新实例。”。
- 可读写属性：使用“获取或设置……”。
- 只读属性：使用“获取……”。
- 布尔属性：使用“获取或设置一个值，该值指示是否……”；只读布尔属性使用“获取一个值，该值指示是否……”。
- 方法：以描述操作的动词开头，例如“应用……”“清除……”“获取……”或“设置……”。
- 事件：使用“当……时发生。”，并说明准确的触发时机。
- 依赖属性字段：使用“标识 `<see cref="属性名"/>` 依赖属性。”。
- 只读依赖属性字段：使用“标识 `<see cref="属性名"/>` 只读依赖属性。”。
- 路由事件字段：使用“标识 `<see cref="事件名"/>` 路由事件。”。

属性说明应表达使用者能够观察到的语义，而不是重复属性名。涉及默认值、绑定行为、有效范围或特殊边界时，应明确记录，但不要描述可替换的模板实现细节。

```csharp
/// <summary>
/// 表示支持水印和自定义圆角的文本输入控件。
/// </summary>
public class ZenTextBox : TextBox
{
    /// <summary>
    /// 获取或设置一个值，该值指示输入框是否为只读。
    /// </summary>
    public bool IsReadOnly { get; set; }
}
```

常用标签：

- `<summary>`：简要说明职责或行为。
- `<value>`：补充属性值的含义和默认值；不要用它重复 `<summary>`。
- `<param>`：说明参数含义、单位、范围或 `null` 约定。
- `<returns>`：说明返回值及特殊值。
- `<exception>`：说明该 API 明确抛出的异常及条件。
- `<remarks>`：补充生命周期、兼容性、线程、性能或安全限制。
- `<inheritdoc />`：继承已有契约，避免复制后产生偏差。
- `<see cref="..."/>`：引用类型或成员，避免在正文中手写可能失效的 API 名称。
- `<see langword="..."/>`：引用 `true`、`false`、`null` 等语言关键字。
- `<paramref name="..."/>`：在正文中引用参数。
- `<c>...</c>`：标记短代码、枚举文本或字面量。

示例：

```csharp
/// <summary>
/// 将指定主题应用到资源字典。
/// </summary>
/// <param name="resources">接收主题资源的资源字典。</param>
/// <param name="theme">要应用的主题。</param>
/// <remarks>
/// 必须在拥有该资源字典的 UI 线程上调用。
/// </remarks>
public static void ApplyTheme(ResourceDictionary resources, ZenTheme theme)
{
}
```

属性的默认值与摘要分开记录：

```csharp
/// <summary>
/// 获取或设置用作浮层触发器的内容。
/// </summary>
/// <value>用作浮层触发器的内容。默认值为 <c>?</c>。</value>
public object Trigger { get; set; }
```

文档应描述公开契约，不要承诺可替换的实现细节。涉及尺寸、时间、比例等数值时说明单位；布尔属性说明 `true` 表示什么；事件说明触发时机。具有验证回调的数值属性应说明有效范围以及是否接受 `NaN` 或无穷大：

```csharp
/// <summary>
/// 获取或设置相邻选项之间的间距。该值必须为大于或等于零的有限值。
/// </summary>
public double Spacing { get; set; }
```

### WPF 属性与事件

依赖属性的 CLR 包装器和标识字段都属于公开 API，应分别提供注释。字段通过 `<see cref="..."/>` 引用对应属性：

```csharp
/// <summary>
/// 获取或设置控件的圆角半径。
/// </summary>
public CornerRadius CornerRadius
{
    get { return (CornerRadius)GetValue(CornerRadiusProperty); }
    set { SetValue(CornerRadiusProperty, value); }
}

/// <summary>
/// 标识 <see cref="CornerRadius"/> 依赖属性。
/// </summary>
public static readonly DependencyProperty CornerRadiusProperty = /* ... */;
```

附加属性的 Getter 和 Setter 是独立的公开方法，除摘要外必须记录全部参数，Getter 还必须记录返回值：

```csharp
/// <summary>
/// 获取指定元素的日期按钮宽度。
/// </summary>
/// <param name="element">要从中读取属性值的元素。</param>
/// <returns>指定元素的日期按钮宽度。</returns>
public static double GetDayButtonWidth(DependencyObject element)
{
    return (double)element.GetValue(DayButtonWidthProperty);
}

/// <summary>
/// 设置指定元素的日期按钮宽度。
/// </summary>
/// <param name="element">要在其上设置属性值的元素。</param>
/// <param name="value">要设置的宽度。</param>
public static void SetDayButtonWidth(DependencyObject element, double value)
{
    element.SetValue(DayButtonWidthProperty, value);
}
```

### 术语与格式

- 项目注释使用中文；类型名、成员名、XAML、WPF、UI Automation 等专有名称保留原文。
- 使用“鼠标悬停”，不使用“鼠标悬浮”。
- 使用“验证错误”，不使用“校验错误”。
- 面向控件使用者时优先使用“滑块手柄”；需要精确引用 WPF 类型时使用 `<see cref="Thumb"/>`。
- 使用 `<see cref="..."/>` 引用 `ToolTip`、属性和枚举等 API，不在正文中用反引号代替 XML 引用。
- `<inheritdoc />` 等自闭合 XML 标签在斜杠前保留一个空格。
- 一条摘要包含多个完整句子时使用句号分隔，不使用分号连接不同契约。

### 构建检查

`ZenUI.Wpf` 和 `ZenUI.Wpf.Converters` 生成 XML 文档文件。仓库启用了 `TreatWarningsAsErrors`，不得通过 `NoWarn`、`#pragma warning disable` 或降低警告级别屏蔽 `CS1591`。新增公开类型或成员缺少 XML 文档注释时，构建应失败。

如果某个类型因 WPF XAML 加载、模板绑定或设计器要求而必须保持 `public`，即使使用 `[EditorBrowsable(EditorBrowsableState.Never)]` 隐藏，它仍是程序集的公开成员，也必须提供 XML 文档注释。

## 代码内注释

仅在以下信息无法通过代码直接表达时添加：

- WPF 依赖属性、模板或布局机制带来的限制；
- 多目标框架或系统版本兼容处理；
- 可访问性、安全、性能或线程约束；
- 有意忽略、降级或重试的异常处理；
- 看似多余但不能删除的实现及其原因。

模板部件优先通过 `PART_` 命名和 `TemplatePartAttribute` 表达契约，资源优先通过语义化 Key 表达用途，不用大段注释代替明确结构。

## 标记注释

允许使用 `TODO`、`FIXME`、`HACK`、`COMPAT`、`PERF` 和 `SECURITY`。标记必须写明当前原因和可执行的后续动作；有对应 Issue 时附上编号或链接。

```csharp
// COMPAT: .NET Framework 4.7.2 不提供该重载；移除 net472 目标后可改用新 API。
```

```csharp
// TODO(#42): 基线快照稳定后，将颜色数量检查替换为逐像素差异比较。
```

禁止无上下文的标记：

```csharp
// TODO: 优化
```

## 提交前检查

- 公共 API 的注释是否描述了真实契约？
- 类型、属性、布尔属性、事件及依赖属性字段是否使用统一句式？
- 附加属性访问器是否完整记录参数和返回值？
- 参数限制、单位、异常、线程和兼容性要求是否清楚？
- XML 文档构建是否在未屏蔽 `CS1591` 的情况下通过？
- 是否存在重复代码、空泛、过期或被注释掉的代码？
- 是否可以通过更好的命名或拆分删除注释？
- 标记注释是否说明原因和后续动作？
