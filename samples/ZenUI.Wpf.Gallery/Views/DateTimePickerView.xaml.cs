using System;
using System.Windows.Controls;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class DateTimePickerView : UserControl
    {
        public DateTimePickerView()
        {
            InitializeComponent();

            var sample = new DateTime(2026, 8, 3, 14, 30, 25);
            SelectedDateTimePicker.SelectedDateTime = sample;
            TwentyFourHourPicker.SelectedDateTime = sample;
            TwelveHourPicker.SelectedDateTime = sample;
            CustomFormatPicker.SelectedDateTime = sample;

            var today = DateTime.Today;
            ConstrainedDateTimePicker.Minimum = today.AddHours(9);
            ConstrainedDateTimePicker.Maximum = today.AddDays(7).AddHours(18);
        }
    }
}
