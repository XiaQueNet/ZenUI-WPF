using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
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
            Assert.IsNull(numberBox.IncreaseButtonContent);
            Assert.IsNull(numberBox.IncreaseButtonContentTemplate);
            Assert.IsNull(numberBox.DecreaseButtonContent);
            Assert.IsNull(numberBox.DecreaseButtonContentTemplate);
            Assert.IsNull(numberBox.EditorClickCommand);
            Assert.IsNull(numberBox.EditorClickCommandParameter);
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
                var increaseDefaultContent =
                    numberBox.Template.FindName("VerticalIncreaseDefaultContent", numberBox) as Viewbox;
                var decreaseDefaultContent =
                    numberBox.Template.FindName("VerticalDecreaseDefaultContent", numberBox) as Viewbox;
                Assert.IsNotNull(increaseDefaultContent);
                Assert.IsNotNull(decreaseDefaultContent);
                Assert.AreEqual(Visibility.Visible, increaseDefaultContent.Visibility);
                Assert.AreEqual(Visibility.Visible, decreaseDefaultContent.Visibility);
                Assert.IsInstanceOfType<Path>(increaseDefaultContent.Child);
                Assert.IsInstanceOfType<Path>(decreaseDefaultContent.Child);
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
        public void SpinButtonsDisplayCustomContentAndTemplatesInBothLayouts()
        {
            var increaseTemplate = new DataTemplate();
            var decreaseTemplate = new DataTemplate();
            var numberBox = new ZenNumberBox
            {
                IncreaseButtonContent = "添加",
                IncreaseButtonContentTemplate = increaseTemplate,
                DecreaseButtonContent = "移除",
                DecreaseButtonContentTemplate = decreaseTemplate
            };
            var window = CreateWindow(numberBox);

            try
            {
                window.Show();
                window.UpdateLayout();

                AssertCustomButtonContent(
                    numberBox,
                    "HorizontalIncreaseCustomContent",
                    "添加",
                    increaseTemplate);
                AssertCustomButtonContent(
                    numberBox,
                    "HorizontalDecreaseCustomContent",
                    "移除",
                    decreaseTemplate);

                numberBox.SpinButtonLayout = SpinButtonLayout.Vertical;
                window.UpdateLayout();

                AssertCustomButtonContent(
                    numberBox,
                    "VerticalIncreaseCustomContent",
                    "添加",
                    increaseTemplate);
                AssertCustomButtonContent(
                    numberBox,
                    "VerticalDecreaseCustomContent",
                    "移除",
                    decreaseTemplate);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void SpinButtonUiElementContentMovesWhenLayoutChanges()
        {
            var increaseContent = new TextBlock { Text = "添加" };
            var decreaseContent = new TextBlock { Text = "移除" };
            var numberBox = new ZenNumberBox
            {
                IncreaseButtonContent = increaseContent,
                DecreaseButtonContent = decreaseContent
            };
            var window = CreateWindow(numberBox);

            try
            {
                window.Show();
                window.UpdateLayout();

                AssertCustomButtonContent(
                    numberBox,
                    "HorizontalIncreaseCustomContent",
                    increaseContent,
                    null);
                AssertCustomButtonContent(
                    numberBox,
                    "HorizontalDecreaseCustomContent",
                    decreaseContent,
                    null);

                numberBox.SpinButtonLayout = SpinButtonLayout.Vertical;
                window.UpdateLayout();

                AssertCustomButtonContent(
                    numberBox,
                    "VerticalIncreaseCustomContent",
                    increaseContent,
                    null);
                AssertCustomButtonContent(
                    numberBox,
                    "VerticalDecreaseCustomContent",
                    decreaseContent,
                    null);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void EditorClickExecutesBoundCommandInBothLayouts()
        {
            var parameter = new object();
            var command = new RecordingCommand();
            var numberBox = new ZenNumberBox
            {
                EditorClickCommand = command,
                EditorClickCommandParameter = parameter
            };
            var window = CreateWindow(numberBox);

            try
            {
                window.Show();
                window.UpdateLayout();

                RaiseMouseLeftButtonUp(
                    numberBox.Template.FindName("PART_TextBox", numberBox) as TextBox);
                Assert.AreEqual(1, command.ExecutionCount);
                Assert.AreSame(parameter, command.LastParameter);

                numberBox.SpinButtonLayout = SpinButtonLayout.Vertical;
                window.UpdateLayout();

                RaiseMouseLeftButtonUp(
                    numberBox.Template.FindName("PART_VerticalTextBox", numberBox) as TextBox);
                Assert.AreEqual(2, command.ExecutionCount);
                Assert.AreSame(parameter, command.LastParameter);

                command.CanExecuteResult = false;
                RaiseMouseLeftButtonUp(
                    numberBox.Template.FindName("PART_VerticalTextBox", numberBox) as TextBox);
                Assert.AreEqual(2, command.ExecutionCount);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void PropertyChangedBindingUpdatesSourceWhileTyping()
        {
            var source = new NumberValueSource { Value = 1m };
            var numberBox = new ZenNumberBox();
            BindingOperations.SetBinding(
                numberBox,
                ZenNumberBox.ValueProperty,
                new Binding(nameof(NumberValueSource.Value))
                {
                    Mode = BindingMode.TwoWay,
                    Source = source,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            var window = CreateWindow(numberBox);

            try
            {
                window.Show();
                window.UpdateLayout();

                var editor = numberBox.Template.FindName("PART_TextBox", numberBox) as TextBox;
                Assert.IsNotNull(editor);

                editor.Text = 12.5m.ToString(CultureInfo.CurrentCulture);

                Assert.AreEqual(12.5m, numberBox.Value);
                Assert.AreEqual(12.5m, source.Value);
                Assert.AreEqual(
                    12.5m.ToString(CultureInfo.CurrentCulture),
                    editor.Text);
                Assert.IsNotNull(
                    BindingOperations.GetBindingExpression(
                        numberBox,
                        ZenNumberBox.ValueProperty));
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void IncompleteEditorTextDoesNotReplaceCurrentValue()
        {
            var numberBox = new ZenNumberBox { Value = 3m };
            var window = CreateWindow(numberBox);

            try
            {
                window.Show();
                window.UpdateLayout();

                var editor = numberBox.Template.FindName("PART_TextBox", numberBox) as TextBox;
                Assert.IsNotNull(editor);

                editor.Text = "-";

                Assert.AreEqual(3m, numberBox.Value);
                Assert.AreEqual("-", editor.Text);
            }
            finally
            {
                window.Close();
            }
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

        private static void AssertCustomButtonContent(
            ZenNumberBox numberBox,
            string presenterName,
            object expectedContent,
            DataTemplate expectedTemplate)
        {
            var presenter = numberBox.Template.FindName(presenterName, numberBox) as ContentPresenter;

            Assert.IsNotNull(presenter);
            Assert.AreEqual(Visibility.Visible, presenter.Visibility);
            Assert.AreEqual(expectedContent, presenter.Content);
            Assert.AreSame(expectedTemplate, presenter.ContentTemplate);
        }

        private static void RaiseMouseLeftButtonUp(TextBox textBox)
        {
            Assert.IsNotNull(textBox);
            textBox.RaiseEvent(new MouseButtonEventArgs(
                Mouse.PrimaryDevice,
                Environment.TickCount,
                MouseButton.Left)
            {
                RoutedEvent = UIElement.MouseLeftButtonUpEvent
            });
        }

        private sealed class TestZenNumberBox : ZenNumberBox
        {
            public object ExposedDefaultStyleKey => DefaultStyleKey;
            public AutomationPeer ExposedAutomationPeer => OnCreateAutomationPeer();
        }

        private sealed class RecordingCommand : ICommand
        {
            public bool CanExecuteResult { get; set; } = true;
            public int ExecutionCount { get; private set; }
            public object LastParameter { get; private set; }

            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }

            public bool CanExecute(object parameter)
            {
                return CanExecuteResult;
            }

            public void Execute(object parameter)
            {
                ExecutionCount++;
                LastParameter = parameter;
            }
        }

        private sealed class NumberValueSource
        {
            public decimal Value { get; set; }
        }
    }
}
