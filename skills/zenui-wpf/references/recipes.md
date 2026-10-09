# 常用组合

以下片段放入已有视图，默认已声明 `xmlns:zen="https://zenui.mnorg.cn/xaml/wpf"`。绑定名称是示例业务契约，需要映射到目标项目；可变属性应提供适当变更通知。

## 表单

参考 [FormView.xaml](../assets/FormView.xaml)。先按 [接入](getting-started.md) 为应用合并 Generic.xaml；视图使用其 Token，DataContext 由宿主提供。

| 绑定名 | 类型/责任 |
| --- | --- |
| Name | string；项目现有验证机制提供错误 |
| Quantity | decimal；示例范围 0～999 |
| DueDate | DateTime? |
| IsEnabled | bool，表示业务选项 |
| IsBusy | bool，异步操作开始/结束时更新 |
| SaveCommand | ICommand；CanExecute 控制保存是否可用 |

`ValidatesOnDataErrors=True` 需要 IDataErrorInfo；`ValidatesOnNotifyDataErrors=True` 对接 INotifyDataErrorInfo。设置标记本身不会实现业务校验，按项目已有机制使用。输入中的即时校验使用 `UpdateSourceTrigger=PropertyChanged`，不需要实时提交的字段可保留项目策略。

## 日期时间与选项

```xaml
<StackPanel>
    <zen:ZenTimePicker Watermark="开始时间" Is24HourFormat="True"
        MinuteIncrement="15" SelectedTime="{Binding StartTime, Mode=TwoWay}" />
    <zen:ZenDateTimePicker Margin="0,12,0,0" Watermark="预约时间"
        DateTimeFormat="yyyy-MM-dd HH:mm" IsSecondVisible="False"
        SelectedDateTime="{Binding Appointment, Mode=TwoWay}" />
    <zen:ZenRadioGroup Margin="0,12,0,0" Appearance="Segmented"
        ItemsSource="{Binding Categories}" DisplayMemberPath="Name"
        SelectedValuePath="Id" SelectedValue="{Binding CategoryId, Mode=TwoWay}" />
</StackPanel>
```

StartTime 为 TimeSpan?，Appointment 为 DateTime?，Categories 中每项有 Name 和 Id，CategoryId 类型与 Id 相同。仅需日期使用 ZenDatePicker.SelectedDate。

## 数据表格

```xaml
<zen:ZenDataGrid ItemsSource="{Binding Orders}" AutoGenerateColumns="False"
    IsReadOnly="True" CanUserAddRows="False" EmptyContent="暂无订单">
    <zen:ZenDataGrid.Columns>
        <zen:ZenDataGridTextColumn Header="订单号" Binding="{Binding Number}" Width="*" />
        <zen:ZenDataGridTextColumn Header="金额" Binding="{Binding Amount, StringFormat=N2}"
            Width="120" CellHorizontalContentAlignment="Right" />
    </zen:ZenDataGrid.Columns>
</zen:ZenDataGrid>
```

Orders 是集合，元素提供 Number 和数值 Amount。可编辑场景再开启编辑，并为列绑定提供业务验证。DataGridColumn 不属于常规可视树：Binding 指向行数据；绑定页面命令或列属性时需显式确认来源，不能假定继承页面 DataContext。

## 补充说明

```xaml
<zen:ZenPopover Anchor="配送说明" Placement="Bottom" ShowArrow="True">
    <TextBlock Width="240" TextWrapping="Wrap" Text="工作日安排发货，实际时间以订单信息为准。" />
</zen:ZenPopover>
```

需要控制开关时绑定 IsOpen。使用 AnchorButtonStyle 前确认样式 TargetType 为 ToggleButton，必要时基于已加载的 ZenPopoverAnchorButtonStyle。

## 右键菜单

```xaml
<Border Background="Transparent">
    <Border.ContextMenu>
        <zen:ZenContextMenu
            DataContext="{Binding PlacementTarget.DataContext, RelativeSource={RelativeSource Self}}">
            <zen:ZenMenuItem Header="刷新" Command="{Binding RefreshCommand}" />
        </zen:ZenContextMenu>
    </Border.ContextMenu>
    <TextBlock Text="右键打开菜单" />
</Border>
```

此处宿主 DataContext 提供 RefreshCommand。列表行上的菜单其 PlacementTarget.DataContext 通常为行对象；页面命令和行参数需要分别指定来源。
