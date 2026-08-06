using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Calendar = System.Windows.Controls.Calendar;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持文本输入以及组合日期和时间弹层的日期时间选择控件。
    /// </summary>
    [TemplatePart(Name = PartTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PartPopup, Type = typeof(Popup))]
    [TemplatePart(Name = PartCalendar, Type = typeof(Calendar))]
    [TemplatePart(Name = PartTimeSelector, Type = typeof(Control))]
    [TemplatePart(Name = PartNowButton, Type = typeof(Button))]
    [TemplatePart(Name = PartConfirmButton, Type = typeof(Button))]
    public class ZenDateTimePicker : Control
    {
        internal const string PartTextBox = "PART_TextBox";
        internal const string PartPopup = "PART_Popup";
        internal const string PartDropDownBorder = "PART_DropDownBorder";
        internal const string PartSelectionPanel = "PART_SelectionPanel";
        internal const string PartTimePanel = "PART_TimePanel";
        internal const string PartCalendar = "PART_Calendar";
        internal const string PartTimeSelector = "PART_TimeSelector";
        internal const string PartNowButton = "PART_NowButton";
        internal const string PartConfirmButton = "PART_ConfirmButton";

        private static readonly Type SelfType = typeof(ZenDateTimePicker);
        private readonly List<CalendarDateRange> constraintBlackoutRanges =
            new List<CalendarDateRange>();
        private TextBox textBox;
        private Popup popup;
        private Border dropDownBorder;
        private Grid selectionPanel;
        private Border timePanel;
        private Calendar calendar;
        private ZenTimeSelector timeSelector;
        private Button nowButton;
        private Button confirmButton;
        private DateTime draftValue;
        private bool isSynchronizingDraft;

        static ZenDateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 初始化 <see cref="ZenDateTimePicker"/> 类的新实例。
        /// </summary>
        public ZenDateTimePicker()
        {
            IsEnabledChanged += HandleIsEnabledChanged;
        }

        /// <summary>
        /// 获取或设置选中的日期和时间。
        /// </summary>
        [Bindable(true)]
        public DateTime? SelectedDateTime
        {
            get { return (DateTime?)GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="SelectedDateTime"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register(
                nameof(SelectedDateTime),
                typeof(DateTime?),
                SelfType,
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    HandleSelectedDateTimeChanged,
                    (dependencyObject, baseValue) =>
                        CoerceSelectedDateTime(dependencyObject, baseValue)));

        /// <summary>
        /// 当 <see cref="SelectedDateTime"/> 的值发生变化时发生。
        /// </summary>
        public event RoutedPropertyChangedEventHandler<DateTime?> SelectedDateTimeChanged
        {
            add { AddHandler(SelectedDateTimeChangedEvent, value); }
            remove { RemoveHandler(SelectedDateTimeChangedEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="SelectedDateTimeChanged"/> 路由事件。
        /// </summary>
        public static readonly RoutedEvent SelectedDateTimeChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectedDateTimeChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<DateTime?>),
                SelfType);

        /// <summary>
        /// 获取或设置允许选择的最早日期和时间；<see langword="null"/> 表示不限制。
        /// </summary>
        [Bindable(true)]
        public DateTime? Minimum
        {
            get { return (DateTime?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Minimum"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(DateTime?),
                SelfType,
                new FrameworkPropertyMetadata(null, HandleRangeChanged));

        /// <summary>
        /// 获取或设置允许选择的最晚日期和时间；<see langword="null"/> 表示不限制。
        /// </summary>
        [Bindable(true)]
        public DateTime? Maximum
        {
            get { return (DateTime?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Maximum"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(DateTime?),
                SelfType,
                new FrameworkPropertyMetadata(
                    null,
                    HandleRangeChanged,
                    (dependencyObject, baseValue) =>
                        CoerceMaximum(dependencyObject, baseValue)));

        /// <summary>
        /// 获取或设置分钟列表的递增步长，取值范围为 1 到 59。
        /// </summary>
        [Bindable(true)]
        public int MinuteIncrement
        {
            get { return (int)GetValue(MinuteIncrementProperty); }
            set { SetValue(MinuteIncrementProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="MinuteIncrement"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MinuteIncrementProperty =
            DependencyProperty.Register(
                nameof(MinuteIncrement),
                typeof(int),
                SelfType,
                new FrameworkPropertyMetadata(1, HandleSelectorOptionsChanged),
                value => (int)value >= 1 && (int)value <= 59);

        /// <summary>
        /// 获取或设置秒列表的递增步长，取值范围为 1 到 59。
        /// </summary>
        [Bindable(true)]
        public int SecondIncrement
        {
            get { return (int)GetValue(SecondIncrementProperty); }
            set { SetValue(SecondIncrementProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="SecondIncrement"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SecondIncrementProperty =
            DependencyProperty.Register(
                nameof(SecondIncrement),
                typeof(int),
                SelfType,
                new FrameworkPropertyMetadata(1, HandleSelectorOptionsChanged),
                value => (int)value >= 1 && (int)value <= 59);

        /// <summary>
        /// 获取或设置一个值，该值指示是否显示和编辑秒。
        /// </summary>
        [Bindable(true)]
        public bool IsSecondVisible
        {
            get { return (bool)GetValue(IsSecondVisibleProperty); }
            set { SetValue(IsSecondVisibleProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsSecondVisible"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsSecondVisibleProperty =
            DependencyProperty.Register(
                nameof(IsSecondVisible),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(true, HandleDisplayOptionsChanged));

        /// <summary>
        /// 获取或设置一个值，该值指示是否使用 24 小时制。
        /// </summary>
        [Bindable(true)]
        public bool Is24HourFormat
        {
            get { return (bool)GetValue(Is24HourFormatProperty); }
            set { SetValue(Is24HourFormatProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Is24HourFormat"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty Is24HourFormatProperty =
            DependencyProperty.Register(
                nameof(Is24HourFormat),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(true, HandleDisplayOptionsChanged));

        /// <summary>
        /// 获取或设置尚未选择日期时间时显示的水印。
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
                SelfType,
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置日期时间的自定义显示格式；空字符串表示使用当前区域设置生成默认格式。
        /// </summary>
        [Bindable(true)]
        public string DateTimeFormat
        {
            get { return (string)GetValue(DateTimeFormatProperty); }
            set { SetValue(DateTimeFormatProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DateTimeFormat"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register(
                nameof(DateTimeFormat),
                typeof(string),
                SelfType,
                new FrameworkPropertyMetadata(string.Empty, HandleDisplayOptionsChanged));

        /// <summary>
        /// 获取或设置一个值，该值指示日期时间文本输入是否只读。只读时，点击输入区域会打开日期时间选择弹层。
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
                SelfType,
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
                SelfType,
                new FrameworkPropertyMetadata(new CornerRadius(6)));

        /// <summary>
        /// 获取或设置弹层是否打开。
        /// </summary>
        [Bindable(true)]
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsDropDownOpen"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                nameof(IsDropDownOpen),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    HandleIsDropDownOpenChanged));

        /// <summary>
        /// 获取或设置一周的第一天。
        /// </summary>
        [Bindable(true)]
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="FirstDayOfWeek"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register(
                nameof(FirstDayOfWeek),
                typeof(DayOfWeek),
                SelfType,
                new FrameworkPropertyMetadata(CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek),
                value => (DayOfWeek)value >= DayOfWeek.Sunday &&
                    (DayOfWeek)value <= DayOfWeek.Saturday);

        /// <summary>
        /// 获取或设置一个值，该值指示是否突出显示当前日期。
        /// </summary>
        [Bindable(true)]
        public bool IsTodayHighlighted
        {
            get { return (bool)GetValue(IsTodayHighlightedProperty); }
            set { SetValue(IsTodayHighlightedProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsTodayHighlighted"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsTodayHighlightedProperty =
            DependencyProperty.Register(
                nameof(IsTodayHighlighted),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 获取或设置输入框右侧日历图标的边长。该值必须为大于或等于零的有限值。
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
                SelfType,
                new FrameworkPropertyMetadata(
                    16d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidDropDownButtonIconSize);

        /// <summary>
        /// 获取或设置下拉弹层的宽度。<see cref="double.NaN"/> 表示根据选择单元自然测量。
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(LengthConverter))]
        public double DropDownWidth
        {
            get { return (double)GetValue(DropDownWidthProperty); }
            set { SetValue(DropDownWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DropDownWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DropDownWidthProperty =
            DependencyProperty.Register(
                nameof(DropDownWidth),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(double.NaN, HandleSelectionMetricsChanged),
                IsValidAutoOrPositiveDimension);

        /// <summary>
        /// 获取或设置下拉弹层的高度。<see cref="double.NaN"/> 表示根据选择单元自然测量。
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(LengthConverter))]
        public double DropDownHeight
        {
            get { return (double)GetValue(DropDownHeightProperty); }
            set { SetValue(DropDownHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DropDownHeight"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register(
                nameof(DropDownHeight),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(double.NaN, HandleSelectionMetricsChanged),
                IsValidAutoOrPositiveDimension);

        /// <summary>
        /// 获取或设置日历网格单元的宽度。<see cref="double.NaN"/> 表示根据弹层宽度自动分配。
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
                SelfType,
                new FrameworkPropertyMetadata(40d, HandleSelectionMetricsChanged),
                IsValidAutoOrPositiveDimension);

        /// <summary>
        /// 获取或设置日历网格单元的高度。<see cref="double.NaN"/> 表示根据弹层高度自动分配。
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
                SelfType,
                new FrameworkPropertyMetadata(36d, HandleSelectionMetricsChanged),
                IsValidAutoOrPositiveDimension);

        /// <summary>
        /// 获取或设置时间选择项的宽度。<see cref="double.NaN"/> 表示根据时间面板宽度自动均分。
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(LengthConverter))]
        public double TimeItemWidth
        {
            get { return (double)GetValue(TimeItemWidthProperty); }
            set { SetValue(TimeItemWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TimeItemWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TimeItemWidthProperty =
            DependencyProperty.Register(
                nameof(TimeItemWidth),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(64d, HandleSelectionMetricsChanged),
                IsValidAutoOrPositiveDimension);

        /// <summary>
        /// 获取或设置时间选择项的高度。<see cref="double.NaN"/> 表示根据时间列表高度自动计算。
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(LengthConverter))]
        public double TimeItemHeight
        {
            get { return (double)GetValue(TimeItemHeightProperty); }
            set { SetValue(TimeItemHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TimeItemHeight"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TimeItemHeightProperty =
            DependencyProperty.Register(
                nameof(TimeItemHeight),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(36d, HandleSelectionMetricsChanged),
                IsValidAutoOrPositiveDimension);

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            DetachTemplateHandlers();
            base.OnApplyTemplate();

            textBox = GetTemplateChild(PartTextBox) as TextBox;
            popup = GetTemplateChild(PartPopup) as Popup;
            dropDownBorder = GetTemplateChild(PartDropDownBorder) as Border;
            selectionPanel = GetTemplateChild(PartSelectionPanel) as Grid;
            timePanel = GetTemplateChild(PartTimePanel) as Border;
            calendar = GetTemplateChild(PartCalendar) as Calendar;
            timeSelector = GetTemplateChild(PartTimeSelector) as ZenTimeSelector;
            nowButton = GetTemplateChild(PartNowButton) as Button;
            confirmButton = GetTemplateChild(PartConfirmButton) as Button;

            AttachTemplateHandlers();
            UpdateSelectionLayout();
            ConfigureCalendarRange();
            SynchronizeText();
            if (IsDropDownOpen)
            {
                BeginDraft();
            }
        }

        /// <inheritdoc />
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Handled)
            {
                return;
            }

            if (e.Key == Key.Down &&
                (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                SetCurrentValue(IsDropDownOpenProperty, true);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape && IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
                textBox?.Focus();
                e.Handled = true;
            }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ZenDateTimePickerAutomationPeer(this);
        }

        private static bool IsValidAutoOrPositiveDimension(object value)
        {
            var dimension = (double)value;
            return double.IsNaN(dimension) ||
                (!double.IsInfinity(dimension) && dimension > 0d);
        }

        private static bool IsValidDropDownButtonIconSize(object value)
        {
            var size = (double)value;
            return !double.IsNaN(size) &&
                !double.IsInfinity(size) &&
                size >= 0d;
        }

        private static DateTime? CoerceMaximum(DependencyObject dependencyObject, object baseValue)
        {
            var picker = (ZenDateTimePicker)dependencyObject;
            var maximum = (DateTime?)baseValue;
            if (maximum.HasValue && picker.Minimum.HasValue && maximum < picker.Minimum)
            {
                return picker.Minimum;
            }

            return maximum;
        }

        private static DateTime? CoerceSelectedDateTime(
            DependencyObject dependencyObject,
            object baseValue)
        {
            var picker = (ZenDateTimePicker)dependencyObject;
            var value = (DateTime?)baseValue;
            if (!value.HasValue)
            {
                return null;
            }

            return picker.Clamp(value.Value);
        }

        private static void HandleSelectedDateTimeChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var picker = (ZenDateTimePicker)dependencyObject;
            picker.SynchronizeText();
            picker.RaiseEvent(
                new RoutedPropertyChangedEventArgs<DateTime?>(
                    (DateTime?)e.OldValue,
                    (DateTime?)e.NewValue,
                    SelectedDateTimeChangedEvent));
        }

        private static void HandleRangeChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var picker = (ZenDateTimePicker)dependencyObject;
            picker.CoerceValue(MaximumProperty);
            picker.CoerceValue(SelectedDateTimeProperty);
            picker.ConfigureCalendarRange();
            if (picker.IsDropDownOpen)
            {
                picker.draftValue = picker.Clamp(picker.draftValue);
                picker.SynchronizeDraft();
            }
        }

        private static void HandleSelectorOptionsChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var picker = (ZenDateTimePicker)dependencyObject;
            picker.ConfigureTimeSelector();
            picker.SynchronizeDraft();
        }

        private static void HandleDisplayOptionsChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var picker = (ZenDateTimePicker)dependencyObject;
            picker.ConfigureTimeSelector();
            picker.UpdateSelectionLayout();
            picker.SynchronizeText();
            picker.SynchronizeDraft();
        }

        private static void HandleSelectionMetricsChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            ((ZenDateTimePicker)dependencyObject).UpdateSelectionLayout();
        }

        private static void HandleIsDropDownOpenChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var picker = (ZenDateTimePicker)dependencyObject;
            if ((bool)e.NewValue)
            {
                picker.BeginDraft();
                picker.Dispatcher.BeginInvoke(new Action(picker.ScrollTimeIntoView));
            }
        }

        private void AttachTemplateHandlers()
        {
            if (textBox != null)
            {
                textBox.LostKeyboardFocus += HandleTextBoxLostKeyboardFocus;
                textBox.KeyDown += HandleTextBoxKeyDown;
                textBox.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(HandleTextBoxMouseLeftButtonUp),
                    true);
            }

            if (popup != null)
            {
                popup.Closed += HandlePopupClosed;
            }

            if (calendar != null)
            {
                calendar.SelectedDatesChanged += HandleCalendarSelectedDatesChanged;
                calendar.PreviewMouseLeftButtonUp +=
                    HandleCalendarPreviewMouseLeftButtonUp;
                calendar.SizeChanged += HandleSelectionMetricsSizeChanged;
            }

            if (timeSelector != null)
            {
                timeSelector.SelectedTimeChanged += HandleTimeSelectorSelectedTimeChanged;
            }

            if (selectionPanel != null)
            {
                selectionPanel.SizeChanged += HandleSelectionMetricsSizeChanged;
            }

            if (timePanel != null)
            {
                timePanel.SizeChanged += HandleSelectionMetricsSizeChanged;
            }

            if (nowButton != null)
            {
                nowButton.Click += HandleNowButtonClick;
            }

            if (confirmButton != null)
            {
                confirmButton.Click += HandleConfirmButtonClick;
            }
        }

        private void DetachTemplateHandlers()
        {
            RemoveConstraintBlackoutRanges();
            if (textBox != null)
            {
                textBox.LostKeyboardFocus -= HandleTextBoxLostKeyboardFocus;
                textBox.KeyDown -= HandleTextBoxKeyDown;
                textBox.RemoveHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(HandleTextBoxMouseLeftButtonUp));
            }

            if (popup != null)
            {
                popup.Closed -= HandlePopupClosed;
            }

            if (calendar != null)
            {
                calendar.SelectedDatesChanged -= HandleCalendarSelectedDatesChanged;
                calendar.PreviewMouseLeftButtonUp -=
                    HandleCalendarPreviewMouseLeftButtonUp;
                calendar.SizeChanged -= HandleSelectionMetricsSizeChanged;
            }

            if (timeSelector != null)
            {
                timeSelector.SelectedTimeChanged -= HandleTimeSelectorSelectedTimeChanged;
            }

            if (selectionPanel != null)
            {
                selectionPanel.SizeChanged -= HandleSelectionMetricsSizeChanged;
            }

            if (timePanel != null)
            {
                timePanel.SizeChanged -= HandleSelectionMetricsSizeChanged;
            }

            if (nowButton != null)
            {
                nowButton.Click -= HandleNowButtonClick;
            }

            if (confirmButton != null)
            {
                confirmButton.Click -= HandleConfirmButtonClick;
            }
        }

        private void HandleSelectionMetricsSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTimeSelectorMetrics();
        }

        private void UpdateSelectionLayout()
        {
            if (dropDownBorder == null || selectionPanel == null || calendar == null)
            {
                return;
            }

            dropDownBorder.Width = DropDownWidth;
            dropDownBorder.Height = DropDownHeight;

            var visibleTimeColumnCount = GetVisibleTimeColumnCount();
            var fixedCalendarCellWidth = !double.IsNaN(CalendarCellWidth);
            var fixedCalendarCellHeight = !double.IsNaN(CalendarCellHeight);
            var fixedTimeItemWidth = !double.IsNaN(TimeItemWidth);
            var fixedTimeItemHeight = !double.IsNaN(TimeItemHeight);

            selectionPanel.Width = double.NaN;
            selectionPanel.Height = double.NaN;
            selectionPanel.HorizontalAlignment =
                fixedCalendarCellWidth && fixedTimeItemWidth
                ? HorizontalAlignment.Left
                : HorizontalAlignment.Stretch;
            selectionPanel.VerticalAlignment =
                fixedCalendarCellHeight && fixedTimeItemHeight
                ? VerticalAlignment.Top
                : VerticalAlignment.Stretch;

            if (selectionPanel.ColumnDefinitions.Count >= 2)
            {
                selectionPanel.ColumnDefinitions[0].Width = fixedCalendarCellWidth
                    ? GridLength.Auto
                    : new GridLength(7d, GridUnitType.Star);
                selectionPanel.ColumnDefinitions[1].Width = fixedTimeItemWidth
                    ? GridLength.Auto
                    : new GridLength(visibleTimeColumnCount, GridUnitType.Star);
            }

            calendar.Width = fixedCalendarCellWidth
                ? 7d * CalendarCellWidth
                : double.NaN;
            calendar.Height = fixedCalendarCellHeight
                ? 8.25d * CalendarCellHeight
                : double.NaN;

            UpdateTimeSelectorMetrics();
        }

        private void UpdateTimeSelectorMetrics()
        {
            if (calendar == null || timeSelector == null || selectionPanel == null)
            {
                return;
            }

            var visibleTimeColumnCount = GetVisibleTimeColumnCount();
            var itemWidth = TimeItemWidth;
            if (double.IsNaN(itemWidth))
            {
                if (timePanel != null)
                {
                    var horizontalChrome = timePanel.BorderThickness.Left +
                        timePanel.BorderThickness.Right +
                        timePanel.Padding.Left +
                        timePanel.Padding.Right;
                    itemWidth = Math.Max(
                        1d,
                        (timePanel.ActualWidth - horizontalChrome) /
                        visibleTimeColumnCount);
                }
                else
                {
                    itemWidth = 1d;
                }
            }

            var itemHeight = TimeItemHeight;
            if (double.IsNaN(itemHeight))
            {
                itemHeight = timeSelector.ListHeight > 0d
                    ? timeSelector.ListHeight / 8.25d
                    : 1d;
            }

            timeSelector.SetCurrentValue(ZenTimeSelector.ColumnWidthProperty, itemWidth);
            timeSelector.SetCurrentValue(ZenTimeSelector.PeriodColumnWidthProperty, itemWidth);
            timeSelector.SetCurrentValue(ZenTimeSelector.ItemHeightProperty, itemHeight);
        }

        private int GetVisibleTimeColumnCount()
        {
            return 2 + (IsSecondVisible ? 1 : 0) + (Is24HourFormat ? 0 : 1);
        }

        private void BeginDraft()
        {
            draftValue = Clamp(SelectedDateTime ?? Normalize(DateTime.Now));
            ConfigureCalendarRange();
            SynchronizeDraft();
        }

        private void SynchronizeDraft()
        {
            if (calendar == null || timeSelector == null || !IsDropDownOpen)
            {
                return;
            }

            isSynchronizingDraft = true;
            try
            {
                ConfigureTimeSelector();
                calendar.SetCurrentValue(Calendar.SelectedDateProperty, (DateTime?)draftValue.Date);
                calendar.SetCurrentValue(Calendar.DisplayDateProperty, draftValue.Date);
                timeSelector.SetCurrentValue(
                    ZenTimeSelector.SelectedTimeProperty,
                    (TimeSpan?)draftValue.TimeOfDay);
            }
            finally
            {
                isSynchronizingDraft = false;
            }
        }

        private void ConfigureCalendarRange()
        {
            if (calendar == null)
            {
                return;
            }

            RemoveConstraintBlackoutRanges();
            if (Minimum.HasValue)
            {
                var start = Minimum.Value.Date;
                var monthStart = new DateTime(start.Year, start.Month, 1);
                calendar.DisplayDateStart = monthStart;
                if (monthStart < start)
                {
                    AddConstraintBlackoutRange(monthStart, start.AddDays(-1));
                }
            }
            else
            {
                calendar.DisplayDateStart = null;
            }

            if (Maximum.HasValue)
            {
                var end = Maximum.Value.Date;
                var monthEnd = new DateTime(
                    end.Year,
                    end.Month,
                    DateTime.DaysInMonth(end.Year, end.Month));
                calendar.DisplayDateEnd = monthEnd;
                if (end < monthEnd)
                {
                    AddConstraintBlackoutRange(end.AddDays(1), monthEnd);
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
            calendar.BlackoutDates.Add(range);
            constraintBlackoutRanges.Add(range);
        }

        private void RemoveConstraintBlackoutRanges()
        {
            if (calendar != null)
            {
                foreach (var range in constraintBlackoutRanges)
                {
                    calendar.BlackoutDates.Remove(range);
                }
            }

            constraintBlackoutRanges.Clear();
        }

        private void ConfigureTimeSelector()
        {
            if (timeSelector == null)
            {
                return;
            }

            var minimumTime = TimeSpan.Zero;
            var maximumTime = new TimeSpan(23, 59, 59);
            if (Minimum.HasValue && draftValue.Date == Minimum.Value.Date)
            {
                minimumTime = Minimum.Value.TimeOfDay;
            }

            if (Maximum.HasValue && draftValue.Date == Maximum.Value.Date)
            {
                maximumTime = Maximum.Value.TimeOfDay;
            }

            timeSelector.SetCurrentValue(
                ZenTimeSelector.Is24HourFormatProperty,
                Is24HourFormat);
            timeSelector.SetCurrentValue(
                ZenTimeSelector.IsSecondVisibleProperty,
                IsSecondVisible);
            timeSelector.SetCurrentValue(
                ZenTimeSelector.MinuteIncrementProperty,
                MinuteIncrement);
            timeSelector.SetCurrentValue(
                ZenTimeSelector.SecondIncrementProperty,
                SecondIncrement);
            timeSelector.SetCurrentValue(ZenTimeSelector.MinimumProperty, minimumTime);
            timeSelector.SetCurrentValue(ZenTimeSelector.MaximumProperty, maximumTime);
        }

        private DateTime Clamp(DateTime value)
        {
            if (Minimum.HasValue && value < Minimum.Value)
            {
                return Minimum.Value;
            }

            if (Maximum.HasValue && value > Maximum.Value)
            {
                return Maximum.Value;
            }

            return value;
        }

        private DateTime Normalize(DateTime value)
        {
            if (!IsSecondVisible)
            {
                return new DateTime(
                    value.Year,
                    value.Month,
                    value.Day,
                    value.Hour,
                    value.Minute,
                    0,
                    value.Kind);
            }

            return new DateTime(
                value.Year,
                value.Month,
                value.Day,
                value.Hour,
                value.Minute,
                value.Second,
                value.Kind);
        }

        private void SynchronizeText()
        {
            if (textBox != null)
            {
                textBox.Text = FormatDateTime(SelectedDateTime);
            }
        }

        private string FormatDateTime(DateTime? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            var format = DateTimeFormat;
            if (string.IsNullOrWhiteSpace(format))
            {
                var timeFormat = Is24HourFormat
                    ? (IsSecondVisible ? "HH:mm:ss" : "HH:mm")
                    : (IsSecondVisible ? "hh:mm:ss tt" : "hh:mm tt");
                format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern +
                    " " +
                    timeFormat;
            }

            try
            {
                return value.Value.ToString(format, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                return value.Value.ToString("g", CultureInfo.CurrentCulture);
            }
        }

        private void CommitText()
        {
            if (textBox == null || IsTextInputReadOnly)
            {
                return;
            }

            var value = textBox.Text?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                SetCurrentValue(SelectedDateTimeProperty, null);
                return;
            }

            if (DateTime.TryParse(
                    value,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out var dateTime))
            {
                SetCurrentValue(SelectedDateTimeProperty, (DateTime?)dateTime);
            }
            else
            {
                SynchronizeText();
            }
        }

        private void HandleCalendarSelectedDatesChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (isSynchronizingDraft || !calendar.SelectedDate.HasValue)
            {
                return;
            }

            draftValue = Clamp(calendar.SelectedDate.Value.Date + draftValue.TimeOfDay);
            isSynchronizingDraft = true;
            try
            {
                ConfigureTimeSelector();
                timeSelector.SetCurrentValue(
                    ZenTimeSelector.SelectedTimeProperty,
                    (TimeSpan?)draftValue.TimeOfDay);
            }
            finally
            {
                isSynchronizingDraft = false;
            }
        }

        private void HandleCalendarPreviewMouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(ReleaseCalendarMouseCapture));
        }

        private void ReleaseCalendarMouseCapture()
        {
            // CalendarItem 在日期选择手势中使用鼠标捕获；模板状态同步后仍可能
            // 留在日历子树中，导致弹层内的下一个按钮点击只用于释放捕获。
            if (calendar != null && calendar.IsMouseCaptureWithin)
            {
                Mouse.Capture(null);
            }
        }

        private void HandleTimeSelectorSelectedTimeChanged(object sender, EventArgs e)
        {
            if (isSynchronizingDraft || !timeSelector.SelectedTime.HasValue)
            {
                return;
            }

            draftValue = Clamp(draftValue.Date + timeSelector.SelectedTime.Value);
            SynchronizeDraft();
        }

        private void HandleTextBoxLostKeyboardFocus(
            object sender,
            KeyboardFocusChangedEventArgs e)
        {
            CommitText();
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

        private void HandleTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitText();
                SetCurrentValue(IsDropDownOpenProperty, false);
                e.Handled = true;
            }
        }

        private void HandleNowButtonClick(object sender, RoutedEventArgs e)
        {
            draftValue = Clamp(Normalize(DateTime.Now));
            SynchronizeDraft();
            ScrollTimeIntoView();
        }

        private void HandleConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(SelectedDateTimeProperty, (DateTime?)draftValue);
            SetCurrentValue(IsDropDownOpenProperty, false);
            textBox?.Focus();
        }

        private void HandlePopupClosed(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
        }

        private void ScrollTimeIntoView()
        {
            timeSelector?.ScrollSelectedItemsIntoView();
        }

        private void HandleIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
        }
    }

    internal sealed class ZenDateTimePickerAutomationPeer :
        FrameworkElementAutomationPeer,
        IValueProvider,
        IExpandCollapseProvider
    {
        public ZenDateTimePickerAutomationPeer(ZenDateTimePicker owner)
            : base(owner)
        {
        }

        private ZenDateTimePicker DateTimePicker => (ZenDateTimePicker)Owner;

        public bool IsReadOnly =>
            !DateTimePicker.IsEnabled || DateTimePicker.IsTextInputReadOnly;

        public string Value => DateTimePicker.SelectedDateTime.HasValue
            ? DateTimePicker.SelectedDateTime.Value.ToString("g", CultureInfo.CurrentCulture)
            : string.Empty;

        public ExpandCollapseState ExpandCollapseState => DateTimePicker.IsDropDownOpen
            ? ExpandCollapseState.Expanded
            : ExpandCollapseState.Collapsed;

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value ||
                patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        public void SetValue(string value)
        {
            if (IsReadOnly)
            {
                throw new ElementNotEnabledException();
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                DateTimePicker.SetCurrentValue(
                    ZenDateTimePicker.SelectedDateTimeProperty,
                    null);
                return;
            }

            if (!DateTime.TryParse(
                    value,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out var dateTime))
            {
                throw new ArgumentException("无法将指定值解析为日期和时间。", nameof(value));
            }

            DateTimePicker.SetCurrentValue(
                ZenDateTimePicker.SelectedDateTimeProperty,
                (DateTime?)dateTime);
        }

        public void Expand()
        {
            EnsureEnabled();
            DateTimePicker.SetCurrentValue(ZenDateTimePicker.IsDropDownOpenProperty, true);
        }

        public void Collapse()
        {
            EnsureEnabled();
            DateTimePicker.SetCurrentValue(ZenDateTimePicker.IsDropDownOpenProperty, false);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return nameof(ZenDateTimePicker);
        }

        private void EnsureEnabled()
        {
            if (!DateTimePicker.IsEnabled)
            {
                throw new ElementNotEnabledException();
            }
        }
    }
}
