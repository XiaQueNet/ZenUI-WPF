using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持水印和自定义圆角的日期选择控件。
    /// </summary>
    public class ZenDatePicker : DatePicker
    {
        private DatePickerTextBox _textBox;
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
            if (_textBox != null)
            {
                _textBox.RemoveHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(HandleTextBoxMouseLeftButtonUp));
            }

            base.OnApplyTemplate();
            _textBox = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            if (_textBox != null)
            {
                _textBox.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(HandleTextBoxMouseLeftButtonUp),
                    true);
            }

            ApplyPopupBindings();
        }

        private void HandleTextBoxMouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            if (!IsTextInputReadOnly || !IsEnabled || IsDropDownOpen)
            {
                return;
            }

            SetCurrentValue(IsDropDownOpenProperty, true);
            e.Handled = true;
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

        private static void HandleCalendarCellSizeChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (ZenDatePicker)dependencyObject;
            if (datePicker._popupCalendar != null)
            {
                datePicker.ApplyPopupDimensions(datePicker._popupCalendar);
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

            BindPopupProperty(calendar, Calendar.FirstDayOfWeekProperty, nameof(FirstDayOfWeek));
            BindPopupProperty(calendar, FrameworkElement.FlowDirectionProperty, nameof(FlowDirection));
            BindPopupProperty(calendar, Control.FontSizeProperty, nameof(CalendarFontSize));
            BindPopupProperty(calendar, FrameworkElement.StyleProperty, nameof(CalendarStyle));
            ApplyPopupDimensions(calendar);
            ApplyPopupDateConstraints(calendar);
        }

        private void ApplyPopupDimensions(Calendar calendar)
        {
            if (double.IsNaN(CalendarCellWidth))
            {
                BindPopupProperty(
                    calendar,
                    FrameworkElement.WidthProperty,
                    nameof(CalendarPopupWidth));
            }
            else
            {
                BindingOperations.ClearBinding(calendar, FrameworkElement.WidthProperty);
                calendar.Width =
                    (7d * CalendarCellWidth) +
                    calendar.Padding.Left +
                    calendar.Padding.Right +
                    calendar.BorderThickness.Left +
                    calendar.BorderThickness.Right;
            }

            if (double.IsNaN(CalendarCellHeight))
            {
                BindPopupProperty(
                    calendar,
                    FrameworkElement.HeightProperty,
                    nameof(CalendarPopupHeight));
            }
            else
            {
                BindingOperations.ClearBinding(calendar, FrameworkElement.HeightProperty);
                calendar.Height =
                    (8.25d * CalendarCellHeight) +
                    calendar.Padding.Top +
                    calendar.Padding.Bottom +
                    calendar.BorderThickness.Top +
                    calendar.BorderThickness.Bottom;
            }
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
        /// 获取或设置尚未选择日期时，获得键盘焦点是否仍显示水印。
        /// </summary>
        [Bindable(true)]
        public bool ShowWatermarkOnFocus
        {
            get { return (bool)GetValue(ShowWatermarkOnFocusProperty); }
            set { SetValue(ShowWatermarkOnFocusProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ShowWatermarkOnFocus"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ShowWatermarkOnFocusProperty =
            DependencyProperty.Register(
                nameof(ShowWatermarkOnFocus),
                typeof(bool),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 获取或设置一个值，该值指示日期文本输入是否只读。只读时，点击输入区域会打开日期选择弹层。
        /// </summary>
        [Bindable(true)]
        public bool IsTextInputReadOnly
        {
            get { return (bool)GetValue(IsTextInputReadOnlyProperty); }
            set { SetValue(IsTextInputReadOnlyProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsTextInputReadOnly"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsTextInputReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsTextInputReadOnly),
                typeof(bool),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(false));

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
        /// 获取或设置下拉按钮的宽度。该值必须为大于或等于零的有限值。
        /// </summary>
        [Bindable(true)]
        public double DropDownButtonWidth
        {
            get { return (double)GetValue(DropDownButtonWidthProperty); }
            set { SetValue(DropDownButtonWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DropDownButtonWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DropDownButtonWidthProperty =
            DependencyProperty.Register(
                nameof(DropDownButtonWidth),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(
                    28d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                IsValidNonNegativeDimension);

        /// <summary>
        /// 获取或设置下拉按钮的高度。该值必须为大于或等于零的有限值。
        /// </summary>
        [Bindable(true)]
        public double DropDownButtonHeight
        {
            get { return (double)GetValue(DropDownButtonHeightProperty); }
            set { SetValue(DropDownButtonHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DropDownButtonHeight"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DropDownButtonHeightProperty =
            DependencyProperty.Register(
                nameof(DropDownButtonHeight),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(
                    28d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                IsValidNonNegativeDimension);

        /// <summary>
        /// 获取或设置下拉按钮中日历图标的边长。该值必须为大于或等于零的有限值。
        /// </summary>
        [Bindable(true)]
        public double DropDownButtonIconSize
        {
            get { return (double)GetValue(DropDownButtonIconSizeProperty); }
            set { SetValue(DropDownButtonIconSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DropDownButtonIconSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DropDownButtonIconSizeProperty =
            DependencyProperty.Register(
                nameof(DropDownButtonIconSize),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(
                    16d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidNonNegativeDimension);

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
        /// 获取或设置日历网格单元的宽度（以与设备无关的像素为单位）。
        /// <see cref="double.NaN"/> 表示使用 <see cref="CalendarPopupWidth"/>。
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(LengthConverter))]
        public double CalendarCellWidth
        {
            get { return (double)GetValue(CalendarCellWidthProperty); }
            set { SetValue(CalendarCellWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CalendarCellWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CalendarCellWidthProperty =
            DependencyProperty.Register(
                nameof(CalendarCellWidth),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(double.NaN, HandleCalendarCellSizeChanged),
                IsValidAutoOrPositiveDimension);

        /// <summary>
        /// 获取或设置日历网格单元的高度（以与设备无关的像素为单位）。
        /// <see cref="double.NaN"/> 表示使用 <see cref="CalendarPopupHeight"/>。
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(LengthConverter))]
        public double CalendarCellHeight
        {
            get { return (double)GetValue(CalendarCellHeightProperty); }
            set { SetValue(CalendarCellHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CalendarCellHeight"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CalendarCellHeightProperty =
            DependencyProperty.Register(
                nameof(CalendarCellHeight),
                typeof(double),
                typeof(ZenDatePicker),
                new FrameworkPropertyMetadata(double.NaN, HandleCalendarCellSizeChanged),
                IsValidAutoOrPositiveDimension);

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

        private static bool IsValidAutoOrPositiveDimension(object value)
        {
            var dimension = (double)value;
            return double.IsNaN(dimension) ||
                (!double.IsInfinity(dimension) && dimension > 0d);
        }

        private static bool IsValidNonNegativeDimension(object value)
        {
            var dimension = (double)value;
            return !double.IsNaN(dimension) &&
                !double.IsInfinity(dimension) &&
                dimension >= 0d;
        }
    }
}
