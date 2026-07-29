using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class ContextMenuTests
    {
        [TestInitialize]
        public void EnsureApplicationExists()
        {
            if (Application.Current == null)
            {
                _ = new Application();
            }
        }

        [TestMethod]
        public void GenericThemeProvidesContextMenuStylesAndTokens()
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var contextMenu = new ZenContextMenu();

            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenContextMenu)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenMenuItem)]);
            Assert.IsInstanceOfType<Style>(dictionary["ZenContextMenuSeparatorStyle"]);
            Assert.AreEqual(184d, dictionary["ZenContextMenuMinWidth"]);
            Assert.AreEqual(new Thickness(5), dictionary["ZenContextMenuPadding"]);
            Assert.AreEqual(new CornerRadius(8), contextMenu.CornerRadius);
        }

        [TestMethod]
        public void ContextMenuCreatesZenContainersForDataItems()
        {
            var target = new Border { Width = 120, Height = 40 };
            var contextMenu = new ZenContextMenu
            {
                PlacementTarget = target
            };
            contextMenu.Items.Add("打开");
            target.ContextMenu = contextMenu;
            var window = CreateTestWindow(target, 240, 160);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });

            try
            {
                window.Show();
                contextMenu.IsOpen = true;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                contextMenu.UpdateLayout();

                var item = contextMenu.ItemContainerGenerator.ContainerFromIndex(0) as ZenMenuItem;
                Assert.IsNotNull(item);
                item.ApplyTemplate();
                Assert.IsNotNull(item.Template.FindName("ItemBorder", item));
                Assert.AreEqual("打开", item.Header);
            }
            finally
            {
                contextMenu.IsOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void MenuItemTemplateSupportsCheckedGestureAndSubmenuStates()
        {
            var item = new ZenMenuItem
            {
                Header = "固定",
                InputGestureText = "Ctrl+K",
                IsCheckable = true,
                IsChecked = true
            };
            item.Items.Add(new ZenMenuItem { Header = "子项" });
            var contextMenu = new ZenContextMenu
            {
                Items = { item }
            };
            var target = new Border { Width = 120, Height = 40, ContextMenu = contextMenu };
            contextMenu.PlacementTarget = target;
            var window = CreateTestWindow(target, 240, 160);

            try
            {
                window.Show();
                contextMenu.IsOpen = true;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                item.ApplyTemplate();

                var checkGlyph = item.Template.FindName("CheckGlyph", item) as FrameworkElement;
                var submenuArrow = item.Template.FindName("SubmenuArrow", item) as FrameworkElement;
                var gestureText = item.Template.FindName("GestureText", item) as TextBlock;
                Assert.IsNotNull(checkGlyph);
                Assert.IsNotNull(submenuArrow);
                Assert.IsNotNull(gestureText);
                Assert.AreEqual(Visibility.Visible, checkGlyph.Visibility);
                Assert.AreEqual(Visibility.Visible, submenuArrow.Visibility);
                Assert.AreEqual("Ctrl+K", gestureText.Text);
            }
            finally
            {
                contextMenu.IsOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void ContextMenuItemsRespondToDensityChanges()
        {
            var item = new ZenMenuItem { Header = "打开" };
            var contextMenu = new ZenContextMenu
            {
                Items = { item }
            };
            var target = new Border { Width = 120, Height = 40, ContextMenu = contextMenu };
            contextMenu.PlacementTarget = target;
            var window = CreateTestWindow(target, 240, 160);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });

            try
            {
                window.Show();
                contextMenu.IsOpen = true;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                Assert.AreEqual(36d, item.MinHeight);
                Assert.AreEqual(new Thickness(8, 4, 8, 4), item.Padding);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                Assert.AreEqual(32d, item.MinHeight);
                Assert.AreEqual(new Thickness(7, 2, 7, 2), item.Padding);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Comfortable);
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                Assert.AreEqual(42d, item.MinHeight);
                Assert.AreEqual(new Thickness(10, 6, 10, 6), item.Padding);
            }
            finally
            {
                contextMenu.IsOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }
    }
}
