using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class LoadingTests
    {
        [TestMethod]
        public void LoadingPresentsOverlayAndPreservesContent()
        {
            var content = new Button { Content = "保存" };
            var loading = new ZenLoading
            {
                Width = 260,
                Height = 140,
                Content = content,
                IsLoading = true,
                LoadingText = "正在保存…"
            };
            var window = CreateTestWindow(loading, 320, 200);
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

                var loadingLayer = loading.Template.FindName("LoadingLayer", loading) as Border;
                var indicator = loading.Template.FindName("Indicator", loading) as FrameworkElement;
                var indicatorPanel = loading.Template.FindName("IndicatorPanel", loading) as StackPanel;
                var contentSpacing = loading.Template.FindName("ContentSpacing", loading) as FrameworkElement;
                var loadingText = loading.Template.FindName("LoadingText", loading) as TextBlock;
                Assert.IsNotNull(loadingLayer);
                Assert.IsNotNull(indicator);
                Assert.IsNotNull(indicatorPanel);
                Assert.IsNotNull(contentSpacing);
                Assert.IsNotNull(loadingText);
                Assert.AreSame(content, loading.Content);
                Assert.AreEqual(Visibility.Visible, loadingLayer.Visibility);
                Assert.IsTrue(loadingLayer.IsHitTestVisible);
                Assert.AreEqual(24d, indicator.Width);
                Assert.AreEqual("正在保存…", loadingText.Text);
                Assert.AreEqual(Orientation.Vertical, indicatorPanel.Orientation);
                Assert.AreEqual(10d, contentSpacing.ActualHeight);

                var rotation = indicator.RenderTransform as RotateTransform;
                Assert.IsNotNull(rotation);
                var initialAngle = rotation.Angle;
                WaitForDispatcher(TimeSpan.FromMilliseconds(250));
                Assert.AreNotEqual(initialAngle, rotation.Angle);

                loading.Orientation = Orientation.Horizontal;
                window.UpdateLayout();
                Assert.AreEqual(Orientation.Horizontal, indicatorPanel.Orientation);
                Assert.AreEqual(10d, contentSpacing.ActualWidth);
                Assert.AreEqual(0d, contentSpacing.ActualHeight);

                loading.FlowDirection = FlowDirection.RightToLeft;
                window.UpdateLayout();
                Assert.IsGreaterThan(
                    loadingText.PointToScreen(new Point()).X,
                    indicator.PointToScreen(new Point()).X);

                loading.IsContentInteractionBlocked = false;
                window.UpdateLayout();
                Assert.IsFalse(loadingLayer.IsHitTestVisible);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.UpdateLayout();
                Assert.AreEqual(20d, loading.IndicatorSize);
                Assert.AreEqual(20d, indicator.Width);
                Assert.AreEqual(8d, contentSpacing.ActualWidth);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Comfortable);
                window.UpdateLayout();
                Assert.AreEqual(28d, loading.IndicatorSize);
                Assert.AreEqual(28d, indicator.Width);
                Assert.AreEqual(12d, contentSpacing.ActualWidth);

                loading.IsLoading = false;
                window.UpdateLayout();
                Assert.AreEqual(Visibility.Collapsed, loadingLayer.Visibility);
                Assert.AreSame(content, loading.Content);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void LoadingValidatesIndicatorSizeAndExposesAutomationSemantics()
        {
            var loading = new TestZenLoading
            {
                LoadingText = "正在读取数据"
            };

            Assert.ThrowsExactly<ArgumentException>(() => loading.IndicatorSize = 0d);
            Assert.ThrowsExactly<ArgumentException>(() => loading.IndicatorSize = double.NaN);
            Assert.ThrowsExactly<ArgumentException>(
                () => loading.Orientation = (Orientation)99);
            Assert.AreEqual(typeof(ZenLoading), loading.ExposedDefaultStyleKey);
            Assert.AreEqual(
                AutomationControlType.ProgressBar,
                loading.ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual("正在读取数据", loading.ExposedAutomationPeer.GetName());
            Assert.AreEqual(AutomationLiveSetting.Polite, AutomationProperties.GetLiveSetting(loading));
        }

        [TestMethod]
        public void LoadingUsesThemeOverlayAndHighContrastState()
        {
            var loading = new ZenLoading
            {
                Width = 180,
                Height = 100,
                IsLoading = true
            };
            var window = CreateTestWindow(loading, 240, 160);
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

                var lightOverlay = loading.OverlayBackground as SolidColorBrush;
                Assert.IsNotNull(lightOverlay);
                Assert.AreEqual(Color.FromArgb(0xE6, 0xFF, 0xFF, 0xFF), lightOverlay.Color);

                ZenThemeManager.ApplyTheme(window.Resources, ZenTheme.Dark, false);
                window.UpdateLayout();
                var darkOverlay = loading.OverlayBackground as SolidColorBrush;
                Assert.IsNotNull(darkOverlay);
                Assert.AreEqual(Color.FromArgb(0xE6, 0x1D, 0x21, 0x29), darkOverlay.Color);

                loading.IsEnabled = false;
                ZenThemeManager.ApplyTheme(window.Resources, ZenTheme.HighContrast, false);
                window.UpdateLayout();
                var indicator = loading.Template.FindName("Indicator", loading) as FrameworkElement;
                Assert.IsNotNull(indicator);
                Assert.AreEqual(1d, indicator.Opacity);
            }
            finally
            {
                window.Close();
            }
        }

        private sealed class TestZenLoading : ZenLoading
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;

            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
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
