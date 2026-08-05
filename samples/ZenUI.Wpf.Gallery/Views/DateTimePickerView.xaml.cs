using System;
using System.Globalization;
using System.Windows;
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
            MondayFirstPicker.SelectedDateTime = sample;
            WithoutTodayHighlightPicker.SelectedDateTime = sample;
            NaturalMetricsPicker.SelectedDateTime = sample;

            var today = DateTime.Today;
            ConstrainedDateTimePicker.Minimum = today.AddHours(9);
            ConstrainedDateTimePicker.Maximum = today.AddDays(7).AddHours(18);
        }

        private void DateTimePicker_OnSelectedDateTimeChanged(
            object sender,
            RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            DateTimeChangedStatus.Text = e.NewValue.HasValue
                ? "已选择：" + e.NewValue.Value.ToString(
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.CurrentCulture)
                : "选择已清除。";
        }
    }
}
