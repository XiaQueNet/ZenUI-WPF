using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class TimePickerTests
    {
        [TestMethod]
        public void DefaultsExposeExpectedTimePickerContract()
        {
            var picker = new TestTimePicker();

            Assert.AreEqual(typeof(ZenTimePicker), picker.ExposedDefaultStyleKey);
            Assert.IsNull(picker.SelectedTime);
            Assert.AreEqual(TimeSpan.Zero, picker.Minimum);
            Assert.AreEqual(new TimeSpan(23, 59, 59), picker.Maximum);
            Assert.AreEqual(1, picker.MinuteIncrement);
            Assert.AreEqual(1, picker.SecondIncrement);
            Assert.IsTrue(picker.IsSecondVisible);
            Assert.IsTrue(picker.Is24Hour);
            Assert.IsTrue(picker.IsTextInputEnabled);
            Assert.IsFalse(picker.IsDropDownOpen);
            Assert.AreEqual(new CornerRadius(6), picker.CornerRadius);
        }

        [TestMethod]
        public void SelectedTimeIsCoercedToConfiguredRange()
        {
            var picker = new ZenTimePicker
            {
                Minimum = new TimeSpan(9, 0, 0),
                Maximum = new TimeSpan(18, 0, 0)
            };

            picker.SelectedTime = new TimeSpan(7, 30, 0);
            Assert.AreEqual(new TimeSpan(9, 0, 0), picker.SelectedTime);

            picker.SelectedTime = new TimeSpan(20, 0, 0);
            Assert.AreEqual(new TimeSpan(18, 0, 0), picker.SelectedTime);
        }

        [TestMethod]
        public void GenericThemeProvidesTemplateAndSelectorOptions()
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var picker = new ZenTimePicker
            {
                Style = (Style)dictionary[typeof(ZenTimePicker)],
                MinuteIncrement = 15,
                IsSecondVisible = true,
                SecondIncrement = 10
            };

            picker.ApplyTemplate();

            var hourList = (ListBox)picker.Template.FindName("PART_HourList", picker);
            var minuteList = (ListBox)picker.Template.FindName("PART_MinuteList", picker);
            var secondList = (ListBox)picker.Template.FindName("PART_SecondList", picker);
            Assert.IsNotNull(hourList);
            Assert.IsNotNull(minuteList);
            Assert.IsNotNull(secondList);
            Assert.AreEqual(24, hourList.Items.Count);
            Assert.AreEqual(4, minuteList.Items.Count);
            Assert.AreEqual(6, secondList.Items.Count);
            Assert.AreEqual(
                ScrollBarVisibility.Hidden,
                ScrollViewer.GetVerticalScrollBarVisibility(hourList));
        }

        [TestMethod]
        public void TwelveHourModeMapsPeriodSelectionToTimeOfDay()
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var picker = new ZenTimePicker
            {
                Style = (Style)dictionary[typeof(ZenTimePicker)],
                Is24Hour = false,
                SelectedTime = new TimeSpan(9, 0, 0)
            };
            picker.ApplyTemplate();

            var hourList = (ListBox)picker.Template.FindName("PART_HourList", picker);
            var minuteList = (ListBox)picker.Template.FindName("PART_MinuteList", picker);
            var periodList = (ListBox)picker.Template.FindName("PART_PeriodList", picker);
            hourList.SelectedItem = FindOption(hourList, 6);
            minuteList.SelectedItem = FindOption(minuteList, 30);
            periodList.SelectedIndex = 1;

            Assert.AreEqual(new TimeSpan(18, 30, 0), picker.SelectedTime);
        }

        [TestMethod]
        public void PopupActionsSelectCurrentTimeAndClosePopup()
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var picker = new ZenTimePicker
            {
                Style = (Style)dictionary[typeof(ZenTimePicker)],
                IsDropDownOpen = true
            };
            picker.ApplyTemplate();

            var nowButton = (Button)picker.Template.FindName("PART_NowButton", picker);
            var confirmButton = (Button)picker.Template.FindName("PART_ConfirmButton", picker);
            Assert.IsNotNull(nowButton);
            Assert.IsNotNull(confirmButton);

            nowButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.IsTrue(picker.SelectedTime.HasValue);
            Assert.AreEqual(0, picker.SelectedTime.Value.Milliseconds);
            Assert.AreEqual(
                picker.SelectedTime.Value.Hours.ToString("00", CultureInfo.CurrentCulture),
                ((ListBox)picker.Template.FindName("PART_HourList", picker)).SelectedItem.ToString());
            Assert.AreEqual(
                picker.SelectedTime.Value.Minutes.ToString("00", CultureInfo.CurrentCulture),
                ((ListBox)picker.Template.FindName("PART_MinuteList", picker)).SelectedItem.ToString());
            Assert.AreEqual(
                picker.SelectedTime.Value.Seconds.ToString("00", CultureInfo.CurrentCulture),
                ((ListBox)picker.Template.FindName("PART_SecondList", picker)).SelectedItem.ToString());

            confirmButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.IsFalse(picker.IsDropDownOpen);
        }

        [TestMethod]
        public void OptionsOutsideConfiguredRangeAreDisabled()
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var picker = new ZenTimePicker
            {
                Style = (Style)dictionary[typeof(ZenTimePicker)],
                Minimum = new TimeSpan(9, 30, 0),
                Maximum = new TimeSpan(10, 15, 30),
                SelectedTime = new TimeSpan(9, 30, 0)
            };
            picker.ApplyTemplate();

            var hourList = (ListBox)picker.Template.FindName("PART_HourList", picker);
            var minuteList = (ListBox)picker.Template.FindName("PART_MinuteList", picker);
            var secondList = (ListBox)picker.Template.FindName("PART_SecondList", picker);

            Assert.IsFalse(FindOption(hourList, 8).IsEnabled);
            Assert.IsTrue(FindOption(hourList, 9).IsEnabled);
            Assert.IsTrue(FindOption(hourList, 10).IsEnabled);
            Assert.IsFalse(FindOption(hourList, 11).IsEnabled);
            Assert.IsFalse(FindOption(minuteList, 29).IsEnabled);
            Assert.IsTrue(FindOption(minuteList, 30).IsEnabled);

            hourList.SelectedItem = FindOption(hourList, 10);
            minuteList.SelectedItem = FindOption(minuteList, 15);
            Assert.IsTrue(FindOption(secondList, 30).IsEnabled);
            Assert.IsFalse(FindOption(secondList, 31).IsEnabled);
        }

        private static TimePickerOption FindOption(ListBox selector, int value)
        {
            foreach (var item in selector.Items)
            {
                if (item is TimePickerOption option && option.Value == value)
                {
                    return option;
                }
            }

            Assert.Fail("未找到时间选项。");
            return null;
        }

        private sealed class TestTimePicker : ZenTimePicker
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
        }
    }
}
