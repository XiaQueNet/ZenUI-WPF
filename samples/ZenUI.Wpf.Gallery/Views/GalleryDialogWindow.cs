using System.Windows;
using System.Windows.Data;

using Prism.Services.Dialogs;

namespace ZenUI.Wpf.Gallery.Views
{
    /// <summary>
    /// 为 Gallery 的 Prism 弹窗统一应用 ZenUI 窗口外观。
    /// </summary>
    public class GalleryDialogWindow : Window, IDialogWindow
    {
        public GalleryDialogWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SetBinding(TitleProperty, new Binding("Title"));
            Style = (Style)Application.Current.FindResource("ZenDialogWindowStyle");
        }

        public IDialogResult Result { get; set; }
    }
}
