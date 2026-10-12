using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

namespace ZenUI.Wpf.Tests.Controls
{
    [STATestClass]
    public class DialogWindowStyleTests
    {
        [TestMethod]
        public void DialogWindowShowsSharedActionsAndRoutesCommands()
        {
            _ = new ZenButton();
            var resources = new ResourceDictionary
            {
                Source = new Uri("/ZenUI.Wpf;component/Themes/Generic.xaml", UriKind.Relative)
            };
            var viewModel = new DialogViewModel();
            var content = new UserControl { DataContext = viewModel };
            ZenDialogWindowOptions.SetConfirmCommand(content, viewModel.SaveCommand);
            ZenDialogWindowOptions.SetCancelCommand(content, viewModel.DiscardCommand);
            var window = new Window
            {
                Title = "编辑内容",
                Style = (Style)resources["ZenDialogWindowStyle"],
                Content = content
            };
            window.Resources.MergedDictionaries.Add(resources);

            try
            {
                window.Show();
                window.UpdateLayout();

                Assert.IsTrue(window.AllowsTransparency);
                Assert.IsNull(WindowChrome.GetWindowChrome(window));
                Assert.AreEqual(440d, window.ActualWidth);
                Assert.AreEqual(240d, window.ActualHeight);
                Assert.IsTrue(ZenDialogWindowOptions.GetIsDraggable(window));
                Assert.IsTrue(Descendants<TextBlock>(window).Any(text => text.Text == "编辑内容"));
                var buttons = Descendants<ZenButton>(window).ToList();
                Assert.IsTrue(buttons.Any(button => button is ZenDialogCloseButton));

                var confirm = buttons.Single(button => Equals(button.Content, "确定"));
                var cancel = buttons.Single(button => Equals(button.Content, "取消"));
                Assert.AreSame(viewModel.SaveCommand, confirm.Command);
                Assert.AreSame(viewModel.DiscardCommand, cancel.Command);

                confirm.Command.Execute(null);
                cancel.Command.Execute(null);
                Assert.AreEqual(1, viewModel.ConfirmCount);
                Assert.AreEqual(1, viewModel.CancelCount);
            }
            finally
            {
                window.Close();
            }
        }

        [TestMethod]
        public void DialogActionVisibilityCanBeChangedIndependently()
        {
            _ = new ZenButton();
            var resources = new ResourceDictionary
            {
                Source = new Uri("/ZenUI.Wpf;component/Themes/Generic.xaml", UriKind.Relative)
            };
            var style = new Style(typeof(Window), (Style)resources["ZenDialogWindowStyle"]);
            style.Setters.Add(new Setter(ZenDialogWindowOptions.ShowCancelButtonProperty, false));
            style.Setters.Add(new Setter(ZenDialogWindowOptions.IsDraggableProperty, false));
            style.Setters.Add(new Setter(ZenDialogWindowOptions.ConfirmContentProperty, "知道了"));
            style.Setters.Add(new Setter(ZenDialogWindowOptions.CancelContentProperty, "返回"));
            style.Setters.Add(new Setter(ZenDialogWindowOptions.ContentMarginProperty, new Thickness(24, 0, 24, 20)));
            style.Setters.Add(new Setter(ZenDialogWindowOptions.FooterPaddingProperty, new Thickness(24, 12, 24, 12)));
            var window = new Window
            {
                Style = style,
                Content = new UserControl { DataContext = new DialogViewModel() }
            };
            window.Resources.MergedDictionaries.Add(resources);

            try
            {
                window.Show();
                window.UpdateLayout();

                var cancel = (ZenButton)window.Template.FindName("CancelButton", window);
                var confirm = (ZenButton)window.Template.FindName("ConfirmButton", window);
                var actions = (Border)window.Template.FindName("ActionsBorder", window);

                Assert.AreEqual("知道了", confirm.Content);
                Assert.IsFalse(ZenDialogWindowOptions.GetIsDraggable(window));
                Assert.AreEqual("返回", cancel.Content);
                Assert.AreEqual(new Thickness(24, 0, 24, 20), Descendants<ScrollViewer>(window).Single().Margin);
                Assert.AreEqual(new Thickness(24, 12, 24, 12), actions.Padding);
                Assert.AreEqual(Visibility.Collapsed, cancel.Visibility);
                Assert.AreEqual(Visibility.Visible, confirm.Visibility);
                Assert.AreEqual(Visibility.Visible, actions.Visibility);

                ZenDialogWindowOptions.SetShowConfirmButton(window, false);
                Assert.AreEqual(Visibility.Collapsed, confirm.Visibility);
                Assert.AreEqual(Visibility.Collapsed, actions.Visibility);

                ZenDialogWindowOptions.SetShowCancelButton(window, true);
                Assert.AreEqual(Visibility.Visible, cancel.Visibility);
                Assert.AreEqual(Visibility.Visible, actions.Visibility);
            }
            finally
            {
                window.Close();
            }
        }

        private static IEnumerable<T> Descendants<T>(DependencyObject root) where T : DependencyObject
        {
            for (var index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
            {
                var child = VisualTreeHelper.GetChild(root, index);
                if (child is T match)
                {
                    yield return match;
                }

                foreach (var descendant in Descendants<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private sealed class DialogViewModel
        {
            public DialogViewModel()
            {
                SaveCommand = new CounterCommand(() => ConfirmCount++);
                DiscardCommand = new CounterCommand(() => CancelCount++);
            }

            public ICommand SaveCommand { get; }

            public ICommand DiscardCommand { get; }

            public int ConfirmCount { get; private set; }

            public int CancelCount { get; private set; }
        }

        private sealed class CounterCommand : ICommand
        {
            private readonly Action execute;

            public CounterCommand(Action execute)
            {
                this.execute = execute;
            }

            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter) => execute();
        }
    }
}
