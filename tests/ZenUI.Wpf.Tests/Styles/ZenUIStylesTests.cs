using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

using static ZenUI.Wpf.Tests.Controls.ControlTestHelper;

namespace ZenUI.Wpf.Tests.Styles
{
    [STATestClass]
    public class ZenUIStylesTests
    {
        [TestMethod]
        public void ImplicitButtonStyleOverridesLibraryDefaultsAndTemplateValues()
        {
            var resources = LoadStyles();
            var button = new ZenButton { Content = "Styled button" };
            var window = CreateTestWindow(button, 240, 100);
            window.Resources.MergedDictionaries.Add(resources);

            try
            {
                window.Show();
                window.UpdateLayout();
                button.ApplyTemplate();

                var expectedStyle = (Style)resources["XqZenButtonStyle"];
                var expectedFontFamily =
                    (FontFamily)resources["DefaultFontFamily"];
                var expectedFontSize =
                    (double)resources["DefaultFontSize"];
                var expectedCornerRadius =
                    (CornerRadius)resources["DefaultCornerRadius"];
                var backgroundBorder =
                    button.Template.FindName("BackgroundBorder", button) as Border;
                var contentPresenter = FindVisualDescendant<ContentPresenter>(button);

                Assert.AreSame(expectedStyle, button.Style.BasedOn);
                Assert.AreEqual(expectedFontFamily, button.FontFamily);
                Assert.AreEqual(expectedFontSize, button.FontSize);
                Assert.AreEqual(expectedCornerRadius, button.CornerRadius);
                Assert.IsNotNull(backgroundBorder);
                Assert.AreEqual(expectedCornerRadius, backgroundBorder.CornerRadius);
                Assert.IsNotNull(contentPresenter);
                Assert.AreEqual(
                    expectedFontFamily,
                    contentPresenter.GetValue(TextElement.FontFamilyProperty));
                Assert.AreEqual(
                    expectedFontSize,
                    contentPresenter.GetValue(TextElement.FontSizeProperty));
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void ImplicitStylesOverrideAllLibraryControlDefaults()
        {
            var resources = LoadStyles();
            var controls = new Dictionary<Control, string>
            {
                [new ZenAlert()] = "XqZenAlertStyle",
                [new ZenButton()] = "XqZenButtonStyle",
                [new ZenCalendar()] = "XqZenCalendarStyle",
                [new ZenCheckBox()] = "XqZenCheckBoxStyle",
                [new ZenComboBox()] = "XqZenComboBoxStyle",
                [new ZenDataGrid()] = "XqZenDataGridStyle",
                [new ZenDatePicker()] = "XqZenDatePickerStyle",
                [new ZenListBox()] = "XqZenListBoxStyle",
                [new ZenNumberBox()] = "XqZenNumberBoxStyle",
                [new ZenPasswordBox()] = "XqZenPasswordBoxStyle",
                [new ZenProgressBar()] = "XqZenProgressBarStyle",
                [new ZenRadioButton()] = "XqZenRadioButtonStyle",
                [new ZenSlider()] = "XqZenSliderStyle",
                [new ZenSwitch()] = "XqZenSwitchStyle",
                [new ZenTextBox()] = "XqZenTextBoxStyle",
                [new ScrollBar()] = "XqZenScrollBarStyle"
            };
            var panel = new Grid();
            foreach (var control in controls.Keys)
            {
                panel.Children.Add(control);
            }

            var window = CreateTestWindow(panel, 480, 480);
            window.Resources.MergedDictionaries.Add(resources);
            var expectedFontFamily =
                (FontFamily)resources["DefaultFontFamily"];
            var expectedFontSize =
                (double)resources["DefaultFontSize"];
            var expectedCornerRadius =
                (CornerRadius)resources["DefaultCornerRadius"];

            try
            {
                window.Show();
                window.UpdateLayout();

                foreach (var entry in controls)
                {
                    var namedStyle = (Style)resources[entry.Value];
                    var fontSizeSetter = namedStyle.Setters
                        .OfType<Setter>()
                        .First(setter => setter.Property == Control.FontSizeProperty);
                    Assert.AreSame(
                        namedStyle,
                        entry.Key.Style.BasedOn,
                        entry.Key.GetType().Name);
                    Assert.AreEqual(
                        expectedFontSize,
                        fontSizeSetter.Value,
                        $"{entry.Key.GetType().Name} style setter");
                    Assert.AreEqual(
                        expectedFontFamily,
                        entry.Key.FontFamily,
                        entry.Key.GetType().Name);
                    Assert.AreEqual(
                        expectedFontSize,
                        entry.Key.FontSize,
                        entry.Key.GetType().Name);
                }

                Assert.AreEqual(expectedCornerRadius, FindControl<ZenButton>(controls).CornerRadius);
                Assert.AreEqual(expectedCornerRadius, FindControl<ZenTextBox>(controls).CornerRadius);
                Assert.AreEqual(expectedCornerRadius, FindControl<ZenPasswordBox>(controls).CornerRadius);
                Assert.AreEqual(expectedCornerRadius, FindControl<ZenComboBox>(controls).CornerRadius);
                Assert.AreEqual(expectedCornerRadius, FindControl<ZenListBox>(controls).CornerRadius);
                Assert.AreEqual(expectedCornerRadius, FindControl<ZenDatePicker>(controls).CornerRadius);
                Assert.AreEqual(expectedCornerRadius, FindControl<ZenProgressBar>(controls).CornerRadius);
                Assert.AreEqual(expectedCornerRadius, FindControl<ZenDataGrid>(controls).CornerRadius);
            }
            finally
            {
                window.Close();
            }
        }

        private static ResourceDictionary LoadStyles()
        {
            return (ResourceDictionary)Application.LoadComponent(
                new Uri(
                    "/ZenUI.Wpf.Tests;component/Resources/ZenUIStyles.xaml",
                    UriKind.Relative));
        }

        private static T FindControl<T>(Dictionary<Control, string> controls)
            where T : Control
        {
            return controls.Keys.OfType<T>().First();
        }
    }
}
