using System.Windows;
using System.Windows.Controls;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class CheckBoxView : UserControl
    {
        private bool isSynchronizingNotifications;

        public CheckBoxView() { InitializeComponent(); }

        private void SelectAllNotifications_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (isSynchronizingNotifications || !SelectAllNotifications.IsChecked.HasValue)
            {
                return;
            }

            isSynchronizingNotifications = true;
            var isChecked = SelectAllNotifications.IsChecked.Value;
            ProductUpdates.IsChecked = isChecked;
            SecurityAlerts.IsChecked = isChecked;
            EventRecommendations.IsChecked = isChecked;
            isSynchronizingNotifications = false;
        }

        private void NotificationOption_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (isSynchronizingNotifications ||
                ProductUpdates == null ||
                SecurityAlerts == null ||
                EventRecommendations == null)
            {
                return;
            }

            isSynchronizingNotifications = true;

            var selectedCount = (ProductUpdates.IsChecked == true ? 1 : 0) +
                                (SecurityAlerts.IsChecked == true ? 1 : 0) +
                                (EventRecommendations.IsChecked == true ? 1 : 0);

            SelectAllNotifications.IsChecked = selectedCount == 0
                ? false
                : selectedCount == 3
                    ? true
                    : (bool?)null;

            isSynchronizingNotifications = false;
        }
    }
}
