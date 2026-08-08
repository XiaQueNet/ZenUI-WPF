using System;
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
    [TemplatePart(Name = PartTimeSelector, Type = typeof(Control))]
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
        internal const string PartTimeSelector = "PART_TimeSelector";
        internal const string PartHourList = "PART_HourList";
        internal const string PartMinuteList = "PART_MinuteList";
        internal const string PartSecondList = "PART_SecondList";
        internal const string PartPeriodList = "PART_PeriodList";
        internal const string PartNowButton = "PART_NowButton";
        internal const string PartConfirmButton = "PART_ConfirmButton";

        private static readonly Type SelfType = typeof(ZenTimePicker);
        private TextBox textBox;
        private Popup popup;
        private ZenTimeSelector timeSelector;
        private TimeSelectorCoordinator legacyTimeSelector;
        private Button nowButton;
        private Button confirmButton;

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
        /// 获取或设置是否使用 24 小时制。设置为 <see langword="false"/> 时显示 AM/PM 选择。
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
        /// 获取或设置尚未选择时间时，获得键盘焦点是否仍显示水印。
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
                SelfType,
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 获取或设置一个值，该值指示时间文本输入是否只读。只读时，点击输入区域会打开时间选择弹层。
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
                SelfType,
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
                SelfType,
                new FrameworkPropertyMetadata(
                    28d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                IsValidNonNegativeDimension);

        /// <summary>
        /// 获取或设置下拉按钮中时钟图标的边长。该值必须为大于或等于零的有限值。
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
                IsValidNonNegativeDimension);

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
            timeSelector = GetTemplateChild(PartTimeSelector) as ZenTimeSelector;
            nowButton = GetTemplateChild(PartNowButton) as Button;
            confirmButton = GetTemplateChild(PartConfirmButton) as Button;
            if (timeSelector == null)
            {
                legacyTimeSelector = new TimeSelectorCoordinator(
                    GetTemplateChild(PartHourList) as ListBox,
                    GetTemplateChild(PartMinuteList) as ListBox,
                    GetTemplateChild(PartSecondList) as ListBox,
                    GetTemplateChild(PartPeriodList) as ListBox,
                    HandleSelectorSelectedTimeChanged);
            }

            AttachTemplateHandlers();
            ConfigureTimeSelector();
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
            control.ConfigureTimeSelector();
            control.SynchronizeTemplate();
        }

        private static void HandleDisplayOptionsChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (ZenTimePicker)dependencyObject;
            control.ConfigureTimeSelector();
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
                textBox.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(HandleTextBoxMouseLeftButtonUp),
                    true);
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

            if (timeSelector != null)
            {
                timeSelector.SelectedTimeChanged += HandleTimeSelectorSelectedTimeChanged;
            }
        }

        private void DetachTemplateHandlers()
        {
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

            if (nowButton != null)
            {
                nowButton.Click -= HandleNowButtonClick;
            }

            if (confirmButton != null)
            {
                confirmButton.Click -= HandleConfirmButtonClick;
            }

            if (timeSelector != null)
            {
                timeSelector.SelectedTimeChanged -= HandleTimeSelectorSelectedTimeChanged;
            }

            legacyTimeSelector?.Detach();
            timeSelector = null;
            legacyTimeSelector = null;
        }

        private void ConfigureTimeSelector()
        {
            if (timeSelector != null)
            {
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
                timeSelector.SetCurrentValue(ZenTimeSelector.MinimumProperty, Minimum);
                timeSelector.SetCurrentValue(ZenTimeSelector.MaximumProperty, Maximum);
            }

            legacyTimeSelector?.Configure(
                Is24HourFormat,
                IsSecondVisible,
                MinuteIncrement,
                SecondIncrement,
                Minimum,
                Maximum);
        }

        private void SynchronizeTemplate()
        {
            if (textBox != null)
            {
                textBox.Text = FormatTime(SelectedTime);
            }

            if (timeSelector != null &&
                timeSelector.SelectedTime != SelectedTime)
            {
                timeSelector.SetCurrentValue(
                    ZenTimeSelector.SelectedTimeProperty,
                    SelectedTime);
            }

            legacyTimeSelector?.Synchronize(SelectedTime ?? DateTime.Now.TimeOfDay);
        }

        private string FormatTime(TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            var dateTime = DateTime.Today.Add(value.Value);
            var format = Is24HourFormat
                ? (IsSecondVisible ? "HH:mm:ss" : "HH:mm")
                : (IsSecondVisible ? "hh:mm:ss tt" : "hh:mm tt");
            return dateTime.ToString(format, CultureInfo.CurrentCulture);
        }

        private void HandleSelectorSelectedTimeChanged(TimeSpan selectedTime)
        {
            SetCurrentValue(SelectedTimeProperty, (TimeSpan?)selectedTime);
        }

        private void HandleTimeSelectorSelectedTimeChanged(object sender, EventArgs e)
        {
            if (timeSelector != null && timeSelector.SelectedTime != SelectedTime)
            {
                SetCurrentValue(SelectedTimeProperty, timeSelector.SelectedTime);
            }
        }

        private void HandleTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
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
                IsDropDownOpen = false;
                e.Handled = true;
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
            timeSelector?.ScrollSelectedItemsIntoView();
            legacyTimeSelector?.ScrollSelectedItemsIntoView();
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

        private static bool IsValidNonNegativeDimension(object value)
        {
            var dimension = (double)value;
            return !double.IsNaN(dimension) &&
                !double.IsInfinity(dimension) &&
                dimension >= 0d;
        }
    }
}
