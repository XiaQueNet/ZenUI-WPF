using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class PopoverTests
    {
        [TestMethod]
        public void GenericThemeProvidesMatchingPopoverAndToolTipStyles()
        {
            var popover = new ZenPopover();
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };

            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenPopover)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ToolTip)]);
            Assert.IsInstanceOfType<Style>(dictionary["ZenCalloutChromeStyle"]);
            Assert.AreEqual(new Thickness(14, 10, 14, 10), dictionary["ZenCalloutPadding"]);
            Assert.AreEqual(new CornerRadius(6), dictionary["ZenCalloutCornerRadius"]);
            Assert.AreEqual(360d, dictionary["ZenCalloutMaxWidth"]);
            Assert.AreEqual(24d, dictionary["ZenCalloutTriggerSize"]);
            Assert.AreEqual(4d, dictionary["ZenCalloutTargetGap"]);
            Assert.IsNull(popover.Anchor);
            Assert.AreEqual(new CornerRadius(6), popover.CornerRadius);
            Assert.IsTrue(popover.ShowArrow);
            Assert.AreEqual(0d, popover.MinPopupWidth);
            Assert.AreEqual(360d, popover.MaxPopupWidth);
            Assert.AreEqual(4d, popover.TargetGap);
            Assert.IsNull(popover.AnchorButtonStyle);
        }

        [TestMethod]
        public void PopoverTemplateConnectsTriggerAndPopupToIsOpen()
        {
            var anchorButtonStyle = new Style(typeof(ToggleButton));
            var popover = new ZenPopover
            {
                Content = "气泡内容",
                Placement = PlacementMode.Top,
                Padding = new Thickness(18, 12, 18, 12),
                Background = Brushes.Beige,
                BorderBrush = Brushes.Orange,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                ShowArrow = false,
                TargetGap = 8,
                MinPopupWidth = 120,
                MaxPopupWidth = 280,
                AnchorButtonStyle = anchorButtonStyle
            };
            var window = CreateTestWindow(popover, 320, 240);
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
                popover.ApplyTemplate();

                var trigger = popover.Template.FindName("PART_Trigger", popover) as ToggleButton;
                var popup = popover.Template.FindName("PART_Popup", popover) as Popup;
                var chrome = popup?.Child as ZenCalloutChrome;

                Assert.IsNotNull(trigger);
                Assert.IsNotNull(popup);
                Assert.IsNotNull(chrome);
                Assert.IsNull(trigger.Content);
                Assert.AreEqual(HorizontalAlignment.Left, popover.HorizontalAlignment);
                Assert.AreEqual(anchorButtonStyle, trigger.Style);
                Assert.AreEqual(trigger, popup.PlacementTarget);
                Assert.AreEqual(PlacementMode.Top, popup.Placement);
                Assert.AreEqual(popover.Padding, chrome.Padding);
                Assert.AreEqual(popover.Background, chrome.Background);
                Assert.AreEqual(popover.BorderBrush, chrome.BorderBrush);
                Assert.AreEqual(popover.BorderThickness, chrome.BorderThickness);
                Assert.AreEqual(popover.CornerRadius, chrome.CornerRadius);
                Assert.AreEqual(popover.ShowArrow, chrome.ShowArrow);
                Assert.AreEqual(popover.MinPopupWidth, chrome.MinWidth);
                Assert.AreEqual(popover.MaxPopupWidth, chrome.MaxWidth);

                popover.IsOpen = true;
                window.UpdateLayout();

                Assert.IsTrue(trigger.IsChecked);
                Assert.IsTrue(popup.IsOpen);
                Assert.AreEqual(-8d, popup.VerticalOffset);

                popover.IsOpen = false;
                window.UpdateLayout();

                Assert.IsFalse(trigger.IsChecked);
                Assert.IsFalse(popup.IsOpen);
            }
            finally
            {
                popover.IsOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void DefaultAnchorUsesQuestionMarkChrome()
        {
            var popover = new ZenPopover
            {
                Content = "气泡内容"
            };
            var window = CreateTestWindow(popover, 320, 240);
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
                popover.ApplyTemplate();

                var trigger = popover.Template.FindName("PART_Trigger", popover) as ToggleButton;
                Assert.IsNotNull(trigger);
                trigger.ApplyTemplate();

                var defaultAnchor = trigger.Template.FindName(
                    "DefaultAnchorBorder",
                    trigger) as FrameworkElement;
                var customAnchor = trigger.Template.FindName(
                    "CustomAnchorPresenter",
                    trigger) as ContentPresenter;

                Assert.IsNull(trigger.Content);
                Assert.IsNotNull(defaultAnchor);
                Assert.IsNotNull(customAnchor);
                Assert.AreEqual(Visibility.Visible, defaultAnchor.Visibility);
                Assert.AreEqual(Visibility.Collapsed, customAnchor.Visibility);
                Assert.AreEqual("?", System.Windows.Automation.AutomationProperties.GetName(trigger));
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void CustomAnchorReplacesDefaultQuestionMarkChrome()
        {
            var anchor = new TextBlock
            {
                Text = "查看说明"
            };
            var popover = new ZenPopover
            {
                Anchor = anchor,
                Content = "气泡内容"
            };
            var window = CreateTestWindow(popover, 320, 240);
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
                popover.ApplyTemplate();

                var trigger = popover.Template.FindName("PART_Trigger", popover) as ToggleButton;
                Assert.IsNotNull(trigger);
                trigger.ApplyTemplate();

                var defaultAnchor = trigger.Template.FindName(
                    "DefaultAnchorBorder",
                    trigger) as FrameworkElement;
                var customAnchor = trigger.Template.FindName(
                    "CustomAnchorPresenter",
                    trigger) as ContentPresenter;

                Assert.AreEqual(anchor, trigger.Content);
                Assert.IsNotNull(defaultAnchor);
                Assert.IsNotNull(customAnchor);
                Assert.AreEqual(Visibility.Collapsed, defaultAnchor.Visibility);
                Assert.AreEqual(Visibility.Visible, customAnchor.Visibility);
                Assert.AreEqual(anchor, customAnchor.Content);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void TopPopoverCentersCalloutArrowOverTrigger()
        {
            var popover = new ZenPopover
            {
                Content = new TextBlock
                {
                    Width = 260,
                    Text = "较宽的气泡内容"
                },
                Placement = PlacementMode.Top
            };
            var host = new Grid
            {
                Width = 500,
                Height = 300,
                Children = { popover }
            };
            popover.HorizontalAlignment = HorizontalAlignment.Center;
            popover.VerticalAlignment = VerticalAlignment.Center;
            var window = CreateTestWindow(host, 500, 300);
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
                popover.ApplyTemplate();
                popover.IsOpen = true;
                window.UpdateLayout();

                var trigger = popover.Template.FindName("PART_Trigger", popover) as ToggleButton;
                var popup = popover.Template.FindName("PART_Popup", popover) as Popup;
                var callout = popup?.Child as FrameworkElement;

                Assert.IsNotNull(trigger);
                Assert.IsNotNull(callout);

                var triggerOrigin = trigger.PointToScreen(new Point());
                var triggerEnd = trigger.PointToScreen(new Point(trigger.ActualWidth, 0d));
                var calloutOrigin = callout.PointToScreen(new Point());
                var calloutEnd = callout.PointToScreen(new Point(callout.ActualWidth, 0d));
                var triggerCenter = (triggerOrigin.X + triggerEnd.X) / 2d;
                var calloutCenter = (calloutOrigin.X + calloutEnd.X) / 2d;
                var calloutBottom = callout.PointToScreen(
                    new Point(0d, callout.ActualHeight));
                var expectedCalloutBottom = trigger.PointToScreen(
                    new Point(0d, -popover.TargetGap));

                Assert.AreEqual(
                    triggerCenter,
                    calloutCenter,
                    1.5d,
                    $"TriggerWidth={trigger.ActualWidth}; CalloutWidth={callout.ActualWidth}; " +
                    $"Offset={popup.HorizontalOffset}; TriggerX={triggerOrigin.X}; CalloutX={calloutOrigin.X}");
                Assert.AreEqual(
                    expectedCalloutBottom.Y,
                    calloutBottom.Y,
                    1.5d,
                    $"TargetGap={popover.TargetGap}; VerticalOffset={popup.VerticalOffset}");
            }
            finally
            {
                popover.IsOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }
    }
}
