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
    public class ScrollBarTests
    {
        [TestMethod]
        public void ScrollBarMetricTokensCanBeOverriddenInWindowResources()
        {
            var vertical = new ScrollBar
            {
                Height = 120,
                Maximum = 100,
                Orientation = Orientation.Vertical,
                ViewportSize = 10
            };
            var horizontal = new ScrollBar
            {
                Maximum = 100,
                Orientation = Orientation.Horizontal,
                ViewportSize = 10,
                Width = 120
            };
            var panel = new StackPanel();
            panel.Children.Add(vertical);
            panel.Children.Add(horizontal);
            var window = CreateTestWindow(panel, 220, 220);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });
            window.Resources["ZenScrollBarThickness"] = 18d;
            window.Resources["ZenScrollBarTrackThickness"] = 8d;
            window.Resources["ZenScrollBarThumbMinLength"] = 40d;
            window.Resources["ZenVerticalScrollBarThumbMargin"] = new Thickness(5, 0, 5, 0);
            window.Resources["ZenHorizontalScrollBarThumbMargin"] = new Thickness(0, 5, 0, 5);
            window.Resources["ZenScrollBarCornerRadius"] = new CornerRadius(4);

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.AreEqual(18d, vertical.Width);
                Assert.AreEqual(18d, horizontal.Height);
                AssertScrollBarMetrics(
                    vertical,
                    8d,
                    40d,
                    new Thickness(5, 0, 5, 0),
                    new CornerRadius(4));
                AssertScrollBarMetrics(
                    horizontal,
                    8d,
                    40d,
                    new Thickness(0, 5, 0, 5),
                    new CornerRadius(4));
            }
            finally
            {
                window.Close();
            }
        }
    }
}
