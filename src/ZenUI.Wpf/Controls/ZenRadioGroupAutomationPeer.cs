using System.Windows.Automation.Peers;

namespace ZenUI.Wpf.Controls
{
    internal sealed class ZenRadioGroupAutomationPeer : SelectorAutomationPeer
    {
        internal ZenRadioGroupAutomationPeer(ZenRadioGroup owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return nameof(ZenRadioGroup);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new ZenRadioItemAutomationPeer(item, this);
        }
    }

    internal sealed class ZenRadioItemAutomationPeer : SelectorItemAutomationPeer
    {
        internal ZenRadioItemAutomationPeer(
            object item,
            SelectorAutomationPeer selectorAutomationPeer)
            : base(item, selectorAutomationPeer)
        {
        }

        protected override string GetClassNameCore()
        {
            return nameof(ZenRadioItem);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.RadioButton;
        }
    }
}
