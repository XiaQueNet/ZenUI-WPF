using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class TextBoxView : UserControl
    {
        public TextBoxView() { InitializeComponent(); }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchInput.Clear();
            SearchInput.Focus();
        }
    }
}
