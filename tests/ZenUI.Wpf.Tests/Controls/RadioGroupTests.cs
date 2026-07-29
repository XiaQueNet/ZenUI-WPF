using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class RadioGroupTests
    {
        [TestMethod]
        public void RadioGroupGeneratesDedicatedContainersAndKeepsSingleSelection()
        {
            var group = new ZenRadioGroup
            {
                Width = 600,
                ItemsSource = new[] { "全部", "用品百货", "测试" },
                SelectedIndex = 1
            };
            var window = CreateTestWindow(group, 660, 120);

            try
            {
                window.Show();
                window.UpdateLayout();

                var containers = Enumerable.Range(0, group.Items.Count)
                    .Select(index => group.ItemContainerGenerator.ContainerFromIndex(index))
                    .ToList();

                Assert.IsTrue(containers.All(item => item is ZenRadioItem));
                Assert.AreEqual("用品百货", group.SelectedItem);

                group.SelectedIndex = 2;
                Assert.AreEqual(2, group.SelectedIndex);
                Assert.IsTrue(((ZenRadioItem)containers[2]).IsSelected);
                Assert.IsFalse(((ZenRadioItem)containers[1]).IsSelected);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void RadioGroupLaysOutEqualItemsWithConfiguredSpacing()
        {
            var group = new ZenRadioGroup
            {
                Width = 620,
                Spacing = 10,
                IsItemWidthUniform = true,
                ItemsSource = new List<string>
                {
                    "全部",
                    "用品百货",
                    "测试",
                    "运动品牌"
                }
            };
            var window = CreateTestWindow(group, 680, 120);

            try
            {
                window.Show();
                window.UpdateLayout();

                var items = Enumerable.Range(0, group.Items.Count)
                    .Select(index => (ZenRadioItem)group.ItemContainerGenerator.ContainerFromIndex(index))
                    .ToList();

                Assert.IsTrue(items.All(item => item != null));
                Assert.AreEqual(items[0].ActualWidth, items[3].ActualWidth, 0.1d);

                var firstRight = items[0].TranslatePoint(
                    new Point(items[0].ActualWidth, 0),
                    group).X;
                var secondLeft = items[1].TranslatePoint(new Point(), group).X;
                Assert.AreEqual(10d, secondLeft - firstRight, 0.1d);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void RadioGroupSupportsVerticalNonUniformLayout()
        {
            var group = new ZenRadioGroup
            {
                Width = 260,
                Orientation = Orientation.Vertical,
                Spacing = 6,
                IsItemWidthUniform = false
            };
            group.Items.Add("标准版");
            group.Items.Add("带有更多说明的专业版");
            var window = CreateTestWindow(group, 320, 180);

            try
            {
                window.Show();
                window.UpdateLayout();

                var first = (ZenRadioItem)group.ItemContainerGenerator.ContainerFromIndex(0);
                var second = (ZenRadioItem)group.ItemContainerGenerator.ContainerFromIndex(1);
                var firstBottom = first.TranslatePoint(
                    new Point(0, first.ActualHeight),
                    group).Y;
                var secondTop = second.TranslatePoint(new Point(), group).Y;

                Assert.AreEqual(6d, secondTop - firstBottom, 0.1d);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void RadioGroupSupportsClickKeyboardAndAutomationSemantics()
        {
            var group = new TestZenRadioGroup
            {
                Width = 480
            };
            group.Items.Add("第一项");
            group.Items.Add("第二项");
            group.Items.Add("第三项");
            var window = CreateTestWindow(group, 540, 120);

            try
            {
                window.Show();
                window.UpdateLayout();

                var second =
                    (ZenRadioItem)group.ItemContainerGenerator.ContainerFromIndex(1);
                second.RaiseEvent(new MouseButtonEventArgs(
                    Mouse.PrimaryDevice,
                    System.Environment.TickCount,
                    MouseButton.Left)
                {
                    RoutedEvent = UIElement.MouseLeftButtonDownEvent
                });
                Assert.AreEqual(1, group.SelectedIndex);

                group.RaiseEvent(new KeyEventArgs(
                    Keyboard.PrimaryDevice,
                    PresentationSource.FromVisual(window),
                    System.Environment.TickCount,
                    Key.Right)
                {
                    RoutedEvent = Keyboard.KeyDownEvent
                });
                Assert.AreEqual(2, group.SelectedIndex);

                var peer = group.ExposedAutomationPeer;
                Assert.AreEqual(
                    AutomationControlType.Group,
                    peer.GetAutomationControlType());
                Assert.AreEqual(
                    AutomationControlType.RadioButton,
                    peer.GetChildren().First().GetAutomationControlType());
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void RadioGroupForwardsMouseWheelToOuterScrollerWhenItCannotScroll()
        {
            var group = new ZenRadioGroup
            {
                ItemsSource = new[] { "第一项", "第二项" },
                Orientation = Orientation.Vertical
            };
            var content = new StackPanel();
            content.Children.Add(group);
            content.Children.Add(new Border { Height = 600 });
            var outerScroller = new ScrollViewer
            {
                Height = 120,
                Content = content,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            var window = CreateTestWindow(outerScroller, 320, 180);

            try
            {
                window.Show();
                window.UpdateLayout();

                var forwardedDelta = 0;
                outerScroller.AddHandler(
                    UIElement.MouseWheelEvent,
                    new MouseWheelEventHandler(
                        (sender, args) => forwardedDelta = args.Delta),
                    true);

                var firstItem =
                    (ZenRadioItem)group.ItemContainerGenerator.ContainerFromIndex(0);
                var wheelEvent = new MouseWheelEventArgs(
                    Mouse.PrimaryDevice,
                    System.Environment.TickCount,
                    -120)
                {
                    RoutedEvent = UIElement.PreviewMouseWheelEvent
                };
                firstItem.RaiseEvent(wheelEvent);

                Assert.IsTrue(wheelEvent.Handled);
                Assert.AreEqual(-120, forwardedDelta);
            }
            finally
            {
                window.Close();
            }
        }

        private sealed class TestZenRadioGroup : ZenRadioGroup
        {
            public AutomationPeer ExposedAutomationPeer =>
                OnCreateAutomationPeer();
        }
    }
}
