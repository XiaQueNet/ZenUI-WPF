using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 在日期选择器弹层内传递日历尺寸，避免依赖跨 Popup 的资源查找。
    /// </summary>
    internal sealed class ZenCalendar : Calendar
    {
        public double DayButtonWidth
        {
            get { return (double)GetValue(DayButtonWidthProperty); }
            set { SetValue(DayButtonWidthProperty, value); }
        }

        public static readonly DependencyProperty DayButtonWidthProperty =
            DependencyProperty.Register(
                nameof(DayButtonWidth),
                typeof(double),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(48d));

        public double DayButtonHeight
        {
            get { return (double)GetValue(DayButtonHeightProperty); }
            set { SetValue(DayButtonHeightProperty, value); }
        }

        public static readonly DependencyProperty DayButtonHeightProperty =
            DependencyProperty.Register(
                nameof(DayButtonHeight),
                typeof(double),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(44d));

        public Thickness ButtonPadding
        {
            get { return (Thickness)GetValue(ButtonPaddingProperty); }
            set { SetValue(ButtonPaddingProperty, value); }
        }

        public static readonly DependencyProperty ButtonPaddingProperty =
            DependencyProperty.Register(
                nameof(ButtonPadding),
                typeof(Thickness),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(new Thickness(12, 16, 12, 16)));

        public double NavigationButtonSize
        {
            get { return (double)GetValue(NavigationButtonSizeProperty); }
            set { SetValue(NavigationButtonSizeProperty, value); }
        }

        public static readonly DependencyProperty NavigationButtonSizeProperty =
            DependencyProperty.Register(
                nameof(NavigationButtonSize),
                typeof(double),
                typeof(ZenCalendar),
                new FrameworkPropertyMetadata(40d));
    }
}
