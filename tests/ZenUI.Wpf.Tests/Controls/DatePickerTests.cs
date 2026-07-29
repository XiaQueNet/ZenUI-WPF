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
    public class DatePickerTests
    {
        [TestMethod]
        public void DatePickerTemplateOpensThemedCalendar()
        {
            var datePicker = new ZenDatePicker
            {
                Watermark = "请选择日期",
                SelectedDate = new DateTime(2026, 7, 23)
            };
            var window = CreateTestWindow(datePicker, 260, 320);

            try
            {
                window.Show();
                datePicker.IsDropDownOpen = true;
                window.UpdateLayout();

                var popup = datePicker.Template.FindName("PART_Popup", datePicker) as Popup;
                Assert.IsNotNull(popup);
                Assert.IsTrue(popup.IsOpen);
                var popupContainer = popup.Child as FrameworkElement;
                Assert.IsNotNull(popupContainer);
                var calendar = GetDatePickerCalendar(datePicker);
                Assert.IsNotNull(calendar);
                Assert.IsNotNull(calendar.Style);
                Assert.AreEqual(368d, datePicker.CalendarPopupWidth);
                Assert.AreEqual(368d, popupContainer.Width);
                Assert.AreEqual(368d, popupContainer.ActualWidth);
                Assert.AreEqual(376d, popupContainer.ActualHeight);
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.UpdateLayout();
                Assert.AreEqual(16d, calendar.FontSize);
                Assert.AreEqual(datePicker.SelectedDate, calendar.SelectedDate);
                calendar.ApplyTemplate();
                var calendarItem =
                    calendar.Template.FindName("PART_CalendarItem", calendar) as CalendarItem;
                Assert.IsNotNull(calendarItem);
                calendarItem.ApplyTemplate();
                window.UpdateLayout();
                var root = calendarItem.Template.FindName("PART_Root", calendarItem) as Grid;
                var monthView =
                    calendarItem.Template.FindName("PART_MonthView", calendarItem) as Grid;
                Assert.IsNotNull(root);
                Assert.IsNotNull(monthView);
                Assert.AreEqual(
                    GridUnitType.Auto,
                    root.RowDefinitions[0].Height.GridUnitType);
                Assert.AreEqual(
                    GridUnitType.Star,
                    monthView.RowDefinitions[0].Height.GridUnitType);
                Assert.AreEqual(
                    7,
                    monthView.Children
                        .OfType<FrameworkElement>()
                        .Count(element =>
                            Grid.GetRow(element) == 0 &&
                            element.Visibility == Visibility.Visible));
            }
            finally
            {
                datePicker.IsDropDownOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void DatePickerCalendarStyleOverrideCrossesPopupBoundary()
        {
            var datePicker = new ZenDatePicker();
            var window = CreateTestWindow(datePicker, 320, 360);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });

            var calendarBackground = new SolidColorBrush(Colors.MistyRose);
            var dayPadding = new Thickness(7);
            var dayStyle = new Style(
                typeof(CalendarDayButton),
                (Style)window.Resources["ZenCalendarDayButtonStyle"]);
            dayStyle.Setters.Add(new Setter(Control.PaddingProperty, dayPadding));
            var calendarStyle = new Style(
                typeof(Calendar),
                (Style)window.Resources["ZenCalendarStyle"]);
            calendarStyle.Setters.Add(new Setter(Control.BackgroundProperty, calendarBackground));
            calendarStyle.Setters.Add(new Setter(Calendar.CalendarDayButtonStyleProperty, dayStyle));
            datePicker.CalendarStyle = calendarStyle;

            try
            {
                window.Show();
                datePicker.IsDropDownOpen = true;
                window.UpdateLayout();

                var calendar = GetDatePickerCalendar(datePicker);
                Assert.IsNotNull(calendar);
                Assert.AreSame(calendarStyle, calendar.Style);
                Assert.AreSame(calendarBackground, calendar.Background);
                Assert.AreEqual(14d, datePicker.FontSize);
                Assert.AreEqual(datePicker.CalendarFontSize, calendar.FontSize);

                calendar.ApplyTemplate();
                var calendarItem = calendar.Template.FindName("PART_CalendarItem", calendar) as CalendarItem;
                Assert.IsNotNull(calendarItem);
                calendarItem.ApplyTemplate();
                var monthView = calendarItem.Template.FindName("PART_MonthView", calendarItem) as Grid;
                Assert.IsNotNull(monthView);
                var dayButton = monthView.Children.OfType<CalendarDayButton>()
                    .FirstOrDefault(button => button.Visibility == Visibility.Visible);
                Assert.IsNotNull(dayButton);
                Assert.AreEqual(dayPadding, dayButton.Padding);
            }
            finally
            {
                datePicker.IsDropDownOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void DatePickerCalendarPreservesOwnerDateConstraints()
        {
            var start = new DateTime(2026, 1, 1);
            var end = new DateTime(2026, 12, 31);
            var selected = new DateTime(2026, 7, 23);
            var datePicker = new ZenDatePicker
            {
                DisplayDateStart = start,
                DisplayDateEnd = end,
                FirstDayOfWeek = DayOfWeek.Monday,
                IsTodayHighlighted = false,
                SelectedDate = selected
            };
            var window = CreateTestWindow(datePicker, 260, 320);

            try
            {
                window.Show();
                datePicker.IsDropDownOpen = true;
                window.UpdateLayout();

                var calendar = GetDatePickerCalendar(datePicker);
                Assert.IsNotNull(calendar);
                Assert.AreEqual(new DateTime(2026, 1, 1), calendar.DisplayDateStart);
                Assert.AreEqual(new DateTime(2026, 12, 31), calendar.DisplayDateEnd);
                Assert.AreEqual(DayOfWeek.Monday, calendar.FirstDayOfWeek);
                Assert.IsFalse(calendar.IsTodayHighlighted);
                Assert.AreEqual(selected, calendar.SelectedDate);

                var changed = new DateTime(2026, 8, 8);
                calendar.SelectedDate = changed;
                Assert.AreEqual(changed, datePicker.SelectedDate);
            }
            finally
            {
                datePicker.IsDropDownOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void DatePickerCalendarShowsConstrainedDatesAsDisabled()
        {
            var datePicker = new ZenDatePicker
            {
                DisplayDate = new DateTime(2026, 7, 15),
                DisplayDateStart = new DateTime(2026, 7, 10),
                DisplayDateEnd = new DateTime(2026, 8, 20)
            };
            var window = CreateTestWindow(datePicker, 420, 440);

            try
            {
                window.Show();
                datePicker.IsDropDownOpen = true;
                window.UpdateLayout();

                var calendar = GetDatePickerCalendar(datePicker);
                Assert.IsNotNull(calendar);
                calendar.ApplyTemplate();
                var calendarItem = calendar.Template.FindName("PART_CalendarItem", calendar) as CalendarItem;
                Assert.IsNotNull(calendarItem);
                calendarItem.ApplyTemplate();
                window.UpdateLayout();

                var monthView = calendarItem.Template.FindName("PART_MonthView", calendarItem) as Grid;
                Assert.IsNotNull(monthView);
                var buttons = monthView.Children.OfType<CalendarDayButton>().ToList();
                var beforeStart = buttons.Single(
                    button => button.DataContext is DateTime date && date == new DateTime(2026, 7, 9));
                var nextMonth = buttons.Single(
                    button => button.DataContext is DateTime date && date == new DateTime(2026, 8, 1));
                var selectable = buttons.Single(
                    button => button.DataContext is DateTime date && date == new DateTime(2026, 7, 15));

                beforeStart.ApplyTemplate();
                nextMonth.ApplyTemplate();
                var disabledBorder = beforeStart.Template.FindName("DayBorder", beforeStart) as Border;
                var nextMonthBorder = nextMonth.Template.FindName("DayBorder", nextMonth) as Border;
                var disabledBackground =
                    beforeStart.Template.FindName("BlackoutBackground", beforeStart) as Border;
                var nextMonthBackground =
                    nextMonth.Template.FindName("BlackoutBackground", nextMonth) as Border;
                Assert.IsNotNull(disabledBorder);
                Assert.IsNotNull(nextMonthBorder);
                Assert.IsNotNull(disabledBackground);
                Assert.IsNotNull(nextMonthBackground);
                Assert.AreEqual(Visibility.Visible, beforeStart.Visibility);
                Assert.IsTrue(beforeStart.IsBlackedOut);
                Assert.IsFalse(beforeStart.Focusable);
                Assert.IsFalse(beforeStart.IsHitTestVisible);
                Assert.AreEqual(Visibility.Visible, nextMonth.Visibility);
                Assert.IsTrue(nextMonth.IsInactive);
                Assert.IsFalse(nextMonth.IsBlackedOut);
                Assert.IsTrue(nextMonth.IsHitTestVisible);
                Assert.AreEqual(1d, disabledBackground.Opacity);
                Assert.AreEqual(0d, nextMonthBackground.Opacity);
                Assert.AreEqual(1d, beforeStart.Opacity);
                Assert.AreEqual(0.72d, nextMonth.Opacity);
                Assert.IsFalse(selectable.IsBlackedOut);
                Assert.IsTrue(selectable.IsHitTestVisible);

                calendar.DisplayDate = new DateTime(2026, 8, 15);
                window.UpdateLayout();
                var afterEnd = monthView.Children.OfType<CalendarDayButton>().Single(
                    button => button.DataContext is DateTime date && date == new DateTime(2026, 8, 21));
                Assert.AreEqual(Visibility.Visible, afterEnd.Visibility);
                Assert.IsTrue(afterEnd.IsBlackedOut);
                Assert.IsFalse(afterEnd.IsHitTestVisible);
            }
            finally
            {
                datePicker.IsDropDownOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void DatePickerCalendarHeaderSwitchesDisplayModes()
        {
            var datePicker = new ZenDatePicker();
            var window = CreateTestWindow(datePicker, 320, 360);

            try
            {
                window.Show();
                datePicker.IsDropDownOpen = true;
                window.UpdateLayout();

                var calendar = GetDatePickerCalendar(datePicker);
                Assert.IsNotNull(calendar);
                calendar.ApplyTemplate();
                window.UpdateLayout();
                var calendarItem = calendar.Template.FindName("PART_CalendarItem", calendar) as CalendarItem;
                Assert.IsNotNull(calendarItem);
                calendarItem.ApplyTemplate();

                var headerButton = calendarItem.Template.FindName("PART_HeaderButton", calendarItem) as Button;
                Assert.IsNotNull(headerButton);
                Assert.AreEqual(CalendarMode.Month, calendar.DisplayMode);

                headerButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                window.UpdateLayout();
                Assert.AreEqual(CalendarMode.Year, calendar.DisplayMode);
                Assert.IsTrue(datePicker.IsDropDownOpen);
                Assert.AreEqual(CalendarMode.Year, calendarItem.Tag);
                var yearView = calendarItem.Template.FindName("PART_YearView", calendarItem) as Grid;
                Assert.IsNotNull(yearView);
                Assert.AreEqual(Visibility.Visible, yearView.Visibility);
                Assert.AreEqual(12, yearView.Children.Count);

                var monthButton = yearView.Children.OfType<CalendarButton>()
                    .FirstOrDefault(button => button.Visibility == Visibility.Visible && button.IsEnabled);
                Assert.IsNotNull(monthButton);
                monthButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                window.UpdateLayout();
                Assert.AreEqual(CalendarMode.Month, calendar.DisplayMode);

                headerButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                window.UpdateLayout();
                headerButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                window.UpdateLayout();
                Assert.AreEqual(CalendarMode.Decade, calendar.DisplayMode);

                var yearButton = yearView.Children.OfType<CalendarButton>()
                    .FirstOrDefault(button => button.Visibility == Visibility.Visible && button.IsEnabled);
                Assert.IsNotNull(yearButton);
                yearButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                window.UpdateLayout();
                Assert.AreEqual(CalendarMode.Year, calendar.DisplayMode);
            }
            finally
            {
                datePicker.IsDropDownOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void DatePickerPaddingIsAppliedOnceAndInputHasNoDeadZone()
        {
            var datePicker = new ZenDatePicker
            {
                Width = 240,
                Height = 36,
                Watermark = "请选择日期"
            };
            var window = CreateTestWindow(datePicker, 280, 100);

            try
            {
                window.Show();
                window.UpdateLayout();

                var textBox = datePicker.Template.FindName("PART_TextBox", datePicker) as DatePickerTextBox;
                var button = datePicker.Template.FindName("PART_Button", datePicker) as Button;
                Assert.IsNotNull(textBox);
                Assert.IsNotNull(button);
                textBox.ApplyTemplate();

                var contentHost = textBox.Template.FindName("PART_ContentHost", textBox) as FrameworkElement;
                var watermark = textBox.Template.FindName("WatermarkText", textBox) as TextBlock;
                Assert.IsNotNull(contentHost);
                Assert.IsNotNull(watermark);
                Assert.AreEqual(datePicker.Padding, textBox.Padding);
                Assert.AreEqual(new Thickness(), contentHost.Margin);
                Assert.AreEqual(textBox.Padding, watermark.Margin);
                Assert.AreEqual(Visibility.Visible, watermark.Visibility);
                var contentLeft = contentHost.TranslatePoint(new Point(), textBox).X;
                var watermarkLeft = watermark.TranslatePoint(new Point(), textBox).X;
                Assert.AreEqual(0d, contentLeft, 0.5d);
                Assert.AreEqual(textBox.Padding.Left, watermarkLeft, 0.5d);

                var buttonLeft = button.TranslatePoint(
                    new Point(0, button.ActualHeight / 2d),
                    datePicker).X;
                Assert.AreEqual(datePicker.ActualWidth, textBox.ActualWidth, 0.5d);
                Assert.AreEqual(28d, button.ActualWidth, 0.5d);

                var inputPoint = new Point(buttonLeft - 2d, datePicker.ActualHeight / 2d);
                Assert.AreSame(
                    textBox.InputHitTest(inputPoint),
                    datePicker.InputHitTest(inputPoint));
                Assert.IsNotNull(textBox.InputHitTest(
                    new Point(textBox.ActualWidth - 2d, textBox.ActualHeight / 2d)));
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DatePickerToggleButtonDoesNotRequireCalendarAncestor()
        {
            var datePicker = new ZenDatePicker
            {
                FontSize = 19
            };
            var window = CreateTestWindow(datePicker, 260, 120);

            try
            {
                window.Show();
                window.UpdateLayout();

                var button = datePicker.Template.FindName("PART_Button", datePicker) as Button;
                Assert.IsNotNull(button);
                Assert.AreEqual(datePicker.FontSize, button.FontSize);
                Assert.IsNull(
                    BindingOperations.GetBindingExpression(
                        button,
                        Control.FontSizeProperty));
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DatePickerCanDisableTextInputWithoutDisablingCalendarSelection()
        {
            var datePicker = new ZenDatePicker
            {
                IsTextInputEnabled = false
            };
            var window = CreateTestWindow(datePicker, 260, 120);

            try
            {
                window.Show();
                window.UpdateLayout();

                var textBox = datePicker.Template.FindName("PART_TextBox", datePicker) as DatePickerTextBox;
                var button = datePicker.Template.FindName("PART_Button", datePicker) as Button;
                Assert.IsNotNull(textBox);
                Assert.IsNotNull(button);
                Assert.IsTrue(textBox.IsReadOnly);
                Assert.IsTrue(button.IsEnabled);

                datePicker.IsTextInputEnabled = true;
                Assert.IsFalse(textBox.IsReadOnly);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
