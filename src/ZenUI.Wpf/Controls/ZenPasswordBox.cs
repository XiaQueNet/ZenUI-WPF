using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持水印和自定义圆角的密码输入控件。
    /// </summary>
    [TemplatePart(Name = PasswordBoxPartName, Type = typeof(PasswordBox))]
    [TemplatePart(Name = RevealTextBoxPartName, Type = typeof(TextBox))]
    [TemplatePart(Name = RevealButtonPartName, Type = typeof(ToggleButton))]
    public class ZenPasswordBox : Control
    {
        private const string PasswordBoxPartName = "PART_PasswordBox";
        private const string RevealTextBoxPartName = "PART_RevealTextBox";
        private const string RevealButtonPartName = "PART_RevealButton";
        private PasswordBox passwordBox;
        private TextBox revealTextBox;
        private bool isSynchronizingPassword;

        static ZenPasswordBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenPasswordBox),
                new FrameworkPropertyMetadata(typeof(ZenPasswordBox)));
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            if (passwordBox != null)
            {
                passwordBox.PasswordChanged -= OnPasswordChanged;
            }

            if (revealTextBox != null)
            {
                revealTextBox.TextChanged -= OnRevealTextChanged;
                revealTextBox.Clear();
            }

            base.OnApplyTemplate();
            passwordBox = GetTemplateChild(PasswordBoxPartName) as PasswordBox;
            revealTextBox = GetTemplateChild(RevealTextBoxPartName) as TextBox;

            if (passwordBox != null)
            {
                passwordBox.PasswordChanged += OnPasswordChanged;
                HasPassword = passwordBox.Password.Length > 0;
            }

            if (revealTextBox != null)
            {
                revealTextBox.TextChanged += OnRevealTextChanged;
            }

            UpdateRevealState(false);
        }

        /// <summary>
        /// 获取当前密码的安全副本。调用方负责在使用后释放返回值。
        /// </summary>
        [Browsable(false)]
        public SecureString SecurePassword
        {
            get { return passwordBox == null ? new SecureString() : passwordBox.SecurePassword.Copy(); }
        }

        /// <summary>
        /// 当密码内容发生变化时发生。事件参数不会携带密码明文。
        /// </summary>
        public event RoutedEventHandler PasswordChanged
        {
            add { AddHandler(PasswordChangedEvent, value); }
            remove { RemoveHandler(PasswordChangedEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="PasswordChanged"/> 路由事件。
        /// </summary>
        public static readonly RoutedEvent PasswordChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(PasswordChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ZenPasswordBox));

        /// <summary>
        /// 清除密码内容。
        /// </summary>
        public void Clear()
        {
            passwordBox?.Clear();
            HasPassword = false;
        }

        /// <summary>
        /// 获取或设置是否显示密码明文切换按钮。按钮的可见性不依赖密码是否为空。
        /// </summary>
        [Bindable(true)]
        public bool IsPasswordRevealEnabled
        {
            get { return (bool)GetValue(IsPasswordRevealEnabledProperty); }
            set { SetValue(IsPasswordRevealEnabledProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsPasswordRevealEnabled"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsPasswordRevealEnabledProperty =
            DependencyProperty.Register(
                nameof(IsPasswordRevealEnabled),
                typeof(bool),
                typeof(ZenPasswordBox),
                new FrameworkPropertyMetadata(false, OnIsPasswordRevealEnabledChanged));

        /// <summary>
        /// 获取或设置当前是否以明文显示密码。
        /// </summary>
        [Bindable(true)]
        public bool IsPasswordRevealed
        {
            get { return (bool)GetValue(IsPasswordRevealedProperty); }
            set { SetValue(IsPasswordRevealedProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsPasswordRevealed"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsPasswordRevealedProperty =
            DependencyProperty.Register(
                nameof(IsPasswordRevealed),
                typeof(bool),
                typeof(ZenPasswordBox),
                new FrameworkPropertyMetadata(false, OnIsPasswordRevealedChanged, CoerceIsPasswordRevealed));

        /// <summary>
        /// 获取或设置密码框没有内容时显示的水印。
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
            DependencyProperty.Register(nameof(Watermark), typeof(string), typeof(ZenPasswordBox), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置显示在密码输入区域之前的内容。
        /// </summary>
        [Bindable(true)]
        public object LeadingContent
        {
            get { return GetValue(LeadingContentProperty); }
            set { SetValue(LeadingContentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="LeadingContent"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty LeadingContentProperty =
            DependencyProperty.Register(
                nameof(LeadingContent),
                typeof(object),
                typeof(ZenPasswordBox),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置用于显示 <see cref="LeadingContent"/> 的数据模板。
        /// </summary>
        [Bindable(true)]
        public DataTemplate LeadingContentTemplate
        {
            get { return (DataTemplate)GetValue(LeadingContentTemplateProperty); }
            set { SetValue(LeadingContentTemplateProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="LeadingContentTemplate"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty LeadingContentTemplateProperty =
            DependencyProperty.Register(
                nameof(LeadingContentTemplate),
                typeof(DataTemplate),
                typeof(ZenPasswordBox),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置显示在密码输入区域和显隐按钮之间的内容。
        /// </summary>
        [Bindable(true)]
        public object TrailingContent
        {
            get { return GetValue(TrailingContentProperty); }
            set { SetValue(TrailingContentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TrailingContent"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TrailingContentProperty =
            DependencyProperty.Register(
                nameof(TrailingContent),
                typeof(object),
                typeof(ZenPasswordBox),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置用于显示 <see cref="TrailingContent"/> 的数据模板。
        /// </summary>
        [Bindable(true)]
        public DataTemplate TrailingContentTemplate
        {
            get { return (DataTemplate)GetValue(TrailingContentTemplateProperty); }
            set { SetValue(TrailingContentTemplateProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TrailingContentTemplate"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TrailingContentTemplateProperty =
            DependencyProperty.Register(
                nameof(TrailingContentTemplate),
                typeof(DataTemplate),
                typeof(ZenPasswordBox),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 获取或设置密码框的圆角。
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
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ZenPasswordBox), new FrameworkPropertyMetadata(new CornerRadius(6)));

        internal bool HasPassword
        {
            get { return (bool)GetValue(HasPasswordProperty); }
            private set { SetValue(HasPasswordPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey HasPasswordPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasPassword), typeof(bool), typeof(ZenPasswordBox), new FrameworkPropertyMetadata(false));

        internal static readonly DependencyProperty HasPasswordProperty = HasPasswordPropertyKey.DependencyProperty;

        private static void OnIsPasswordRevealEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ZenPasswordBox)d;
            control.CoerceValue(IsPasswordRevealedProperty);
        }

        private static object CoerceIsPasswordRevealed(DependencyObject d, object baseValue)
        {
            var control = (ZenPasswordBox)d;
            return control.IsPasswordRevealEnabled && (bool)baseValue;
        }

        private static void OnIsPasswordRevealedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ZenPasswordBox)d;
            control.UpdateRevealState(control.IsKeyboardFocusWithin && control.IsEnabled);
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (isSynchronizingPassword)
            {
                return;
            }

            isSynchronizingPassword = true;
            if (IsPasswordRevealed && revealTextBox != null)
            {
                revealTextBox.Text = passwordBox.Password;
            }

            isSynchronizingPassword = false;
            HasPassword = passwordBox.Password.Length > 0;
            RaiseEvent(new RoutedEventArgs(PasswordChangedEvent, this));
        }

        private void OnRevealTextChanged(object sender, TextChangedEventArgs e)
        {
            if (isSynchronizingPassword || passwordBox == null || !IsPasswordRevealed)
            {
                return;
            }

            isSynchronizingPassword = true;
            passwordBox.Password = revealTextBox.Text;
            isSynchronizingPassword = false;
            HasPassword = revealTextBox.Text.Length > 0;
            RaiseEvent(new RoutedEventArgs(PasswordChangedEvent, this));
        }

        private void UpdateRevealState(bool moveFocus)
        {
            if (revealTextBox == null)
            {
                return;
            }

            isSynchronizingPassword = true;
            if (IsPasswordRevealed)
            {
                revealTextBox.Text = passwordBox?.Password ?? string.Empty;
            }
            else
            {
                revealTextBox.Clear();
            }

            isSynchronizingPassword = false;

            if (!moveFocus)
            {
                return;
            }

            if (IsPasswordRevealed)
            {
                revealTextBox.Focus();
                revealTextBox.CaretIndex = revealTextBox.Text.Length;
            }
            else if (passwordBox != null)
            {
                passwordBox.Focus();
            }
        }

        /// <inheritdoc/>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            if (!IsKeyboardFocusWithin && IsPasswordRevealed)
            {
                SetCurrentValue(IsPasswordRevealedProperty, false);
            }
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsEnabledProperty && !(bool)e.NewValue && IsPasswordRevealed)
            {
                SetCurrentValue(IsPasswordRevealedProperty, false);
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ZenPasswordBoxAutomationPeer(this);
        }

        private sealed class ZenPasswordBoxAutomationPeer : FrameworkElementAutomationPeer
        {
            public ZenPasswordBoxAutomationPeer(ZenPasswordBox owner)
                : base(owner)
            {
            }

            protected override string GetClassNameCore()
            {
                return nameof(ZenPasswordBox);
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Edit;
            }

            protected override bool IsPasswordCore()
            {
                return true;
            }
        }
    }
}
