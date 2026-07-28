using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示用于展示简短状态信息的提示条。
    /// </summary>
    public class ZenAlert : ContentControl
    {
        private static readonly System.Type SelfType = typeof(ZenAlert);

        static ZenAlert()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(SelfType));
            AutomationProperties.LiveSettingProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(AutomationLiveSetting.Polite));
        }

        /// <summary>
        /// 获取或设置提示图标的强调画刷。
        /// </summary>
        [Bindable(true)]
        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        public static readonly DependencyProperty AccentBrushProperty =
            RegisterBrush(nameof(AccentBrush));

        /// <summary>
        /// 获取或设置提示图标的前景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush IconForeground
        {
            get { return (Brush)GetValue(IconForegroundProperty); }
            set { SetValue(IconForegroundProperty, value); }
        }

        public static readonly DependencyProperty IconForegroundProperty =
            RegisterBrush(nameof(IconForeground));

        /// <summary>
        /// 获取或设置提示图标的边长。
        /// </summary>
        [Bindable(true)]
        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IconSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(
                nameof(IconSize),
                typeof(double),
                SelfType,
                new FrameworkPropertyMetadata(
                    18d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidIconSize);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ThemeBackground
        {
            get { return (Brush)GetValue(ThemeBackgroundProperty); }
            set { SetValue(ThemeBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ThemeBackgroundProperty =
            RegisterBrush(nameof(ThemeBackground));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ThemeBorderBrush
        {
            get { return (Brush)GetValue(ThemeBorderBrushProperty); }
            set { SetValue(ThemeBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty ThemeBorderBrushProperty =
            RegisterBrush(nameof(ThemeBorderBrush));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ThemeAccentBrush
        {
            get { return (Brush)GetValue(ThemeAccentBrushProperty); }
            set { SetValue(ThemeAccentBrushProperty, value); }
        }

        public static readonly DependencyProperty ThemeAccentBrushProperty =
            RegisterBrush(nameof(ThemeAccentBrush));

        /// <summary>
        /// 获取或设置提示条所传达信息的严重级别。
        /// </summary>
        [Bindable(true)]
        public AlertSeverity Severity
        {
            get { return (AlertSeverity)GetValue(SeverityProperty); }
            set { SetValue(SeverityProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Severity"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty SeverityProperty =
            DependencyProperty.Register(
                nameof(Severity),
                typeof(AlertSeverity),
                SelfType,
                new FrameworkPropertyMetadata(AlertSeverity.Info));

        private static DependencyProperty RegisterBrush(string name)
        {
            return DependencyProperty.Register(
                name,
                typeof(Brush),
                SelfType,
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));
        }

        private static bool IsValidIconSize(object value)
        {
            var size = (double)value;
            return !double.IsNaN(size) &&
                !double.IsInfinity(size) &&
                size >= 0d;
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ZenAlertAutomationPeer(this);
        }

        private sealed class ZenAlertAutomationPeer : FrameworkElementAutomationPeer
        {
            public ZenAlertAutomationPeer(ZenAlert owner)
                : base(owner)
            {
            }

            protected override string GetClassNameCore()
            {
                return nameof(ZenAlert);
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Text;
            }

            protected override string GetNameCore()
            {
                var name = base.GetNameCore();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }

                return ((ZenAlert)Owner).Content?.ToString() ?? string.Empty;
            }
        }
    }
}
