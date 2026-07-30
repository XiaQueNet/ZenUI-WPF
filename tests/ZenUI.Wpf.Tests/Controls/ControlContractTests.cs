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
    public class ControlContractTests
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
            var listBox = new TestZenListBox();
            var datePicker = new TestZenDatePicker();
            var dataGrid = new TestZenDataGrid();
            var passwordBox = new TestZenPasswordBox();
            var slider = new TestZenSlider();
            var progressBar = new TestZenProgressBar();
            var alert = new TestZenAlert();
            var expander = new TestZenExpander();

            Assert.AreEqual(typeof(ZenButton), button.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenSwitch), @switch.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenTextBox), textBox.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenCheckBox), checkBox.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenRadioButton), radioButton.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenComboBox), comboBox.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenListBox), listBox.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenDatePicker), datePicker.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenDataGrid), dataGrid.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenPasswordBox), passwordBox.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenSlider), slider.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenProgressBar), progressBar.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenAlert), alert.ExposedDefaultStyleKey);
            Assert.AreEqual(typeof(ZenExpander), expander.ExposedDefaultStyleKey);
            Assert.AreEqual(ButtonVariant.Primary, button.Variant);
            Assert.AreEqual(ButtonAppearance.Filled, button.Appearance);
            Assert.AreEqual(string.Empty, textBox.Watermark);
            Assert.AreEqual(default(CornerRadius), textBox.CornerRadius);
            Assert.IsNull(textBox.LeadingContent);
            Assert.IsNull(textBox.LeadingContentTemplate);
            Assert.IsNull(textBox.TrailingContent);
            Assert.IsNull(textBox.TrailingContentTemplate);
            Assert.AreEqual(18d, checkBox.IndicatorSize);
            Assert.AreEqual(18d, radioButton.IndicatorSize);
            Assert.AreEqual(string.Empty, comboBox.Watermark);
            Assert.AreEqual(new CornerRadius(8), listBox.CornerRadius);
            Assert.AreEqual(string.Empty, datePicker.Watermark);
            Assert.AreEqual(new CornerRadius(6), datePicker.CornerRadius);
            Assert.AreEqual(new CornerRadius(8), dataGrid.CornerRadius);
            Assert.IsFalse(dataGrid.IsRowSelectionHighlightEnabled);
            Assert.IsFalse(dataGrid.IsCellFocusVisualEnabled);
            Assert.AreEqual(new Thickness(1), dataGrid.CellFocusVisualBorderThickness);
            Assert.AreEqual(new Thickness(2), dataGrid.CellValidationBorderThickness);
            Assert.AreEqual("暂无数据", dataGrid.EmptyContent);
            Assert.IsFalse(passwordBox.IsPasswordRevealButtonEnabled);
            Assert.IsFalse(passwordBox.IsPasswordRevealed);
            Assert.IsNull(passwordBox.LeadingContent);
            Assert.IsNull(passwordBox.LeadingContentTemplate);
            Assert.IsNull(passwordBox.TrailingContent);
            Assert.IsNull(passwordBox.TrailingContentTemplate);
            Assert.AreEqual(18d, alert.IconSize);
            Assert.AreEqual(AlertSeverity.Info, alert.Severity);
            Assert.AreEqual(new CornerRadius(8), expander.CornerRadius);
            Assert.AreEqual(new Thickness(14, 12, 14, 12), expander.HeaderPadding);
            Assert.AreEqual(16d, expander.GlyphSize);
        }

        [TestMethod]
        public void GenericThemeContainsControlStylesAndTokens()
        {
            _ = new ZenButton();
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
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenListBox)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenDatePicker)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenDataGrid)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenPasswordBox)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenSlider)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenProgressBar)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenAlert)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenExpander)]);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ScrollBar)]);
            Assert.IsInstanceOfType<Style>(dictionary["ZenScrollBarStyle"]);
            Assert.IsNotNull(dictionary["ZenPrimaryBrush"]);
            Assert.IsNotNull(dictionary["ZenFocusBrush"]);
            Assert.IsNotNull(dictionary["ZenErrorBrush"]);
            Assert.IsNotNull(dictionary["ZenOnAccentBrush"]);
            Assert.IsNotNull(dictionary["ZenControlThumbBrush"]);
            Assert.IsNotNull(dictionary["ZenControlThumbBorderBrush"]);
            Assert.IsNotNull(dictionary["ZenListBoxItemSelectedBrush"]);
            Assert.IsNotNull(dictionary["ZenRadioSegmentedSelectedBrush"]);
            Assert.IsNotNull(dictionary["ZenRadioSegmentedSelectedHoverBrush"]);
            Assert.IsNotNull(dictionary["ZenRadioSegmentedSelectedForegroundBrush"]);
            Assert.IsInstanceOfType<Style>(dictionary["ZenListBoxStyle"]);
            Assert.IsInstanceOfType<Style>(dictionary["ZenListBoxItemStyle"]);
            Assert.AreEqual(36d, dictionary["ZenInputControlMinHeight"]);
            Assert.AreEqual(new Thickness(8, 4, 8, 4), dictionary["ZenInputControlPadding"]);
            Assert.AreEqual(new CornerRadius(6), dictionary["ZenInputControlCornerRadius"]);
            Assert.AreEqual(new CornerRadius(9), dictionary["ZenInputFocusVisualCornerRadius"]);
            Assert.AreEqual(new Thickness(1), dictionary["ZenControlBorderThickness"]);
            Assert.AreEqual(new Thickness(-2), dictionary["ZenFocusVisualMargin"]);
            Assert.AreEqual(new Thickness(1), dictionary["ZenFocusVisualBorderThickness"]);
            Assert.AreEqual(36d, dictionary["ZenButtonMinHeight"]);
            Assert.AreEqual(new Thickness(10, 4, 10, 4), dictionary["ZenButtonPadding"]);
            Assert.AreEqual(new CornerRadius(8), dictionary["ZenButtonCornerRadius"]);
            Assert.AreEqual(new CornerRadius(11), dictionary["ZenButtonFocusVisualCornerRadius"]);
            Assert.AreEqual(new Thickness(4), dictionary["ZenListBoxPadding"]);
            Assert.AreEqual(new CornerRadius(8), dictionary["ZenListBoxCornerRadius"]);
            Assert.AreEqual(new Thickness(12, 9, 12, 9), dictionary["ZenListBoxItemPadding"]);
            Assert.AreEqual(new Thickness(0, 1, 0, 1), dictionary["ZenListBoxItemMargin"]);
            Assert.AreEqual(new CornerRadius(5), dictionary["ZenListBoxItemCornerRadius"]);
            Assert.AreEqual(12d, dictionary["ZenScrollBarThickness"]);
            Assert.AreEqual(6d, dictionary["ZenScrollBarTrackThickness"]);
            Assert.AreEqual(32d, dictionary["ZenScrollBarThumbMinLength"]);
            Assert.AreEqual(new Thickness(0, 4, 0, 4), dictionary["ZenVerticalScrollBarMargin"]);
            Assert.AreEqual(new Thickness(4, 0, 4, 0), dictionary["ZenHorizontalScrollBarMargin"]);
            Assert.AreEqual(new Thickness(0, 0, 1, 0), dictionary["ZenVerticalScrollBarTrackMargin"]);
            Assert.AreEqual(new Thickness(0, 0, 0, 1), dictionary["ZenHorizontalScrollBarTrackMargin"]);
            Assert.AreEqual(new Thickness(5, 0, 1, 0), dictionary["ZenVerticalScrollBarThumbMargin"]);
            Assert.AreEqual(new Thickness(0, 5, 0, 1), dictionary["ZenHorizontalScrollBarThumbMargin"]);
            Assert.AreEqual(new CornerRadius(3), dictionary["ZenScrollBarCornerRadius"]);
            Assert.AreEqual(34d, dictionary["ZenNumberBoxSpinButtonWidth"]);
            Assert.AreEqual(new Thickness(0, 4, 0, 0), dictionary["ZenComboBoxPopupMargin"]);
            Assert.AreEqual(new Thickness(4), dictionary["ZenComboBoxPopupPadding"]);
            Assert.AreEqual(new CornerRadius(6), dictionary["ZenComboBoxPopupCornerRadius"]);
            Assert.AreEqual(new Thickness(0, 4, 0, 8), dictionary["ZenTimePickerPopupMargin"]);
            Assert.AreEqual(new Thickness(6), dictionary["ZenTimePickerPopupPadding"]);
            Assert.AreEqual(new CornerRadius(8), dictionary["ZenTimePickerPopupCornerRadius"]);
            Assert.AreEqual(64d, dictionary["ZenTimePickerColumnWidth"]);
            Assert.AreEqual(74d, dictionary["ZenTimePickerPeriodColumnWidth"]);
            Assert.AreEqual(196d, dictionary["ZenTimePickerListHeight"]);
            Assert.AreEqual(36d, dictionary["ZenTimePickerItemHeight"]);
            Assert.AreEqual(new Thickness(0, 2, 0, 2), dictionary["ZenTimePickerItemMargin"]);
            Assert.AreEqual(44d, dictionary["ZenDataGridColumnHeaderHeight"]);
            Assert.AreEqual(44d, dictionary["ZenDataGridRowMinHeight"]);
            Assert.AreEqual(new Thickness(14, 0, 14, 0), dictionary["ZenDataGridCellPadding"]);
            Assert.AreEqual(new Thickness(1), dictionary["ZenDataGridCellFocusVisualBorderThickness"]);
            Assert.AreEqual(new Thickness(2), dictionary["ZenDataGridCellValidationBorderThickness"]);
            Assert.AreEqual(368d, dictionary["ZenCalendarPopupWidth"]);
            Assert.AreEqual(376d, dictionary["ZenCalendarPopupHeight"]);
            Assert.AreEqual(new Thickness(8), dictionary["ZenCalendarContentMargin"]);
            Assert.AreEqual(new Thickness(12, 16, 12, 16), dictionary["ZenCalendarButtonPadding"]);
            Assert.AreEqual(40d, dictionary["ZenCalendarNavigationButtonSize"]);
            Assert.AreEqual(60d, dictionary["ZenSwitchWidth"]);
            Assert.AreEqual(30d, dictionary["ZenSwitchHeight"]);
            Assert.AreEqual(new Thickness(4), dictionary["ZenSwitchThumbMargin"]);
            Assert.AreEqual(18d, dictionary["ZenSelectionIndicatorSize"]);
            Assert.AreEqual(new CornerRadius(4), dictionary["ZenSelectionIndicatorCornerRadius"]);
            Assert.AreEqual(new CornerRadius(4), dictionary["ZenSelectionFocusVisualCornerRadius"]);
            Assert.AreEqual(36d, dictionary["ZenRadioItemMinHeight"]);
            Assert.AreEqual(new Thickness(14, 0, 14, 0), dictionary["ZenRadioItemPadding"]);
            Assert.AreEqual(new CornerRadius(8), dictionary["ZenRadioItemCornerRadius"]);
            Assert.AreEqual(new CornerRadius(6), dictionary["ZenRadioItemInnerCornerRadius"]);
            Assert.AreEqual(new CornerRadius(11), dictionary["ZenRadioItemFocusVisualCornerRadius"]);
            Assert.AreEqual(18d, dictionary["ZenSliderThumbSize"]);
            Assert.AreEqual(4d, dictionary["ZenSliderTrackThickness"]);
            Assert.AreEqual(24d, dictionary["ZenSliderCrossAxisMinSize"]);
            Assert.AreEqual(8d, dictionary["ZenProgressBarThickness"]);
            Assert.AreEqual(new Thickness(14, 11, 14, 11), dictionary["ZenAlertPadding"]);
            Assert.AreEqual(new CornerRadius(6), dictionary["ZenAlertCornerRadius"]);
            Assert.AreEqual(new Thickness(14, 12, 14, 12), dictionary["ZenExpanderHeaderPadding"]);
            Assert.AreEqual(new Thickness(14, 10, 14, 14), dictionary["ZenExpanderContentPadding"]);
            Assert.AreEqual(new CornerRadius(8), dictionary["ZenExpanderCornerRadius"]);
            Assert.AreEqual(16d, dictionary["ZenExpanderGlyphSize"]);
            Assert.AreEqual(0.35d, dictionary["ZenFocusVisualOpacity"]);
            Assert.AreEqual(0.35d, dictionary["ZenDisabledAuxiliaryActionOpacity"]);
            Assert.AreEqual(0.4d, dictionary["ZenDisabledActionOpacity"]);
            Assert.AreEqual(0.45d, dictionary["ZenDisabledItemOpacity"]);
            Assert.AreEqual(0.55d, dictionary["ZenDisabledInputOpacity"]);
            Assert.AreEqual(0.6d, dictionary["ZenDisabledFieldOpacity"]);
            Assert.AreEqual(0.65d, dictionary["ZenDisabledContainerOpacity"]);
            Assert.AreEqual(12d, dictionary["ZenFontSizeCaption"]);
            Assert.AreEqual(13d, dictionary["ZenFontSizeBodySmall"]);
            Assert.AreEqual(14d, dictionary["ZenFontSizeBody"]);
            Assert.AreEqual(16d, dictionary["ZenFontSizeSubtitle"]);
            Assert.AreEqual(20d, dictionary["ZenFontSizeTitle"]);
            Assert.AreEqual(28d, dictionary["ZenFontSizeDisplay"]);
            Assert.AreEqual(FontWeights.Regular, dictionary["ZenFontWeightRegular"]);
            Assert.AreEqual(FontWeights.SemiBold, dictionary["ZenFontWeightSemibold"]);
            Assert.AreEqual(FontWeights.Bold, dictionary["ZenFontWeightBold"]);
            Assert.AreEqual(18d, dictionary["ZenLineHeightCaption"]);
            Assert.AreEqual(21d, dictionary["ZenLineHeightBody"]);
            Assert.AreEqual(28d, dictionary["ZenLineHeightTitle"]);
            Assert.AreEqual(36d, dictionary["ZenLineHeightDisplay"]);
            Assert.IsInstanceOfType<Style>(dictionary["ZenFocusVisualBorderStyle"]);
        }

        [TestMethod]
        public void ZenControlsUseBodyFontSizeByDefaultAndAllowOverrides()
        {
            var button = new ZenButton { Content = "Button" };
            var controls = new Control[]
            {
                button,
                new ZenSwitch(),
                new ZenTextBox(),
                new ZenNumberBox(),
                new ZenPasswordBox(),
                new ZenCheckBox { Content = "CheckBox" },
                new ZenRadioButton { Content = "RadioButton" },
                new ZenComboBox(),
                new ZenListBox(),
                new ZenDatePicker(),
                new ZenDataGrid(),
                new ZenSlider(),
                new ZenProgressBar(),
                new ZenAlert(),
                new ZenExpander { Header = "Expander" }
            };
            var panel = new StackPanel();
            foreach (var control in controls)
            {
                panel.Children.Add(control);
            }

            var window = CreateTestWindow(panel, 420, 800);
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

                foreach (var control in controls)
                {
                    Assert.AreEqual(14d, control.FontSize, $"{control.GetType().Name} 未使用正文默认字号。");
                }

                button.FontSize = 18d;
                window.Resources["ZenFontSizeBody"] = 15d;
                window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));
                window.UpdateLayout();

                Assert.AreEqual(18d, button.FontSize);
                foreach (var control in controls.Where(control => !ReferenceEquals(control, button)))
                {
                    Assert.AreEqual(15d, control.FontSize, $"{control.GetType().Name} 未响应正文字号覆盖。");
                }
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void SelectionControlsApplyCustomIndicatorSize()
        {
            var checkBox = new ZenCheckBox
            {
                Content = "CheckBox",
                IndicatorSize = 28d
            };
            var radioButton = new ZenRadioButton
            {
                Content = "RadioButton",
                IndicatorSize = 30d
            };
            var panel = new StackPanel();
            panel.Children.Add(checkBox);
            panel.Children.Add(radioButton);
            var window = CreateTestWindow(panel, 300, 120);
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

                var checkBoxIndicator =
                    checkBox.Template.FindName("IndicatorHost", checkBox) as FrameworkElement;
                var radioButtonIndicator =
                    radioButton.Template.FindName("IndicatorHost", radioButton) as FrameworkElement;

                Assert.IsNotNull(checkBoxIndicator);
                Assert.IsNotNull(radioButtonIndicator);
                Assert.AreEqual(28d, checkBoxIndicator.ActualWidth, 0.1d);
                Assert.AreEqual(28d, checkBoxIndicator.ActualHeight, 0.1d);
                Assert.AreEqual(30d, radioButtonIndicator.ActualWidth, 0.1d);
                Assert.AreEqual(30d, radioButtonIndicator.ActualHeight, 0.1d);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void AlertAppliesCustomIconSize()
        {
            var alert = new ZenAlert
            {
                Content = "Saved",
                IconSize = 28d
            };
            var window = CreateTestWindow(alert, 300, 100);
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

                var iconHost = alert.Template.FindName("IconHost", alert) as FrameworkElement;
                var iconText = alert.Template.FindName("IconText", alert) as FrameworkElement;

                Assert.IsNotNull(iconHost);
                Assert.IsNotNull(iconText);
                Assert.AreEqual(28d, iconHost.ActualWidth, 0.1d);
                Assert.AreEqual(28d, iconHost.ActualHeight, 0.1d);

                var renderedTextBounds = iconText
                    .TransformToAncestor(iconHost)
                    .TransformBounds(new Rect(iconText.RenderSize));
                Assert.IsGreaterThan(iconText.ActualHeight, renderedTextBounds.Height);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void HighContrastKeepsDisabledControlsAtFullOpacity()
        {
            var button = new ZenButton { IsEnabled = false };
            var toggleSwitch = new ZenSwitch { IsEnabled = false };
            var textBox = new ZenTextBox { IsEnabled = false };
            var passwordBox = new ZenPasswordBox { IsEnabled = false };
            var numberBox = new ZenNumberBox { IsEnabled = false };
            var comboBox = new ZenComboBox { IsEnabled = false };
            var checkBox = new ZenCheckBox { IsEnabled = false };
            var radioButton = new ZenRadioButton { IsEnabled = false };
            var listBox = new ZenListBox { IsEnabled = false, Height = 80 };
            var slider = new ZenSlider { IsEnabled = false };
            var scrollBar = new ScrollBar { IsEnabled = false, Height = 80 };
            var datePicker = new ZenDatePicker { IsEnabled = false };
            var dataGrid = new ZenDataGrid { IsEnabled = false, Height = 80 };
            listBox.Items.Add("Disabled item");

            var panel = new StackPanel();
            panel.Children.Add(button);
            panel.Children.Add(toggleSwitch);
            panel.Children.Add(textBox);
            panel.Children.Add(passwordBox);
            panel.Children.Add(numberBox);
            panel.Children.Add(comboBox);
            panel.Children.Add(checkBox);
            panel.Children.Add(radioButton);
            panel.Children.Add(listBox);
            panel.Children.Add(slider);
            panel.Children.Add(scrollBar);
            panel.Children.Add(datePicker);
            panel.Children.Add(dataGrid);

            var window = CreateTestWindow(panel, 420, 800);
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

                Assert.AreEqual(0.4d, button.Opacity);
                Assert.AreEqual(0.4d, toggleSwitch.Opacity);
                Assert.AreEqual(0.6d, textBox.Opacity);
                Assert.AreEqual(0.55d, passwordBox.Opacity);
                Assert.AreEqual(0.6d, numberBox.Opacity);
                Assert.AreEqual(0.55d, comboBox.Opacity);
                Assert.AreEqual(0.45d, checkBox.Opacity);
                Assert.AreEqual(0.45d, radioButton.Opacity);
                Assert.AreEqual(0.65d, listBox.Opacity);
                Assert.AreEqual(0.4d, slider.Opacity);
                Assert.AreEqual(0.45d, scrollBar.Opacity);
                Assert.AreEqual(0.55d, datePicker.Opacity);
                Assert.AreEqual(0.55d, dataGrid.Opacity);

                ZenThemeManager.ApplyTheme(window.Resources, ZenTheme.HighContrast, false);
                window.UpdateLayout();

                Assert.AreEqual(1d, button.Opacity);
                Assert.AreEqual(1d, toggleSwitch.Opacity);
                Assert.AreEqual(1d, textBox.Opacity);
                Assert.AreEqual(1d, passwordBox.Opacity);
                Assert.AreEqual(1d, numberBox.Opacity);
                Assert.AreEqual(1d, comboBox.Opacity);
                Assert.AreEqual(1d, checkBox.Opacity);
                Assert.AreEqual(1d, radioButton.Opacity);
                Assert.AreEqual(1d, listBox.Opacity);
                Assert.AreEqual(1d, slider.Opacity);
                Assert.AreEqual(1d, scrollBar.Opacity);
                Assert.AreEqual(1d, datePicker.Opacity);
                Assert.AreEqual(1d, dataGrid.Opacity);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void FocusVisualTemplatesResolveSharedResourcesWhenInstantiated()
        {
            _ = new ZenButton();
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            var styleKeys = new[]
            {
                "ZenButtonFocusVisualStyle",
                "ZenSwitchFocusVisualStyle",
                "ZenTextBoxFocusVisualStyle",
                "ZenSelectionFocusVisualStyle",
                "ZenListBoxItemFocusVisualStyle",
                "ZenExpanderHeaderFocusVisualStyle"
            };

            foreach (var styleKey in styleKeys)
            {
                var style = dictionary[styleKey] as Style;
                Assert.IsNotNull(style, $"Missing focus visual style '{styleKey}'.");
                var templateSetter = style.Setters
                    .OfType<Setter>()
                    .Single(setter => setter.Property == Control.TemplateProperty);
                var template = templateSetter.Value as ControlTemplate;
                Assert.IsNotNull(template, $"Style '{styleKey}' does not define a control template.");

                var content = template.LoadContent();
                Assert.IsInstanceOfType<Border>(
                    content,
                    $"Focus visual template '{styleKey}' could not be instantiated.");
            }
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
            var listBox = new ZenListBox { Height = 80 };
            listBox.Items.Add("第一项");
            listBox.Items.Add("第二项");
            listBox.SelectedIndex = 0;
            var datePicker = new ZenDatePicker { Watermark = "请选择日期" };
            var slider = new ZenSlider { Value = 50 };
            var progressBar = new ZenProgressBar { Value = 60 };
            var alert = new ZenAlert { Content = "操作成功", Severity = AlertSeverity.Success };

            var panel = new StackPanel();
            panel.Children.Add(button);
            panel.Children.Add(@switch);
            panel.Children.Add(checkBox);
            panel.Children.Add(radioButton);
            panel.Children.Add(comboBox);
            panel.Children.Add(listBox);
            panel.Children.Add(datePicker);
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
                Assert.IsNotNull(FindVisualDescendant<ScrollViewer>(listBox));
                Assert.IsNotNull(datePicker.Template.FindName("PART_TextBox", datePicker));
                Assert.IsNotNull(datePicker.Template.FindName("PART_Button", datePicker));
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
                IsPasswordRevealButtonEnabled = true
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
                Assert.AreEqual(new Thickness(), textLeadingHost.Margin);
                Assert.AreEqual(new Thickness(), textTrailingHost.Margin);
                Assert.AreEqual(new Thickness(), passwordLeadingHost.Margin);
                Assert.AreEqual(new Thickness(), passwordTrailingHost.Margin);
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
        public void InputTemplatesHonorBorderThickness()
        {
            var thickness = new Thickness(3, 4, 5, 6);
            var comboBox = new ZenComboBox { BorderThickness = thickness };
            var datePicker = new ZenDatePicker { BorderThickness = thickness };
            var passwordBox = new ZenPasswordBox { BorderThickness = thickness };
            var panel = new StackPanel();
            panel.Children.Add(comboBox);
            panel.Children.Add(datePicker);
            panel.Children.Add(passwordBox);
            var window = CreateTestWindow(panel, 260, 180);

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.AreEqual(
                    thickness,
                    ((Border)comboBox.Template.FindName("InputBorder", comboBox)).BorderThickness);
                Assert.AreEqual(
                    thickness,
                    ((Border)datePicker.Template.FindName("InputBorder", datePicker)).BorderThickness);
                Assert.AreEqual(
                    thickness,
                    ((Border)passwordBox.Template.FindName("InputBorder", passwordBox)).BorderThickness);
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
            Assert.AreEqual(AutomationControlType.List, new TestZenListBox().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.Custom, new TestZenDatePicker().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.DataGrid, new TestZenDataGrid().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.Slider, new TestZenSlider().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.ProgressBar, new TestZenProgressBar().ExposedAutomationPeer.GetAutomationControlType());
            Assert.AreEqual(AutomationControlType.Group, new TestZenExpander().ExposedAutomationPeer.GetAutomationControlType());
        }
    }
}
