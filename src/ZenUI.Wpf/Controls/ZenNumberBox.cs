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

        /// <summary>
        /// 获取将当前值增加一个 <see cref="Increment"/> 的命令。
        /// </summary>
        public static readonly RoutedUICommand IncreaseCommand =
            new RoutedUICommand("增加", nameof(IncreaseCommand), SelfType);

        /// <summary>
        /// 获取将当前值减少一个 <see cref="Increment"/> 的命令。
        /// </summary>
        public static readonly RoutedUICommand DecreaseCommand =
            new RoutedUICommand("减少", nameof(DecreaseCommand), SelfType);

        private TextBox textBox;
        private TextBox verticalTextBox;
        private RepeatButton increaseButton;
        private RepeatButton decreaseButton;
        private RepeatButton verticalIncreaseButton;
        private RepeatButton verticalDecreaseButton;
        private bool isUpdatingText;
        private bool isUpdatingValueFromText;

        static ZenNumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
            CommandManager.RegisterClassCommandBinding(
                SelfType,
                new CommandBinding(IncreaseCommand, OnExecuteIncrease, OnCanExecuteIncrease));
            CommandManager.RegisterClassCommandBinding(
                SelfType,
                new CommandBinding(DecreaseCommand, OnExecuteDecrease, OnCanExecuteDecrease));
        }

        /// <summary>
        /// 初始化 <see cref="ZenNumberBox"/> 类的新实例。
        /// </summary>
        public ZenNumberBox()
        {
            IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
            IsEnabledChanged += OnIsEnabledChanged;
        }

        /// <summary>
        /// 获取或设置当前值。编辑器中的文本可解析为有效数字时会立即更新该值。
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
        public decimal Increment
        {
            get { return (decimal)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Increment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(
                nameof(Increment),
                typeof(decimal),
                SelfType,
                new FrameworkPropertyMetadata(1m),
                value => (decimal)value > 0m);

        /// <summary>
        /// 获取或设置增减按钮的布局方式。
        /// </summary>
        [Bindable(true)]
        public SpinButtonLayout SpinButtonLayout
        {
            get { return (SpinButtonLayout)GetValue(SpinButtonLayoutProperty); }
            set { SetValue(SpinButtonLayoutProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="SpinButtonLayout"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SpinButtonLayoutProperty =
            DependencyProperty.Register(
                nameof(SpinButtonLayout),
                typeof(SpinButtonLayout),
                SelfType,
                new FrameworkPropertyMetadata(SpinButtonLayout.Horizontal));

        /// <summary>
        /// 获取或设置增减按钮的宽度。该值必须为大于或等于零的有限值。
        /// </summary>
        [Bindable(true)]
        public double SpinButtonWidth
        {
            get { return (double)GetValue(SpinButtonWidthProperty); }
            set { SetValue(SpinButtonWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="SpinButtonWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SpinButtonWidthProperty =
            DependencyProperty.Register(
                nameof(SpinButtonWidth),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(
                    34d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure),
                IsValidSpinButtonWidth);

        /// <summary>
        /// 获取或设置增加按钮中显示的自定义内容。值为 <see langword="null"/> 时显示默认图标。
        /// </summary>
        [Bindable(true)]
        public object IncreaseButtonContent
        {
            get { return GetValue(IncreaseButtonContentProperty); }
            set { SetValue(IncreaseButtonContentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IncreaseButtonContent"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IncreaseButtonContentProperty =
            DependencyProperty.Register(
                nameof(IncreaseButtonContent),
                typeof(object),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置用于显示 <see cref="IncreaseButtonContent"/> 的数据模板。
        /// </summary>
        [Bindable(true)]
        public DataTemplate IncreaseButtonContentTemplate
        {
            get { return (DataTemplate)GetValue(IncreaseButtonContentTemplateProperty); }
            set { SetValue(IncreaseButtonContentTemplateProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IncreaseButtonContentTemplate"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IncreaseButtonContentTemplateProperty =
            DependencyProperty.Register(
                nameof(IncreaseButtonContentTemplate),
                typeof(DataTemplate),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置减少按钮中显示的自定义内容。值为 <see langword="null"/> 时显示默认图标。
        /// </summary>
        [Bindable(true)]
        public object DecreaseButtonContent
        {
            get { return GetValue(DecreaseButtonContentProperty); }
            set { SetValue(DecreaseButtonContentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DecreaseButtonContent"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DecreaseButtonContentProperty =
            DependencyProperty.Register(
                nameof(DecreaseButtonContent),
                typeof(object),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置用于显示 <see cref="DecreaseButtonContent"/> 的数据模板。
        /// </summary>
        [Bindable(true)]
        public DataTemplate DecreaseButtonContentTemplate
        {
            get { return (DataTemplate)GetValue(DecreaseButtonContentTemplateProperty); }
            set { SetValue(DecreaseButtonContentTemplateProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DecreaseButtonContentTemplate"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DecreaseButtonContentTemplateProperty =
            DependencyProperty.Register(
                nameof(DecreaseButtonContentTemplate),
                typeof(DataTemplate),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置在用户点击数字输入区域时执行的命令。
        /// </summary>
        [Bindable(true)]
        public ICommand EditorClickCommand
        {
            get { return (ICommand)GetValue(EditorClickCommandProperty); }
            set { SetValue(EditorClickCommandProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="EditorClickCommand"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty EditorClickCommandProperty =
            DependencyProperty.Register(
                nameof(EditorClickCommand),
                typeof(ICommand),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置传递给 <see cref="EditorClickCommand"/> 的参数。
        /// </summary>
        [Bindable(true)]
        public object EditorClickCommandParameter
        {
            get { return GetValue(EditorClickCommandParameterProperty); }
            set { SetValue(EditorClickCommandParameterProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="EditorClickCommandParameter"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty EditorClickCommandParameterProperty =
            DependencyProperty.Register(
                nameof(EditorClickCommandParameter),
                typeof(object),
                SelfType,
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置一个值，该值指示是否禁止直接编辑文本。增减按钮仍然可用。
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

        private static bool IsValidSpinButtonWidth(object value)
        {
            var width = (double)value;
            return !double.IsNaN(width) &&
                !double.IsInfinity(width) &&
                width >= 0d;
        }

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

        /// <inheritdoc />
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Handled)
            {
                return;
            }

            if (e.Key == Key.Up)
            {
                ExecuteCommand(IncreaseCommand);
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                ExecuteCommand(DecreaseCommand);
                e.Handled = true;
            }
        }

        /// <inheritdoc />
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
            if (!owner.isUpdatingValueFromText)
            {
                owner.UpdateText();
            }

            owner.UpdateButtonStates();
            owner.RaiseEvent(new RoutedPropertyChangedEventArgs<decimal>(
                (decimal)e.OldValue,
                (decimal)e.NewValue,
                ValueChangedEvent));
        }

        private void OnIncreaseClick(object sender, RoutedEventArgs e)
        {
            ExecuteCommand(IncreaseCommand);
        }

        private void OnDecreaseClick(object sender, RoutedEventArgs e)
        {
            ExecuteCommand(DecreaseCommand);
        }

        private static void OnCanExecuteIncrease(object sender, CanExecuteRoutedEventArgs e)
        {
            var owner = (ZenNumberBox)sender;
            e.CanExecute = owner.IsEnabled && owner.Value < owner.Maximum;
            e.Handled = true;
        }

        private static void OnCanExecuteDecrease(object sender, CanExecuteRoutedEventArgs e)
        {
            var owner = (ZenNumberBox)sender;
            e.CanExecute = owner.IsEnabled && owner.Value > owner.Minimum;
            e.Handled = true;
        }

        private static void OnExecuteIncrease(object sender, ExecutedRoutedEventArgs e)
        {
            ((ZenNumberBox)sender).ChangeValue(((ZenNumberBox)sender).Increment);
            e.Handled = true;
        }

        private static void OnExecuteDecrease(object sender, ExecutedRoutedEventArgs e)
        {
            ((ZenNumberBox)sender).ChangeValue(-((ZenNumberBox)sender).Increment);
            e.Handled = true;
        }

        private void ExecuteCommand(RoutedCommand command)
        {
            if (command.CanExecute(null, this))
            {
                command.Execute(null, this);
            }
        }

        private void ChangeValue(decimal delta)
        {
            CommitInput();
            try
            {
                SetCurrentValue(
                    ValueProperty,
                    delta > 0m
                        ? Math.Min(Maximum, checked(Value + delta))
                        : Math.Max(Minimum, checked(Value + delta)));
            }
            catch (OverflowException)
            {
                SetCurrentValue(ValueProperty, delta > 0m ? Maximum : Minimum);
            }
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var activeTextBox = GetActiveTextBox();
            if (activeTextBox == null ||
                isUpdatingText ||
                IsReadOnly ||
                !ReferenceEquals(sender, activeTextBox) ||
                !decimal.TryParse(
                    activeTextBox.Text,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out var parsedValue))
            {
                return;
            }

            isUpdatingValueFromText = true;
            try
            {
                SetCurrentValue(ValueProperty, parsedValue);
            }
            finally
            {
                isUpdatingValueFromText = false;
            }

            if (Value != parsedValue)
            {
                UpdateText();
            }
            else
            {
                UpdateInactiveTextBox(activeTextBox);
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

        private void OnEditorMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var command = EditorClickCommand;
            var parameter = EditorClickCommandParameter;
            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
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
            var activeTextBox = GetActiveTextBox();
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
                SetCurrentValue(ValueProperty, parsedValue);
            }

            UpdateText();
        }

        private TextBox GetActiveTextBox()
        {
            return SpinButtonLayout == SpinButtonLayout.Vertical
                ? verticalTextBox
                : textBox;
        }

        private void UpdateText()
        {
            isUpdatingText = true;
            UpdateTextBox(textBox);
            UpdateTextBox(verticalTextBox);
            isUpdatingText = false;
        }

        private void UpdateInactiveTextBox(TextBox activeTextBox)
        {
            isUpdatingText = true;
            UpdateTextBox(ReferenceEquals(activeTextBox, textBox) ? verticalTextBox : textBox);
            isUpdatingText = false;
        }

        private void UpdateButtonStates()
        {
            SetButtonEnabled(increaseButton, Value < Maximum);
            SetButtonEnabled(verticalIncreaseButton, Value < Maximum);
            SetButtonEnabled(decreaseButton, Value > Minimum);
            SetButtonEnabled(verticalDecreaseButton, Value > Minimum);
            CommandManager.InvalidateRequerySuggested();
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
                target.TextChanged += OnTextBoxTextChanged;
                target.KeyDown += OnTextBoxKeyDown;
                target.LostKeyboardFocus += OnTextBoxLostKeyboardFocus;
                target.AddHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(OnEditorMouseLeftButtonUp),
                    true);
            }
        }

        private void DetachTextBoxHandlers(TextBox target)
        {
            if (target != null)
            {
                target.TextChanged -= OnTextBoxTextChanged;
                target.KeyDown -= OnTextBoxKeyDown;
                target.LostKeyboardFocus -= OnTextBoxLostKeyboardFocus;
                target.RemoveHandler(
                    MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(OnEditorMouseLeftButtonUp));
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
        public double LargeChange => (double)NumberBox.Increment;
        public double Maximum => (double)NumberBox.Maximum;
        public double Minimum => (double)NumberBox.Minimum;
        public double SmallChange => (double)NumberBox.Increment;
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
