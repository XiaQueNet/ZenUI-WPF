using System;
using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示支持水印和自定义圆角的密码输入控件。
    /// </summary>
    [TemplatePart(Name = PasswordBoxPartName, Type = typeof(PasswordBox))]
    public class ZenPasswordBox : Control
    {
        private const string PasswordBoxPartName = "PART_PasswordBox";
        private PasswordBox passwordBox;
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

            base.OnApplyTemplate();
            passwordBox = GetTemplateChild(PasswordBoxPartName) as PasswordBox;

            if (passwordBox != null)
            {
                if (EnableInsecurePasswordBinding)
                {
                    passwordBox.Password = (string)GetValue(PasswordProperty) ?? string.Empty;
                }

                passwordBox.PasswordChanged += OnPasswordChanged;
                HasPassword = passwordBox.Password.Length > 0;
            }
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
            if (EnableInsecurePasswordBinding)
            {
                SetCurrentValue(PasswordProperty, string.Empty);
            }

            HasPassword = false;
        }

        /// <summary>
        /// 获取或设置密码框中的密码。
        /// </summary>
        /// <remarks>启用此属性会在托管内存和 WPF 属性系统中保存密码明文。</remarks>
        [Obsolete("明文 Password 绑定不安全。请使用 PasswordChanged 和 SecurePassword；如需兼容旧代码，请显式设置 EnableInsecurePasswordBinding=true。")]
        [Bindable(true)]
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Password"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                "Password",
                typeof(string),
                typeof(ZenPasswordBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

        /// <summary>
        /// 获取或设置是否启用旧版明文 <see cref="Password"/> 双向绑定。默认关闭。
        /// </summary>
        public bool EnableInsecurePasswordBinding
        {
            get { return (bool)GetValue(EnableInsecurePasswordBindingProperty); }
            set { SetValue(EnableInsecurePasswordBindingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="EnableInsecurePasswordBinding"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty EnableInsecurePasswordBindingProperty =
            DependencyProperty.Register(
                nameof(EnableInsecurePasswordBinding),
                typeof(bool),
                typeof(ZenPasswordBox),
                new FrameworkPropertyMetadata(false, OnEnableInsecurePasswordBindingChanged));

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

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ZenPasswordBox)d;
            if (!control.EnableInsecurePasswordBinding || control.passwordBox == null || control.isSynchronizingPassword)
            {
                return;
            }

            control.passwordBox.Password = (string)e.NewValue ?? string.Empty;
            control.HasPassword = control.passwordBox.Password.Length > 0;
        }

        private static void OnEnableInsecurePasswordBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ZenPasswordBox)d;
            if ((bool)e.NewValue)
            {
                if (control.passwordBox != null)
                {
                    control.passwordBox.Password = (string)control.GetValue(PasswordProperty) ?? string.Empty;
                    control.HasPassword = control.passwordBox.Password.Length > 0;
                }

                return;
            }

            control.SetCurrentValue(PasswordProperty, string.Empty);
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            isSynchronizingPassword = true;
            if (EnableInsecurePasswordBinding)
            {
                SetCurrentValue(PasswordProperty, passwordBox.Password);
            }
            isSynchronizingPassword = false;
            HasPassword = passwordBox.Password.Length > 0;
            RaiseEvent(new RoutedEventArgs(PasswordChangedEvent, this));
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
