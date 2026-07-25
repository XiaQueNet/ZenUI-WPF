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
        /// 获取或设置是否允许通过键盘直接输入日期。
        /// </summary>
        [Bindable(true)]
        public bool IsTextInputEnabled
        {
            get { return (bool)GetValue(IsTextInputEnabledProperty); }
            set { SetValue(IsTextInputEnabledProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsTextInputEnabled"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsTextInputEnabledProperty =
            DependencyProperty.Register(
                nameof(IsTextInputEnabled),
                typeof(bool),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(true));

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

        /// <summary>
        /// 获取或设置日历日期按钮的宽度。
        /// </summary>
        [Bindable(true)]
        public double CalendarDayButtonWidth
        {
            get { return (double)GetValue(CalendarDayButtonWidthProperty); }
            set { SetValue(CalendarDayButtonWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CalendarDayButtonWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CalendarDayButtonWidthProperty =
            DependencyProperty.Register(
                nameof(CalendarDayButtonWidth),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(34d));

        /// <summary>
        /// 获取或设置日历日期按钮的高度。
        /// </summary>
        [Bindable(true)]
        public double CalendarDayButtonHeight
        {
            get { return (double)GetValue(CalendarDayButtonHeightProperty); }
            set { SetValue(CalendarDayButtonHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CalendarDayButtonHeight"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CalendarDayButtonHeightProperty =
            DependencyProperty.Register(
                nameof(CalendarDayButtonHeight),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(32d));

        /// <summary>
        /// 获取或设置日历月份和年份按钮的内边距。
        /// </summary>
        [Bindable(true)]
        public Thickness CalendarButtonPadding
        {
            get { return (Thickness)GetValue(CalendarButtonPaddingProperty); }
            set { SetValue(CalendarButtonPaddingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CalendarButtonPadding"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CalendarButtonPaddingProperty =
            DependencyProperty.Register(
                nameof(CalendarButtonPadding),
                typeof(Thickness),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(new Thickness(8, 10, 8, 10)));

        /// <summary>
        /// 获取或设置日历导航按钮的边长。
        /// </summary>
        [Bindable(true)]
        public double CalendarNavigationButtonSize
        {
            get { return (double)GetValue(CalendarNavigationButtonSizeProperty); }
            set { SetValue(CalendarNavigationButtonSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CalendarNavigationButtonSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CalendarNavigationButtonSizeProperty =
            DependencyProperty.Register(
                nameof(CalendarNavigationButtonSize),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(30d));
    }
}
