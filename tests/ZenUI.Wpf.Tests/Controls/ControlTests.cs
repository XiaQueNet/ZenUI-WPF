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
using System.Windows.Media;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class ControlTests
    {
        [TestMethod]
        public void ControlsExposeTheirOwnDefaultStyleKeys()
        {
            var button = new TestZenButton();
            var @switch = new TestZenSwitch();
            var textBox = new TestZenTextBox();
            var checkBox = new TestZenCheckBox();
            var radioButton = new TestZenRadioButton();
            var comboBox = new TestZenComboBox();
            var dataGrid = new TestZenDataGrid();
            var passwordBox = new TestZenPasswordBox();
            var slider = new TestZenSlider();
            var progressBar = new TestZenProgressBar();
            var alert = new TestZenAlert();

            Assert.AreEqual(typeof(ZenButton), button.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenSwitch), @switch.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenTextBox), textBox.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenCheckBox), checkBox.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenRadioButton), radioButton.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenComboBox), comboBox.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenDataGrid), dataGrid.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenPasswordBox), passwordBox.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenSlider), slider.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenProgressBar), progressBar.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenAlert), alert.ExposedDefaultStyleKey);
            Assert.AreEqual(ButtonVariant.Primary, button.Variant);
            Assert.AreEqual(ButtonAppearance.Filled, button.Appearance);
            Assert.AreEqual(string.Empty, textBox.Watermark);
            Assert.AreEqual(default(CornerRadius), textBox.CornerRadius);
            Assert.IsNull(textBox.LeadingContent);
            Assert.IsNull(textBox.LeadingContentTemplate);
            Assert.IsNull(textBox.TrailingContent);
            Assert.IsNull(textBox.TrailingContentTemplate);
            Assert.AreEqual(string.Empty, comboBox.Watermark);
            Assert.AreEqual(new CornerRadius(8), dataGrid.CornerRadius);
            Assert.AreEqual("暂无数据", dataGrid.EmptyContent);
            Assert.IsFalse(passwordBox.EnableInsecurePasswordBinding);
            Assert.IsFalse(passwordBox.IsPasswordRevealEnabled);
            Assert.IsFalse(passwordBox.IsPasswordRevealed);
            Assert.IsNull(passwordBox.LeadingContent);
            Assert.IsNull(passwordBox.LeadingContentTemplate);
            Assert.IsNull(passwordBox.TrailingContent);
            Assert.IsNull(passwordBox.TrailingContentTemplate);
            Assert.AreEqual(AlertVariant.Info, alert.Variant);
        }

        [TestMethod]
        public void ButtonInteractionBrushesAreCustomizable()
        {
            var button = new ZenButton();
            var hoverBackground = new SolidColorBrush(Colors.AliceBlue);
            var pressedBackground = new SolidColorBrush(Colors.LightBlue);
            var hoverForeground = new SolidColorBrush(Colors.Navy);
            var pressedForeground = new SolidColorBrush(Colors.DarkBlue);
            var hoverBorderBrush = new SolidColorBrush(Colors.CornflowerBlue);
            var pressedBorderBrush = new SolidColorBrush(Colors.RoyalBlue);

            button.HoverBackground = hoverBackground;
            button.PressedBackground = pressedBackground;
            button.HoverForeground = hoverForeground;
            button.PressedForeground = pressedForeground;
            button.HoverBorderBrush = hoverBorderBrush;
            button.PressedBorderBrush = pressedBorderBrush;

            Assert.AreSame(hoverBackground, button.HoverBackground);
            Assert.AreSame(pressedBackground, button.PressedBackground);
            Assert.AreSame(hoverForeground, button.HoverForeground);
            Assert.AreSame(pressedForeground, button.PressedForeground);
            Assert.AreSame(hoverBorderBrush, button.HoverBorderBrush);
            Assert.AreSame(pressedBorderBrush, button.PressedBorderBrush);
        }

        [TestMethod]
        public void GenericThemeContainsControlStylesAndTokens()
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };

            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenButton)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenSwitch)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenTextBox)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenCheckBox)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenRadioButton)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenComboBox)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenDataGrid)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenPasswordBox)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenSlider)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenProgressBar)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenAlert)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ScrollBar)]);
            Assert.IsInstanceOfType<Style>(dictionary["ZenScrollBarStyle"]);
            Assert.IsNotNull(dictionary["ZenPrimaryBrush"]);
            Assert.IsNotNull(dictionary["ZenFocusBrush"]);
            Assert.IsNotNull(dictionary["ZenErrorBrush"]);
            Assert.IsInstanceOfType<Style>(dictionary["ZenFocusVisualBorderStyle"]);
        }

        [TestMethod]
        public void BasicControlsLoadTemplatesWithoutApplicationResources()
        {
            var button = new ZenButton();
            var @switch = new ZenSwitch
            {
                Width = 64,
                Height = 30
            };
            var checkBox = new ZenCheckBox { Content = "复选", IsChecked = true };
            var radioButton = new ZenRadioButton { Content = "单选", IsChecked = true };
            var comboBox = new ZenComboBox { Watermark = "请选择" };
            comboBox.Items.Add("第一项");
            var slider = new ZenSlider { Value = 50 };
            var progressBar = new ZenProgressBar { Value = 60 };
            var alert = new ZenAlert { Content = "操作成功", Variant = AlertVariant.Success };

            var panel = new StackPanel();
            panel.Children.Add(button);
            panel.Children.Add(@switch);
            panel.Children.Add(checkBox);
            panel.Children.Add(radioButton);
            panel.Children.Add(comboBox);
            panel.Children.Add(slider);
            panel.Children.Add(progressBar);
            panel.Children.Add(alert);

            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 200,
                Height = 420,
                Content = panel
            };

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.IsNotNull(button.Template);
                Assert.IsNotNull(@switch.Template);
                Assert.IsNotNull(button.Template.FindName("BackgroundBorder", button));

                var thumbHost = @switch.Template.FindName("ThumbHost", @switch) as FrameworkElement;
                Assert.IsNotNull(thumbHost);
                Assert.AreEqual(30d, thumbHost.ActualWidth, 0.5d);

                Assert.IsNotNull(checkBox.Template.FindName("Box", checkBox));
                Assert.IsNotNull(radioButton.Template.FindName("Ring", radioButton));
                Assert.IsNotNull(comboBox.Template.FindName("InputBorder", comboBox));
                Assert.IsNotNull(slider.Template.FindName("PART_Track", slider));
                Assert.IsNotNull(progressBar.Template.FindName("PART_Indicator", progressBar));
                Assert.IsNotNull(alert.Template.FindName("AlertBorder", alert));
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void TextBoxTemplateAppliesWatermarkAndCornerRadius()
        {
            var textBox = new ZenTextBox
            {
                Watermark = "请输入内容",
                CornerRadius = new CornerRadius(12)
            };
            var window = CreateTestWindow(textBox, 200, 100);

            try
            {
                window.Show();
                window.UpdateLayout();

                var inputBorder = textBox.Template.FindName("InputBorder", textBox) as Border;
                var watermark = textBox.Template.FindName("WatermarkText", textBox) as TextBlock;
                var watermarkHost = textBox.Template.FindName("WatermarkHost", textBox) as Border;
                Assert.IsNotNull(inputBorder);
                Assert.IsNotNull(watermark);
                Assert.IsNotNull(watermarkHost);
                Assert.AreEqual(new CornerRadius(12), inputBorder.CornerRadius);
                Assert.AreEqual("请输入内容", watermark.Text);
                Assert.AreEqual(textBox.Padding, watermarkHost.Margin);
                Assert.AreEqual(new Thickness(2, 0, 2, 0), watermark.Margin);
                Assert.AreEqual(Visibility.Visible, watermark.Visibility);

                var textView = FindVisualDescendant(textBox, "TextBoxView");
                Assert.IsNotNull(textView);
                var watermarkOrigin = watermark.TransformToAncestor(inputBorder).Transform(new Point());
                var textOrigin = textView.TransformToAncestor(inputBorder).Transform(new Point());
                Assert.AreEqual(
                    textOrigin.X,
                    watermarkOrigin.X,
                    0.01d,
                    $"Watermark starts at {watermarkOrigin.X}, while text starts at {textOrigin.X}.");

                textBox.Text = "ZenUI";
                window.UpdateLayout();
                Assert.AreEqual(Visibility.Collapsed, watermark.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void InputControlsDisplayLeadingAndTrailingContent()
        {
            var textBoxLeading = new TextBlock { Text = "用户" };
            var textBoxTrailing = new Button { Content = "清除" };
            var passwordBoxLeading = new TextBlock { Text = "密码" };
            var passwordBoxTrailing = new TextBlock { Text = "必填" };
            var leadingTemplate = new DataTemplate();
            var trailingTemplate = new DataTemplate();
            var textBox = new ZenTextBox
            {
                LeadingContent = textBoxLeading,
                LeadingContentTemplate = leadingTemplate,
                TrailingContent = textBoxTrailing,
                TrailingContentTemplate = trailingTemplate
            };
            var passwordBox = new ZenPasswordBox
            {
                LeadingContent = passwordBoxLeading,
                TrailingContent = passwordBoxTrailing,
                IsPasswordRevealEnabled = true
            };
            var panel = new StackPanel();
            panel.Children.Add(textBox);
            panel.Children.Add(passwordBox);
            var window = CreateTestWindow(panel, 320, 140);

            try
            {
                window.Show();
                window.UpdateLayout();

                var textLeadingHost = textBox.Template.FindName("LeadingContentHost", textBox) as ContentPresenter;
                var textTrailingHost = textBox.Template.FindName("TrailingContentHost", textBox) as ContentPresenter;
                var passwordLeadingHost = passwordBox.Template.FindName("LeadingContentHost", passwordBox) as ContentPresenter;
                var passwordTrailingHost = passwordBox.Template.FindName("TrailingContentHost", passwordBox) as ContentPresenter;
                var revealButton = passwordBox.Template.FindName("PART_RevealButton", passwordBox) as ToggleButton;

                Assert.IsNotNull(textLeadingHost);
                Assert.IsNotNull(textTrailingHost);
                Assert.IsNotNull(passwordLeadingHost);
                Assert.IsNotNull(passwordTrailingHost);
                Assert.IsNotNull(revealButton);
                Assert.AreSame(textBoxLeading, textLeadingHost.Content);
                Assert.AreSame(textBoxTrailing, textTrailingHost.Content);
                Assert.AreSame(leadingTemplate, textLeadingHost.ContentTemplate);
                Assert.AreSame(trailingTemplate, textTrailingHost.ContentTemplate);
                Assert.AreSame(passwordBoxLeading, passwordLeadingHost.Content);
                Assert.AreSame(passwordBoxTrailing, passwordTrailingHost.Content);
                Assert.AreEqual(Visibility.Visible, textLeadingHost.Visibility);
                Assert.AreEqual(Visibility.Visible, textTrailingHost.Visibility);
                Assert.AreEqual(Visibility.Visible, passwordLeadingHost.Visibility);
                Assert.AreEqual(Visibility.Visible, passwordTrailingHost.Visibility);
                Assert.AreEqual(Visibility.Visible, revealButton.Visibility);
                Assert.AreEqual(2, Grid.GetColumn(passwordTrailingHost));
                Assert.AreEqual(3, Grid.GetColumn(revealButton));

                textBox.LeadingContent = null;
                textBox.TrailingContent = null;
                passwordBox.LeadingContent = null;
                passwordBox.TrailingContent = null;
                window.UpdateLayout();

                Assert.AreEqual(Visibility.Collapsed, textLeadingHost.Visibility);
                Assert.AreEqual(Visibility.Collapsed, textTrailingHost.Visibility);
                Assert.AreEqual(Visibility.Collapsed, passwordLeadingHost.Visibility);
                Assert.AreEqual(Visibility.Collapsed, passwordTrailingHost.Visibility);
                Assert.AreEqual(Visibility.Visible, revealButton.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DataGridTemplateDisplaysHeadersAndEmptyContent()
        {
            var dataGrid = new ZenDataGrid
            {
                EmptyContent = "没有数据",
                Height = 120
            };
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "名称",
                Binding = new Binding("Name")
            });
            var window = CreateTestWindow(dataGrid, 240, 180);

            try
            {
                window.Show();
                window.UpdateLayout();

                var scrollViewer = dataGrid.Template.FindName("DG_ScrollViewer", dataGrid) as ScrollViewer;
                Assert.IsNotNull(scrollViewer);
                scrollViewer.ApplyTemplate();
                var columnHeaders = scrollViewer.Template.FindName(
                    "PART_ColumnHeadersPresenter",
                    scrollViewer) as DataGridColumnHeadersPresenter;
                Assert.IsNotNull(columnHeaders);
                Assert.AreEqual(Visibility.Visible, columnHeaders.Visibility);
                Assert.IsGreaterThan(0d, columnHeaders.ActualHeight);

                var emptyPresenter = dataGrid.Template.FindName("EmptyPresenter", dataGrid) as ContentControl;
                Assert.IsNotNull(emptyPresenter);
                Assert.AreEqual("没有数据", emptyPresenter.Content);
                Assert.AreEqual(Visibility.Visible, emptyPresenter.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void PasswordBoxTemplateProtectsPasswordAndAlignsWatermark()
        {
            var textBox = new ZenTextBox { Watermark = "请输入内容" };
            var passwordBox = new ZenPasswordBox { Watermark = "请输入密码" };
            var passwordChangedCount = 0;
            passwordBox.PasswordChanged += (sender, args) => passwordChangedCount++;
            var panel = new StackPanel();
            panel.Children.Add(textBox);
            panel.Children.Add(passwordBox);
            var window = CreateTestWindow(panel, 240, 140);

            try
            {
                window.Show();
                window.UpdateLayout();

                var nativePasswordBox = passwordBox.Template.FindName("PART_PasswordBox", passwordBox) as PasswordBox;
                Assert.IsNotNull(nativePasswordBox);
                nativePasswordBox.Password = "secret";
                Assert.AreEqual(1, passwordChangedCount);
                Assert.AreEqual(string.Empty, passwordBox.GetValue(ZenPasswordBox.PasswordProperty));
                using (var securePassword = passwordBox.SecurePassword)
                {
                    Assert.AreEqual(6, securePassword.Length);
                }

                var watermark = textBox.Template.FindName("WatermarkText", textBox) as TextBlock;
                var passwordWatermark = passwordBox.Template.FindName("WatermarkText", passwordBox) as TextBlock;
                Assert.IsNotNull(watermark);
                Assert.IsNotNull(passwordWatermark);
                Assert.AreEqual(textBox.Padding, passwordBox.Padding);
                Assert.AreEqual(watermark.HorizontalAlignment, passwordWatermark.HorizontalAlignment);
                Assert.AreEqual(watermark.VerticalAlignment, passwordWatermark.VerticalAlignment);
                Assert.AreEqual(watermark.FontFamily, passwordWatermark.FontFamily);
                Assert.AreEqual(watermark.FontSize, passwordWatermark.FontSize);
                Assert.AreEqual(new Thickness(), passwordWatermark.Margin);

                var passwordTextView = FindVisualDescendant(passwordBox, "TextBoxView");
                var passwordBorder = passwordBox.Template.FindName("InputBorder", passwordBox) as Border;
                Assert.IsNotNull(passwordTextView);
                Assert.IsNotNull(passwordBorder);
                var watermarkOrigin = passwordWatermark.TransformToAncestor(passwordBorder).Transform(new Point());
                var textOrigin = passwordTextView.TransformToAncestor(passwordBorder).Transform(new Point());
                Assert.AreEqual(
                    textOrigin.X,
                    watermarkOrigin.X,
                    0.01d,
                    $"Password watermark starts at {watermarkOrigin.X}, while password text starts at {textOrigin.X}.");
            }
            finally
            {
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
                Assert.IsNotNull(selectionPresenter);
                Assert.AreSame(itemTemplate, comboBox.SelectionBoxItemTemplate);
                Assert.AreSame(itemTemplate, selectionPresenter.ContentTemplate);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void SliderSupportsVerticalOrientation()
        {
            var slider = new ZenSlider
            {
                Orientation = Orientation.Vertical,
                Height = 180,
                Value = 40
            };
            var window = CreateTestWindow(slider, 100, 220);

            try
            {
                window.Show();
                window.UpdateLayout();

                var track = slider.Template.FindName("PART_Track", slider) as Track;
                Assert.IsNotNull(track);
                Assert.AreEqual(Orientation.Vertical, track.Orientation);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void ProgressBarSupportsVerticalAndIndeterminateStates()
        {
            var progressBar = new ZenProgressBar
            {
                Orientation = Orientation.Vertical,
                Height = 180,
                Value = 60
            };
            var window = CreateTestWindow(progressBar, 100, 220);

            try
            {
                window.Show();
                window.UpdateLayout();

                var indicator = progressBar.Template.FindName("PART_Indicator", progressBar) as FrameworkElement;
                Assert.IsNotNull(indicator);
                Assert.AreEqual(HorizontalAlignment.Stretch, indicator.HorizontalAlignment);
                Assert.AreEqual(VerticalAlignment.Bottom, indicator.VerticalAlignment);
                Assert.AreEqual(progressBar.ActualHeight * 0.6d, indicator.ActualHeight, 1d);

                progressBar.IsIndeterminate = true;
                window.UpdateLayout();
                var indeterminate = progressBar.Template.FindName("IndeterminateIndicator", progressBar) as FrameworkElement;
                Assert.IsNotNull(indeterminate);
                Assert.AreEqual(Visibility.Visible, indeterminate.Visibility);
            }
            finally
            {
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

        [TestMethod]
        public void PasswordCompatibilityBindingRequiresExplicitOptIn()
        {
            var passwordBox = new ZenPasswordBox
            {
                EnableInsecurePasswordBinding = true
            };
            passwordBox.SetValue(ZenPasswordBox.PasswordProperty, "legacy");
            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 240,
                Height = 100,
                Content = passwordBox
            };

            try
            {
                window.Show();
                window.UpdateLayout();
                var nativePasswordBox = passwordBox.Template.FindName("PART_PasswordBox", passwordBox) as PasswordBox;
                Assert.IsNotNull(nativePasswordBox);
                Assert.AreEqual("legacy", nativePasswordBox.Password);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void PasswordRevealButtonIsVisibleWhenEnabledAndPasswordIsEmpty()
        {
            var passwordBox = new ZenPasswordBox
            {
                IsPasswordRevealEnabled = true,
                Width = 240
            };
            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 280,
                Height = 100,
                Content = passwordBox
            };

            try
            {
                window.Show();
                window.UpdateLayout();

                var revealButton = passwordBox.Template.FindName("PART_RevealButton", passwordBox) as ToggleButton;
                var nativePasswordBox = passwordBox.Template.FindName("PART_PasswordBox", passwordBox) as PasswordBox;
                var revealTextBox = passwordBox.Template.FindName("PART_RevealTextBox", passwordBox) as TextBox;
                Assert.IsNotNull(revealButton);
                Assert.IsNotNull(nativePasswordBox);
                Assert.IsNotNull(revealTextBox);
                Assert.AreEqual(Visibility.Visible, revealButton.Visibility);
                Assert.AreEqual(string.Empty, nativePasswordBox.Password);

                nativePasswordBox.Password = "secret";
                passwordBox.IsPasswordRevealed = true;
                window.UpdateLayout();
                Assert.AreEqual(Visibility.Collapsed, nativePasswordBox.Visibility);
                Assert.AreEqual(Visibility.Visible, revealTextBox.Visibility);
                Assert.AreEqual("secret", revealTextBox.Text);

                passwordBox.IsPasswordRevealed = false;
                window.UpdateLayout();
                Assert.AreEqual(Visibility.Visible, nativePasswordBox.Visibility);
                Assert.AreEqual(Visibility.Collapsed, revealTextBox.Visibility);
                Assert.AreEqual(string.Empty, revealTextBox.Text);
                Assert.AreEqual("secret", nativePasswordBox.Password);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void AutomationPeersExposeControlSemantics()
        {
            var alert = new TestZenAlert { Content = "保存成功" };
            var alertPeer = alert.ExposedAutomationPeer;
            Assert.AreEqual(AutomationControlType.Text, alertPeer.GetAutomationControlType());
            Assert.AreEqual("保存成功", alertPeer.GetName());
            Assert.AreEqual(AutomationLiveSetting.Polite, AutomationProperties.GetLiveSetting(alert));

            var passwordBox = new TestZenPasswordBox();
            Assert.IsTrue(passwordBox.ExposedAutomationPeer.IsPassword());

            Assert.AreEqual(AutomationControlType.Button, new TestZenButton().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.CheckBox, new TestZenSwitch().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.Edit, new TestZenTextBox().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.CheckBox, new TestZenCheckBox().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.RadioButton, new TestZenRadioButton().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.ComboBox, new TestZenComboBox().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.DataGrid, new TestZenDataGrid().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.Slider, new TestZenSlider().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.ProgressBar, new TestZenProgressBar().ExposedAutomationPeer.GetAutomationControlType());
        }

        [TestMethod]
        public void TextInputDisplaysValidationErrors()
        {
            var textBox = new ZenTextBox();
            textBox.SetBinding(TextBox.TextProperty, new Binding(nameof(InvalidModel.Value))
            {
                Source = new InvalidModel(),
                ValidatesOnDataErrors = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            var window = new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 240,
                Height = 100,
                Content = textBox
            };

            try
            {
                window.Show();
                textBox.Text = "invalid";
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                window.UpdateLayout();

                Assert.IsTrue(Validation.GetHasError(textBox));
                var inputBorder = textBox.Template.FindName("InputBorder", textBox) as Border;
                Assert.IsNotNull(inputBorder);
                Assert.AreEqual(new Thickness(2), inputBorder.BorderThickness);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DataGridPreservesSelectionLayoutAndVirtualizationContracts()
        {
            var rows = Enumerable.Range(0, 1000)
                .Select(index => new EditableRow(index, "成员 " + index))
                .ToList();
            var grid = CreateAdvancedDataGrid(rows, out _);
            var window = CreateTestWindow(grid, 680, 340);

            try
            {
                window.Show();
                grid.SelectedItem = rows[0];
                grid.SelectedItems.Add(rows[1]);
                grid.ScrollIntoView(rows[0]);
                window.UpdateLayout();

                Assert.AreEqual(2, grid.SelectedItems.Count);
                Assert.AreEqual(1, grid.FrozenColumnCount);
                var generatedRows = Enumerable.Range(0, rows.Count)
                    .Count(index => grid.ItemContainerGenerator.ContainerFromIndex(index) != null);
                Assert.IsGreaterThan(0, generatedRows);
                Assert.IsLessThan(100, generatedRows, "Row virtualization should avoid materializing the full data set.");

                var firstRow = grid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                Assert.IsNotNull(firstRow);
                firstRow.ApplyTemplate();
                var rowHeader = firstRow.Template.FindName("PART_RowHeader", firstRow) as DataGridRowHeader;
                var details = firstRow.Template.FindName("PART_DetailsPresenter", firstRow) as DataGridDetailsPresenter;
                Assert.IsNotNull(rowHeader);
                Assert.AreEqual(Visibility.Visible, rowHeader.Visibility);
                Assert.IsNotNull(details);
                Assert.AreEqual(Visibility.Visible, details.Visibility);
                Assert.IsGreaterThan(0d, details.ActualHeight);

                var scrollViewer = grid.Template.FindName("DG_ScrollViewer", grid) as ScrollViewer;
                Assert.IsNotNull(scrollViewer);
                scrollViewer.ApplyTemplate();
                var selectAllButton = scrollViewer.Template.FindName("PART_SelectAllButton", scrollViewer) as Button;
                Assert.IsNotNull(selectAllButton);
                Assert.AreEqual(Visibility.Visible, selectAllButton.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DataGridSupportsEditingAndSorting()
        {
            var rows = Enumerable.Range(0, 20)
                .Select(index => new EditableRow(index, "成员 " + index))
                .ToList();
            var grid = CreateAdvancedDataGrid(rows, out var nameColumn);
            var window = CreateTestWindow(grid, 680, 340);

            try
            {
                window.Show();
                grid.ScrollIntoView(rows[0]);
                window.UpdateLayout();

                var firstRow = grid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                Assert.IsNotNull(firstRow);
                firstRow.ApplyTemplate();
                var cellsPresenter = firstRow.Template.FindName("PART_CellsPresenter", firstRow) as DataGridCellsPresenter;
                Assert.IsNotNull(cellsPresenter);
                var firstCell = cellsPresenter.ItemContainerGenerator.ContainerFromIndex(0) as DataGridCell;
                Assert.IsNotNull(firstCell);
                grid.CurrentCell = new DataGridCellInfo(rows[0], nameColumn);
                firstCell.Focus();
                Assert.IsTrue(grid.BeginEdit());
                window.UpdateLayout();
                var editor = FindVisualDescendant<TextBox>(firstCell);
                Assert.IsNotNull(editor);
                editor.Text = "已编辑";
                editor.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Assert.AreEqual("已编辑", rows[0].Name);
                Assert.IsTrue(grid.CommitEdit(DataGridEditingUnit.Cell, true));

                var header = FindVisualDescendants<DataGridColumnHeader>(grid)
                    .FirstOrDefault(candidate => candidate.Column == nameColumn);
                Assert.IsNotNull(header);
                var onClick = typeof(ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(onClick);
                onClick.Invoke(header, null);
                window.UpdateLayout();
                Assert.AreEqual(ListSortDirection.Ascending, nameColumn.SortDirection);
            }
            finally
            {
                window.Close();
            }
        }

        private static ZenDataGrid CreateAdvancedDataGrid(
            IList<EditableRow> rows,
            out DataGridTextColumn nameColumn)
        {
            nameColumn = new DataGridTextColumn
            {
                Header = "姓名",
                SortMemberPath = nameof(EditableRow.Name),
                Binding = new Binding(nameof(EditableRow.Name))
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                }
            };
            var detailsFactory = new FrameworkElementFactory(typeof(TextBlock));
            detailsFactory.SetBinding(TextBlock.TextProperty, new Binding(nameof(EditableRow.Name)));
            var grid = new ZenDataGrid
            {
                Width = 620,
                Height = 280,
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                HeadersVisibility = DataGridHeadersVisibility.All,
                RowHeaderWidth = 36,
                RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected,
                SelectionMode = DataGridSelectionMode.Extended,
                FlowDirection = FlowDirection.RightToLeft,
                ItemsSource = rows,
                RowDetailsTemplate = new DataTemplate { VisualTree = detailsFactory },
                FrozenColumnCount = 1
            };
            grid.Columns.Add(nameColumn);
            grid.Columns.Add(new DataGridTextColumn
            {
                Header = "编号",
                Binding = new Binding(nameof(EditableRow.Id))
            });
            return grid;
        }

        private static Window CreateTestWindow(UIElement content, double width, double height)
        {
            return new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = width,
                Height = height,
                Content = content
            };
        }

        private static FrameworkElement FindVisualDescendant(DependencyObject parent, string typeName)
        {
            for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            {
                var child = VisualTreeHelper.GetChild(parent, index);
                if (child is FrameworkElement element && child.GetType().Name == typeName)
                {
                    return element;
                }

                var descendant = FindVisualDescendant(child, typeName);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        private static T FindVisualDescendant<T>(DependencyObject parent)
            where T : DependencyObject
        {
            return FindVisualDescendants<T>(parent).FirstOrDefault();
        }

        private static IEnumerable<T> FindVisualDescendants<T>(DependencyObject parent)
            where T : DependencyObject
        {
            for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            {
                var child = VisualTreeHelper.GetChild(parent, index);
                if (child is T match)
                {
                    yield return match;
                }

                foreach (var descendant in FindVisualDescendants<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private sealed class TestZenButton : ZenButton
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }

        private sealed class TestZenSwitch : ZenSwitch
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }

        private sealed class TestZenTextBox : ZenTextBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }

        private sealed class TestZenCheckBox : ZenCheckBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        private sealed class TestZenRadioButton : ZenRadioButton
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        private sealed class TestZenComboBox : ZenComboBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        private sealed class TestZenDataGrid : ZenDataGrid
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        private sealed class TestZenPasswordBox : ZenPasswordBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        private sealed class TestZenSlider : ZenSlider
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        private sealed class TestZenProgressBar : ZenProgressBar
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
        private sealed class TestZenAlert : ZenAlert
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }

        private sealed class InvalidModel : IDataErrorInfo
        {
            public string Value { get; set; }

            public string Error => "输入无效";

            public string this[string columnName] => columnName == nameof(Value) ? Error : null;
        }

        private sealed class EditableRow
        {
            public EditableRow(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public int Id { get; }

            public string Name { get; set; }
        }

        private sealed class DisplayItem
        {
            public DisplayItem(string displayName)
            {
                DisplayName = displayName;
            }

            public string DisplayName { get; }
        }
    }
}
