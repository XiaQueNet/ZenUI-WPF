using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class ExpanderTests
    {
        [TestMethod]
        public void DefaultsAndGenericStyleFollowControlContract()
        {
            var expander = new TestZenExpander();
            Assert.AreEqual(typeof(ZenExpander), expander.ExposedDefaultStyleKey);
            Assert.AreEqual(new CornerRadius(8), expander.CornerRadius);
            Assert.AreEqual(new Thickness(14, 12, 14, 12), expander.HeaderPadding);
            Assert.AreEqual(16d, expander.GlyphSize);

            var dictionary = LoadGenericTheme();
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenExpander)]);
            Assert.IsInstanceOfType<Style>(dictionary["ZenExpanderStyle"]);
            Assert.AreEqual(new Thickness(14, 12, 14, 12), dictionary["ZenExpanderHeaderPadding"]);
            Assert.AreEqual(new Thickness(14, 10, 14, 14), dictionary["ZenExpanderContentPadding"]);
            Assert.AreEqual(new CornerRadius(8), dictionary["ZenExpanderCornerRadius"]);
            Assert.AreEqual(16d, dictionary["ZenExpanderGlyphSize"]);
        }

        [TestMethod]
        public void HeaderTogglesExpandedStateAndContentVisibility()
        {
            var expander = new ZenExpander
            {
                Header = "_Details",
                Content = new TextBlock { Text = "Content" }
            };
            var window = CreateTestWindow(expander, 320, 180);
            window.Resources.MergedDictionaries.Add(LoadGenericTheme());

            try
            {
                window.Show();
                window.UpdateLayout();

                var header = expander.Template.FindName("HeaderSite", expander) as ToggleButton;
                var content = expander.Template.FindName("ContentBorder", expander) as Border;
                Assert.IsNotNull(header);
                Assert.IsNotNull(content);
                Assert.AreEqual(Visibility.Collapsed, content.Visibility);

                header.IsChecked = true;
                window.UpdateLayout();
                Assert.IsTrue(expander.IsExpanded);
                Assert.AreEqual(Visibility.Visible, content.Visibility);

                header.IsChecked = false;
                Assert.IsFalse(expander.IsExpanded);
                Assert.AreEqual(Visibility.Collapsed, content.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void TemplateDocksHeaderForEveryExpandDirection()
        {
            AssertHeaderDock(ExpandDirection.Down, Dock.Top);
            AssertHeaderDock(ExpandDirection.Up, Dock.Bottom);
            AssertHeaderDock(ExpandDirection.Left, Dock.Right);
            AssertHeaderDock(ExpandDirection.Right, Dock.Left);
        }

        [TestMethod]
        public void GlyphSizeRejectsInvalidValues()
        {
            var expander = new ZenExpander();

            Assert.ThrowsExactly<ArgumentException>(() => expander.GlyphSize = -1d);
            Assert.ThrowsExactly<ArgumentException>(() => expander.GlyphSize = double.NaN);
            Assert.ThrowsExactly<ArgumentException>(() => expander.GlyphSize = double.PositiveInfinity);
        }

        [TestMethod]
        public void AutomationPeerExposesNativeExpandCollapseSemantics()
        {
            var expander = new TestZenExpander { Header = "Details" };
            var peer = expander.ExposedAutomationPeer;

            Assert.AreEqual(nameof(ZenExpander), peer.GetClassName());
            Assert.AreEqual(AutomationControlType.Group, peer.GetAutomationControlType());
            Assert.IsInstanceOfType<IExpandCollapseProvider>(
                peer.GetPattern(PatternInterface.ExpandCollapse));
        }

        private static ResourceDictionary LoadGenericTheme()
        {
            _ = new ZenExpander();
            return new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
        }

        private static void AssertHeaderDock(ExpandDirection direction, Dock expectedDock)
        {
            var expander = new ZenExpander
            {
                Header = direction.ToString(),
                Content = "Content",
                ExpandDirection = direction,
                IsExpanded = true
            };
            var window = CreateTestWindow(expander, 240, 140);
            window.Resources.MergedDictionaries.Add(LoadGenericTheme());

            try
            {
                window.Show();
                window.UpdateLayout();

                var header = expander.Template.FindName("HeaderSite", expander) as ToggleButton;
                Assert.IsNotNull(header);
                Assert.AreEqual(expectedDock, DockPanel.GetDock(header));
            }
            finally
            {
                window.Close();
            }
        }

    }
}
