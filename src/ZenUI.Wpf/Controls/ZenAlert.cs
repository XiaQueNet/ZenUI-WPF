using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示用于展示简短状态信息的提示条。
    /// </summary>
    public class ZenAlert : ContentControl
    {
        static ZenAlert()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenAlert),
                new FrameworkPropertyMetadata(typeof(ZenAlert)));
            AutomationProperties.LiveSettingProperty.OverrideMetadata(
                typeof(ZenAlert),
                new FrameworkPropertyMetadata(AutomationLiveSetting.Polite));
        }

        /// <summary>
        /// 获取或设置提示条的语义外观。
        /// </summary>
        [Bindable(true)]
        public AlertVariant Variant
        {
            get { return (AlertVariant)GetValue(VariantProperty); }
            set { SetValue(VariantProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Variant"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty VariantProperty =
            DependencyProperty.Register(
                nameof(Variant),
                typeof(AlertVariant),
                typeof(ZenAlert),
                new FrameworkPropertyMetadata(AlertVariant.Info));

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
