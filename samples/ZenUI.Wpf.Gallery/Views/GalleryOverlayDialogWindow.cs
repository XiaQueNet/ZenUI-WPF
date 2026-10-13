using System.Windows;
using System.Windows.Data;

using Prism.Services.Dialogs;

using ZenUI.Wpf.Controls;

namespace ZenUI.Wpf.Gallery.Views
{
    public sealed class GalleryOverlayDialogWindow : ZenOverlayDialogWindow, IDialogWindow
    {
        public GalleryOverlayDialogWindow()
        {
            SetBinding(TitleProperty, new Binding("Title"));
            Style = (Style)Application.Current.FindResource("ZenOverlayDialogWindowStyle");
        }

        public IDialogResult Result { get; set; }
    }
}
