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
    public class ProgressBarTests
    {
        [TestMethod]
        public void ProgressBarSupportsVerticalAndIndeterminateStates()
        {
            var progressBar = new ZenProgressBar
            {
                Orientation = Orientation.Vertical,
                Height = 180,
                Value = 60
            };
            var window = CreateTestWindow(progressBar, 100, 220);
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

                var indicator = progressBar.Template.FindName("PART_Indicator", progressBar) as FrameworkElement;
                Assert.IsNotNull(indicator);
                Assert.AreEqual(HorizontalAlignment.Stretch, indicator.HorizontalAlignment);
                Assert.AreEqual(VerticalAlignment.Bottom, indicator.VerticalAlignment);
                Assert.AreEqual(progressBar.ActualHeight * 0.6d, indicator.ActualHeight, 1d);
                Assert.AreEqual(8d, progressBar.Width);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.UpdateLayout();
                Assert.AreEqual(6d, progressBar.Width);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Comfortable);
                window.UpdateLayout();
                Assert.AreEqual(10d, progressBar.Width);

                progressBar.IsIndeterminate = true;
                window.UpdateLayout();
                var indeterminate = progressBar.Template.FindName("IndeterminateIndicator", progressBar) as FrameworkElement;
                Assert.IsNotNull(indeterminate);
                Assert.AreEqual(Visibility.Visible, indeterminate.Visibility);
                Assert.AreEqual(1d, indeterminate.Opacity);

                var horizontalTransform = progressBar.Template.FindName(
                    "HorizontalIndeterminateTransform",
                    progressBar) as TranslateTransform;
                var verticalTransform = progressBar.Template.FindName(
                    "VerticalIndeterminateTransform",
                    progressBar) as TranslateTransform;
                Assert.IsNotNull(horizontalTransform);
                Assert.IsNotNull(verticalTransform);
                WaitForDispatcher(TimeSpan.FromMilliseconds(250));
                Assert.AreEqual(-1d, horizontalTransform.X);
                Assert.AreNotEqual(1d, verticalTransform.Y);

                progressBar.Orientation = Orientation.Horizontal;
                window.UpdateLayout();
                WaitForDispatcher(TimeSpan.FromMilliseconds(250));
                Assert.AreNotEqual(-1d, horizontalTransform.X);
                Assert.AreEqual(1d, verticalTransform.Y);

                progressBar.Visibility = Visibility.Collapsed;
                window.UpdateLayout();
                WaitForDispatcher(TimeSpan.FromMilliseconds(100));
                var hiddenHorizontalOffset = horizontalTransform.X;
                var hiddenVerticalOffset = verticalTransform.Y;
                WaitForDispatcher(TimeSpan.FromMilliseconds(150));
                Assert.AreEqual(hiddenHorizontalOffset, horizontalTransform.X);
                Assert.AreEqual(hiddenVerticalOffset, verticalTransform.Y);
                progressBar.Visibility = Visibility.Visible;

                progressBar.IsEnabled = false;
                window.UpdateLayout();
                Assert.AreEqual(0.45d, progressBar.Opacity);

                ZenThemeManager.ApplyTheme(window.Resources, ZenTheme.HighContrast, false);
                window.UpdateLayout();
                Assert.AreEqual(1d, progressBar.Opacity);
            }
            finally
            {
                window.Close();
            }
        }

        private static void WaitForDispatcher(TimeSpan duration)
        {
            var frame = new DispatcherFrame();
            var timer = new DispatcherTimer(
                duration,
                DispatcherPriority.Background,
                (sender, e) =>
                {
                    ((DispatcherTimer)sender).Stop();
                    frame.Continue = false;
                },
                Dispatcher.CurrentDispatcher);
            timer.Start();
            Dispatcher.PushFrame(frame);
        }
    }
}
