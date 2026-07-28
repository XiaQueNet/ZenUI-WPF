using System;
using System.Windows.Controls;

namespace ZenUI.Wpf.Gallery.Views
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
        Cancelled
    }

    public partial class ComboBoxView : UserControl
    {
        public Array OrderStatuses { get; } = Enum.GetValues(typeof(OrderStatus));

        public ComboBoxView() { InitializeComponent(); }
    }
}
