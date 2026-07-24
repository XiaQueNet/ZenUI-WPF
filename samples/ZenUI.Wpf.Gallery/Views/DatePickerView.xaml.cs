using System;
using System.Windows.Controls;

namespace ZenUI.Wpf.Gallery.Views
{
    public partial class DatePickerView : UserControl
    {
        public DatePickerView()
        {
            InitializeComponent();

            var today = DateTime.Today;
            SelectedDatePicker.SelectedDate = today;
            ShortDatePicker.SelectedDate = today;
            LongDatePicker.SelectedDate = today;

            ConstrainedDatePicker.DisplayDateStart = today;
            ConstrainedDatePicker.DisplayDateEnd = today.AddDays(30);
            for (var date = today; date <= today.AddDays(30); date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    ConstrainedDatePicker.BlackoutDates.Add(new CalendarDateRange(date));
                }
            }
        }
    }
}
