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

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class NumberBoxTests
    {
        [TestMethod]
        public void NumberBoxExposesDefaultsAndThemeStyle()
        {
            var numberBox = new TestZenNumberBox();
            var dictionary = new ResourceDictionary
            {
                Source = new Uri("/ZenUI.Wpf;component/Themes/Generic.xaml", UriKind.Relative)
            };

            Assert.AreEqual(typeof(ZenNumberBox), numberBox.ExposedDefaultStyleKey);
            Assert.AreEqual(0m, numberBox.Value);
            Assert.AreEqual(1m, numberBox.Step);
            Assert.AreEqual(NumberBoxButtonMode.Horizontal, numberBox.ButtonMode);
            Assert.IsInstanceOfType<Style>(dictionary[typeof(ZenNumberBox)]);

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
                ButtonMode = NumberBoxButtonMode.Vertical,
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
