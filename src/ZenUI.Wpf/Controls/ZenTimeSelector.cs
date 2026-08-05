using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示供日期和时间输入控件复用的内部时间选择面板。
    /// </summary>
    [TemplatePart(Name = PartHourList, Type = typeof(ListBox))]
    [TemplatePart(Name = PartMinuteList, Type = typeof(ListBox))]
    [TemplatePart(Name = PartSecondList, Type = typeof(ListBox))]
    [TemplatePart(Name = PartPeriodList, Type = typeof(ListBox))]
    internal class ZenTimeSelector : Control
    {
        internal const string PartHourList = "PART_HourList";
        internal const string PartMinuteList = "PART_MinuteList";
        internal const string PartSecondList = "PART_SecondList";
        internal const string PartPeriodList = "PART_PeriodList";

        private static readonly Type SelfType = typeof(ZenTimeSelector);
        private TimeSelectorCoordinator coordinator;

        static ZenTimeSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        /// <summary>
        /// 获取或设置当前选中的一天内时间。
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
                new FrameworkPropertyMetadata(1, HandleOptionsChanged),
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
                new FrameworkPropertyMetadata(1, HandleOptionsChanged),
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
                new FrameworkPropertyMetadata(true, HandleOptionsChanged));

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
                new FrameworkPropertyMetadata(true, HandleOptionsChanged));

        internal double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        internal static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register(
                nameof(ColumnWidth),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(64d),
                value => (double)value > 0d);

        internal double PeriodColumnWidth
        {
            get { return (double)GetValue(PeriodColumnWidthProperty); }
            set { SetValue(PeriodColumnWidthProperty, value); }
        }

        internal static readonly DependencyProperty PeriodColumnWidthProperty =
            DependencyProperty.Register(
                nameof(PeriodColumnWidth),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(74d),
                value => (double)value > 0d);

        internal double ListHeight
        {
            get { return (double)GetValue(ListHeightProperty); }
            set { SetValue(ListHeightProperty, value); }
        }

        internal static readonly DependencyProperty ListHeightProperty =
            DependencyProperty.Register(
                nameof(ListHeight),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(196d),
                value =>
                {
                    var height = (double)value;
                    return height >= 0d &&
                           !double.IsNaN(height) &&
                           !double.IsInfinity(height);
                });

        internal double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        internal static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(
                nameof(ItemHeight),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(36d),
                value => (double)value > 0d);

        internal event EventHandler SelectedTimeChanged;

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            coordinator?.Detach();
            base.OnApplyTemplate();

            coordinator = new TimeSelectorCoordinator(
                GetTemplateChild(PartHourList) as ListBox,
                GetTemplateChild(PartMinuteList) as ListBox,
                GetTemplateChild(PartSecondList) as ListBox,
                GetTemplateChild(PartPeriodList) as ListBox,
                HandleSelectorSelectedTimeChanged);
            ConfigureCoordinator();
            SynchronizeTemplate();
        }

        internal void ScrollSelectedItemsIntoView()
        {
            coordinator?.ScrollSelectedItemsIntoView();
        }

        private static bool IsTimeOfDay(object value)
        {
            var time = (TimeSpan)value;
            return time >= TimeSpan.Zero && time < TimeSpan.FromDays(1);
        }

        private static TimeSpan CoerceMaximum(
            DependencyObject dependencyObject,
            object baseValue)
        {
            var selector = (ZenTimeSelector)dependencyObject;
            var maximum = (TimeSpan)baseValue;
            return maximum < selector.Minimum ? selector.Minimum : maximum;
        }

        private static TimeSpan? CoerceSelectedTime(
            DependencyObject dependencyObject,
            object baseValue)
        {
            var selector = (ZenTimeSelector)dependencyObject;
            var value = (TimeSpan?)baseValue;
            if (!value.HasValue)
            {
                return null;
            }

            var time = value.Value;
            if (!IsTimeOfDay(time))
            {
                time = time < TimeSpan.Zero
                    ? TimeSpan.Zero
                    : new TimeSpan(23, 59, 59);
            }

            if (time < selector.Minimum)
            {
                return selector.Minimum;
            }

            return time > selector.Maximum ? selector.Maximum : time;
        }

        private static void HandleSelectedTimeChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var selector = (ZenTimeSelector)dependencyObject;
            selector.SynchronizeTemplate();
            selector.SelectedTimeChanged?.Invoke(selector, EventArgs.Empty);
        }

        private static void HandleRangeChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var selector = (ZenTimeSelector)dependencyObject;
            selector.CoerceValue(MaximumProperty);
            selector.CoerceValue(SelectedTimeProperty);
            selector.ConfigureCoordinator();
            selector.SynchronizeTemplate();
        }

        private static void HandleOptionsChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var selector = (ZenTimeSelector)dependencyObject;
            selector.ConfigureCoordinator();
            selector.SynchronizeTemplate();
        }

        private void ConfigureCoordinator()
        {
            coordinator?.Configure(
                Is24Hour,
                IsSecondVisible,
                MinuteIncrement,
                SecondIncrement,
                Minimum,
                Maximum);
        }

        private void SynchronizeTemplate()
        {
            coordinator?.Synchronize(SelectedTime ?? DateTime.Now.TimeOfDay);
        }

        private void HandleSelectorSelectedTimeChanged(TimeSpan selectedTime)
        {
            SetCurrentValue(SelectedTimeProperty, (TimeSpan?)selectedTime);
        }
    }
}
