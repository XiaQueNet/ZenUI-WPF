<p align="center">
  <img src="https://raw.githubusercontent.com/XiaQueNet/ZenUI-WPF/main/assets/brand/zenui-logo.png" alt="ZenUI for WPF" width="570" />
</p>

面向 .NET Framework 4.7.2 与现代 .NET 8 WPF 的控件库。

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
    xmlns:zen="http://zenui.mnorg.com/zenui-wpf">
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
        <zen:ZenDatePicker Watermark="请选择日期" />
        <zen:ZenSlider Maximum="100" Value="60" />
        <zen:ZenProgressBar Maximum="100" Value="60" />
        <zen:ZenAlert Content="保存成功" Variant="Success" />
        <zen:ZenDataGrid ItemsSource="{Binding Items}" />
    </StackPanel>
</Window>
```

目前提供 Button、TextBox、NumberBox、PasswordBox、Switch、CheckBox、RadioButton、ComboBox、ListBox、DatePicker、DataGrid、Slider、ProgressBar 和 Alert 共 14 个常用控件。所有控件均自带默认主题，并覆盖悬停、焦点、选中和禁用等常见交互状态。

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

`ApplyTheme` 默认尊重 Windows 高对比度设置；如需预览指定主题，可以将第三个参数设为 `false`。也可以在应用资源中把 `Themes/Dark.xaml` 或 `Themes/HighContrast.xaml` 合并到 `Generic.xaml` 之后。所有控件颜色均通过语义化 `DynamicResource` 获取，应用仍可覆盖单个 Token。

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
- 控件 Gallery：`dotnet build samples/ZenUI.Wpf.Gallery/ZenUI.Wpf.Gallery.csproj -c Release`
- 自动化测试：`dotnet test tests/ZenUI.Wpf.Tests/ZenUI.Wpf.Tests.csproj -c Release`
- NuGet 包：`dotnet pack src/ZenUI.Wpf/ZenUI.Wpf.csproj -c Release`

`samples/ZenUI.Wpf.Gallery` 是 ZenUI 控件目录，使用 Prism Region Navigation 和 MVVM：`MainWindow` 只负责 Shell 布局，菜单由 `MainWindowViewModel` 驱动，每个组件位于独立的 `Views/*View.xaml` 页面。Gallery 仍以 .NET Framework 4.7.2 为目标框架，Prism 依赖不会传递到 `ZenUI.Wpf` 控件库。`samples/ZenUI.Wpf.PosDemo` 则负责展示完整的业务应用场景。

仓库在 Windows CI 中将编译器与 .NET 分析器警告视为错误，并同时运行 `net472`、`net8.0-windows` 测试及 NuGet/Symbol 包验证。

CI 还会生成 Light、Dark、HighContrast 在 100%、150%、200% DPI 下的 PNG 快照，作为 `visual-regression-snapshots` 构件供界面审查。自动化测试同时覆盖 DataGrid 的虚拟化、编辑、排序、行头、行详情、冻结列、多选与 RTL 布局，以及各控件的基础 UI Automation 类型。

## 维护文档

- [文档索引](docs/README.md)
- [控件设计规范](docs/design/component-design.md)
- [测试规范](docs/development/testing.md)
- [AI 测试工作流](docs/development/ai-testing-workflow.md)
- [C# 注释规范](docs/development/commenting.md)
- [文档编写规范](docs/development/documentation.md)
- [版本与发布规范](docs/maintainers/releasing.md)
- [贡献指南](CONTRIBUTING.md)
- [变更记录](CHANGELOG.md)

## License

ZenUI.Wpf 使用 [MIT License](LICENSE)。
