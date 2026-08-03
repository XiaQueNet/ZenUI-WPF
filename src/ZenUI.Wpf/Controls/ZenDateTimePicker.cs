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
        internal const string PartCalendar = "PART_Calendar";
        internal const string PartTimeSelector = "PART_TimeSelector";
        internal const string PartNowButton = "PART_NowButton";
        internal const string PartConfirmButton = "PART_ConfirmButton";

        private static readonly Type SelfType = typeof(ZenDateTimePicker);
        private readonly List<CalendarDateRange> constraintBlackoutRanges =
            new List<CalendarDateRange>();
        private TextBox textBox;
        private Popup popup;
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
        public bool Is24Hour
        {
            get { return (bool)GetValue(Is24HourProperty); }
            set { SetValue(Is24HourProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Is24Hour"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty Is24HourProperty =
            DependencyProperty.Register(
                nameof(Is24Hour),
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
        /// 获取或设置一个值，该值指示是否允许通过键盘直接输入日期时间。
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
                SelfType,
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

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            DetachTemplateHandlers();
            base.OnApplyTemplate();

            textBox = GetTemplateChild(PartTextBox) as TextBox;
            popup = GetTemplateChild(PartPopup) as Popup;
            calendar = GetTemplateChild(PartCalendar) as Calendar;
            timeSelector = GetTemplateChild(PartTimeSelector) as ZenTimeSelector;
            nowButton = GetTemplateChild(PartNowButton) as Button;
            confirmButton = GetTemplateChild(PartConfirmButton) as Button;

            AttachTemplateHandlers();
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
            picker.SynchronizeText();
            picker.SynchronizeDraft();
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
            }

            if (popup != null)
            {
                popup.Closed += HandlePopupClosed;
            }

            if (calendar != null)
            {
                calendar.SelectedDatesChanged += HandleCalendarSelectedDatesChanged;
            }

            if (timeSelector != null)
            {
                timeSelector.SelectedTimeChanged += HandleTimeSelectorSelectedTimeChanged;
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
            }

            if (popup != null)
            {
                popup.Closed -= HandlePopupClosed;
            }

            if (calendar != null)
            {
                calendar.SelectedDatesChanged -= HandleCalendarSelectedDatesChanged;
            }

            if (timeSelector != null)
            {
                timeSelector.SelectedTimeChanged -= HandleTimeSelectorSelectedTimeChanged;
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

            timeSelector.SetCurrentValue(ZenTimeSelector.Is24HourProperty, Is24Hour);
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
                var timeFormat = Is24Hour
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
            if (textBox == null || !IsTextInputEnabled)
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
            SynchronizeDraft();
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
            !DateTimePicker.IsEnabled || !DateTimePicker.IsTextInputEnabled;

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
