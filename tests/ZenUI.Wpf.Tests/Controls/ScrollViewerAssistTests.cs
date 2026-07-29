using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class ScrollViewerAssistTests
    {
        [TestMethod]
        public void NestedScrollableViewerKeepsMouseWheelEvent()
        {
            var innerContent = new Border { Height = 500 };
            var innerScroller = new ScrollViewer
            {
                Height = 100,
                Content = innerContent,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            ScrollViewerAssist.SetIsMouseWheelChainingEnabled(
                innerScroller,
                true);

            var outerContent = new StackPanel();
            outerContent.Children.Add(innerScroller);
            outerContent.Children.Add(new Border { Height = 500 });
            var outerScroller = new ScrollViewer
            {
                Height = 200,
                Content = outerContent,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            ScrollViewerAssist.SetIsMouseWheelChainingEnabled(
                outerScroller,
                true);
            var window = CreateTestWindow(outerScroller, 320, 260);

            try
            {
                window.Show();
                window.UpdateLayout();
                outerScroller.ScrollToEnd();
                innerScroller.ScrollToTop();
                window.UpdateLayout();

                Assert.IsGreaterThan(0d, innerScroller.ScrollableHeight);
                Assert.AreEqual(
                    outerScroller.ScrollableHeight,
                    outerScroller.VerticalOffset);

                var wheelEvent = new MouseWheelEventArgs(
                    Mouse.PrimaryDevice,
                    Environment.TickCount,
                    -120)
                {
                    RoutedEvent = UIElement.PreviewMouseWheelEvent
                };
                innerContent.RaiseEvent(wheelEvent);

                Assert.IsFalse(
                    wheelEvent.Handled,
                    "The outer viewer must not intercept a wheel event that the nested viewer can consume.");
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void ScrollableViewerDoesNotForwardMouseWheelAtBoundary()
        {
            var innerContent = new Border { Height = 500 };
            var innerScroller = new ScrollViewer
            {
                Height = 100,
                Content = innerContent,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            ScrollViewerAssist.SetIsMouseWheelChainingEnabled(
                innerScroller,
                true);

            var outerContent = new StackPanel();
            outerContent.Children.Add(innerScroller);
            outerContent.Children.Add(new Border { Height = 500 });
            var outerScroller = new ScrollViewer
            {
                Height = 200,
                Content = outerContent,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            var window = CreateTestWindow(outerScroller, 320, 260);

            try
            {
                window.Show();
                window.UpdateLayout();
                innerScroller.ScrollToEnd();
                window.UpdateLayout();

                Assert.AreEqual(
                    innerScroller.ScrollableHeight,
                    innerScroller.VerticalOffset);

                var forwardedDelta = 0;
                outerScroller.AddHandler(
                    UIElement.MouseWheelEvent,
                    new MouseWheelEventHandler(
                        (sender, args) => forwardedDelta = args.Delta),
                    true);

                var wheelEvent = new MouseWheelEventArgs(
                    Mouse.PrimaryDevice,
                    Environment.TickCount,
                    -120)
                {
                    RoutedEvent = UIElement.PreviewMouseWheelEvent
                };
                innerContent.RaiseEvent(wheelEvent);

                Assert.IsFalse(wheelEvent.Handled);
                Assert.AreEqual(0, forwardedDelta);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
