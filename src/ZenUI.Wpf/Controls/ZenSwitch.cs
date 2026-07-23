using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示具有开关外观的双态选择控件。
    /// </summary>
    public class ZenSwitch : ToggleButton
    {
        static ZenSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenSwitch),
                new FrameworkPropertyMetadata(typeof(ZenSwitch)));
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ZenSwitchAutomationPeer(this);
        }

        private sealed class ZenSwitchAutomationPeer : ToggleButtonAutomationPeer
        {
            public ZenSwitchAutomationPeer(ZenSwitch owner)
                : base(owner)
            {
            }

            protected override string GetClassNameCore()
            {
                return nameof(ZenSwitch);
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.CheckBox;
            }
        }
    }
}
