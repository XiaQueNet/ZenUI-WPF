# Changelog

本项目遵循语义化版本。尚未发布的改动记录在 `Unreleased`。

## Unreleased

计划发布的首个公开预览包版本为 `0.1.0-preview.1`。

### Added

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
