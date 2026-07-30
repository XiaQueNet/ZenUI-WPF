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
                var popupScrollViewer =
                    FindVisualDescendant<ScrollViewer>(popupBorder);
                Assert.IsNotNull(popupScrollViewer);
                Assert.AreEqual(new Thickness(7), popupScrollViewer.Padding);
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
        public void ComboBoxPopupFlipsInsideWorkAreaNearBottomRightEdge()
        {
            var workArea = SystemParameters.WorkArea;
            var comboBox = new ZenComboBox
            {
                Width = 200,
                Height = 36,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                MaxDropDownHeight = 160
            };
            for (var index = 1; index <= 8; index++)
            {
                comboBox.Items.Add("项目 " + index);
            }

            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = workArea.Right - 240,
                Top = workArea.Bottom - 60,
                Width = 240,
                Height = 60,
                Content = comboBox
            };

            try
            {
                window.Show();
                comboBox.IsDropDownOpen = true;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.UpdateLayout();

                var popup = comboBox.Template.FindName("PART_Popup", comboBox) as Popup;
                Assert.IsNotNull(popup);
                Assert.IsTrue(popup.IsOpen);
                var popupBorder = popup.Child as Border;
                Assert.IsNotNull(popupBorder);
                Assert.IsGreaterThan(0d, popupBorder.ActualWidth);
                Assert.IsGreaterThan(0d, popupBorder.ActualHeight);

                var targetTopLeft = ToDeviceIndependentPoint(
                    comboBox,
                    comboBox.PointToScreen(new Point()));
                var popupTopLeft = ToDeviceIndependentPoint(
                    popupBorder,
                    popupBorder.PointToScreen(new Point()));
                var popupBottomRight = ToDeviceIndependentPoint(
                    popupBorder,
                    popupBorder.PointToScreen(
                        new Point(popupBorder.ActualWidth, popupBorder.ActualHeight)));

                Assert.IsTrue(
                    popupTopLeft.Y < targetTopLeft.Y,
                    "靠近工作区底边时，下拉弹层应翻转到控件上方。");
                Assert.IsGreaterThanOrEqualTo(workArea.Left - 1d, popupTopLeft.X);
                Assert.IsGreaterThanOrEqualTo(workArea.Top - 1d, popupTopLeft.Y);
                Assert.IsLessThanOrEqualTo(workArea.Right + 1d, popupBottomRight.X);
                Assert.IsLessThanOrEqualTo(workArea.Bottom + 1d, popupBottomRight.Y);
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
        public void ComboBoxArrowRotatesWithDropDownState()
        {
            var comboBox = new ZenComboBox { Width = 160 };
            comboBox.Items.Add("Item");
            var window = CreateTestWindow(comboBox, 220, 120);
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

                var arrow = comboBox.Template.FindName("DropDownArrow", comboBox) as Path;
                Assert.IsNotNull(arrow);
                Assert.AreEqual(0d, ((RotateTransform)arrow.RenderTransform).Angle);

                comboBox.IsDropDownOpen = true;
                window.UpdateLayout();
                Assert.AreEqual(180d, ((RotateTransform)arrow.RenderTransform).Angle);

                comboBox.IsDropDownOpen = false;
                window.UpdateLayout();
                Assert.AreEqual(0d, ((RotateTransform)arrow.RenderTransform).Angle);
            }
            finally
            {
                comboBox.IsDropDownOpen = false;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
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
        public void ComboBoxItemsUseListSelectionStateAndDensityTokens()
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
                Assert.AreEqual(new Thickness(12, 9, 12, 9), item.Padding);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.UpdateLayout();
                Assert.AreEqual(new Thickness(10, 6, 10, 6), item.Padding);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Standard);
                window.UpdateLayout();
                Assert.AreEqual(new Thickness(12, 9, 12, 9), item.Padding);

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

        private static Point ToDeviceIndependentPoint(Visual visual, Point devicePoint)
        {
            var source = PresentationSource.FromVisual(visual);
            return source?.CompositionTarget == null
                ? devicePoint
                : source.CompositionTarget.TransformFromDevice.Transform(devicePoint);
        }
    }
}
