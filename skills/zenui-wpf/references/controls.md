# 组件选择与 API

基线见 [接入](getting-started.md)。本表列常用入口；不是完整 API 清单。WPF 基类属性继续可用，但不要把其他 UI 框架或其他 ZenUI 控件的属性直接套用。

| 需求 | 控件 | 常用入口与注意点 |
| --- | --- | --- |
| 操作按钮 | ZenButton | Content、Command、CommandParameter、Variant、Appearance、CornerRadius |
| 文本输入 | ZenTextBox | Text、Watermark、ShowWatermarkOnFocus、LeadingContent、TrailingContent；继承 TextBox，支持 IsReadOnly、AcceptsReturn |
| 密码输入 | ZenPasswordBox | Watermark、PasswordChanged、SecurePassword、Clear()；见下文 |
| 数量或金额 | ZenNumberBox | Value、Minimum、Maximum、Increment 均为 decimal；IsReadOnly、SpinButtonLayout、ValueChanged |
| 开关 | ZenSwitch | IsChecked；使用布尔绑定，不猜测 IsOn |
| 多选标记 | ZenCheckBox | Content、IsChecked、IsThreeState |
| 独立单选按钮 | ZenRadioButton | Content、IsChecked、GroupName |
| 一组选项/分段选择 | ZenRadioGroup | ItemsSource、SelectedItem/SelectedValue、SelectedValuePath、DisplayMemberPath、Appearance、Orientation、Spacing、IsItemSizeUniform |
| 下拉选择 | ZenComboBox | ItemsSource、SelectedItem/SelectedValue、DisplayMemberPath、Watermark、IsEditable |
| 列表选择 | ZenListBox | ItemsSource、SelectedItem、SelectionMode、ItemTemplate |
| 日期 | ZenDatePicker | SelectedDate 为 DateTime?；Watermark、IsTextInputReadOnly；日期范围沿用 DisplayDateStart/DisplayDateEnd |
| 日历 | ZenCalendar | SelectedDate、SelectionMode、BlackoutDates 等 WPF Calendar API |
| 时间 | ZenTimePicker | SelectedTime 为 TimeSpan?；Minimum/Maximum 为 TimeSpan；MinuteIncrement、SecondIncrement、IsSecondVisible、Is24HourFormat |
| 日期和时间 | ZenDateTimePicker | SelectedDateTime、Minimum、Maximum 为 DateTime?；DateTimeFormat、MinuteIncrement、SecondIncrement、IsSecondVisible、Is24HourFormat |
| 连续数值 | ZenSlider | Minimum、Maximum、Value、Orientation、TickFrequency、IsSnapToTickEnabled |
| 进度 | ZenProgressBar | Minimum、Maximum、Value、IsIndeterminate、Orientation |
| 加载遮罩 | ZenLoading | Content、IsLoading、LoadingText、DisplayDelay、IsContentInteractionBlocked；可包裹页面内容 |
| 页面内提示 | ZenAlert | Content、Severity；不等同于全局 Toast 服务 |
| 折叠内容 | ZenExpander | Header、Content、IsExpanded |
| 数据表格 | ZenDataGrid | ItemsSource、Columns、AutoGenerateColumns、EmptyContent、IsReadOnly |
| 文本表格列 | ZenDataGridTextColumn | Header、Binding、Width；CellHorizontalContentAlignment、HeaderHorizontalContentAlignment |
| 补充说明浮层 | ZenPopover | Anchor、Content、IsOpen、Placement、ShowArrow、MinPopupWidth、MaxPopupWidth |
| 右键菜单 | ZenContextMenu / ZenMenuItem | 沿用 ContextMenu/MenuItem 的 Items、Header、Command；注意菜单的数据上下文 |

## 枚举值不能混用

- `ButtonVariant`：Neutral、Primary、Success、Warning、Danger。
- `ButtonAppearance`：Filled、Outlined、Text。按钮没有 Ghost 或 Segmented 外观。
- `RadioGroupAppearance`：Radio、Filled、Outlined、Ghost、Underline、Segmented。
- `AlertSeverity`：Info、Success、Warning、Error。提示错误用 Error，危险按钮用 Danger。
- `SpinButtonLayout`：Horizontal、Vertical。

## 数据与交互约束

- `ZenNumberBox.Value` 不是 double，也不可空。需要表示“未填写”时明确业务表示方式，不强行把 null 写入 Value。
- `ZenDatePicker.SelectedDate`、`ZenTimePicker.SelectedTime`、`ZenDateTimePicker.SelectedDateTime` 是不同类型，不统一绑定为字符串或同一个虚构 Value 属性。
- 日期时间的输入只读开关是 `IsTextInputReadOnly`；它不等同于禁用整个控件。时间增量等参数应核对当前版本有效范围。
- `ZenLoading.DisplayDelay` 是 TimeSpan，例如 `00:00:00.2`；`IsContentInteractionBlocked` 默认 true，控制加载层显示时是否阻止内容交互。它不自动执行异步任务，业务逻辑负责 IsLoading 的状态切换。
- `ZenPopover.Anchor` 是触发入口，Content 是浮层内容；未设置 Anchor 时有默认问号入口。不要按 Popup 的用法猜测 PlacementTarget 属性。
- `ZenDataGrid` 可使用 WPF 原生列，包括 DataGridTemplateColumn；不虚构 ZenDataGridTemplateColumn。

## 密码

`ZenPasswordBox` 是包装控件，没有可绑定的明文 Password 依赖属性。PasswordChanged 不携带明文；可在需要时获取并释放 SecurePassword 副本：

```csharp
private void PasswordBox_OnPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
{
    var passwordBox = (ZenUI.Wpf.Controls.ZenPasswordBox)sender;
    using (var password = passwordBox.SecurePassword)
    {
        // 在此调用项目已有的密码处理逻辑；不要长期保存此副本。
    }
}
```

将处理器接到视图中的 `PasswordChanged` 事件。保留应用已有认证边界，不凭空生成认证服务；不要为方便 MVVM 新增明文密码同步属性。提供 `IsPasswordRevealButtonEnabled` 和 `IsPasswordRevealed` 控制查看密码的界面行为。

## 进一步确认

有源码时按类型查 `src/ZenUI.Wpf/Controls/<类型>.cs` 及相应 `Themes/Controls` 模板；依赖属性注册可确认类型、默认值和绑定元数据。没有源码时检查实际安装版本的程序集/XML 文档。API 未确认时使用已确认的组合方式，或说明缺少的信息。
