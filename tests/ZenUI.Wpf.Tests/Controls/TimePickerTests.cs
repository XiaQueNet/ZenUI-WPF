using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

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
            Assert.IsTrue(picker.Is24HourFormat);
            Assert.IsFalse(picker.IsTextInputReadOnly);
            Assert.IsTrue(picker.ShowWatermarkOnFocus);
            Assert.IsFalse(picker.IsDropDownOpen);
            Assert.AreEqual(new CornerRadius(6), picker.CornerRadius);
            Assert.AreEqual(28d, picker.DropDownButtonWidth);
            Assert.AreEqual(28d, picker.DropDownButtonHeight);
            Assert.AreEqual(16d, picker.DropDownButtonIconSize);
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

            var hourList = GetTimeSelectorPart<ListBox>(picker, "PART_HourList");
            var minuteList = GetTimeSelectorPart<ListBox>(picker, "PART_MinuteList");
            var secondList = GetTimeSelectorPart<ListBox>(picker, "PART_SecondList");
            Assert.IsNotNull(hourList);
            Assert.IsNotNull(minuteList);
            Assert.IsNotNull(secondList);
            Assert.AreEqual(24, hourList.Items.Count);
            Assert.AreEqual(4, minuteList.Items.Count);
            Assert.AreEqual(6, secondList.Items.Count);
            Assert.AreEqual(
                ScrollBarVisibility.Visible,
                ScrollViewer.GetVerticalScrollBarVisibility(hourList));
        }

        [TestMethod]
        public void DisablingTextInputMakesTemplateTextBoxReadOnly()
        {
            _ = new ZenButton();
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var picker = new ZenTimePicker
            {
                Style = (Style)dictionary[typeof(ZenTimePicker)],
                IsTextInputReadOnly = true
            };
            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Content = picker
            };

            try
            {
                window.Show();
                picker.ApplyTemplate();

                var textBox = (TextBox)picker.Template.FindName("PART_TextBox", picker);
                Assert.IsTrue(textBox.IsReadOnly);

                textBox.RaiseEvent(new MouseButtonEventArgs(
                    Mouse.PrimaryDevice,
                    Environment.TickCount,
                    MouseButton.Left)
                {
                    RoutedEvent = UIElement.MouseLeftButtonUpEvent
                });
                picker.Dispatcher.Invoke(
                    DispatcherPriority.ContextIdle,
                    new Action(() => { }));
                Assert.IsTrue(picker.IsDropDownOpen);

                picker.IsDropDownOpen = false;
                picker.IsTextInputReadOnly = false;
                textBox.RaiseEvent(new MouseButtonEventArgs(
                    Mouse.PrimaryDevice,
                    Environment.TickCount,
                    MouseButton.Left)
                {
                    RoutedEvent = UIElement.MouseLeftButtonUpEvent
                });
                Assert.IsFalse(picker.IsDropDownOpen);
            }
            finally
            {
                picker.IsDropDownOpen = false;
                window.Close();
            }
        }

        [TestMethod]
        public void InternalTimeSelectorCanBeUsedWithoutTimePicker()
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var selector = new ZenTimeSelector
            {
                Style = (Style)dictionary[typeof(ZenTimeSelector)],
                MinuteIncrement = 15,
                SelectedTime = new TimeSpan(9, 30, 0)
            };

            selector.ApplyTemplate();

            var minuteList = GetTimeSelectorPart<ListBox>(
                selector,
                "PART_MinuteList");
            minuteList.SelectedItem = FindOption(minuteList, 45);

            Assert.AreEqual(new TimeSpan(9, 45, 0), selector.SelectedTime);
        }

        [TestMethod]
        public void InternalTimeSelectorListHeightAcceptsUnarrangedLayout()
        {
            var selector = new ZenTimeSelector();

            selector.ListHeight = 0d;

            Assert.AreEqual(0d, selector.ListHeight);
            Assert.ThrowsExactly<ArgumentException>(() => selector.ListHeight = -1d);
            Assert.ThrowsExactly<ArgumentException>(() => selector.ListHeight = double.NaN);
            Assert.ThrowsExactly<ArgumentException>(
                () => selector.ListHeight = double.PositiveInfinity);
        }

        [TestMethod]
        public void PopupMetricsFollowDensityChanges()
        {
            var picker = new ZenTimePicker
            {
                Is24HourFormat = false,
                IsDropDownOpen = true
            };
            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 420,
                Height = 360,
                Content = picker
            };
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });

            try
            {
                window.Show();
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.UpdateLayout();

                var popup = (Popup)picker.Template.FindName("PART_Popup", picker);
                var popupBorder = popup.Child as Border;
                var hourList = GetTimeSelectorPart<ListBox>(picker, "PART_HourList");
                var periodList = GetTimeSelectorPart<ListBox>(picker, "PART_PeriodList");
                hourList.ScrollIntoView(hourList.Items[0]);
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                hourList.UpdateLayout();
                var firstItem = hourList.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                Assert.IsNotNull(popupBorder);
                Assert.IsNotNull(firstItem);
                AssertPopupMetrics(
                    popupBorder,
                    hourList,
                    periodList,
                    firstItem,
                    new Thickness(0, 4, 0, 8),
                    new Thickness(6),
                    64d,
                    64d,
                    196d,
                    36d,
                    new Thickness(0, 2, 0, 2));

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.UpdateLayout();
                AssertPopupMetrics(
                    popupBorder,
                    hourList,
                    periodList,
                    firstItem,
                    new Thickness(0, 3, 0, 6),
                    new Thickness(4),
                    60d,
                    60d,
                    172d,
                    32d,
                    new Thickness(0, 1, 0, 1));

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Comfortable);
                window.UpdateLayout();
                AssertPopupMetrics(
                    popupBorder,
                    hourList,
                    periodList,
                    firstItem,
                    new Thickness(0, 6, 0, 10),
                    new Thickness(8),
                    68d,
                    68d,
                    220d,
                    40d,
                    new Thickness(0, 3, 0, 3));
            }
            finally
            {
                picker.IsDropDownOpen = false;
                window.Close();
            }
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
                Is24HourFormat = false,
                SelectedTime = new TimeSpan(9, 0, 0)
            };
            picker.ApplyTemplate();

            var hourList = GetTimeSelectorPart<ListBox>(picker, "PART_HourList");
            var minuteList = GetTimeSelectorPart<ListBox>(picker, "PART_MinuteList");
            var periodList = GetTimeSelectorPart<ListBox>(picker, "PART_PeriodList");
            Assert.AreEqual("AM", periodList.Items[0].ToString());
            Assert.AreEqual("PM", periodList.Items[1].ToString());
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
                GetTimeSelectorPart<ListBox>(picker, "PART_HourList").SelectedItem.ToString());
            Assert.AreEqual(
                picker.SelectedTime.Value.Minutes.ToString("00", CultureInfo.CurrentCulture),
                GetTimeSelectorPart<ListBox>(picker, "PART_MinuteList").SelectedItem.ToString());
            Assert.AreEqual(
                picker.SelectedTime.Value.Seconds.ToString("00", CultureInfo.CurrentCulture),
                GetTimeSelectorPart<ListBox>(picker, "PART_SecondList").SelectedItem.ToString());

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

            var hourList = GetTimeSelectorPart<ListBox>(picker, "PART_HourList");
            var minuteList = GetTimeSelectorPart<ListBox>(picker, "PART_MinuteList");
            var secondList = GetTimeSelectorPart<ListBox>(picker, "PART_SecondList");

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

        private static void AssertPopupMetrics(
            Border popupBorder,
            ListBox hourList,
            ListBox periodList,
            ListBoxItem firstItem,
            Thickness popupMargin,
            Thickness popupPadding,
            double columnWidth,
            double periodColumnWidth,
            double listHeight,
            double itemHeight,
            Thickness itemMargin)
        {
            Assert.AreEqual(popupMargin, popupBorder.Margin);
            Assert.AreEqual(popupPadding, popupBorder.Padding);
            Assert.AreEqual(columnWidth, hourList.Width);
            Assert.AreEqual(periodColumnWidth, periodList.Width);
            Assert.AreEqual(listHeight, hourList.Height);
            Assert.AreEqual(itemHeight, firstItem.Height);
            Assert.AreEqual(itemMargin, firstItem.Margin);
        }

        private static TimeSelectorOption FindOption(ListBox selector, int value)
        {
            foreach (var item in selector.Items)
            {
                if (item is TimeSelectorOption option && option.Value == value)
                {
                    return option;
                }
            }

            Assert.Fail("未找到时间选项。");
            return null;
        }

        private static T GetTimeSelectorPart<T>(ZenTimePicker picker, string partName)
            where T : FrameworkElement
        {
            var selector = picker.Template.FindName("PART_TimeSelector", picker) as Control;
            Assert.IsNotNull(selector);
            selector.ApplyTemplate();

            return GetTimeSelectorPart<T>(selector, partName);
        }

        private static T GetTimeSelectorPart<T>(Control selector, string partName)
            where T : FrameworkElement
        {
            var part = selector.Template.FindName(partName, selector) as T;
            Assert.IsNotNull(part);
            return part;
        }

        private sealed class TestTimePicker : ZenTimePicker
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
        }
    }
}
