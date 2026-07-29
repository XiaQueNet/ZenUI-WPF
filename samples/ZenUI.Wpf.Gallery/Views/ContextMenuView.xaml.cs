using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class ContextMenuView : UserControl
    {
        private const string WebsiteUrl = "https://zenui.mnorg.cn/";

        public ContextMenuView()
        {
            InitializeComponent();
        }

        private void OpenWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = WebsiteUrl,
                UseShellExecute = true
            });
        }

        private void CopyLink_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(WebsiteUrl);
            ActionFeedback.Visibility = Visibility.Visible;
        }

        private void OpenButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var menu = button.ContextMenu;
            menu.PlacementTarget = button;
            menu.Placement = PlacementMode.Bottom;
            menu.VerticalOffset = 4;
            menu.IsOpen = true;
        }
    }
}
