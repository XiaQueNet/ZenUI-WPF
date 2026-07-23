using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持水印和自定义圆角的下拉选择控件。
    /// </summary>
    public class ZenComboBox : ComboBox
    {
        static ZenComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenComboBox),
                new FrameworkPropertyMetadata(typeof(ZenComboBox)));
        }

        /// <summary>
        /// 获取或设置下拉框没有选中项时显示的水印。
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
                typeof(ZenComboBox),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置下拉框的圆角。
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
                typeof(ZenComboBox),
                new FrameworkPropertyMetadata(new CornerRadius(6)));
    }
}
