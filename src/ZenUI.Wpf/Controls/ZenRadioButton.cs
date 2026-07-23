using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示使用 ZenUI 外观的单选按钮。
    /// </summary>
    public class ZenRadioButton : RadioButton
    {
        static ZenRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenRadioButton),
                new FrameworkPropertyMetadata(typeof(ZenRadioButton)));
        }
    }
}
