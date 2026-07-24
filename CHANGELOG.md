# Changelog

本项目遵循语义化版本。尚未发布的改动记录在 `Unreleased`。

## Unreleased

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

[Unreleased]: https://github.com/XiaQueNet/ZenUI-WPF/compare/v0.1.0-preview.2...HEAD
[0.1.0-preview.2]: https://github.com/XiaQueNet/ZenUI-WPF/compare/v0.1.0-preview.1...v0.1.0-preview.2
[0.1.0-preview.1]: https://github.com/XiaQueNet/ZenUI-WPF/releases/tag/v0.1.0-preview.1
