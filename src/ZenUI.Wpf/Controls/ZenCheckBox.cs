using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示使用 ZenUI 外观的复选框。
    /// </summary>
    public class ZenCheckBox : CheckBox
    {
        static ZenCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenCheckBox),
                new FrameworkPropertyMetadata(typeof(ZenCheckBox)));
        }
    }
}
