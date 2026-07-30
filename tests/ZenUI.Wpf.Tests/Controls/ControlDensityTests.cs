using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class ControlDensityTests
    {
        [TestMethod]
        public void DensitySwitchUpdatesRealizedControlMetrics()
        {
            var textBox = new ZenTextBox();
            var button = new ZenButton { Content = "Action" };
            var listBox = new ZenListBox { Height = 100 };
            listBox.Items.Add("Item");
            var scrollBar = new ScrollBar
            {
                Height = 100,
                Orientation = Orientation.Vertical
            };
            var @switch = new ZenSwitch();
            var slider = new ZenSlider();
            var progressBar = new ZenProgressBar();
            var alert = new ZenAlert { Content = "Status" };

            var panel = new StackPanel();
            panel.Children.Add(textBox);
            panel.Children.Add(button);
            panel.Children.Add(listBox);
            panel.Children.Add(scrollBar);
            panel.Children.Add(@switch);
            panel.Children.Add(slider);
            panel.Children.Add(progressBar);
            panel.Children.Add(alert);

            var window = CreateTestWindow(panel, 320, 520);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });

            try
            {
                window.Show();
                window.UpdateLayout();

                var item = listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                var sliderTrack = slider.Template.FindName("PART_Track", slider) as Track;
                var switchThumb = FindVisualDescendant<Ellipse>(@switch);
                Assert.IsNotNull(item);
                Assert.IsNotNull(sliderTrack);
                Assert.IsNotNull(sliderTrack.Thumb);
                Assert.IsNotNull(sliderTrack.DecreaseRepeatButton);
                Assert.IsNotNull(switchThumb);
                Assert.AreEqual(36d, textBox.MinHeight);
                Assert.AreEqual(new Thickness(10, 4, 10, 4), button.Padding);
                Assert.AreEqual(new Thickness(12, 9, 12, 9), item.Padding);
                Assert.AreEqual(12d, scrollBar.Width);
                Assert.AreEqual(60d, @switch.Width);
                Assert.AreEqual(30d, @switch.Height);
                Assert.AreEqual(new Thickness(4), switchThumb.Margin);
                Assert.AreEqual(24d, slider.MinHeight);
                Assert.AreEqual(4d, slider.TrackThickness);
                Assert.AreEqual(18d, sliderTrack.Thumb.Width);
                Assert.AreEqual(4d, sliderTrack.DecreaseRepeatButton.Height);
                Assert.AreEqual(8d, progressBar.Height);
                Assert.AreEqual(new Thickness(14, 11, 14, 11), alert.Padding);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.UpdateLayout();

                Assert.AreEqual(32d, textBox.MinHeight);
                Assert.AreEqual(new Thickness(8, 3, 8, 3), button.Padding);
                Assert.AreEqual(new Thickness(10, 6, 10, 6), item.Padding);
                Assert.AreEqual(10d, scrollBar.Width);
                Assert.AreEqual(52d, @switch.Width);
                Assert.AreEqual(26d, @switch.Height);
                Assert.AreEqual(new Thickness(3), switchThumb.Margin);
                Assert.AreEqual(20d, slider.MinHeight);
                Assert.AreEqual(3d, slider.TrackThickness);
                Assert.AreEqual(16d, sliderTrack.Thumb.Width);
                Assert.AreEqual(3d, sliderTrack.DecreaseRepeatButton.Height);
                Assert.AreEqual(6d, progressBar.Height);
                Assert.AreEqual(new Thickness(12, 8, 12, 8), alert.Padding);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Comfortable);
                window.UpdateLayout();

                Assert.AreEqual(40d, textBox.MinHeight);
                Assert.AreEqual(new Thickness(12, 6, 12, 6), button.Padding);
                Assert.AreEqual(new Thickness(14, 11, 14, 11), item.Padding);
                Assert.AreEqual(14d, scrollBar.Width);
                Assert.AreEqual(68d, @switch.Width);
                Assert.AreEqual(34d, @switch.Height);
                Assert.AreEqual(new Thickness(4), switchThumb.Margin);
                Assert.AreEqual(28d, slider.MinHeight);
                Assert.AreEqual(6d, slider.TrackThickness);
                Assert.AreEqual(22d, sliderTrack.Thumb.Width);
                Assert.AreEqual(6d, sliderTrack.DecreaseRepeatButton.Height);
                Assert.AreEqual(10d, progressBar.Height);
                Assert.AreEqual(new Thickness(16, 14, 16, 14), alert.Padding);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DensitySwitchUpdatesDataGridAndCalendarMetrics()
        {
            var dataGrid = new ZenDataGrid
            {
                Height = 120,
                AutoGenerateColumns = false,
                ItemsSource = new[] { new EditableRow(1, "Member") }
            };
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Name",
                Binding = new Binding(nameof(EditableRow.Name))
            });
            var datePicker = new ZenDatePicker
            {
                SelectedDate = new DateTime(2026, 7, 23)
            };
            var panel = new StackPanel();
            panel.Children.Add(dataGrid);
            panel.Children.Add(datePicker);

            var window = CreateTestWindow(panel, 420, 520);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });

            try
            {
                window.Show();
                datePicker.IsDropDownOpen = true;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.UpdateLayout();

                var columnHeader = FindVisualDescendants<DataGridColumnHeader>(dataGrid)
                    .FirstOrDefault(header => header.Column != null);
                var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                var cell = FindVisualDescendant<DataGridCell>(row);
                var popup = datePicker.Template.FindName("PART_Popup", datePicker) as Popup;
                var popupContainer = popup?.Child as FrameworkElement;
                var calendar = GetDatePickerCalendar(datePicker);
                Assert.IsNotNull(columnHeader);
                Assert.IsNotNull(row);
                Assert.IsNotNull(cell);
                Assert.IsNotNull(popupContainer);
                Assert.IsNotNull(calendar);
                calendar.ApplyTemplate();
                var calendarItem = calendar.Template.FindName("PART_CalendarItem", calendar) as CalendarItem;
                Assert.IsNotNull(calendarItem);
                calendarItem.ApplyTemplate();
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.UpdateLayout();
                var monthView = calendarItem.Template.FindName("PART_MonthView", calendarItem) as Grid;
                var yearView = calendarItem.Template.FindName("PART_YearView", calendarItem) as Grid;
                var navigationButton = calendarItem.Template.FindName("PART_PreviousButton", calendarItem) as Button;
                var dayButton = monthView?.Children.OfType<CalendarDayButton>()
                    .FirstOrDefault(button =>
                        button.Visibility == Visibility.Visible &&
                        ReferenceEquals(button.Style, calendar.CalendarDayButtonStyle));
                var monthButton = yearView?.Children.OfType<CalendarButton>().FirstOrDefault();
                Assert.IsNotNull(dayButton);
                Assert.IsNotNull(monthButton);
                Assert.IsNotNull(navigationButton);
                Assert.AreEqual(44d, columnHeader.Height);
                Assert.AreEqual(44d, row.MinHeight);
                Assert.AreEqual(new Thickness(14, 0, 14, 0), cell.Padding);
                Assert.AreEqual(368d, popupContainer.ActualWidth);
                Assert.AreEqual(376d, popupContainer.ActualHeight);
                Assert.AreEqual(16d, calendar.FontSize);
                Assert.IsTrue(double.IsNaN(dayButton.Width));
                Assert.IsTrue(double.IsNaN(dayButton.Height));
                Assert.IsGreaterThan(0d, dayButton.ActualWidth);
                Assert.IsGreaterThan(0d, dayButton.ActualHeight);
                Assert.AreEqual(new Thickness(12, 16, 12, 16), monthButton.Padding);
                Assert.AreEqual(40d, navigationButton.Width);
                AssertCalendarDayButtonsFit(monthView);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.UpdateLayout();

                Assert.AreEqual(36d, columnHeader.Height);
                Assert.AreEqual(36d, row.MinHeight);
                Assert.AreEqual(new Thickness(10, 0, 10, 0), cell.Padding);
                Assert.AreEqual(328d, popupContainer.ActualWidth);
                Assert.AreEqual(348d, popupContainer.ActualHeight);
                Assert.IsTrue(double.IsNaN(dayButton.Width));
                Assert.IsTrue(double.IsNaN(dayButton.Height));
                Assert.AreEqual(new Thickness(10, 13, 10, 13), monthButton.Padding);
                Assert.AreEqual(36d, navigationButton.Width);
                AssertCalendarDayButtonsFit(monthView);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Comfortable);
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.UpdateLayout();

                Assert.AreEqual(52d, columnHeader.Height);
                Assert.AreEqual(52d, row.MinHeight);
                Assert.AreEqual(new Thickness(18, 0, 18, 0), cell.Padding);
                Assert.AreEqual(412d, popupContainer.ActualWidth);
                Assert.AreEqual(416d, popupContainer.ActualHeight);
                Assert.IsTrue(double.IsNaN(dayButton.Width));
                Assert.IsTrue(double.IsNaN(dayButton.Height));
                Assert.AreEqual(new Thickness(14, 18, 14, 18), monthButton.Padding);
                Assert.AreEqual(44d, navigationButton.Width);
                AssertCalendarDayButtonsFit(monthView);
            }
            finally
            {
                datePicker.IsDropDownOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        private static void AssertCalendarDayButtonsFit(Grid monthView)
        {
            const double tolerance = 0.01d;

            foreach (var button in monthView.Children
                .OfType<CalendarDayButton>()
                .Where(button => button.Visibility == Visibility.Visible))
            {
                var position = button.TranslatePoint(new Point(0, 0), monthView);
                Assert.IsTrue(position.X >= -tolerance, "日期按钮超出月份视图左边界。");
                Assert.IsTrue(position.Y >= -tolerance, "日期按钮超出月份视图上边界。");
                Assert.IsTrue(
                    position.X + button.ActualWidth <= monthView.ActualWidth + tolerance,
                    "日期按钮超出月份视图右边界。");
                Assert.IsTrue(
                    position.Y + button.ActualHeight <= monthView.ActualHeight + tolerance,
                    "日期按钮超出月份视图下边界。");
            }
        }
    }
}
