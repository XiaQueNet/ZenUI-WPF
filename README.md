<p align="center">
  <img src="https://raw.githubusercontent.com/XiaQueNet/ZenUI-WPF/main/assets/brand/zenui-logo.png" alt="ZenUI for WPF" width="570" />
</p>

<p align="center">
  面向 .NET Framework 4.6.2 及以上版本与现代 .NET 8 WPF 的控件库和通用转换器。
</p>

ZenUI 以克制、清晰的 Zen Style 改善 WPF 控件的默认体验，在保证性能的同时，保留原生属性、事件、命令、键盘操作和可访问性契约。

## 核心能力

- 提供 Button、TextBox、DataGrid、DatePicker、DateTimePicker 等常用 WPF 控件。
- 内置 Light、Dark、HighContrast 主题，支持运行时切换。
- 提供 Compact、Standard、Comfortable 三档界面密度。
- 使用语义化设计 Token，应用无需复制模板即可覆盖颜色与尺寸。
- 覆盖悬停、按下、焦点、选中、禁用、只读和验证错误等交互状态。
- 提供 `net462`、`net471`、`net472`、`net5.0-windows` 与 `net8.0-windows` 资产；正式支持 .NET Framework 4.6.2 及以上版本与 .NET 8 及以上版本，并为已停止维护的 .NET 5/6/7 提供兼容资产。

## 安装

按需安装控件库或转换器包：

```powershell
dotnet add package ZenUI.Wpf
dotnet add package ZenUI.Wpf.Converters
```

| 包 | 用途 |
| --- | --- |
| `ZenUI.Wpf` | 控件、主题与设计 Token |
| `ZenUI.Wpf.Converters` | 可独立使用的通用 WPF 值转换器 |

两个包互不依赖，可以单独安装。

## 快速开始

引入稳定的 XAML 命名空间后即可使用控件，默认样式会由 `Themes/Generic.xaml` 自动加载：

```xaml
<Window
    xmlns:zen="https://zenui.mnorg.cn/xaml/wpf">
    <StackPanel>
        <zen:ZenTextBox Watermark="请输入内容" />
        <zen:ZenButton Content="保存" Variant="Primary" />
        <zen:ZenSwitch IsChecked="True" />
        <zen:ZenLoading IsLoading="True" LoadingText="正在加载…" />
        <zen:ZenAlert Content="保存成功" Severity="Success" />
    </StackPanel>
</Window>
```

应用需要直接使用 ZenUI Token 或具名样式时，可以显式合并默认主题：

```xaml
<ResourceDictionary Source="pack://application:,,,/ZenUI.Wpf;component/Themes/Generic.xaml" />
```

转换器包使用独立的 XAML 命名空间，无需在应用资源中注册实例：

```xaml
<Window
    xmlns:zc="https://zenui.mnorg.cn/xaml/wpf/converters">
    <ProgressBar
        Visibility="{Binding IsLoading,
            Converter={zc:BoolToVisibilityConverter}}" />
</Window>
```

转换器包提供布尔值、空值、集合内容和数值比较到 `Visibility` 的转换，并统一支持结果反转以及 `Collapsed`、`Hidden` 配置。

## 组件

| 类别 | 组件 |
| --- | --- |
| 操作与反馈 | Button、Switch、CheckBox、RadioButton、RadioGroup、Alert、ProgressBar、Loading |
| 文本与数值输入 | TextBox、PasswordBox、NumberBox、Slider |
| 选择与日期时间 | ComboBox、ListBox、Calendar、DatePicker、TimePicker、DateTimePicker |
| 数据与布局 | DataGrid、Expander |
| 浮层与菜单 | Popover、ContextMenu |

TextBox、PasswordBox、ComboBox 和 DataGrid 单元格支持 WPF `Validation.HasError`。Slider 支持水平与垂直方向，ProgressBar 支持垂直方向与 `IsIndeterminate`，ComboBox 支持 `IsEditable`。

### Prism 弹窗外观

合并 `Themes/Generic.xaml` 后，可为 Prism 注册统一的对话框宿主，让新弹窗自动使用 `ZenDialogWindowStyle`：

```csharp
public sealed class AppDialogWindow : Window, Prism.Services.Dialogs.IDialogWindow
{
    public AppDialogWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        SetBinding(TitleProperty, new System.Windows.Data.Binding("Title"));
        Style = (Style)Application.Current.FindResource("ZenDialogWindowStyle");
    }

    public Prism.Services.Dialogs.IDialogResult Result { get; set; }
}

// PrismApplication.RegisterTypes 中
containerRegistry.RegisterDialogWindow<AppDialogWindow>();
```

Gallery 的 `GalleryDialogWindow` 使用此方式。普通弹窗视图无需再写 `Dialog.WindowStyle`；需要单独调整尺寸、按钮或间距时，再继承具名样式。在弹窗视图根元素绑定操作命令，命令名称由应用决定：

```xaml
<UserControl
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:prism="http://prismlibrary.com/"
    xmlns:zen="https://zenui.mnorg.cn/xaml/wpf"
    zen:ZenDialogWindowOptions.ConfirmCommand="{Binding SaveCommand}"
    zen:ZenDialogWindowOptions.CancelCommand="{Binding DiscardCommand}">
    <prism:Dialog.WindowStyle>
        <Style TargetType="{x:Type Window}" BasedOn="{StaticResource ZenDialogWindowStyle}">
            <Setter Property="Width" Value="480" />
            <Setter Property="zen:ZenDialogWindowOptions.ConfirmContent" Value="保存" />
            <Setter Property="zen:ZenDialogWindowOptions.ContentMargin" Value="24,0,24,20" />
        </Style>
    </prism:Dialog.WindowStyle>
    <!-- 弹窗正文 -->
</UserControl>
```

样式统一呈现左侧标题、右侧关闭按钮和底部的取消／确定按钮，并跟随主题切换。标题来自 `Window.Title`；弹窗视图将按钮命令绑定到自己的 ViewModel，由命令请求 Prism 关闭弹窗并传回结果。右上角按钮直接关闭窗口，仍经过 Prism 的窗口关闭流程。内容区由弹窗视图提供；原有 POS 登录弹窗是全屏自定义布局，继续使用自己的窗口样式。

默认窗口大小为 440 × 240；正文过长时在内容区滚动，底部操作区保持可见。应用可在继承样式中覆盖 `Width`、`Height` 或 `SizeToContent`。

两个操作按钮默认都显示。需要只保留“确定”时，在弹窗视图的 `Dialog.WindowStyle` 中设置：

```xaml
<Setter Property="zen:ZenDialogWindowOptions.ShowCancelButton" Value="False" />
```

相应地，使用 `<Setter Property="zen:ZenDialogWindowOptions.ShowConfirmButton" Value="False" />` 可隐藏“确定”。两个选项都为 `False` 时，底部操作区一并收起。这里的 `zen` 使用 `https://zenui.mnorg.cn/xaml/wpf` 命名空间。

`ConfirmContent`、`CancelContent` 可分别更换按钮内容，默认是“确定”和“取消”。`ContentMargin` 控制正文间距（默认 `20,0,20,20`），`FooterPadding` 控制底部操作区内边距（默认 `20,12`）。这些选项设置在窗口样式上；`ConfirmCommand`、`CancelCommand` 绑定在弹窗视图根元素上。两个命令都由应用提供，未绑定的按钮不会执行操作。

标题栏默认可拖动窗口；若需要固定位置，在继承样式中设置 `<Setter Property="zen:ZenDialogWindowOptions.IsDraggable" Value="False" />`。右上角关闭按钮仍可使用。

`ZenDialogWindowStyle` 同时适用于模态和非模态弹窗。使用 Prism `IDialogService.ShowDialog(...)` 打开模态弹窗；使用 `IDialogService.Show(...)` 打开非模态弹窗，主窗口仍可操作。Gallery 的 Dialog 页面提供非模态窗口和主窗口计数按钮，可直接验证两者的区别。

需要遮罩弹窗时，使用独立的 `ZenOverlayDialogWindow` 和 `ZenOverlayDialogWindowStyle`。给窗口设置已显示的 `Owner`，再调用 `ShowDialog()`；宿主会按 Owner 的可见屏幕边界定位，半透明遮罩覆盖整个 Owner，内容面板默认以 600 × 360 居中显示，窗口不可拖动。面板在小窗口中会缩小以适应可用空间。原有 `ZenDialogWindowStyle` 不受影响。Gallery 的 Dialog 页面提供 Prism 示例：注册一个继承 `ZenOverlayDialogWindow` 的具名 `IDialogWindow`，然后通过 `IDialogService.ShowDialog(viewName, parameters, callback, windowName)` 打开。

面板宽高可由调用方在遮罩窗口样式中设置，修改的是居中的内容面板，不改变外层遮罩的尺寸：

```xaml
<Style BasedOn="{StaticResource ZenOverlayDialogWindowStyle}" TargetType="{x:Type Window}">
    <Setter Property="zen:ZenOverlayDialogWindowOptions.DialogWidth" Value="680" />
    <Setter Property="zen:ZenOverlayDialogWindowOptions.DialogHeight" Value="400" />
</Style>
```

`DialogWidth` 和 `DialogHeight` 的默认值分别为 600 和 360；也可在运行时通过 `ZenOverlayDialogWindowOptions.SetDialogWidth/SetDialogHeight` 修改。若主窗口空间不足，面板自动缩小并保持居中。

## 主题与 Density

默认使用浅色主题。可以在运行时分别切换颜色主题和界面密度：

```csharp
using ZenUI.Wpf.Theming;

ZenThemeManager.ApplyTheme(
    Application.Current.Resources,
    ZenTheme.Dark);

ZenDensityManager.ApplyDensity(
    Application.Current.Resources,
    ZenDensity.Compact);
```

主题管理器默认尊重并持续监听 Windows 高对比度设置。所有控件颜色均通过语义化 `DynamicResource` 获取，应用可以只覆盖单个 Token，不必复制完整控件模板。

完整用法参见[主题、Density 定制与迁移指南](docs/guides/theme-customization.md)。

## 设计原则

Zen Style 的核心不是简单减少元素，而是删除噪声、保留必要信息，并将必要信息呈现得从容、清晰：

- 优先使用留白、排版和对齐建立信息层级，避免装饰堆叠。
- 中性色承担主要结构，强调色只用于主要操作、焦点和明确状态。
- 动画只用于解释状态变化、操作反馈或空间关系。
- 次要能力按需呈现，不与当前任务争夺注意力。
- 极简不能牺牲可读性、可访问性、状态辨识或操作效率。
- 默认样式与交互应保持轻量，避免不必要的视觉层级、布局开销和持续动画。

ZenUI 只调整 WPF 控件的默认值、主题资源和控件模板，不以视觉简化为由删除基类能力。非默认视觉通过依赖属性、设计 Token、具名样式或模板入口保留，并由自动化测试覆盖。

公共 API、状态命名、模板契约、主题资源和可访问性的完整要求参见[控件设计规范](docs/design/component-design.md)。

## 密码安全

`ZenPasswordBox` 不会把密码明文复制到依赖属性或 ViewModel。通过不携带明文的 `PasswordChanged` 事件获知变化，并仅在需要时读取和释放 `SecurePassword`：

```csharp
private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
{
    var passwordBox = (ZenPasswordBox)sender;
    using (var password = passwordBox.SecurePassword)
    {
        // 立即验证 password，不要长期保存明文副本。
    }
}
```

## 示例与开发

- `samples/ZenUI.Wpf.Gallery`：控件目录，使用 Prism Region Navigation 和 MVVM；“对话框”页面演示 `ZenDialogWindowStyle` 与 Prism `IDialogService` 的配合。
- `samples/ZenUI.Wpf.PosDemo`：完整业务应用示例。

常用验证命令：

```powershell
dotnet restore ZenUI.Wpf.slnx
dotnet build ZenUI.Wpf.slnx -c Release --no-restore
dotnet test --project tests/ZenUI.Wpf.Tests/ZenUI.Wpf.Tests.csproj -c Release -f net472 --max-parallel-test-modules 1 --no-build
dotnet test --project tests/ZenUI.Wpf.Converters.Tests/ZenUI.Wpf.Converters.Tests.csproj -c Release -f net472 --max-parallel-test-modules 1 --no-build
```

日常开发按组件测试、`net472` 单框架全量测试和全框架矩阵测试三个等级验证，具体触发条件参见[测试规范](docs/development/testing.md#回归与验证)。仓库在 Windows CI 中将编译器与 .NET 分析器警告视为错误；主分支 Push 验证 `net472` 与 `.NET 10 for Windows`，Pull Request 和发布流程通过完整测试矩阵逐版本验证 .NET Framework 4.6.2～4.8.1 与 `.NET 8/9/10 for Windows`，并在对应运行时上对 `.NET 5/6/7 for Windows` 执行兼容性契约测试和依赖漏洞扫描。正式发布包包含 `net462`、`net471`、`net472`、`net5.0-windows` 与 `net8.0-windows` 五套资产，并验证 NuGet/Symbol 包、多目标框架消费者安装以及多主题、多 Density、多 DPI 视觉快照。正式产物通过 `.\scripts\pack-release.ps1 -Version <version> -Package <package-id>` 生成；只发布有实际变更的包。

## 参与贡献

欢迎提交功能、修复和文档改进。开始开发前请阅读[贡献指南](CONTRIBUTING.md)：

- 从 `main` 创建短期分支，建议使用 `feature/*`、`fix/*`、`docs/*` 或 `chore/*`。
- Git 提交的标题和正文统一使用中文，并简洁说明实际变更。
- 新增或修改控件时，应保留 WPF 原有能力，并覆盖适用的交互状态、主题和可访问性契约。
- 提交前运行 Release 构建、自动化测试及相关打包检查，确保编译器和分析器警告为零。
- 新增或修改的公共 API 必须提供符合项目规范的 XML 文档注释。

## 文档

- [文档索引](docs/README.md)
- [快速开始](website/getting-started/quick-start.md)
- [控件设计规范](docs/design/component-design.md)
- [主题 Token 规范](docs/design/theme-tokens.md)
- [主题、Density 定制与迁移指南](docs/guides/theme-customization.md)
- [测试规范](docs/development/testing.md)
- [C# 注释规范](docs/development/commenting.md)
- [贡献指南](CONTRIBUTING.md)
- [变更记录](CHANGELOG.md)

## AI 辅助使用

提供面向应用开发的 [zenui-wpf Skill](skills/zenui-wpf/SKILL.md)，帮助 AI 正确选择组件、编写 XAML 和绑定、配置主题与密度，并排查接入问题。Skill 自带 API 要点、组合示例、表单视图和验收场景，可独立于本仓库使用。

在 Windows PowerShell 5.1 或 PowerShell 7 中执行以下命令，即可下载并安装，无需 Git 或管理员权限。命令会执行仓库公开的[安装脚本](scripts/install-from-git.ps1)：

```powershell
iwr -UseBasicParsing "https://raw.githubusercontent.com/XiaQueNet/ZenUI-WPF/main/scripts/install-from-git.ps1" | iex
```

安装到 `$CODEX_HOME/skills/zenui-wpf`；未设置 `CODEX_HOME` 时使用用户目录下的 `.codex/skills/zenui-wpf`。再次运行同一命令即可更新，旧版本保存在 Codex 目录的 `skill-backups` 中，安装失败时保留或恢复旧版本。脚本下载仓库 ZIP，仅提取 Skill 文件，不修改应用项目或安装 NuGet 包。

安装脚本和 `skills/zenui-wpf` 目录发布到 GitHub 的 `main` 分支后，上述在线命令才可使用。已有本地仓库时，也可在仓库根目录离线安装：

```powershell
& .\scripts\install-from-git.ps1 -SourcePath .\skills\zenui-wpf
```

需要指定安装位置时，为本地脚本添加 `-CodexHome 'D:\Codex'`；在线下载指定分支、Tag 或提交时可使用 `-Ref` 参数。

安装后在 Codex 中调用；如果未出现在技能列表中，重启 Codex：

```text
使用 $zenui-wpf，为当前 WPF 项目实现一个订单编辑页，沿用现有 MVVM 架构和包版本。
```

安装后，Codex 可以在请求使用 ZenUI 或项目已引用 ZenUI 时按任务自动选用此 Skill；显式写出 `$zenui-wpf` 可明确指定使用。

Skill 的参考基线为控件包 `0.1.0-preview.12` 和转换器包 `0.1.0-preview.11` 的本地源码 API，不代表最新发布版本。实际开发优先遵循消费者安装版本；组件 API 变化时应同步更新 Skill 资料。

## 交流与反馈

<h3 align="center">
  <a href="https://qun.qq.com/universal-share/share?ac=1&amp;authKey=kkQCZjWfmhA%2FemIxl7g6kzW0mDbWArzaxhFQWWRm34mSvUdaYJK8X5mYacfvkaWP&amp;busi_data=eyJncm91cENvZGUiOiI2NTA1OTAxNzYiLCJ0b2tlbiI6IkthNXNxYWdkRi9UbDdXdFZCeE1LRjNVT3l4ZSsyNDllYmdNeUtac2U1Z0J1VmlpN0NSVGFKZmdyeUVBY04xd2giLCJ1aW4iOiIxMDQwMTUzNjUzIn0%3D&amp;data=wm78IvqFbkqW1duGxv6xP8Ny_iQXCLWcqSrPYm1rYeDUwUIbY8HwkEAVvMlEx-gYJNpqGKfwh8y9hIXGGQlXig&amp;svctype=4&amp;tempid=h5_group_info">👉 点击加入 ZenUI-WPF QQ 交流群</a>
</h3>

<p align="center">
  群号：<code>650590176</code>
</p>

<p align="center">
  <img src="assets/community/qq-group-650590176-qr.png" alt="ZenUI-WPF QQ 交流群二维码，群号 650590176" width="360" />
</p>

## License

ZenUI.Wpf 使用 [MIT License](LICENSE)。
