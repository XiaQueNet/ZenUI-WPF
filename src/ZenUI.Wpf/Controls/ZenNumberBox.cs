using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示一个支持步进、范围约束和两种按钮布局的数字输入控件。
    /// </summary>
    [TemplatePart(Name = PartTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PartVerticalTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PartIncreaseButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PartDecreaseButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PartVerticalIncreaseButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PartVerticalDecreaseButton, Type = typeof(RepeatButton))]
    public class ZenNumberBox : Control
    {
        internal const string PartTextBox = "PART_TextBox";
        internal const string PartVerticalTextBox = "PART_VerticalTextBox";
        internal const string PartIncreaseButton = "PART_IncreaseButton";
        internal const string PartDecreaseButton = "PART_DecreaseButton";
        internal const string PartVerticalIncreaseButton = "PART_VerticalIncreaseButton";
        internal const string PartVerticalDecreaseButton = "PART_VerticalDecreaseButton";

        private static readonly Type SelfType = typeof(ZenNumberBox);
        private TextBox textBox;
        private TextBox verticalTextBox;
        private RepeatButton increaseButton;
        private RepeatButton decreaseButton;
        private RepeatButton verticalIncreaseButton;
        private RepeatButton verticalDecreaseButton;
        private bool isUpdatingText;

        static ZenNumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
        }

        public ZenNumberBox()
        {
            IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
            IsEnabledChanged += OnIsEnabledChanged;
        }

        /// <summary>
        /// 获取或设置当前值。
        /// </summary>
        [Bindable(true)]
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Value"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(decimal),
                SelfType,
                new FrameworkPropertyMetadata(
                    0m,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChanged,
                    (dependencyObject, baseValue) => CoerceValue(dependencyObject, baseValue)));

        /// <summary>
        /// 获取或设置允许的最小值。
        /// </summary>
        [Bindable(true)]
        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Minimum"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(decimal),
                SelfType,
                new FrameworkPropertyMetadata(decimal.MinValue, OnMinimumChanged));

        /// <summary>
        /// 获取或设置允许的最大值。
        /// </summary>
        [Bindable(true)]
        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Maximum"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(decimal),
                SelfType,
                new FrameworkPropertyMetadata(
                    decimal.MaxValue,
                    OnMaximumChanged,
                    (dependencyObject, baseValue) => CoerceMaximum(dependencyObject, baseValue)));

        /// <summary>
        /// 获取或设置单次增加或减少的步长。该值必须大于零。
        /// </summary>
        [Bindable(true)]
        public decimal Step
        {
            get { return (decimal)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Step"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(
                nameof(Step),
                typeof(decimal),
                SelfType,
                new FrameworkPropertyMetadata(1m),
                value => (decimal)value > 0m);

        /// <summary>
        /// 获取或设置增减按钮的布局方式。
        /// </summary>
        [Bindable(true)]
        public NumberBoxButtonMode ButtonMode
        {
            get { return (NumberBoxButtonMode)GetValue(ButtonModeProperty); }
            set { SetValue(ButtonModeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ButtonMode"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ButtonModeProperty =
            DependencyProperty.Register(
                nameof(ButtonMode),
                typeof(NumberBoxButtonMode),
                SelfType,
                new FrameworkPropertyMetadata(NumberBoxButtonMode.Horizontal));

        /// <summary>
        /// 获取或设置控件是否禁止直接编辑文本。增减按钮仍然可用。
        /// </summary>
        [Bindable(true)]
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsReadOnly"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                SelfType,
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// 当 <see cref="Value"/> 改变时发生。
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="ValueChanged"/> 路由事件。
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ValueChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<decimal>),
                SelfType);

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            DetachTemplateHandlers();
            base.OnApplyTemplate();

            textBox = GetTemplateChild(PartTextBox) as TextBox;
            verticalTextBox = GetTemplateChild(PartVerticalTextBox) as TextBox;
            increaseButton = GetTemplateChild(PartIncreaseButton) as RepeatButton;
            decreaseButton = GetTemplateChild(PartDecreaseButton) as RepeatButton;
            verticalIncreaseButton = GetTemplateChild(PartVerticalIncreaseButton) as RepeatButton;
            verticalDecreaseButton = GetTemplateChild(PartVerticalDecreaseButton) as RepeatButton;

            AttachTextBoxHandlers(textBox);
            AttachTextBoxHandlers(verticalTextBox);

            AttachButtonHandler(increaseButton, OnIncreaseClick);
            AttachButtonHandler(verticalIncreaseButton, OnIncreaseClick);
            AttachButtonHandler(decreaseButton, OnDecreaseClick);
            AttachButtonHandler(verticalDecreaseButton, OnDecreaseClick);

            UpdateText();
            UpdateButtonStates();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Handled)
            {
                return;
            }

            if (e.Key == Key.Up)
            {
                ChangeValue(Step);
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                ChangeValue(-Step);
                e.Handled = true;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ZenNumberBoxAutomationPeer(this);
        }

        private static decimal CoerceValue(DependencyObject dependencyObject, object baseValue)
        {
            var owner = (ZenNumberBox)dependencyObject;
            var value = (decimal)baseValue;
            return Math.Max(owner.Minimum, Math.Min(owner.Maximum, value));
        }

        private static decimal CoerceMaximum(DependencyObject dependencyObject, object baseValue)
        {
            var owner = (ZenNumberBox)dependencyObject;
            return Math.Max(owner.Minimum, (decimal)baseValue);
        }

        private static void OnMinimumChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var owner = (ZenNumberBox)dependencyObject;
            owner.CoerceValue(MaximumProperty);
            owner.CoerceValue(ValueProperty);
            owner.UpdateButtonStates();
        }

        private static void OnMaximumChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var owner = (ZenNumberBox)dependencyObject;
            owner.CoerceValue(ValueProperty);
            owner.UpdateButtonStates();
        }

        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var owner = (ZenNumberBox)dependencyObject;
            owner.UpdateText();
            owner.UpdateButtonStates();
            owner.RaiseEvent(new RoutedPropertyChangedEventArgs<decimal>(
                (decimal)e.OldValue,
                (decimal)e.NewValue,
                ValueChangedEvent));
        }

        private void OnIncreaseClick(object sender, RoutedEventArgs e)
        {
            ChangeValue(Step);
        }

        private void OnDecreaseClick(object sender, RoutedEventArgs e)
        {
            ChangeValue(-Step);
        }

        private void ChangeValue(decimal delta)
        {
            CommitInput();
            try
            {
                Value = delta > 0m
                    ? Math.Min(Maximum, checked(Value + delta))
                    : Math.Max(Minimum, checked(Value + delta));
            }
            catch (OverflowException)
            {
                Value = delta > 0m ? Maximum : Minimum;
            }
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitInput();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                UpdateText();
                e.Handled = true;
            }
        }

        private void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CommitInput();
        }

        private void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                CommitInput();
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateButtonStates();
        }

        private void CommitInput()
        {
            var activeTextBox = ButtonMode == NumberBoxButtonMode.Vertical
                ? verticalTextBox
                : textBox;
            if (activeTextBox == null || isUpdatingText || IsReadOnly)
            {
                return;
            }

            if (decimal.TryParse(
                activeTextBox.Text,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out var parsedValue))
            {
                Value = parsedValue;
            }

            UpdateText();
        }

        private void UpdateText()
        {
            isUpdatingText = true;
            UpdateTextBox(textBox);
            UpdateTextBox(verticalTextBox);
            isUpdatingText = false;
        }

        private void UpdateButtonStates()
        {
            SetButtonEnabled(increaseButton, Value < Maximum);
            SetButtonEnabled(verticalIncreaseButton, Value < Maximum);
            SetButtonEnabled(decreaseButton, Value > Minimum);
            SetButtonEnabled(verticalDecreaseButton, Value > Minimum);
        }

        private void DetachTemplateHandlers()
        {
            DetachTextBoxHandlers(textBox);
            DetachTextBoxHandlers(verticalTextBox);
            DetachButtonHandler(increaseButton, OnIncreaseClick);
            DetachButtonHandler(verticalIncreaseButton, OnIncreaseClick);
            DetachButtonHandler(decreaseButton, OnDecreaseClick);
            DetachButtonHandler(verticalDecreaseButton, OnDecreaseClick);
        }

        private void UpdateTextBox(TextBox target)
        {
            if (target == null)
            {
                return;
            }

            target.Text = Value.ToString(CultureInfo.CurrentCulture);
            target.CaretIndex = target.Text.Length;
        }

        private void SetButtonEnabled(RepeatButton button, bool canChange)
        {
            if (button != null)
            {
                button.IsEnabled = IsEnabled && canChange;
            }
        }

        private void AttachTextBoxHandlers(TextBox target)
        {
            if (target != null)
            {
                target.KeyDown += OnTextBoxKeyDown;
                target.LostKeyboardFocus += OnTextBoxLostKeyboardFocus;
            }
        }

        private void DetachTextBoxHandlers(TextBox target)
        {
            if (target != null)
            {
                target.KeyDown -= OnTextBoxKeyDown;
                target.LostKeyboardFocus -= OnTextBoxLostKeyboardFocus;
            }
        }

        private static void AttachButtonHandler(RepeatButton button, RoutedEventHandler handler)
        {
            if (button != null)
            {
                button.Click += handler;
            }
        }

        private static void DetachButtonHandler(RepeatButton button, RoutedEventHandler handler)
        {
            if (button != null)
            {
                button.Click -= handler;
            }
        }
    }

    internal sealed class ZenNumberBoxAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
        public ZenNumberBoxAutomationPeer(ZenNumberBox owner)
            : base(owner)
        {
        }

        private ZenNumberBox NumberBox => (ZenNumberBox)Owner;

        public bool IsReadOnly => !NumberBox.IsEnabled || NumberBox.IsReadOnly;
        public double LargeChange => (double)NumberBox.Step;
        public double Maximum => (double)NumberBox.Maximum;
        public double Minimum => (double)NumberBox.Minimum;
        public double SmallChange => (double)NumberBox.Step;
        public double Value => (double)NumberBox.Value;

        public override object GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.RangeValue
                ? this
                : base.GetPattern(patternInterface);
        }

        public void SetValue(double value)
        {
            if (IsReadOnly)
            {
                throw new ElementNotEnabledException();
            }

            NumberBox.Value = (decimal)value;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Spinner;
        }

        protected override string GetClassNameCore()
        {
            return nameof(ZenNumberBox);
        }
    }
}
