using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示一个支持文本输入、弹层选择和时间范围约束的时间选择控件。
    /// </summary>
    [TemplatePart(Name = PartTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PartPopup, Type = typeof(Popup))]
    [TemplatePart(Name = PartHourList, Type = typeof(ListBox))]
    [TemplatePart(Name = PartMinuteList, Type = typeof(ListBox))]
    [TemplatePart(Name = PartSecondList, Type = typeof(ListBox))]
    [TemplatePart(Name = PartPeriodList, Type = typeof(ListBox))]
    [TemplatePart(Name = PartNowButton, Type = typeof(Button))]
    [TemplatePart(Name = PartConfirmButton, Type = typeof(Button))]
    public class ZenTimePicker : Control
    {
        internal const string PartTextBox = "PART_TextBox";
        internal const string PartPopup = "PART_Popup";
        internal const string PartHourList = "PART_HourList";
        internal const string PartMinuteList = "PART_MinuteList";
        internal const string PartSecondList = "PART_SecondList";
        internal const string PartPeriodList = "PART_PeriodList";
        internal const string PartNowButton = "PART_NowButton";
        internal const string PartConfirmButton = "PART_ConfirmButton";

        private static readonly Type SelfType = typeof(ZenTimePicker);
        private TextBox textBox;
        private Popup popup;
        private ListBox hourList;
        private ListBox minuteList;
        private ListBox secondList;
        private ListBox periodList;
        private Button nowButton;
        private Button confirmButton;
        private bool isSynchronizing;

        static ZenTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 初始化 <see cref="ZenTimePicker"/> 类的新实例。
        /// </summary>
        public ZenTimePicker()
        {
            IsEnabledChanged += HandleIsEnabledChanged;
        }

        /// <summary>
        /// 当前选中的一天内时间。
        /// </summary>
        [Bindable(true)]
        public TimeSpan? SelectedTime
        {
            get { return (TimeSpan?)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="SelectedTime"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(
                nameof(SelectedTime),
                typeof(TimeSpan?),
                SelfType,
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    HandleSelectedTimeChanged,
                    (dependencyObject, baseValue) =>
                        CoerceSelectedTime(dependencyObject, baseValue)));

        /// <summary>
        /// 获取或设置允许选择的最早时间。
        /// </summary>
        [Bindable(true)]
        public TimeSpan Minimum
        {
            get { return (TimeSpan)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Minimum"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(TimeSpan),
                SelfType,
                new FrameworkPropertyMetadata(TimeSpan.Zero, HandleRangeChanged),
                IsTimeOfDay);

        /// <summary>
        /// 获取或设置允许选择的最晚时间。
        /// </summary>
        [Bindable(true)]
        public TimeSpan Maximum
        {
            get { return (TimeSpan)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Maximum"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(TimeSpan),
                SelfType,
                new FrameworkPropertyMetadata(
                    new TimeSpan(23, 59, 59),
                    HandleRangeChanged,
                    (dependencyObject, baseValue) =>
                        CoerceMaximum(dependencyObject, baseValue)),
                IsTimeOfDay);

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
        /// 获取或设置是否显示和编辑秒。
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
        /// 获取或设置是否使用 24 小时制。设置为 <see langword="false"/> 时显示上午/下午选择。
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
        /// 获取或设置尚未选择时间时显示的水印。
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
        /// 获取或设置是否允许通过键盘直接输入时间。
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
        /// 获取或设置时间选择弹层是否打开。
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

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            DetachTemplateHandlers();
            base.OnApplyTemplate();

            textBox = GetTemplateChild(PartTextBox) as TextBox;
            popup = GetTemplateChild(PartPopup) as Popup;
            hourList = GetTemplateChild(PartHourList) as ListBox;
            minuteList = GetTemplateChild(PartMinuteList) as ListBox;
            secondList = GetTemplateChild(PartSecondList) as ListBox;
            periodList = GetTemplateChild(PartPeriodList) as ListBox;
            nowButton = GetTemplateChild(PartNowButton) as Button;
            confirmButton = GetTemplateChild(PartConfirmButton) as Button;

            AttachTemplateHandlers();
            PopulateSelectors();
            SynchronizeTemplate();
        }

        /// <inheritdoc />
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Handled)
            {
                return;
            }

            if (e.Key == Key.Down && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                IsDropDownOpen = true;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape && IsDropDownOpen)
            {
                IsDropDownOpen = false;
                e.Handled = true;
            }
        }

        private static bool IsTimeOfDay(object value)
        {
            var time = (TimeSpan)value;
            return time >= TimeSpan.Zero && time < TimeSpan.FromDays(1);
        }

        private static TimeSpan CoerceMaximum(DependencyObject dependencyObject, object baseValue)
        {
            var control = (ZenTimePicker)dependencyObject;
            var maximum = (TimeSpan)baseValue;
            return maximum < control.Minimum ? control.Minimum : maximum;
        }

        private static TimeSpan? CoerceSelectedTime(DependencyObject dependencyObject, object baseValue)
        {
            var control = (ZenTimePicker)dependencyObject;
            var value = (TimeSpan?)baseValue;
            if (!value.HasValue)
            {
                return null;
            }

            var time = value.Value;
            if (!IsTimeOfDay(time))
            {
                time = time < TimeSpan.Zero ? TimeSpan.Zero : new TimeSpan(23, 59, 59);
            }

            if (time < control.Minimum)
            {
                return control.Minimum;
            }

            return time > control.Maximum ? control.Maximum : time;
        }

        private static void HandleSelectedTimeChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            ((ZenTimePicker)dependencyObject).SynchronizeTemplate();
        }

        private static void HandleRangeChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (ZenTimePicker)dependencyObject;
            control.CoerceValue(MaximumProperty);
            control.CoerceValue(SelectedTimeProperty);
            control.SynchronizeTemplate();
        }

        private static void HandleSelectorOptionsChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (ZenTimePicker)dependencyObject;
            control.PopulateSelectors();
            control.SynchronizeTemplate();
        }

        private static void HandleDisplayOptionsChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (ZenTimePicker)dependencyObject;
            control.PopulateSelectors();
            control.SynchronizeTemplate();
        }

        private static void HandleIsDropDownOpenChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (ZenTimePicker)dependencyObject;
            if ((bool)e.NewValue)
            {
                control.SynchronizeTemplate();
                control.Dispatcher.BeginInvoke(
                    new Action(control.ScrollSelectorsIntoView));
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

            if (nowButton != null)
            {
                nowButton.Click += HandleNowButtonClick;
            }

            if (confirmButton != null)
            {
                confirmButton.Click += HandleConfirmButtonClick;
            }

            AddSelectorHandler(hourList);
            AddSelectorHandler(minuteList);
            AddSelectorHandler(secondList);
            AddSelectorHandler(periodList);
        }

        private void DetachTemplateHandlers()
        {
            if (textBox != null)
            {
                textBox.LostKeyboardFocus -= HandleTextBoxLostKeyboardFocus;
                textBox.KeyDown -= HandleTextBoxKeyDown;
            }

            if (popup != null)
            {
                popup.Closed -= HandlePopupClosed;
            }

            if (nowButton != null)
            {
                nowButton.Click -= HandleNowButtonClick;
            }

            if (confirmButton != null)
            {
                confirmButton.Click -= HandleConfirmButtonClick;
            }

            RemoveSelectorHandler(hourList);
            RemoveSelectorHandler(minuteList);
            RemoveSelectorHandler(secondList);
            RemoveSelectorHandler(periodList);
        }

        private void AddSelectorHandler(ListBox selector)
        {
            if (selector != null)
            {
                selector.SelectionChanged += HandleSelectorSelectionChanged;
            }
        }

        private void RemoveSelectorHandler(ListBox selector)
        {
            if (selector != null)
            {
                selector.SelectionChanged -= HandleSelectorSelectionChanged;
            }
        }

        private void PopulateSelectors()
        {
            if (hourList != null)
            {
                hourList.ItemsSource = CreateRange(Is24Hour ? 0 : 1, Is24Hour ? 23 : 12, 1);
            }

            if (minuteList != null)
            {
                minuteList.ItemsSource = CreateRange(0, 59, MinuteIncrement);
            }

            if (secondList != null)
            {
                secondList.ItemsSource = CreateRange(0, 59, SecondIncrement);
            }

            if (periodList != null)
            {
                periodList.ItemsSource = new[]
                {
                    new TimePickerOption(
                        0,
                        CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator),
                    new TimePickerOption(
                        1,
                        CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator)
                };
            }
        }

        private static List<TimePickerOption> CreateRange(int start, int end, int increment)
        {
            var values = new List<TimePickerOption>();
            for (var value = start; value <= end; value += increment)
            {
                values.Add(
                    new TimePickerOption(
                        value,
                        value.ToString("00", CultureInfo.CurrentCulture)));
            }

            return values;
        }

        private void SynchronizeTemplate()
        {
            if (isSynchronizing)
            {
                return;
            }

            isSynchronizing = true;
            try
            {
                if (textBox != null)
                {
                    textBox.Text = FormatTime(SelectedTime);
                }

                var time = SelectedTime ?? DateTime.Now.TimeOfDay;
                var displayHour = Is24Hour
                    ? time.Hours
                    : (time.Hours % 12 == 0 ? 12 : time.Hours % 12);

                SelectNearest(hourList, displayHour, 1);
                SelectNearest(minuteList, time.Minutes, MinuteIncrement);
                SelectNearest(secondList, time.Seconds, SecondIncrement);
                if (periodList != null)
                {
                    SelectOption(periodList, time.Hours >= 12 ? 1 : 0);
                }

                UpdateOptionStates();
            }
            finally
            {
                isSynchronizing = false;
            }
        }

        private static void SelectNearest(ListBox selector, int value, int increment)
        {
            if (selector == null)
            {
                return;
            }

            var selected = increment <= 1 ? value : value - (value % increment);
            SelectOption(selector, selected);
        }

        private static void SelectOption(ListBox selector, int value)
        {
            foreach (var item in selector.Items)
            {
                if (item is TimePickerOption option && option.Value == value)
                {
                    selector.SelectedItem = option;
                    return;
                }
            }
        }

        private string FormatTime(TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            var dateTime = DateTime.Today.Add(value.Value);
            var format = Is24Hour
                ? (IsSecondVisible ? "HH:mm:ss" : "HH:mm")
                : (IsSecondVisible ? "hh:mm:ss tt" : "hh:mm tt");
            return dateTime.ToString(format, CultureInfo.CurrentCulture);
        }

        private void HandleSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isSynchronizing ||
                hourList?.SelectedItem == null ||
                minuteList?.SelectedItem == null ||
                (IsSecondVisible && secondList?.SelectedItem == null))
            {
                return;
            }

            if (sender is ListBox selector &&
                selector.SelectedItem is TimePickerOption selectedOption &&
                !selectedOption.IsEnabled)
            {
                SynchronizeTemplate();
                return;
            }

            var hour = ParseSelector(hourList);
            var minute = ParseSelector(minuteList);
            var second = IsSecondVisible ? ParseSelector(secondList) : 0;
            if (!Is24Hour && periodList != null)
            {
                hour %= 12;
                if (periodList.SelectedIndex == 1)
                {
                    hour += 12;
                }
            }

            SetCurrentValue(
                SelectedTimeProperty,
                (TimeSpan?)new TimeSpan(hour, minute, second));
        }

        private static int ParseSelector(ListBox selector)
        {
            return ((TimePickerOption)selector.SelectedItem).Value;
        }

        private void UpdateOptionStates()
        {
            var selectedTime = SelectedTime ?? DateTime.Now.TimeOfDay;
            var period = periodList?.SelectedItem is TimePickerOption periodOption
                ? periodOption.Value
                : (selectedTime.Hours >= 12 ? 1 : 0);
            var selectedHour = hourList?.SelectedItem is TimePickerOption hourOption
                ? hourOption.Value
                : (Is24Hour
                    ? selectedTime.Hours
                    : (selectedTime.Hours % 12 == 0 ? 12 : selectedTime.Hours % 12));
            var actualHour = ConvertToActualHour(selectedHour, period);
            var selectedMinute = minuteList?.SelectedItem is TimePickerOption minuteOption
                ? minuteOption.Value
                : selectedTime.Minutes;

            SetOptionStates(
                hourList,
                option =>
                {
                    var hour = ConvertToActualHour(option.Value, period);
                    return IntersectsRange(
                        new TimeSpan(hour, 0, 0),
                        new TimeSpan(hour, 59, 59));
                });
            SetOptionStates(
                minuteList,
                option =>
                {
                    var start = new TimeSpan(actualHour, option.Value, 0);
                    var end = IsSecondVisible
                        ? new TimeSpan(actualHour, option.Value, 59)
                        : start;
                    return IntersectsRange(start, end);
                });
            SetOptionStates(
                secondList,
                option => IsWithinRange(
                    new TimeSpan(actualHour, selectedMinute, option.Value)));
            SetOptionStates(
                periodList,
                option => IntersectsRange(
                    new TimeSpan(option.Value == 0 ? 0 : 12, 0, 0),
                    new TimeSpan(option.Value == 0 ? 11 : 23, 59, 59)));
        }

        private int ConvertToActualHour(int displayHour, int period)
        {
            if (Is24Hour)
            {
                return displayHour;
            }

            var hour = displayHour % 12;
            return period == 1 ? hour + 12 : hour;
        }

        private bool IntersectsRange(TimeSpan start, TimeSpan end)
        {
            return end >= Minimum && start <= Maximum;
        }

        private bool IsWithinRange(TimeSpan value)
        {
            return value >= Minimum && value <= Maximum;
        }

        private static void SetOptionStates(
            ListBox selector,
            Func<TimePickerOption, bool> predicate)
        {
            if (selector == null)
            {
                return;
            }

            foreach (var item in selector.Items)
            {
                if (item is TimePickerOption option)
                {
                    option.IsEnabled = predicate(option);
                }
            }
        }

        private void HandleTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CommitText();
        }

        private void HandleTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitText();
                IsDropDownOpen = false;
                e.Handled = true;
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
                SetCurrentValue(SelectedTimeProperty, null);
                return;
            }

            if (DateTime.TryParse(
                    value,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.NoCurrentDateDefault,
                    out var dateTime))
            {
                SetCurrentValue(SelectedTimeProperty, (TimeSpan?)dateTime.TimeOfDay);
            }
            else if (TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out var time) &&
                     IsTimeOfDay(time))
            {
                SetCurrentValue(SelectedTimeProperty, (TimeSpan?)time);
            }
            else
            {
                SynchronizeTemplate();
            }
        }

        private void HandlePopupClosed(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
        }

        private void HandleNowButtonClick(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            SetCurrentValue(
                SelectedTimeProperty,
                (TimeSpan?)new TimeSpan(
                    now.Hour,
                    now.Minute,
                    IsSecondVisible ? now.Second : 0));
            SynchronizeTemplate();
            ScrollSelectorsIntoView();
            Dispatcher.BeginInvoke(new Action(ScrollSelectorsIntoView));
        }

        private void ScrollSelectorsIntoView()
        {
            ScrollSelectedItemIntoView(hourList);
            ScrollSelectedItemIntoView(minuteList);
            if (IsSecondVisible)
            {
                ScrollSelectedItemIntoView(secondList);
            }

            if (!Is24Hour)
            {
                ScrollSelectedItemIntoView(periodList);
            }
        }

        private static void ScrollSelectedItemIntoView(ListBox selector)
        {
            if (selector?.SelectedItem != null)
            {
                selector.ScrollIntoView(selector.SelectedItem);
            }
        }

        private void HandleConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = false;
            textBox?.Focus();
        }

        private void HandleIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                IsDropDownOpen = false;
            }
        }
    }

    internal sealed class TimePickerOption : INotifyPropertyChanged
    {
        private bool isEnabled = true;

        internal TimePickerOption(int value, string displayText)
        {
            Value = value;
            DisplayText = displayText;
        }

        internal int Value { get; }

        public string DisplayText { get; }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value)
                {
                    return;
                }

                isEnabled = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(IsEnabled)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
