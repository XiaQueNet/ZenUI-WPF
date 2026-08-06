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
#if ZENUI_LIVE_REGIONS
            AutomationProperties.LiveSettingProperty.OverrideMetadata(
                SelfType,
                new FrameworkPropertyMetadata(AutomationLiveSetting.Polite));
#endif
        }

        /// <summary>
        /// 获取或设置提示图标的背景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush IconBackground
        {
            get { return (Brush)GetValue(IconBackgroundProperty); }
            set { SetValue(IconBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IconBackground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IconBackgroundProperty =
            RegisterBrush(nameof(IconBackground));

        /// <summary>
        /// 获取或设置提示图标的前景画刷。
        /// </summary>
        [Bindable(true)]
        public Brush IconForeground
        {
            get { return (Brush)GetValue(IconForegroundProperty); }
            set { SetValue(IconForegroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IconForeground"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IconForegroundProperty =
            RegisterBrush(nameof(IconForeground));

        /// <summary>
        /// 获取或设置提示图标的边长。该值必须为大于或等于零的有限值。
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

        /// <summary>
        /// 获取或设置由提示级别提供的默认背景画刷。
        /// </summary>
        internal Brush ThemeBackground
        {
            get { return (Brush)GetValue(ThemeBackgroundProperty); }
            set { SetValue(ThemeBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeBackground"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemeBackgroundProperty =
            RegisterBrush(nameof(ThemeBackground));

        /// <summary>
        /// 获取或设置由提示级别提供的默认边框画刷。
        /// </summary>
        internal Brush ThemeBorderBrush
        {
            get { return (Brush)GetValue(ThemeBorderBrushProperty); }
            set { SetValue(ThemeBorderBrushProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeBorderBrush"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemeBorderBrushProperty =
            RegisterBrush(nameof(ThemeBorderBrush));

        /// <summary>
        /// 获取或设置由提示级别提供的默认图标背景画刷。
        /// </summary>
        internal Brush ThemeIconBackground
        {
            get { return (Brush)GetValue(ThemeIconBackgroundProperty); }
            set { SetValue(ThemeIconBackgroundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ThemeIconBackground"/> 依赖属性。
        /// </summary>
        internal static readonly DependencyProperty ThemeIconBackgroundProperty =
            RegisterBrush(nameof(ThemeIconBackground));

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
