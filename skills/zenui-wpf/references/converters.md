# 可见性转换器

仅需要转换器时只引用 `ZenUI.Wpf.Converters`。XAML 命名空间为 `https://zenui.mnorg.cn/xaml/wpf/converters`。转换器继承 MarkupExtension，可在 Binding 中直接使用，不强制在 Resources 注册实例。

| 转换器 | 默认可见条件 |
| --- | --- |
| BoolToVisibilityConverter | bool 为 true |
| NullToVisibilityConverter | 非 null；字符串额外检查不是空字符串，空白字符串仍非空 |
| EnumerableToVisibilityConverter | 可枚举集合至少有一项 |
| ComparisonToVisibilityConverter | 源值与 ConverterParameter 满足 Comparison |

共同参数：`IsInverted=True` 反转判定，`UseCollapsed=False` 将不可见状态设为 Hidden；默认不可见状态为 Collapsed。不可见值或无效比较并不保证满足反转语义，避免把错误输入当成有效业务值。

```xaml
<StackPanel
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:zc="https://zenui.mnorg.cn/xaml/wpf/converters">
    <TextBlock Text="正在处理"
        Visibility="{Binding IsBusy, Converter={zc:BoolToVisibilityConverter}}" />
    <TextBlock Text="可以操作"
        Visibility="{Binding IsBusy, Converter={zc:BoolToVisibilityConverter IsInverted=True, UseCollapsed=False}}" />
    <TextBlock Text="暂无数据"
        Visibility="{Binding Items.Count, Converter={zc:ComparisonToVisibilityConverter Comparison=Equal}, ConverterParameter=0}" />
</StackPanel>
```

Comparison 支持 Equal、NotEqual、GreaterThan、GreaterThanOrEqual、LessThan、LessThanOrEqual。转换器尝试按源类型及绑定区域性解析 ConverterParameter；参数不可转换时降级为不可见。对数字比较绑定数字属性，不先转成显示字符串。

对于动态集合的“暂无数据”，优先绑定有变更通知的 Count，例如 ObservableCollection 的 Count；只绑定集合对象并不意味着添加/删除元素时自动重新执行转换器。

可见性通常是单向派生值。不要给这些绑定默认加 TwoWay；只有 BoolToVisibilityConverter 实现专用 ConvertBack，其余不能假定支持反向转换。

源码定位：`src/ZenUI.Wpf.Converters/Converters`。
