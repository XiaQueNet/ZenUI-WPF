using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class CalendarTests
    {
        [TestMethod]
        public void CalendarUsesZenTemplateAndDefaultMetrics()
        {
            var calendar = new ZenCalendar
            {
                DisplayDate = new DateTime(2026, 7, 1),
                SelectedDate = new DateTime(2026, 7, 27)
            };
            var window = CreateTestWindow(calendar, 460, 460);
            AddZenResources(window);

            try
            {
                window.Show();
                calendar.ApplyTemplate();
                window.UpdateLayout();

                Assert.AreEqual(368d, calendar.Width);
                Assert.AreEqual(372d, calendar.Height);
                Assert.AreEqual(48d, calendar.DayButtonWidth);
                Assert.AreEqual(44d, calendar.DayButtonHeight);
                Assert.AreEqual(new Thickness(12, 16, 12, 16), calendar.ButtonPadding);
                Assert.AreEqual(40d, calendar.NavigationButtonSize);
                Assert.AreEqual(new DateTime(2026, 7, 27), calendar.SelectedDate);
                Assert.IsInstanceOfType<CalendarItem>(
                    calendar.Template.FindName("PART_CalendarItem", calendar));
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void CalendarMetricsFollowDensityChanges()
        {
            var calendar = new ZenCalendar();
            var window = CreateTestWindow(calendar, 500, 500);
            AddZenResources(window);

            try
            {
                window.Show();

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.UpdateLayout();
                Assert.AreEqual(328d, calendar.Width);
                Assert.AreEqual(348d, calendar.Height);
                Assert.AreEqual(42d, calendar.DayButtonWidth);
                Assert.AreEqual(40d, calendar.DayButtonHeight);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Comfortable);
                window.UpdateLayout();
                Assert.AreEqual(412d, calendar.Width);
                Assert.AreEqual(408d, calendar.Height);
                Assert.AreEqual(54d, calendar.DayButtonWidth);
                Assert.AreEqual(50d, calendar.DayButtonHeight);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void CalendarAllowsMetricOverrides()
        {
            var calendar = new ZenCalendar
            {
                DayButtonWidth = 40d,
                DayButtonHeight = 36d,
                ButtonPadding = new Thickness(8),
                NavigationButtonSize = 32d
            };

            Assert.AreEqual(40d, calendar.DayButtonWidth);
            Assert.AreEqual(36d, calendar.DayButtonHeight);
            Assert.AreEqual(new Thickness(8), calendar.ButtonPadding);
            Assert.AreEqual(32d, calendar.NavigationButtonSize);
        }

        private static void AddZenResources(FrameworkElement element)
        {
            element.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });
        }
    }
}
