using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using Calendar = System.Windows.Controls.Calendar;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class DateTimePickerTests
    {
        [TestMethod]
        public void DefaultsExposeExpectedDateTimePickerContract()
        {
            var picker = new TestDateTimePicker();

            Assert.AreEqual(typeof(ZenDateTimePicker), picker.ExposedDefaultStyleKey);
            Assert.IsNull(picker.SelectedDateTime);
            Assert.IsNull(picker.Minimum);
            Assert.IsNull(picker.Maximum);
            Assert.AreEqual(1, picker.MinuteIncrement);
            Assert.AreEqual(1, picker.SecondIncrement);
            Assert.IsTrue(picker.IsSecondVisible);
            Assert.IsTrue(picker.Is24Hour);
            Assert.IsTrue(picker.IsTextInputEnabled);
            Assert.IsFalse(picker.IsDropDownOpen);
            Assert.AreEqual(new CornerRadius(6), picker.CornerRadius);
            Assert.AreEqual(
                CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek,
                picker.FirstDayOfWeek);
            Assert.IsTrue(picker.IsTodayHighlighted);
        }

        [TestMethod]
        public void SelectedDateTimeIsCoercedToConfiguredRange()
        {
            var minimum = new DateTime(2026, 8, 3, 9, 0, 0);
            var maximum = new DateTime(2026, 8, 5, 18, 0, 0);
            var picker = new ZenDateTimePicker
            {
                Minimum = minimum,
                Maximum = maximum
            };

            picker.SelectedDateTime = minimum.AddHours(-2);
            Assert.AreEqual(minimum, picker.SelectedDateTime);

            picker.SelectedDateTime = maximum.AddHours(2);
            Assert.AreEqual(maximum, picker.SelectedDateTime);
        }

        [TestMethod]
        public void GenericThemeProvidesCombinedCalendarAndTimeSelector()
        {
            var picker = CreateTemplatedPicker();

            var calendar = GetPart<Calendar>(picker, "PART_Calendar");
            var timeSelector = GetPart<ZenTimeSelector>(picker, "PART_TimeSelector");
            Assert.IsNotNull(calendar);
            Assert.IsNotNull(timeSelector);
            Assert.IsNotNull(GetPart<TextBox>(picker, "PART_TextBox"));
            Assert.IsNotNull(GetPart<Button>(picker, "PART_NowButton"));
            Assert.IsNotNull(GetPart<Button>(picker, "PART_ConfirmButton"));
        }

        [TestMethod]
        public void SharedTextBoxStyleDoesNotDependOnTimePickerAncestor()
        {
            var picker = CreateTemplatedPicker();
            picker.IsTextInputEnabled = false;

            Assert.IsTrue(GetPart<TextBox>(picker, "PART_TextBox").IsReadOnly);
        }

        [TestMethod]
        public void PopupEditsDraftUntilUserConfirms()
        {
            var original = new DateTime(2026, 8, 3, 9, 30, 0);
            var changedDate = new DateTime(2026, 8, 8);
            var picker = CreateTemplatedPicker();
            picker.SelectedDateTime = original;
            picker.IsDropDownOpen = true;

            var calendar = GetPart<Calendar>(picker, "PART_Calendar");
            calendar.SelectedDate = changedDate;
            picker.IsDropDownOpen = false;

            Assert.AreEqual(original, picker.SelectedDateTime);

            picker.IsDropDownOpen = true;
            calendar.SelectedDate = changedDate;
            GetPart<ZenTimeSelector>(picker, "PART_TimeSelector").SelectedTime =
                new TimeSpan(14, 45, 0);
            GetPart<Button>(picker, "PART_ConfirmButton")
                .RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            Assert.AreEqual(
                new DateTime(2026, 8, 8, 14, 45, 0),
                picker.SelectedDateTime);
            Assert.IsFalse(picker.IsDropDownOpen);
        }

        [TestMethod]
        public void NowActionUpdatesDateAndTimeAsOneDraftValue()
        {
            var picker = CreateTemplatedPicker();
            picker.SelectedDateTime = new DateTime(2020, 1, 1, 1, 2, 3);
            Assert.AreEqual(new DateTime(2020, 1, 1, 1, 2, 3), picker.SelectedDateTime);
            picker.IsDropDownOpen = true;
            Assert.AreEqual(
                new DateTime(2020, 1, 1),
                GetPart<Calendar>(picker, "PART_Calendar").SelectedDate.Value.Date);

            GetPart<Button>(picker, "PART_NowButton")
                .RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.AreEqual(
                0,
                GetPart<ZenTimeSelector>(picker, "PART_TimeSelector")
                    .SelectedTime.Value.Milliseconds);
            GetPart<Button>(picker, "PART_ConfirmButton")
                .RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            Assert.IsTrue(picker.SelectedDateTime.HasValue);
            Assert.AreNotEqual(2020, picker.SelectedDateTime.Value.Year);
            Assert.AreEqual(0, picker.SelectedDateTime.Value.Millisecond);
            Assert.AreEqual(
                picker.SelectedDateTime.Value.Date,
                GetPart<Calendar>(picker, "PART_Calendar").SelectedDate.Value.Date);
            Assert.AreEqual(
                picker.SelectedDateTime.Value.TimeOfDay,
                GetPart<ZenTimeSelector>(picker, "PART_TimeSelector").SelectedTime);
        }

        [TestMethod]
        public void BoundaryDateRestrictsAvailableTimes()
        {
            var minimum = new DateTime(2026, 8, 3, 9, 15, 0);
            var maximum = new DateTime(2026, 8, 4, 18, 30, 0);
            var picker = CreateTemplatedPicker();
            picker.Minimum = minimum;
            picker.Maximum = maximum;
            picker.SelectedDateTime = minimum;
            picker.IsDropDownOpen = true;

            var selector = GetPart<ZenTimeSelector>(picker, "PART_TimeSelector");
            Assert.AreEqual(minimum.TimeOfDay, selector.Minimum);
            Assert.AreEqual(new TimeSpan(23, 59, 59), selector.Maximum);

            GetPart<Calendar>(picker, "PART_Calendar").SelectedDate = maximum.Date;
            Assert.AreEqual(TimeSpan.Zero, selector.Minimum);
            Assert.AreEqual(maximum.TimeOfDay, selector.Maximum);
        }

        [TestMethod]
        public void AutomationPeerExposesValueAndExpandCollapsePatterns()
        {
            var picker = new TestDateTimePicker
            {
                SelectedDateTime = new DateTime(2026, 8, 3, 14, 30, 0)
            };
            var peer = picker.ExposedAutomationPeer;
            var valueProvider = (IValueProvider)peer.GetPattern(PatternInterface.Value);
            var expandProvider =
                (IExpandCollapseProvider)peer.GetPattern(PatternInterface.ExpandCollapse);

            Assert.IsFalse(valueProvider.IsReadOnly);
            Assert.IsFalse(string.IsNullOrWhiteSpace(valueProvider.Value));
            expandProvider.Expand();
            Assert.IsTrue(picker.IsDropDownOpen);
            expandProvider.Collapse();
            Assert.IsFalse(picker.IsDropDownOpen);
        }

        private static ZenDateTimePicker CreateTemplatedPicker()
        {
            _ = new ZenButton();
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var picker = new ZenDateTimePicker
            {
                Style = (Style)dictionary[typeof(ZenDateTimePicker)]
            };
            picker.ApplyTemplate();
            return picker;
        }

        private static T GetPart<T>(ZenDateTimePicker picker, string name)
            where T : DependencyObject
        {
            return (T)picker.Template.FindName(name, picker);
        }

        private sealed class TestDateTimePicker : ZenDateTimePicker
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;

            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
    }
}
