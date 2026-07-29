<p align="center">
  <img src="https://raw.githubusercontent.com/XiaQueNet/ZenUI-WPF/main/assets/brand/zenui-logo.png" alt="ZenUI for WPF" width="570" />
</p>

面向 .NET Framework 4.7.2 与现代 .NET 8 WPF 的控件库和通用转换器。

## NuGet 包

| 包 | 用途 |
| --- | --- |
| `ZenUI.Wpf` | 控件、主题与设计令牌 |
| `ZenUI.Wpf.Converters` | 可独立使用的通用 WPF 值转换器 |

两个包互不依赖，可以按需单独安装。

## 设计原则

ZenUI 的目标是优化 WPF 控件的默认体验，而不是缩减 WPF 已有能力：

- 保留基类控件原有的属性、事件、命令、选择、编辑、键盘操作、自动化与可访问性契约。
- 主要通过默认值、主题资源和控件模板调整外观与交互反馈。
- 某项原生能力即使不适合作为默认视觉，也不应被删除；应保留行为，并提供依赖属性、资源 Token 或样式入口供使用者显式开启和定制。
- 默认体验与可选能力都应有自动化测试，避免外观调整意外破坏原生功能。

组件状态、公共 API、模板契约、主题资源、可访问性与评审清单参见[控件设计规范](docs/design/component-design.md)。

例如，`ZenDataGrid` 默认只显示行 Hover 反馈，但选择行为仍然存在；需要时可分别开启行选中高亮和当前单元格焦点框：

```xaml
<zen:ZenDataGrid
    IsRowSelectionHighlightEnabled="True"
    IsCellFocusVisualEnabled="True" />
```

## 使用

项目引用或安装 NuGet 包后，可以通过稳定的 XAML 命名空间使用控件：

```xaml
<Window
    xmlns:zen="https://zenui.mnorg.cn/xaml/wpf">
    <StackPanel>
        <zen:ZenButton Content="保存" Variant="Primary" />
        <zen:ZenSwitch IsChecked="True" />
        <zen:ZenTextBox Watermark="请输入内容" CornerRadius="8" />
        <zen:ZenPasswordBox Watermark="请输入密码" />
        <zen:ZenCheckBox Content="记住登录状态" />
        <zen:ZenRadioButton Content="选项 A" GroupName="Options" />
        <zen:ZenComboBox Watermark="请选择" />
        <zen:ZenListBox SelectedIndex="0">
            <ListBoxItem Content="选项 A" />
            <ListBoxItem Content="选项 B" />
        </zen:ZenListBox>
        <zen:ZenCalendar SelectionMode="SingleRange" />
        <zen:ZenDatePicker Watermark="请选择日期" />
        <zen:ZenSlider Maximum="100" Value="60" />
        <zen:ZenProgressBar Maximum="100" Value="60" />
        <zen:ZenAlert Content="保存成功" Severity="Success" />
        <zen:ZenDataGrid ItemsSource="{Binding Items}" />
    </StackPanel>
</Window>
```

目前提供 Button、TextBox、NumberBox、PasswordBox、Switch、CheckBox、RadioButton、RadioGroup、ComboBox、ListBox、Calendar、DatePicker、DataGrid、Slider、ProgressBar、Alert、Popover、ContextMenu 等常用控件。所有控件均自带默认主题，并覆盖悬停、焦点、选中和禁用等常见交互状态。

转换器包使用独立的 XAML 命名空间，无需在应用资源中注册实例：

```xaml
<Window
    xmlns:zc="https://zenui.mnorg.cn/xaml/wpf/converters">
    <ProgressBar
        Visibility="{Binding IsLoading,
            Converter={zc:BoolToVisibilityConverter}}" />
</Window>
```

当前提供布尔值、空值、集合内容和数值比较到 `Visibility` 的转换，并统一支持结果反转以及 `Collapsed`/`Hidden` 配置。

控件的默认样式由 `Themes/Generic.xaml` 自动加载。应用需要直接使用 ZenUI 颜色资源或具名样式时，可以显式合并默认主题：

```xaml
<ResourceDictionary Source="pack://application:,,,/ZenUI.Wpf;component/Themes/Generic.xaml" />
```

## 主题

默认使用浅色主题。深色和高对比度主题可在运行时应用，高对比度主题使用 Windows 系统颜色：

```csharp
using ZenUI.Wpf.Theming;

ZenThemeManager.ApplyTheme(Application.Current.Resources, ZenTheme.Dark);
```

`ApplyTheme` 默认尊重并持续监听 Windows 高对比度设置，系统状态变化时会在原资源字典所属的 UI 线程自动重应用主题；如需预览指定主题，可以将第三个参数设为 `false`，同时停止该资源字典的系统高对比度跟随。也可以在应用资源中把 `Themes/Dark.xaml` 或 `Themes/HighContrast.xaml` 合并到 `Generic.xaml` 之后。所有控件颜色均通过语义化 `DynamicResource` 获取，应用仍可覆盖单个 Token。

主题、Density、应用级 Token 覆盖和旧样式迁移的完整示例参见[主题、Density 定制与迁移指南](docs/guides/theme-customization.md)。

TextBox、PasswordBox、ComboBox 和 DataGrid 单元格支持 WPF `Validation.HasError` 错误状态。Slider 支持水平与垂直方向，ProgressBar 支持垂直方向与 `IsIndeterminate`，ComboBox 支持 `IsEditable`。

## 密码安全

`ZenPasswordBox` 默认不会把密码明文复制到依赖属性或 ViewModel。通过不携带明文的 `PasswordChanged` 事件获知变化，并仅在需要时读取和释放 `SecurePassword`：

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

## 构建

- 控件库（`net472` 与 `net8.0-windows`）：`dotnet build src/ZenUI.Wpf/ZenUI.Wpf.csproj -c Release`
- 转换器库（`net472` 与 `net8.0-windows`）：`dotnet build src/ZenUI.Wpf.Converters/ZenUI.Wpf.Converters.csproj -c Release`
- 控件 Gallery：`dotnet build samples/ZenUI.Wpf.Gallery/ZenUI.Wpf.Gallery.csproj -c Release`
- 自动化测试：`dotnet test ZenUI.Wpf.slnx -c Release`
- 开发用 NuGet 包：分别对 `src/ZenUI.Wpf/ZenUI.Wpf.csproj` 和 `src/ZenUI.Wpf.Converters/ZenUI.Wpf.Converters.csproj` 执行 `dotnet pack`
- 正式发布包：执行 `.\scripts\pack-release.ps1 -Version <version>`，产物固定生成到 `artifacts/releases/<version>`

`samples/ZenUI.Wpf.Gallery` 是 ZenUI 控件目录，使用 Prism Region Navigation 和 MVVM：`MainWindow` 只负责 Shell 布局，菜单由 `MainWindowViewModel` 驱动，每个组件位于独立的 `Views/*View.xaml` 页面。Gallery 仍以 .NET Framework 4.7.2 为目标框架，Prism 依赖不会传递到 `ZenUI.Wpf` 控件库。`samples/ZenUI.Wpf.PosDemo` 则负责展示完整的业务应用场景。

仓库在 Windows CI 中将编译器与 .NET 分析器警告视为错误，并同时运行 `net472`、`net8.0-windows` 测试及 NuGet/Symbol 包验证。

CI 还会生成 Light、Dark、HighContrast × Compact、Standard、Comfortable 在 125%、150%、200% DPI 下的 PNG 快照，并单独生成 Calendar Popup 的主题与密度快照，作为 `visual-regression-snapshots` 构件供界面审查。自动化测试同时覆盖 DataGrid 的虚拟化、编辑、排序、行头、行详情、冻结列、多选与 RTL 布局，以及各控件的基础 UI Automation 类型。

## 维护文档

- [文档索引](docs/README.md)
- [控件设计规范](docs/design/component-design.md)
- [主题、Density 定制与迁移指南](docs/guides/theme-customization.md)
- [测试规范](docs/development/testing.md)
- [AI 测试工作流](docs/development/ai-testing-workflow.md)
- [C# 注释规范](docs/development/commenting.md)
- [文档编写规范](docs/development/documentation.md)
- [版本与发布规范](docs/maintainers/releasing.md)
- [贡献指南](CONTRIBUTING.md)
- [变更记录](CHANGELOG.md)

## License

ZenUI.Wpf 使用 [MIT License](LICENSE)。
