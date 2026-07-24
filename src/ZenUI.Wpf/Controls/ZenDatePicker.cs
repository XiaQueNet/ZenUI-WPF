using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持水印和自定义圆角的日期选择控件。
    /// </summary>
    public class ZenDatePicker : DatePicker
    {
        static ZenDatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(typeof(ZenDatePicker)));
        }

        /// <summary>
        /// 获取或设置尚未选择日期时显示的水印。
        /// </summary>
        [Bindable(true)]
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Watermark"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(
                nameof(Watermark),
                typeof(string),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置输入框的圆角。
        /// </summary>
        [Bindable(true)]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CornerRadius"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(new CornerRadius(6)));

    }
}
