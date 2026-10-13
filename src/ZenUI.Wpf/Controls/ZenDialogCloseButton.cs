using System.Windows;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 关闭承载它的对话框窗口的标题栏按钮。
    /// </summary>
    public class ZenDialogCloseButton : ZenButton
    {
        /// <summary>
        /// 触发按钮操作并关闭所在窗口。
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();
            Window.GetWindow(this)?.Close();
        }
    }
}
