using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 表示右键菜单中的命令、勾选项或级联菜单项。
    /// </summary>
    public class ZenMenuItem : MenuItem
    {
        static ZenMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZenMenuItem),
                new FrameworkPropertyMetadata(typeof(ZenMenuItem)));
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MenuItem || item is Separator;
        }

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ZenMenuItem();
        }
    }
}
