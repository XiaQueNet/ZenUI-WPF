# Changelog

本项目遵循语义化版本。尚未发布的改动记录在 `Unreleased`。

## Unreleased

## [0.1.0-preview.7] - 2026-08-03

### Added

- 增加 `ZenDateTimePicker` 日期时间选择控件，支持日历与时分秒一体弹层、确认式提交、文本输入、日期时间范围和 12/24 小时制；“此刻”操作会同步更新年月日时分秒。
- `ZenNumberBox` 增加增减按钮的自定义内容与内容模板属性，支持将数字编辑区域的点击操作绑定到 MVVM 命令，并让有效文本输入实时更新双向绑定的 `Value`。
- 增加 `ZenLoading` 加载状态容器，支持内容遮罩、可选说明文字、水平与垂直布局、交互阻断、主题与 Density 动态切换及 UI Automation 语义。

### Changed

- `ZenPopover` 将触发内容 API 更名为 `Anchor`、`AnchorTemplate` 和 `AnchorButtonStyle`；未设置锚点时保留默认问号样式，设置后完整呈现自定义锚点内容。

### Fixed

- 修正 Gallery 控件属性表的引入版本映射，并让未登记项明确显示为“待核对”，避免回退到错误的固定版本。

## [0.1.0-preview.6] - 2026-07-30

### Added

- `ZenAlert` 增加 `IconSize` 属性，用于自定义提示图标尺寸。
- 增加 `ZenPopover` 轻量浮层控件，支持自定义触发内容、显示方位、箭头和气泡尺寸。
- 增加 `ZenRadioGroup` 单选组控件，支持互斥选择、键盘导航、排列方向、等分布局以及 Radio、Filled、Outline、Ghost、Underline、Segment 六种视觉样式。
- 增加 `ZenContextMenu` 与 `ZenMenuItem` 右键菜单控件，支持图标、快捷键提示、勾选项、分隔线和级联菜单。
- 增加 `ZenExpander` 折叠面板控件，保留原生展开方向、事件和自动化语义，并支持标题、内容区与展开标识的主题化尺寸。
- 增加 `ZenTimePicker` 时间选择控件，支持文本输入、弹层选择、12/24 小时制、秒显示、选择步长和时间范围约束。

### Changed

- Calendar 日期按钮改为自动填充等分网格，并移除不再需要的日期按钮宽高属性与 Density Token。
- 调整 `ZenButton` 三档 Density 的默认内边距；按钮继续使用内容驱动的自动宽高，不设置固定或最小尺寸。
- 带内部滚动区域的控件统一处理嵌套鼠标滚轮：控件存在垂直滚动范围时保留滚轮事件，仅在没有可滚内容时将滚动交给外层容器。

## [0.1.0-preview.5] - 2026-07-27

### Added

- `ButtonVariant` 增加 `Neutral` 中性操作样式。
- `ZenCheckBox` 与 `ZenRadioButton` 增加 `IndicatorSize` 属性，用于自定义选择标识尺寸。
- `ZenNumberBox` 增加 `SpinButtonWidth` 属性，用于统一设置增减按钮宽度。

### Changed

- 标准化公开控件 API 命名：`ZenAlert.Variant` 更名为 `Severity`，`ZenNumberBox.ButtonMode` 更名为 `SpinButtonLayout`，`ZenPasswordBox.IsPasswordRevealEnabled` 更名为 `IsPasswordRevealButtonEnabled`。

## [0.1.0-preview.4] - 2026-07-27

### Added

- 增加可独立使用的 `ZenCalendar` 日历控件及 Gallery 示例，支持单日、单范围、多范围选择和 Density 动态尺寸。

### Changed

- 统一所有 Zen 控件默认正文字号为 `ZenFontSizeBody`（14），并保留 Token 与控件属性覆盖能力。
- 修复 DatePicker 弹层内模板绑定受 Popup 边界影响而回退到小尺寸和小字号，以及范围外日期被隐藏而非显示为禁用态的问题。

## [0.1.0-preview.3] - 2026-07-25

### Added

- 增加 `ZenListBox` 列表选择控件及 Gallery 示例，支持单选、多选、键盘导航、虚拟化和主题化交互状态。
- 增加独立的 `ZenUI.Wpf.Converters` NuGet 包，提供可在任意 WPF 项目中使用的常用值转换器。
- 增加语义颜色、组件颜色、Typography、Interaction、Metrics 与 Component Metrics 分层 Token。
- 增加 Compact、Standard、Comfortable 三档 Density 及运行时切换 API。
- 增加 DataGrid、Calendar、Switch、Slider、ProgressBar 与 Alert 的 Density Token。
- 增加主题 × 密度 × DPI 组合视觉回归及 Calendar Popup 快照。
- 增加 Gallery 的完整 Token 目录与 Density 实时预览。
- 增加中文主题定制、迁移、测试、文档和控件设计规范。
- 增加 DocFX 文档站与 POS 示例商品、购物车交互。

### Changed

- 将转换器及其测试从控件包迁移到独立项目，并使用专属 XAML 命名空间。
- 统一公开 XAML 命名空间 URI，并重构主题颜色与组件尺寸资源分层。
- 统一输入控件、Button、ListBox、ScrollBar 和 ComboBox Popup 的尺寸规格。
- 完善 Calendar Popup 的显式尺寸传递以及 DataGrid 焦点、校验和选择状态模板。
- 高对比度主题使用完整不透明度状态 Token，并在系统状态变化时自动重应用。
- 按控件拆分 WPF 控件测试，保持 `net472` 与 `net8.0-windows` 双目标覆盖。
- 优化 Gallery 侧边栏、主题与密度入口以及各控件示例。

### Fixed

- 修复动态尺寸资源跨 Calendar Popup 边界无法可靠解析的问题。
- 修复系统高对比度仅在首次应用主题时生效、后续状态变化不自动更新的问题。
- 修复 DataGrid 单元格焦点和校验状态改变内容布局的问题。

## [0.1.0-preview.2] - 2026-07-24

### Added

- 增加 `ZenDatePicker` 日期选择控件及 Gallery 示例，支持水印、圆角、日期格式、范围和禁用日期。
- 增加 `ZenNumberBox` 数字输入控件，支持步进、范围、格式化和自动化语义。
- 增加 `ZenTextBox` 与 `ZenPasswordBox` 的前置、后置内容。
- 增加 `ZenPasswordBox` 密码显示与隐藏功能。
- 增加按钮外观及悬停、按下状态画刷的自定义能力。
- 增加 Light、Dark、HighContrast 主题下的滚动条样式。
- 增加 Gallery 主题切换器和 Prism POS 示例。
- 增加版本发布、代码注释和测试规范。

### Changed

- 统一文本框、密码框、组合框、数字输入框和日期选择器的内边距与圆角布局。
- 将演示项目从 Demo 重命名为 Gallery。
- 扩充公共 API 的 XML 文档与主题、转换器测试。

### Removed

- 移除 `ZenPasswordBox.Password` 和 `EnableInsecurePasswordBinding` 明文密码绑定 API；请使用 `PasswordChanged` 与 `SecurePassword`。

### Fixed

- 修复 `ZenDatePicker` 日历标题切换、只读文本输入和点击区域交互。
- 修复焦点装饰模板解析共享资源时的异常。
- 修复连续关闭日期选择器弹层测试时的 WPF 窗口句柄清理竞争。

## [0.1.0-preview.1] - 2026-07-23

### Added

- 增加 ZenUI 品牌 Logo，并作为 README 与 NuGet 包图标。
- 增加 .NET 8 WPF 目标框架。
- 增加 Dark、HighContrast 主题与运行时主题切换 API。
- 增加输入验证、密码框与 Alert 无障碍语义。
- 增加 CI、静态分析、符号包和 Source Link。
- 项目采用 MIT License，并写入 NuGet 包元数据。
- 增加 Light、Dark、HighContrast 在 100%/150%/200% DPI 下的视觉快照产物。

### Changed

- Slider 支持垂直方向。
- ProgressBar 支持垂直方向和不确定状态。
- ComboBox 支持可编辑模式。
- Password 明文绑定默认关闭，并标记为过时兼容 API。
- DataGrid 恢复行虚拟化、行头、行详情、全选、冻结列偏移和高级编辑契约。

[Unreleased]: https://github.com/XiaQueNet/ZenUI-WPF/compare/v0.1.0-preview.7...HEAD
[0.1.0-preview.7]: https://github.com/XiaQueNet/ZenUI-WPF/compare/v0.1.0-preview.6...v0.1.0-preview.7
[0.1.0-preview.6]: https://github.com/XiaQueNet/ZenUI-WPF/compare/v0.1.0-preview.5...v0.1.0-preview.6
[0.1.0-preview.5]: https://github.com/XiaQueNet/ZenUI-WPF/compare/v0.1.0-preview.4...v0.1.0-preview.5
[0.1.0-preview.4]: https://github.com/XiaQueNet/ZenUI-WPF/compare/v0.1.0-preview.3...v0.1.0-preview.4
[0.1.0-preview.3]: https://github.com/XiaQueNet/ZenUI-WPF/compare/v0.1.0-preview.2...v0.1.0-preview.3
[0.1.0-preview.2]: https://github.com/XiaQueNet/ZenUI-WPF/compare/v0.1.0-preview.1...v0.1.0-preview.2
[0.1.0-preview.1]: https://github.com/XiaQueNet/ZenUI-WPF/releases/tag/v0.1.0-preview.1
