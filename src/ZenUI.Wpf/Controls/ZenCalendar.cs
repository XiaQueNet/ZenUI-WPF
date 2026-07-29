using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示使用 ZenUI 主题和密度系统的日历控件。
    /// </summary>
    public class ZenCalendar : Calendar
    {
        static ZenCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(typeof(ZenCalendar)));
        }

        /// <summary>
        /// 获取或设置日历外框的圆角半径。
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return GetCornerRadius(this); }
            set { SetCornerRadius(this, value); }
        }

        /// <summary>
        /// 获取指定元素的日历外框圆角半径。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>指定元素的日历外框圆角半径。</returns>
        public static CornerRadius GetCornerRadius(DependencyObject element)
        {
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }

        /// <summary>
        /// 设置指定元素的日历外框圆角半径。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">要设置的圆角半径。</param>
        public static void SetCornerRadius(DependencyObject element, CornerRadius value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// 标识 <see cref="CornerRadius"/> 附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(new CornerRadius(8)));

        /// <summary>
        /// 获取或设置月份和年份按钮的内边距。
        /// </summary>
        public Thickness ButtonPadding
        {
            get { return GetButtonPadding(this); }
            set { SetButtonPadding(this, value); }
        }

        /// <summary>
        /// 获取指定元素的月份和年份按钮内边距。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>指定元素的月份和年份按钮内边距。</returns>
        public static Thickness GetButtonPadding(DependencyObject element)
        {
            return (Thickness)element.GetValue(ButtonPaddingProperty);
        }

        /// <summary>
        /// 设置指定元素的月份和年份按钮内边距。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">要设置的内边距。</param>
        public static void SetButtonPadding(DependencyObject element, Thickness value)
        {
            element.SetValue(ButtonPaddingProperty, value);
        }

        /// <summary>
        /// 标识 <see cref="ButtonPadding"/> 附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty ButtonPaddingProperty =
            DependencyProperty.RegisterAttached(
                "ButtonPadding",
                typeof(Thickness),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(new Thickness(12, 16, 12, 16)));

        /// <summary>
        /// 获取或设置导航按钮的边长。
        /// </summary>
        public double NavigationButtonSize
        {
            get { return GetNavigationButtonSize(this); }
            set { SetNavigationButtonSize(this, value); }
        }

        /// <summary>
        /// 获取指定元素的导航按钮边长。
        /// </summary>
        /// <param name="element">要从中读取属性值的元素。</param>
        /// <returns>指定元素的导航按钮边长。</returns>
        public static double GetNavigationButtonSize(DependencyObject element)
        {
            return (double)element.GetValue(NavigationButtonSizeProperty);
        }

        /// <summary>
        /// 设置指定元素的导航按钮边长。
        /// </summary>
        /// <param name="element">要在其上设置属性值的元素。</param>
        /// <param name="value">要设置的边长。</param>
        public static void SetNavigationButtonSize(DependencyObject element, double value)
        {
            element.SetValue(NavigationButtonSizeProperty, value);
        }

        /// <summary>
        /// 标识 <see cref="NavigationButtonSize"/> 附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty NavigationButtonSizeProperty =
            DependencyProperty.RegisterAttached(
                "NavigationButtonSize",
                typeof(double),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(40d));
    }
}
