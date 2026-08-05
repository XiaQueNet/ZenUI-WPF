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

    }
}
