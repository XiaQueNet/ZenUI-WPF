using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 在日期选择器创建的原生 Calendar 上传递尺寸，避免依赖跨 Popup 的资源查找。
    /// </summary>
    internal static class ZenCalendar
    {
        public static double GetDayButtonWidth(DependencyObject element)
        {
            return (double)element.GetValue(DayButtonWidthProperty);
        }

        public static void SetDayButtonWidth(DependencyObject element, double value)
        {
            element.SetValue(DayButtonWidthProperty, value);
        }

        public static readonly DependencyProperty DayButtonWidthProperty =
            DependencyProperty.RegisterAttached(
                "DayButtonWidth",
                typeof(double),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(48d));

        public static double GetDayButtonHeight(DependencyObject element)
        {
            return (double)element.GetValue(DayButtonHeightProperty);
        }

        public static void SetDayButtonHeight(DependencyObject element, double value)
        {
            element.SetValue(DayButtonHeightProperty, value);
        }

        public static readonly DependencyProperty DayButtonHeightProperty =
            DependencyProperty.RegisterAttached(
                "DayButtonHeight",
                typeof(double),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(44d));

        public static Thickness GetButtonPadding(DependencyObject element)
        {
            return (Thickness)element.GetValue(ButtonPaddingProperty);
        }

        public static void SetButtonPadding(DependencyObject element, Thickness value)
        {
            element.SetValue(ButtonPaddingProperty, value);
        }

        public static readonly DependencyProperty ButtonPaddingProperty =
            DependencyProperty.RegisterAttached(
                "ButtonPadding",
                typeof(Thickness),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(new Thickness(12, 16, 12, 16)));

        public static double GetNavigationButtonSize(DependencyObject element)
        {
            return (double)element.GetValue(NavigationButtonSizeProperty);
        }

        public static void SetNavigationButtonSize(DependencyObject element, double value)
        {
            element.SetValue(NavigationButtonSizeProperty, value);
        }

        public static readonly DependencyProperty NavigationButtonSizeProperty =
            DependencyProperty.RegisterAttached(
                "NavigationButtonSize",
                typeof(double),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(40d));
    }
}
