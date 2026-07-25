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
    public class ComboBoxTests
    {
        [TestMethod]
        public void ComboBoxPopupMetricTokensCanBeOverriddenInWindowResources()
        {
            var comboBox = new ZenComboBox();
            comboBox.Items.Add("Item");
            var window = CreateTestWindow(comboBox, 320, 240);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });
            window.Resources["ZenControlBorderThickness"] = new Thickness(2);
            window.Resources["ZenComboBoxPopupMargin"] = new Thickness(0, 6, 0, 0);
            window.Resources["ZenComboBoxPopupPadding"] = new Thickness(7);
            window.Resources["ZenComboBoxPopupCornerRadius"] = new CornerRadius(9);

            try
            {
                window.Show();
                comboBox.IsDropDownOpen = true;
                window.UpdateLayout();

                var comboPopup = comboBox.Template.FindName("PART_Popup", comboBox) as Popup;
                Assert.IsNotNull(comboPopup);
                Assert.IsTrue(comboPopup.IsOpen);
                var popupBorder = comboPopup.Child as Border;
                Assert.IsNotNull(popupBorder);
                Assert.AreEqual(new Thickness(0, 6, 0, 0), popupBorder.Margin);
                Assert.AreEqual(new Thickness(7), popupBorder.Padding);
                Assert.AreEqual(new Thickness(2), popupBorder.BorderThickness);
                Assert.AreEqual(new CornerRadius(9), popupBorder.CornerRadius);
            }
            finally
            {
                comboBox.IsDropDownOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void ComboBoxHonorsItemTemplateForSelectedItem()
        {
            var text = new FrameworkElementFactory(typeof(TextBlock));
            text.SetBinding(TextBlock.TextProperty, new Binding(nameof(DisplayItem.DisplayName)));
            var itemTemplate = new DataTemplate { VisualTree = text };
            var comboBox = new ZenComboBox
            {
                ItemTemplate = itemTemplate,
                ItemsSource = new[] { new DisplayItem("浅色") },
                SelectedIndex = 0,
                Width = 160
            };
            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Content = comboBox
            };

            try
            {
                window.Show();
                window.UpdateLayout();

                var selectionPresenter =
                    comboBox.Template.FindName("SelectionPresenter", comboBox) as ContentPresenter;
                var dropDownArrow =
                    comboBox.Template.FindName("DropDownArrow", comboBox) as FrameworkElement;
                var watermark =
                    comboBox.Template.FindName("WatermarkText", comboBox) as TextBlock;
                Assert.IsNotNull(selectionPresenter);
                Assert.IsNotNull(dropDownArrow);
                Assert.IsNotNull(watermark);
                Assert.AreEqual(comboBox.Padding, selectionPresenter.Margin);
                Assert.AreEqual(comboBox.Padding, watermark.Margin);
                Assert.AreEqual(0, Grid.GetColumn(selectionPresenter));
                Assert.AreEqual(1, Grid.GetColumn(dropDownArrow));
                Assert.AreSame(itemTemplate, comboBox.SelectionBoxItemTemplate);
                Assert.AreSame(itemTemplate, selectionPresenter.ContentTemplate);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void EditableComboBoxHidesWatermarkAfterTextIsEntered()
        {
            var comboBox = new ZenComboBox
            {
                IsEditable = true,
                Watermark = "请输入",
                Width = 180
            };
            var window = CreateTestWindow(comboBox, 220, 100);

            try
            {
                window.Show();
                window.UpdateLayout();

                var editableTextBox =
                    comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
                var watermark =
                    comboBox.Template.FindName("WatermarkText", comboBox) as TextBlock;
                Assert.IsNotNull(editableTextBox);
                Assert.IsNotNull(watermark);
                Assert.AreEqual(Visibility.Visible, watermark.Visibility);

                editableTextBox.Text = "自定义值";
                window.UpdateLayout();

                Assert.AreEqual("自定义值", comboBox.Text);
                Assert.AreEqual(-1, comboBox.SelectedIndex);
                Assert.AreEqual(Visibility.Collapsed, watermark.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void ComboBoxItemsUseListSelectionStateTokens()
        {
            var comboBox = new ZenComboBox { Width = 180 };
            comboBox.Items.Add("第一项");
            comboBox.Items.Add("第二项");
            var window = CreateTestWindow(comboBox, 220, 180);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            });

            try
            {
                window.Show();
                comboBox.IsDropDownOpen = true;
                window.UpdateLayout();

                var item = comboBox.ItemContainerGenerator.ContainerFromIndex(0) as ComboBoxItem;
                Assert.IsNotNull(item);
                item.ApplyTemplate();
                var itemBorder = item.Template.FindName("ItemBorder", item) as Border;
                Assert.IsNotNull(itemBorder);
                Assert.AreEqual(new Thickness(0, 1, 0, 1), item.Margin);

                Keyboard.Focus(comboBox);
                comboBox.RaiseEvent(new KeyEventArgs(
                    Keyboard.PrimaryDevice,
                    PresentationSource.FromVisual(comboBox),
                    0,
                    Key.Down)
                {
                    RoutedEvent = Keyboard.KeyDownEvent
                });
                window.UpdateLayout();

                Assert.IsTrue(item.IsHighlighted);
                Assert.AreEqual(
                    ((SolidColorBrush)comboBox.FindResource("ZenListBoxItemHoverBrush")).Color,
                    ((SolidColorBrush)itemBorder.Background).Color);

                comboBox.SelectedIndex = 0;
                window.UpdateLayout();
                Assert.IsTrue(item.IsSelected);
                Assert.AreEqual(
                    ((SolidColorBrush)comboBox.FindResource("ZenListBoxItemSelectedHoverBrush")).Color,
                    ((SolidColorBrush)itemBorder.Background).Color);
                Assert.AreEqual(
                    ((SolidColorBrush)comboBox.FindResource("ZenListBoxItemSelectedForegroundBrush")).Color,
                    ((SolidColorBrush)item.Foreground).Color);

                item.IsEnabled = false;
                Assert.AreEqual(0.45d, item.Opacity);
            }
            finally
            {
                comboBox.IsDropDownOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.Close();
            }
        }

        [TestMethod]
        public void EditableComboBoxSynchronizesText()
        {
            var comboBox = new ZenComboBox
            {
                IsEditable = true,
                Text = "custom value"
            };
            var window = CreateTestWindow(comboBox, 260, 100);

            try
            {
                window.Show();
                window.UpdateLayout();

                var editableTextBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
                Assert.IsNotNull(editableTextBox);
                Assert.AreEqual(comboBox.Padding, editableTextBox.Margin);
                Assert.AreEqual(0, Grid.GetColumn(editableTextBox));
                Assert.AreEqual(Visibility.Visible, editableTextBox.Visibility);
                Assert.AreEqual("custom value", editableTextBox.Text);
                editableTextBox.Text = "updated value";
                Assert.AreEqual("updated value", comboBox.Text);
            }
            finally
            {
                window.Close();
            }
        }
    }
}
