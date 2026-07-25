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
    public class ControlTokenTests
    {
        [TestMethod]
        public void TypographyTokensCanBeOverriddenInWindowResources()
        {
            var alert = new ZenAlert { Content = "Saved" };
            var dataGrid = new ZenDataGrid { Height = 100 };
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name" });

            var panel = new StackPanel();
            panel.Children.Add(alert);
            panel.Children.Add(dataGrid);

            var window = CreateTestWindow(panel, 320, 180);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });
            window.Resources["ZenFontSizeCaption"] = 18d;
            window.Resources["ZenFontWeightBold"] = FontWeights.Regular;
            window.Resources["ZenFontWeightSemibold"] = FontWeights.Bold;

            try
            {
                window.Show();
                window.UpdateLayout();

                var iconText = alert.Template.FindName("IconText", alert) as TextBlock;
                var columnHeader = FindVisualDescendants<DataGridColumnHeader>(dataGrid)
                    .FirstOrDefault(header => header.Column != null);

                Assert.IsNotNull(iconText);
                Assert.IsNotNull(columnHeader);
                Assert.AreEqual(18d, iconText.FontSize);
                Assert.AreEqual(FontWeights.Regular, iconText.FontWeight);
                Assert.AreEqual(FontWeights.Bold, columnHeader.FontWeight);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void InputMetricTokensCanBeOverriddenInWindowResources()
        {
            var textBox = new ZenTextBox();
            var window = CreateTestWindow(textBox, 240, 100);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });
            window.Resources["ZenInputControlMinHeight"] = 44d;
            window.Resources["ZenInputControlPadding"] = new Thickness(12, 6, 12, 6);
            window.Resources["ZenInputControlCornerRadius"] = new CornerRadius(8);
            window.Resources["ZenControlBorderThickness"] = new Thickness(2);

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.AreEqual(44d, textBox.MinHeight);
                Assert.AreEqual(new Thickness(12, 6, 12, 6), textBox.Padding);
                Assert.AreEqual(new CornerRadius(8), textBox.CornerRadius);
                Assert.AreEqual(new Thickness(2), textBox.BorderThickness);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void ComponentMetricTokensCanBeOverriddenInWindowResources()
        {
            var button = new ZenButton
            {
                Appearance = ButtonAppearance.Outlined,
                Content = "Action"
            };
            var listBox = new ZenListBox
            {
                Height = 80
            };
            listBox.Items.Add("Item");
            var panel = new StackPanel();
            panel.Children.Add(button);
            panel.Children.Add(listBox);
            var window = CreateTestWindow(panel, 260, 160);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });
            window.Resources["ZenControlBorderThickness"] = new Thickness(2);
            window.Resources["ZenButtonPadding"] = new Thickness(14, 6, 14, 6);
            window.Resources["ZenButtonCornerRadius"] = new CornerRadius(12);
            window.Resources["ZenListBoxPadding"] = new Thickness(6);
            window.Resources["ZenListBoxCornerRadius"] = new CornerRadius(10);
            window.Resources["ZenListBoxItemPadding"] = new Thickness(16, 10, 16, 10);
            window.Resources["ZenListBoxItemMargin"] = new Thickness(0, 2, 0, 2);
            window.Resources["ZenListBoxItemCornerRadius"] = new CornerRadius(7);

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.AreEqual(new Thickness(14, 6, 14, 6), button.Padding);
                Assert.AreEqual(new CornerRadius(12), button.CornerRadius);
                Assert.AreEqual(new Thickness(2), button.BorderThickness);
                Assert.AreEqual(new Thickness(6), listBox.Padding);
                Assert.AreEqual(new CornerRadius(10), listBox.CornerRadius);
                Assert.AreEqual(new Thickness(2), listBox.BorderThickness);

                var item = listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                Assert.IsNotNull(item);
                item.ApplyTemplate();
                Assert.AreEqual(new Thickness(16, 10, 16, 10), item.Padding);
                Assert.AreEqual(new Thickness(0, 2, 0, 2), item.Margin);
                var itemBorder = item.Template.FindName("ItemBorder", item) as Border;
                Assert.IsNotNull(itemBorder);
                Assert.AreEqual(new CornerRadius(7), itemBorder.CornerRadius);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
