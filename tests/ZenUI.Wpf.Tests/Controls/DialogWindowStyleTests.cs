using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;

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

        [TestMethod]
        public void OverlayDialogTracksOwnerAndCentersItsSurface()
        {
            _ = new ZenButton();
            var resources = new ResourceDictionary
            {
                Source = new Uri("/ZenUI.Wpf;component/Themes/Generic.xaml", UriKind.Relative)
            };
            var owner = new Window
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = 120,
                Top = 100,
                Width = 900,
                Height = 600
            };
            ZenOverlayDialogWindow overlay = null;

            try
            {
                owner.Show();
                overlay = new ZenOverlayDialogWindow
                {
                    Owner = owner,
                    Style = (Style)resources["ZenOverlayDialogWindowStyle"],
                    Content = new TextBlock { Text = "内容" }
                };
                overlay.Resources.MergedDictionaries.Add(resources);
                overlay.Show();
                overlay.UpdateLayout();

                Assert.IsFalse(ZenDialogWindowOptions.GetIsDraggable(overlay));
                Assert.AreEqual(WindowStyle.None, overlay.WindowStyle);
                Assert.AreEqual(ResizeMode.NoResize, overlay.ResizeMode);
                AssertOverlayMatchesVisibleOwner(owner, overlay);

                var mask = (Border)overlay.Template.FindName("OverlayMask", overlay);
                var bitmap = new RenderTargetBitmap((int)mask.ActualWidth, (int)mask.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(mask);
                var pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
                bitmap.CopyPixels(pixels, bitmap.PixelWidth * 4, 0);
                Assert.AreEqual(0, pixels[3]);
                Assert.IsTrue(pixels[((16 * bitmap.PixelWidth) + 16) * 4 + 3] > 0);

                var surface = (Border)overlay.Template.FindName("DialogSurface", overlay);
                Assert.AreEqual(600, surface.ActualWidth, 1);
                Assert.AreEqual(360, surface.ActualHeight, 1);
                var center = surface.TranslatePoint(new Point(surface.ActualWidth / 2, surface.ActualHeight / 2), overlay);
                Assert.AreEqual(overlay.ActualWidth / 2, center.X, 1);
                Assert.AreEqual(overlay.ActualHeight / 2, center.Y, 1);

                ZenOverlayDialogWindowOptions.SetDialogWidth(overlay, 680);
                ZenOverlayDialogWindowOptions.SetDialogHeight(overlay, 400);
                overlay.UpdateLayout();
                Assert.AreEqual(680, surface.ActualWidth, 1);
                Assert.AreEqual(400, surface.ActualHeight, 1);

                owner.Width = 500;
                owner.Height = 350;
                overlay.UpdateLayout();
                Assert.IsTrue(surface.ActualWidth < 680);
                Assert.IsTrue(surface.ActualHeight < 400);

                owner.Width = 900;
                owner.Height = 600;

                owner.Left += 35;
                owner.Top += 25;
                owner.Width += 80;
                owner.Height += 40;
                overlay.UpdateLayout();

                AssertOverlayMatchesVisibleOwner(owner, overlay);

                owner.Left = 2200;
                owner.Top = 120;
                overlay.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
                overlay.UpdateLayout();
                AssertOverlayMatchesVisibleOwner(owner, overlay);
                center = surface.TranslatePoint(new Point(surface.ActualWidth / 2, surface.ActualHeight / 2), overlay);
                Assert.AreEqual(overlay.ActualWidth / 2, center.X, 1);
                Assert.AreEqual(overlay.ActualHeight / 2, center.Y, 1);
            }
            finally
            {
                overlay?.Close();
                owner.Close();
            }
        }

        private static void AssertOverlayMatchesVisibleOwner(Window owner, Window overlay)
        {
            var ownerHandle = new WindowInteropHelper(owner).Handle;
            var overlayHandle = new WindowInteropHelper(overlay).Handle;
            Assert.IsTrue(DwmGetWindowAttribute(ownerHandle, 9, out var visibleOwner, Marshal.SizeOf<WindowRect>()) >= 0);
            Assert.IsTrue(DwmGetWindowAttribute(overlayHandle, 9, out var overlayBounds, Marshal.SizeOf<WindowRect>()) >= 0);
            Assert.AreEqual(visibleOwner.Left, overlayBounds.Left);
            Assert.AreEqual(visibleOwner.Top, overlayBounds.Top);
            Assert.AreEqual(visibleOwner.Right, overlayBounds.Right);
            Assert.AreEqual(visibleOwner.Bottom, overlayBounds.Bottom);
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr window, int attribute, out WindowRect rectangle, int size);

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
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
