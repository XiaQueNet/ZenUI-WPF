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
    public class ListBoxTests
    {
        [TestMethod]
        public void ListBoxPreservesSelectionModesAndVirtualization()
        {
            var listBox = new ZenListBox
            {
                Width = 260,
                Height = 120,
                SelectionMode = SelectionMode.Extended
            };
            listBox.Items.Add("第一项");
            listBox.Items.Add("第二项");
            listBox.Items.Add("第三项");
            var window = CreateTestWindow(listBox, 320, 180);

            try
            {
                window.Show();
                window.UpdateLayout();

                var first = listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                var second = listBox.ItemContainerGenerator.ContainerFromIndex(1) as ListBoxItem;
                Assert.IsNotNull(first);
                Assert.IsNotNull(second);

                first.IsSelected = true;
                second.IsSelected = true;
                Assert.HasCount(2, listBox.SelectedItems);
                Assert.AreEqual(SelectionMode.Extended, listBox.SelectionMode);
                Assert.IsTrue(VirtualizingPanel.GetIsVirtualizing(listBox));
                Assert.AreEqual(
                    VirtualizationMode.Recycling,
                    VirtualizingPanel.GetVirtualizationMode(listBox));
                Assert.IsTrue(ScrollViewer.GetCanContentScroll(listBox));
                Assert.IsInstanceOfType<VirtualizingStackPanel>(
                    FindVisualDescendant<VirtualizingStackPanel>(listBox));

                listBox.SelectionMode = SelectionMode.Single;
                listBox.SelectedIndex = 0;
                first.Focus();
                var keyEvent = new KeyEventArgs(
                    Keyboard.PrimaryDevice,
                    PresentationSource.FromVisual(window),
                    Environment.TickCount,
                    Key.Down)
                {
                    RoutedEvent = Keyboard.KeyDownEvent
                };
                listBox.RaiseEvent(keyEvent);
                Assert.AreEqual(1, listBox.SelectedIndex);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
