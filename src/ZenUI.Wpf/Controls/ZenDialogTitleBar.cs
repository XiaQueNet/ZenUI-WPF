using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 为无系统边框的对话框提供可拖动的标题区域。
    /// </summary>
    public class ZenDialogTitleBar : Grid
    {
        /// <summary>
        /// 按住标题空白区域时移动所在窗口。
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled || e.ClickCount != 1 || IsInsideButton(e.OriginalSource as DependencyObject))
            {
                return;
            }

            var window = Window.GetWindow(this);
            if (window != null && ZenDialogWindowOptions.GetIsDraggable(window))
            {
                window.DragMove();
            }
        }

        private bool IsInsideButton(DependencyObject source)
        {
            while (source != null && source != this)
            {
                if (source is ButtonBase)
                {
                    return true;
                }

                source = VisualTreeHelper.GetParent(source);
            }

            return false;
        }
    }
}
