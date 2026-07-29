using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持水印和自定义圆角的日期选择控件。
    /// </summary>
    public class ZenDatePicker : DatePicker
    {
        private Calendar _popupCalendar;
        private readonly List<CalendarDateRange> _constraintBlackoutRanges =
            new List<CalendarDateRange>();

        static ZenDatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(typeof(ZenDatePicker)));
            DisplayDateStartProperty.OverrideMetadata(
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(null, HandleDateConstraintChanged));
            DisplayDateEndProperty.OverrideMetadata(
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(null, HandleDateConstraintChanged));
        }

        /// <summary>
        /// 初始化 <see cref="ZenDatePicker"/> 类的新实例。
        /// </summary>
        public ZenDatePicker()
        {
            CalendarOpened += HandleCalendarOpened;
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyPopupBindings();
        }

        private void HandleCalendarOpened(object sender, RoutedEventArgs e)
        {
            ApplyPopupBindings();
        }

        private static void HandleDateConstraintChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (ZenDatePicker)dependencyObject;
            if (datePicker._popupCalendar != null)
            {
                datePicker.ApplyPopupDateConstraints(datePicker._popupCalendar);
            }
        }

        private void ApplyPopupBindings()
        {
            var popup =
                GetTemplateChild("PART_Popup") as Popup ??
                Template?.FindName("PART_Popup", this) as Popup;
            var calendar = popup?.Child as Calendar;
            if (calendar == null)
            {
                return;
            }

            BindPopupProperty(calendar, FrameworkElement.WidthProperty, nameof(CalendarPopupWidth));
            BindPopupProperty(calendar, FrameworkElement.HeightProperty, nameof(CalendarPopupHeight));
            BindPopupProperty(calendar, ZenCalendar.ButtonPaddingProperty, nameof(CalendarButtonPadding));
            BindPopupProperty(calendar, ZenCalendar.DayButtonHeightProperty, nameof(CalendarDayButtonHeight));
            BindPopupProperty(calendar, ZenCalendar.DayButtonWidthProperty, nameof(CalendarDayButtonWidth));
            BindPopupProperty(calendar, Calendar.FirstDayOfWeekProperty, nameof(FirstDayOfWeek));
            BindPopupProperty(calendar, FrameworkElement.FlowDirectionProperty, nameof(FlowDirection));
            BindPopupProperty(calendar, Control.FontSizeProperty, nameof(CalendarFontSize));
            BindPopupProperty(
                calendar,
                ZenCalendar.NavigationButtonSizeProperty,
                nameof(CalendarNavigationButtonSize));
            BindPopupProperty(calendar, FrameworkElement.StyleProperty, nameof(CalendarStyle));
            ApplyPopupDateConstraints(calendar);
        }

        private void ApplyPopupDateConstraints(Calendar calendar)
        {
            RemoveConstraintBlackoutRanges();
            _popupCalendar = calendar;

            var rangeStart = DisplayDateStart;
            if (rangeStart.HasValue)
            {
                var monthStart = new DateTime(
                    rangeStart.Value.Year,
                    rangeStart.Value.Month,
                    1);
                calendar.DisplayDateStart = monthStart;

                if (monthStart < rangeStart.Value.Date)
                {
                    AddConstraintBlackoutRange(
                        monthStart,
                        rangeStart.Value.Date.AddDays(-1));
                }
            }
            else
            {
                calendar.DisplayDateStart = null;
            }

            var rangeEnd = DisplayDateEnd;
            if (rangeEnd.HasValue)
            {
                var monthEnd = new DateTime(
                    rangeEnd.Value.Year,
                    rangeEnd.Value.Month,
                    DateTime.DaysInMonth(rangeEnd.Value.Year, rangeEnd.Value.Month));
                calendar.DisplayDateEnd = monthEnd;

                if (rangeEnd.Value.Date < monthEnd)
                {
                    AddConstraintBlackoutRange(
                        rangeEnd.Value.Date.AddDays(1),
                        monthEnd);
                }
            }
            else
            {
                calendar.DisplayDateEnd = null;
            }
        }

        private void AddConstraintBlackoutRange(DateTime start, DateTime end)
        {
            var range = new CalendarDateRange(start, end);
            _popupCalendar.BlackoutDates.Add(range);
            _constraintBlackoutRanges.Add(range);
        }

        private void RemoveConstraintBlackoutRanges()
        {
            if (_popupCalendar != null)
            {
                foreach (var range in _constraintBlackoutRanges)
                {
                    _popupCalendar.BlackoutDates.Remove(range);
                }
            }

            _constraintBlackoutRanges.Clear();
        }

        private void BindPopupProperty(
            DependencyObject target,
            DependencyProperty targetProperty,
            string sourceProperty,
            BindingMode mode = BindingMode.OneWay)
        {
            BindingOperations.SetBinding(
                target,
                targetProperty,
                new Binding(sourceProperty)
                {
                    Mode = mode,
                    Source = this
                });
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
        /// 获取或设置一个值，该值指示是否允许通过键盘直接输入日期。
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
                new FrameworkPropertyMetadata(48d));

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
                new FrameworkPropertyMetadata(44d));

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
                new FrameworkPropertyMetadata(new Thickness(12, 16, 12, 16)));

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
                new FrameworkPropertyMetadata(40d));

        /// <summary>
        /// 获取或设置日历弹层的宽度。
        /// </summary>
        [Bindable(true)]
        public double CalendarPopupWidth
        {
            get { return (double)GetValue(CalendarPopupWidthProperty); }
            set { SetValue(CalendarPopupWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CalendarPopupWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CalendarPopupWidthProperty =
            DependencyProperty.Register(
                nameof(CalendarPopupWidth),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(368d));

        /// <summary>
        /// 获取或设置日历弹层的高度。
        /// </summary>
        [Bindable(true)]
        public double CalendarPopupHeight
        {
            get { return (double)GetValue(CalendarPopupHeightProperty); }
            set { SetValue(CalendarPopupHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CalendarPopupHeight"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CalendarPopupHeightProperty =
            DependencyProperty.Register(
                nameof(CalendarPopupHeight),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(376d));

        /// <summary>
        /// 获取或设置日历弹层内容的字号。
        /// </summary>
        [Bindable(true)]
        public double CalendarFontSize
        {
            get { return (double)GetValue(CalendarFontSizeProperty); }
            set { SetValue(CalendarFontSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CalendarFontSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CalendarFontSizeProperty =
            DependencyProperty.Register(
                nameof(CalendarFontSize),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(16d));
    }
}
