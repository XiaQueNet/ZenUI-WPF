---
name: zenui-wpf
description: 使用 ZenUI.Wpf 和 ZenUI.Wpf.Converters 开发 WPF 应用界面，包括组件选型、XAML、数据绑定、表单与数据表格、主题与密度定制，以及接入问题排查。用户要求使用 ZenUI，或目标项目已经引用 ZenUI 时使用；不用于组件库内部控件实现、打包发布或其他 UI 框架。
---

# 使用 ZenUI 开发 WPF 界面

交付符合目标项目架构、使用真实 ZenUI API、能构建和运行的界面。保留用户选择的布局、功能和视觉要求。

## 先确认使用环境

- 查看目标项目的目标框架、包引用和版本管理文件、App 资源、现有页面及 DataContext 设置方式。
- 本 Skill 的源码基线是控件包 `0.1.0-preview.12`、转换器包 `0.1.0-preview.11`。这是参考基线，不代表 NuGet 最新版本，不自动升级或统一两个包的版本。
- 优先使用目标项目实际版本的 API。遇到本资料未覆盖的属性或版本差异，查对应版本源码、包内 XML 文档或可访问的官方示例；未确认的 API 不写入成品。
- 本 Skill 自带必要资料，不要求使用者拥有组件库源码。资料中源码路径仅用于在有源码时定位实现。

## 按任务读取资料

| 当前任务 | 阅读资料 |
| --- | --- |
| 首次接入、命名空间或资源加载 | [接入](references/getting-started.md) |
| 选择控件、确认属性与数据类型 | [组件](references/controls.md) |
| 表单、列表、日期时间、浮层与绑定 | [组合示例](references/recipes.md) |
| 颜色、主题切换、Density 和 Token | [主题](references/theming.md) |
| 可见性转换、转换器包 | [转换器](references/converters.md) |
| 编译失败、无样式、绑定或主题异常 | [排查与验收](references/troubleshooting.md) |

只读取本次任务需要的资料。需要表单起点时参考 [表单视图](assets/FormView.xaml)，其 DataContext 契约在组合示例中；它不是完整应用，不把示例业务字段直接当成用户需求。

## 生成界面的规则

- 对库已覆盖的需求优先选用 ZenUI 控件。布局使用 WPF 的 Grid、StackPanel、Border 等；不要因其他控件库的命名习惯创造 ZenCard、ZenDialog、ZenWindow 等类型。库未覆盖的需求使用现有项目能力或 WPF 组合实现。
- 控件沿用 WPF 的绑定、命令和事件机制。保持现有 MVVM 框架，不因 Gallery 使用 Prism 就安装 Prism，不在可复用视图中强行覆盖调用方的 DataContext。
- 使用真实的属性名、枚举值和数据类型；尤其区分按钮的 Variant 与 Appearance、日期与时间的选择属性，以及 decimal 数值与可空时间。
- 定制依次考虑公开属性、语义 Token、具名样式、模板。模板调整只用于前三种不能表达的结构需求，保留键盘焦点、禁用、验证错误和原生交互。
- 需要随主题变化的颜色和尺寸使用 DynamicResource。确认资源已进入应用可见范围，不复制 Gallery 专用资源名到业务项目。
- 保留可见标签、键盘操作和必要的 AutomationProperties.Name。以留白、排版和对齐组织信息，强调色优先用于主操作、焦点和状态。
- ZenPasswordBox 不支持明文密码绑定；按组件资料使用 SecurePassword，不添加明文依赖属性绕过其设计。

## 完成与验证

构建受影响的消费者项目，检查新增绑定、资源和实际交互。主题变更覆盖 Light、Dark、HighContrast；密度变更检查受影响的尺寸和内容裁切。针对具体改动选择检查范围，不要求每个页面任务跑组件库全框架发布矩阵。

交付时简述文件变化、如何使用、已验证项目和未验证项。编译通过不代表运行时绑定或视觉已通过；无法启动界面时明确说明。

维护者更新本 Skill 时，按目标包版本重新核对引用的 API、资源键和示例，并使用 [验收场景](references/troubleshooting.md) 做行为检查。
