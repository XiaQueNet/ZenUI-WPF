using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;
using ZenUI.Wpf.Theming;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class NumberBoxTests
    {
        [TestMethod]
        public void NumberBoxExposesDefaults()
        {
            var numberBox = new TestZenNumberBox();

            Assert.AreEqual(typeof(ZenNumberBox), numberBox.ExposedDefaultStyleKey);
            Assert.AreEqual(0m, numberBox.Value);
            Assert.AreEqual(1m, numberBox.Step);
            Assert.AreEqual(SpinButtonLayout.Horizontal, numberBox.SpinButtonLayout);
            Assert.AreEqual(34d, numberBox.SpinButtonWidth);
        }

        [TestMethod]
        public void GenericThemeContainsNumberBoxStyle()
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri("/ZenUI.Wpf;component/Themes/Generic.xaml", UriKind.Relative)
            };

            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenNumberBox)]);
        }

        [TestMethod]
        public void NumberBoxAutomationPeerExposesRangeValuePattern()
        {
            var numberBox = new TestZenNumberBox();

            var peer = numberBox.ExposedAutomationPeer;

            Assert.AreEqual(AutomationControlType.Spinner, peer.GetAutomationControlType());
            Assert.IsInstanceOfType<IRangeValueProvider>(
                peer.GetPattern(PatternInterface.RangeValue));
        }

        [TestMethod]
        public void ButtonsUseConfiguredStepAndValueIsCoercedToRange()
        {
            var numberBox = new ZenNumberBox
            {
                Minimum = 0m,
                Maximum = 2m,
                Step = 0.5m,
                Value = 1m
            };
            var window = CreateWindow(numberBox);

            try
            {
                window.Show();
                window.UpdateLayout();

                var increase = numberBox.Template.FindName("PART_IncreaseButton", numberBox) as RepeatButton;
                var decrease = numberBox.Template.FindName("PART_DecreaseButton", numberBox) as RepeatButton;
                Assert.IsNotNull(increase);
                Assert.IsNotNull(decrease);
                Assert.AreEqual(34d, increase.Width);
                Assert.AreEqual(34d, decrease.Width);
                increase.ApplyTemplate();
                decrease.ApplyTemplate();
                var increaseBackground = increase.Template.FindName("ButtonBackground", increase) as Border;
                var decreaseBackground = decrease.Template.FindName("ButtonBackground", decrease) as Border;
                Assert.IsNotNull(increaseBackground);
                Assert.IsNotNull(decreaseBackground);
                Assert.AreEqual(new CornerRadius(0, 5, 5, 0), increaseBackground.CornerRadius);
                Assert.AreEqual(new CornerRadius(5, 0, 0, 5), decreaseBackground.CornerRadius);

                increase.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.AreEqual(1.5m, numberBox.Value);
                decrease.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.AreEqual(1m, numberBox.Value);

                numberBox.Value = 10m;
                Assert.AreEqual(2m, numberBox.Value);
                Assert.IsFalse(increase.IsEnabled);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void VerticalModeUsesRightSideButtons()
        {
            var numberBox = new ZenNumberBox
            {
                SpinButtonLayout = SpinButtonLayout.Vertical,
                SpinButtonWidth = 40d,
                Step = 2m,
                Value = 4m
            };
            var window = CreateWindow(numberBox);

            try
            {
                window.Show();
                window.UpdateLayout();

                var verticalLayout = numberBox.Template.FindName("VerticalLayout", numberBox) as Grid;
                var increase = numberBox.Template.FindName("PART_VerticalIncreaseButton", numberBox) as RepeatButton;
                var decrease = numberBox.Template.FindName("PART_VerticalDecreaseButton", numberBox) as RepeatButton;
                var divider = numberBox.Template.FindName("VerticalButtonDivider", numberBox) as Border;
                Assert.IsNotNull(verticalLayout);
                Assert.AreEqual(Visibility.Visible, verticalLayout.Visibility);
                Assert.IsNotNull(increase);
                Assert.IsNotNull(decrease);
                Assert.AreEqual(40d, increase.Width);
                Assert.AreEqual(40d, decrease.Width);
                Assert.IsNotNull(divider);
                Assert.AreEqual(1d, divider.Height);
                Assert.IsInstanceOfType<Path>(((Viewbox)increase.Content).Child);
                Assert.IsInstanceOfType<Path>(((Viewbox)decrease.Content).Child);
                increase.ApplyTemplate();
                decrease.ApplyTemplate();
                var increaseBackground = increase.Template.FindName("ButtonBackground", increase) as Border;
                var decreaseBackground = decrease.Template.FindName("ButtonBackground", decrease) as Border;
                Assert.IsNotNull(increaseBackground);
                Assert.IsNotNull(decreaseBackground);
                Assert.AreEqual(new CornerRadius(0, 5, 0, 0), increaseBackground.CornerRadius);
                Assert.AreEqual(new CornerRadius(0, 0, 5, 0), decreaseBackground.CornerRadius);

                increase.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.AreEqual(6m, numberBox.Value);
                decrease.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                Assert.AreEqual(4m, numberBox.Value);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void InvalidStepIsRejected()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new ZenNumberBox { Step = 0m });
        }

        [TestMethod]
        public void DensityAndDisabledStateKeepSpinButtonsBalanced()
        {
            var numberBox = new ZenNumberBox();
            var window = CreateWindow(numberBox);
            window.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/ZenUI.Wpf;component/Themes/Generic.xaml", UriKind.Relative)
            });

            try
            {
                window.Show();
                window.UpdateLayout();

                var increase = numberBox.Template.FindName("PART_IncreaseButton", numberBox) as RepeatButton;
                Assert.IsNotNull(increase);
                Assert.AreEqual(34d, numberBox.SpinButtonWidth);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Compact);
                window.UpdateLayout();
                Assert.AreEqual(32d, numberBox.SpinButtonWidth);

                ZenDensityManager.ApplyDensity(window.Resources, ZenDensity.Comfortable);
                window.UpdateLayout();
                Assert.AreEqual(40d, numberBox.SpinButtonWidth);

                numberBox.IsEnabled = false;
                window.UpdateLayout();
                Assert.AreEqual(0.6d, numberBox.Opacity);
                Assert.AreEqual(1d, increase.Opacity);

                ZenThemeManager.ApplyTheme(window.Resources, ZenTheme.HighContrast, false);
                window.UpdateLayout();
                Assert.AreEqual(1d, numberBox.Opacity);
                Assert.AreEqual(1d, increase.Opacity);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void InvalidSpinButtonWidthIsRejected()
        {
            Assert.ThrowsExactly<ArgumentException>(
                () => new ZenNumberBox { SpinButtonWidth = double.NaN });
            Assert.ThrowsExactly<ArgumentException>(
                () => new ZenNumberBox { SpinButtonWidth = -1d });
        }

        private static Window CreateWindow(UIElement content)
        {
            return new Window
            {
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                Width = 260,
                Height = 100,
                Content = content
            };
        }

        private sealed class TestZenNumberBox : ZenNumberBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }
    }
}
